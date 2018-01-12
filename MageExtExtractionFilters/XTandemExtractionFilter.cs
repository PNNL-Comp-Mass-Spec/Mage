using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{

    public class XTandemExtractionFilter : ExtractionFilter
    {

        #region Member Variables

        // Working copy of X!Tandem filter object

        // Indexes into the synopsis row field array
        private int peptideSequenceIndex;
        private int delCN2ValueIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        private int hyperScoreValueIndex;
        private int logEValueIndex;
        private int msgfSpecProbIndex;

        private MergeProteinData mProteinMerger;
        private bool mOutputAllProteins;


        #endregion

        #region Properties

        public FilterXTResults ResultChecker { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters()
        {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.XTandem);
            mOutputAllProteins = (mExtractionType.RType.ResultName == ResultType.XTANDEM_ALL_PROTEINS);
        }

        public override void Prepare()
        {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mFilterResultsColumnName = "Passed_Filter";
            OutputColumnList = "Job, Result_ID, Passed_Filter|+|text, *, Cleavage_State|+|text, Terminus_State|+|text, Protein_Name|+|text, Protein_Expectation_Value_Log(e)|+|double, Protein_Intensity_Log(I)|+|double";
            base.HandleColumnDef(sender, args);

            /// List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            /// OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));

            PrecalculateFieldIndexes();
            if (mProteinMerger != null)
            {
                mProteinMerger.GetProteinLookupData(mMergeFiles["ResultToSeqMap"], mMergeFiles["SeqToProteinMap"], mResultFolderPath);
                mProteinMerger.LookupColumn = OutputColumnPos[mExtractionType.RType.ResultIDColName];
                mProteinMerger.SetMergeSourceColIndexes(OutputColumnPos);
            }
        }

        /// <summary>
        /// Process a single result record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {

                var outRow = MapDataRow(args.Fields);

                if (!mOutputAllProteins)
                {
                    string warningMessage;
                    if (!mProteinMerger.MergeFirstProtein(outRow, out warningMessage))
                    {
                        OnWarningMessage(
                            new MageStatusEventArgs("ProteinMerger reports " + warningMessage + " for row " + mTotalRowsCounter));
                    }

                    CheckFilter(outRow);
                }
                else
                {
                    bool matchFound;
                    var rows = mProteinMerger.MergeAllProteins(outRow, out matchFound);
                    if (rows == null)
                    {
                        CheckFilter(outRow);
                        if (!matchFound)
                            OnWarningMessage(new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
                    }
                    else
                    {
                        foreach (var rowItem in rows)
                        {
                            var row = rowItem;
                            CheckFilter(row);
                        }
                    }
                }

                ++mTotalRowsCounter;
                ReportProgress();

            }
            else
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        /// <summary>
        /// Evaluate result against filter (if one is active)
        /// and annotate the appropriate column in the result (if one is specified)
        /// and pass on result if it passed the filter
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        protected bool CheckFilter(string[] vals)
        {
            var accept = true;
            if (ResultChecker == null)
            {
                if (mFilterResultsColIdx >= 0)
                {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            }
            else
            {
                var peptideSequence = GetColumnValue(vals, peptideSequenceIndex, "");
                var delCN2Value = GetColumnValue(vals, delCN2ValueIndex, -1d);
                var hyperScoreValue = GetColumnValue(vals, hyperScoreValueIndex, -1d);
                var logEValue = GetColumnValue(vals, logEValueIndex, -1d);
                var chargeState = GetColumnValue(vals, chargeStateIndex, -1);
                var peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
                var msgfSpecProb = GetColumnValue(vals, msgfSpecProbIndex, -1d);

                var pass = ResultChecker.EvaluateXTandem(peptideSequence, hyperScoreValue, logEValue, delCN2Value, chargeState, peptideMass, msgfSpecProb);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0)
                {
                    vals[mFilterResultsColIdx] = ((pass) ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
            }

            if (accept)
            {
                mPassedRowsCounter++;
                OnDataRowAvailable(new MageDataEventArgs(vals));
            }

            return accept;
        }


        /// <summary>
        /// Set up indexes into row fields array based on column name
        /// (saves time when referencing result columns later)
        /// </summary>
        private void PrecalculateFieldIndexes()
        {
            if (string.IsNullOrEmpty(OutputColumnList))
            {
                PrecalculateFieldIndexes(InputColumnPos);
            }
            else
            {
                PrecalculateFieldIndexes(OutputColumnPos);
            }
        }

        private void PrecalculateFieldIndexes(Dictionary<string, int> columnPos)
        {
            peptideSequenceIndex = GetColumnIndex(columnPos, "Peptide_Sequence");
            hyperScoreValueIndex = GetColumnIndex(columnPos, "Peptide_Hyperscore");
            logEValueIndex = GetColumnIndex(columnPos, "Peptide_Expectation_Value_Log(e)");
            delCN2ValueIndex = GetColumnIndex(columnPos, "DeltaCn2");
            chargeStateIndex = GetColumnIndex(columnPos, "Charge");
            peptideMassIndex = GetColumnIndex(columnPos, "Peptide_MH");
            msgfSpecProbIndex = GetColumnIndex(columnPos, "MSGF_SpecProb");
        }

        /// <summary>
        /// Return an XTandem filter object that is preset with filter criteria
        /// that is obtained (my means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterXTResults MakeXTandemResultChecker(string FilterSetID)
        {

            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            var runtimeParms = new Dictionary<string, string>() { { "Filter_Set_ID", FilterSetID } };
            var reader = new MSSQLReader(queryDefXML, runtimeParms);

            // Create Mage module to receive query results
            var filterCriteria = new SimpleSink();

            // Build pipeline and run it
            var pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // Create new X!Tandem filter object with retrieved filter criteria
            return new FilterXTResults(filterCriteria.Rows, FilterSetID);
        }

    }
}
