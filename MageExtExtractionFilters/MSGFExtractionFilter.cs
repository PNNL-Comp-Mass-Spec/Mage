using System;
using System.Collections.Generic;
using System.IO;
using Mage;

namespace MageExtExtractionFilters
{

    class MSGFExtractionFilter : ExtractionFilter
    {

        #region Member Variables

        private MergeMSGFData mMSGFMerger;

        // Input column that contains key for looking up score to merge
        private string mMatchColumnName = string.Empty;

        // Output column in which merged score will be placed
        private string mMergeColumnName = string.Empty;

        #endregion

        #region Initialization

        private void InitializeParameters()
        {

            mMatchColumnName = mExtractionType.RType.ResultIDColName;
            mMergeColumnName = "MSGF_SpecProb";

            mMSGFMerger = new MergeMSGFData();
            mMSGFMerger.SetCutoff(mExtractionType.MSGFCutoff);
            mMSGFMerger.SetAcceptMissingMerges(mKeepAllResults);
        }

        /// <summary>
        /// Initialize a new MSGF extraction filter
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

        /// <summary>
        /// Terminate execution of this module
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();
            mMSGFMerger?.Cancel();
        }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            OutputColumnList = "Job|+|text, MSGF_SpecProb|+|double, *";
            base.HandleColumnDef(sender, args);

            // List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            // OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));

            if (mMSGFMerger != null && !Abort)
            {
                if (!mMergeFiles.TryGetValue("MSGF_Name", out var msgfFile))
                {
                    msgfFile = new ResultType.MergeFile(string.Empty, string.Empty, string.Empty, string.Empty);
                }

                mMSGFMerger.GetMSGFLookupData(msgfFile, mResultsDirectoryPath);
                mMSGFMerger.MatchColIdx = OutputColumnPos[mMatchColumnName];
                mMSGFMerger.MergeColIdx = OutputColumnPos[mMergeColumnName];
            }
        }

        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {

                var accepted = false;
                var outRow = MapDataRow(args.Fields);
                if (mMSGFMerger != null)
                {
                    accepted = mMSGFMerger.MergeMSGFDataFields(outRow);
                }

                if (accepted)
                {
                    mPassedRowsCounter++;
                    OnDataRowAvailable(new MageDataEventArgs(outRow));
                }

                ++mTotalRowsCounter;
                ReportProgress();

            }
            else
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }


        #region Internal Classes

        protected class MergeMSGFData
        {

            #region Member Variables

            private Dictionary<string, string> mMergeValueLookup;

            ProcessingPipeline mMSGFReaderPipeline;

            private double mCutoffThreshold;
            private bool mCheckCutoff;
            private bool mAcceptMissingMerges = true;

            #endregion

            #region Properties

            public int MatchColIdx { get; set; }

            public int MergeColIdx { get; set; }

            #endregion

            /// <summary>
            /// Terminate execution of this module
            /// </summary>
            public void Cancel()
            {
                mMSGFReaderPipeline?.Cancel();
            }

            /// <summary>
            /// Set the cutoff threshold
            /// </summary>
            /// <param name="cutoff"></param>
            public void SetCutoff(string cutoff)
            {
                if (string.Equals(cutoff, ALL_PASS_CUTOFF, StringComparison.OrdinalIgnoreCase))
                {
                    mCheckCutoff = false;
                    mCutoffThreshold = 0;
                }
                else
                {
                    mCheckCutoff = true;
                    double.TryParse(cutoff, out mCutoffThreshold);
                }
            }

            /// <summary>
            /// If an MSGF filter is defined and a peptide
            /// does not have an entry in the MSGF file
            /// and if "keep all results" is unchecked,
            /// </summary>
            /// <param name="keepAllResults"></param>
            public void SetAcceptMissingMerges(bool keepAllResults)
            {
                mAcceptMissingMerges = keepAllResults || Math.Abs(mCutoffThreshold) < Single.Epsilon;
            }

            /// <summary>
            /// Merge MSGF data into output row from lookup table
            /// and conditionally apply cutoff test
            /// </summary>
            /// <param name="outRow"></param>
            /// <returns></returns>
            public bool MergeMSGFDataFields(string[] outRow)
            {
                var accepted = true;
                if (mMergeValueLookup == null)
                {
                    // MSGF values were not loaded (this will be true for MSPathFinder results)
                    return true;
                }

                var joinKey = outRow[MatchColIdx];
                if (mMergeValueLookup.ContainsKey(joinKey))
                {
                    var score = mMergeValueLookup[joinKey];
                    outRow[MergeColIdx] = score;
                    if (!mCheckCutoff)
                    {
                        return true;
                    }

                    if (double.TryParse(score, out var msgf))
                    {
                        accepted = (msgf <= mCutoffThreshold);
                    }
                }
                else
                {
                    accepted = mAcceptMissingMerges;
                }

                return accepted;
            }

            /// <summary>
            /// Populate given KVSink module with MSGF lookup data
            /// </summary>
            /// <param name="mergeFile"></param>
            /// <param name="resultDirectoryPath"></param>
            public void GetMSGFLookupData(ResultType.MergeFile mergeFile, string resultDirectoryPath)
            {
                var msgfFileName = mergeFile.MergeFileName;
                if (!string.IsNullOrEmpty(msgfFileName))
                {
                    var msgfFileReader = new DelimitedFileReader
                    {
                        FilePath = Path.Combine(resultDirectoryPath, msgfFileName)
                    };

                    var msgfScoreLookup = new KVSink
                    {
                        KeyColumnName = mergeFile.KeyCol,
                        ValueColumnName = "SpecProb"
                    };

                    mMSGFReaderPipeline = ProcessingPipeline.Assemble("Process File To Lookup", msgfFileReader, msgfScoreLookup);
                    mMSGFReaderPipeline.RunRoot(null);

                    mMergeValueLookup = msgfScoreLookup.Values;
                }
            }
        }

        #endregion

    }
}
