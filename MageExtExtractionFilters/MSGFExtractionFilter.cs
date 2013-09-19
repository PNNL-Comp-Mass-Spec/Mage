using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.IO;

namespace MageExtExtractionFilters {

    class MSGFExtractionFilter : ExtractionFilter {

        #region Member Variables

        private MergeMSGFData mMSGFMerger = null;

        // Input column that contains key for looking up score to merge
        private string mMatchColumnName = "";

        // Output column in which merged score will be placed
        private string mMergeColumnName = "";

        #endregion

        #region Initialization

        private void InitializeParameters() {

            mMatchColumnName = mExtractionType.RType.ResultIDColName;
            mMergeColumnName = "MSGF_SpecProb";

            mMSGFMerger = new MergeMSGFData();
            mMSGFMerger.SetCutoff(mExtractionType.MSGFCutoff);
            mMSGFMerger.SetAcceptMissingMerges(mKeepAllResults);
        }

		/// <summary>
		/// Initialize a new MSGF extraction filter
		/// </summary>
        public override void Prepare() {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

		/// <summary>
		/// Terminate execution of this module
		/// </summary>
		public override void Cancel() {
			base.Cancel();
			if (mMSGFMerger != null)
				mMSGFMerger.Cancel();
		}

        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            OutputColumnList = "Job|+|text, MSGF_SpecProb|+|double, *";
            base.HandleColumnDef(sender, args);

            ////List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            ////OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));

            if (mMSGFMerger != null && !Abort) {
                mMSGFMerger.GetMSGFLookupData(mMergeFiles["MSGF_Name"], mResultFolderPath);
                mMSGFMerger.MatchColIdx = OutputColumnPos[mMatchColumnName];
                mMSGFMerger.MergeColIdx = OutputColumnPos[mMergeColumnName];
            }
        }

        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {

                bool accepted = false;
				string[] outRow = MapDataRow(args.Fields);
                if (mMSGFMerger != null) {
                    accepted = mMSGFMerger.MergeMSGFDataFields(ref outRow);
                }

                if (accepted) {
                    mPassedRowsCounter++;
                    OnDataRowAvailable(new MageDataEventArgs(outRow));
                }

				++mTotalRowsCounter;
				ReportProgress();

            } else {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }


        #region Internal Classes

        protected class MergeMSGFData {

            #region Member Variables

            private Dictionary<string, string> mMergeValueLookup = null;

			ProcessingPipeline mMSGFReaderPipeline;

            private double mCutoffThreshold = 0;
            private bool mCheckCutoff = false;
            private bool mAcceptMissingMerges = true;

            private int mMatchColIdx;
            private int mMergeColIdx;

            #endregion

            #region Properties

            public int MatchColIdx {
                get { return mMatchColIdx; }
                set { mMatchColIdx = value; }
            }

            public int MergeColIdx {
                get { return mMergeColIdx; }
                set { mMergeColIdx = value; }
            }

            #endregion

			/// <summary>
			/// Terminate execution of this module
			/// </summary>
			public void Cancel() {
				if (mMSGFReaderPipeline != null)
					mMSGFReaderPipeline.Cancel();
			}


            /// <summary>
            /// Set the cutoff threshold
            /// </summary>
            /// <param name="cutoff"></param>
            public void SetCutoff(string cutoff) {
				if (cutoff.ToLower() == "All Pass".ToLower()) {
                    mCheckCutoff = false;
                    mCutoffThreshold = 0;
                } else {
                    mCheckCutoff = true;
                    double.TryParse(cutoff, out mCutoffThreshold);
                }
            }

            /// <summary>
            /// if an MSGF filter is defined and a peptide 
            /// does not have an entry in the MSGF file 
            /// and if "keep all results" is unchecked,
            /// </summary>
            /// <param name="keepAllResults"></param>
            public void SetAcceptMissingMerges(bool keepAllResults) {
                mAcceptMissingMerges = keepAllResults || mCutoffThreshold == 0;
            }

            /// <summary>
            /// Merge MSGF data into output row from lookup table
            /// and conditionally apply cutoff test
            /// </summary>
            /// <param name="outRow"></param>
            /// <returns></returns>
			public bool MergeMSGFDataFields(ref string[] outRow)
			{
                bool accepted = true;
                if (mMergeValueLookup != null) {
                    string joinKey = outRow[mMatchColIdx];
                    if (mMergeValueLookup.ContainsKey(joinKey)) {
                        string score = mMergeValueLookup[joinKey];
                        outRow[mMergeColIdx] = score;
                        if (mCheckCutoff) {
                            double msgf;
                            if (double.TryParse(score, out msgf)) {
                                accepted = (msgf <= mCutoffThreshold);
                            }
                        }
                    } else {
                        accepted = mAcceptMissingMerges;
                    }
                }
                return accepted;
            }

            /// <summary>
            /// Populate given KVSink module with MSGF lookup data
            /// </summary>
            /// <param name="mergeFiles"></param>
            /// <param name="resultFolderPath"></param>
            /// <param name="msgfScoreLookup"></param>
            public void GetMSGFLookupData(ResultType.MergeFile mergeFile, string resultFolderPath) {
                string msgfFileName = mergeFile.MergeFileName;
                if (!string.IsNullOrEmpty(msgfFileName)) {
                    DelimitedFileReader msgfFileReader = new DelimitedFileReader();
                    msgfFileReader.FilePath = Path.Combine(resultFolderPath, msgfFileName);

                    KVSink msgfScoreLookup = new KVSink();
                    msgfScoreLookup.KeyColumnName = mergeFile.KeyCol;
                    msgfScoreLookup.ValueColumnName = "SpecProb";

					mMSGFReaderPipeline = ProcessingPipeline.Assemble("Process File To Lookup", msgfFileReader, msgfScoreLookup);
					mMSGFReaderPipeline.RunRoot(null);

                    mMergeValueLookup = msgfScoreLookup.Values;
                }
            }
        }

        #endregion

    }
}
