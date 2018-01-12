using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{

    /// <summary>
    /// Sequest Extraction Filter
    /// </summary>
    public class SequestExtractionFilter : ExtractionFilter
    {

        #region Member Variables

        // Working copy of SEQUEST hit checker object

        // Indexes into the synopsis row field array
        private int peptideSequenceIndex;
        private int xCorrValueIndex;
        private int delCNValueIndex;
        private int delCN2ValueIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        private int cleavageStateIndex;
        private int msgfSpecProbIndex;
        private int rankXCIndex;

        #endregion

        #region Properties

        public FilterSequestResults ResultChecker { get; set; }

        #endregion

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mFilterResultsColumnName = "Passed_Filter";
            OutputColumnList = "Job,Passed_Filter|+|text, *";
            base.HandleColumnDef(sender, args);

            /// List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            /// OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
            PrecalculateFieldIndexes();
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

                var accepted = CheckFilter(outRow);

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
                var xCorrValue = GetColumnValue(vals, xCorrValueIndex, -1d);
                var delCNValue = GetColumnValue(vals, delCNValueIndex, -1d);
                var delCN2Value = GetColumnValue(vals, delCN2ValueIndex, -1d);
                var chargeState = GetColumnValue(vals, chargeStateIndex, -1);
                var peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
                var cleavageState = GetColumnValue(vals, cleavageStateIndex, -1);
                var msgfSpecProb = GetColumnValue(vals, msgfSpecProbIndex, -1d);
                var rankXC = GetColumnValue(vals, rankXCIndex, -1);

                // Legacy columns; no longer used
                var spectrumCount = -1;
                double discriminantScore = -1;
                double NETAbsoluteDifference = -1;

                var pass = ResultChecker.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, spectrumCount, discriminantScore, NETAbsoluteDifference, cleavageState, msgfSpecProb, rankXC);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0)
                {
                    vals[mFilterResultsColIdx] = ((pass) ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
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
            peptideSequenceIndex = GetColumnIndex(columnPos, "Peptide");
            xCorrValueIndex = GetColumnIndex(columnPos, "XCorr");
            delCNValueIndex = GetColumnIndex(columnPos, "DelCn");
            delCN2ValueIndex = GetColumnIndex(columnPos, "DelCn2");
            chargeStateIndex = GetColumnIndex(columnPos, "ChargeState");
            peptideMassIndex = GetColumnIndex(columnPos, "MH");
            cleavageStateIndex = GetColumnIndex(columnPos, "NumTrypticEnds");
            msgfSpecProbIndex = GetColumnIndex(columnPos, "MSGF_SpecProb");
            rankXCIndex = GetColumnIndex(columnPos, "RankXc");
        }

        /// <summary>
        /// Make a new FilterSequestResults object that is configured to use the given filter set ID
        /// </summary>
        /// <param name="FilterSetID"></param>
        /// <returns></returns>
        public static FilterSequestResults MakeSequestResultChecker(string FilterSetID)
        {

            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            var runtimeParms = new Dictionary<string, string>() { { "Filter_Set_ID", FilterSetID } };
            var reader = new MSSQLReader(queryDefXML, runtimeParms);

            // Create Mage module to receive query results
            var filterCriteria = new SimpleSink();

            // Build pipeline and run it
            var pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // Create new Sequest filter object with retrieved filter criteria
            return new FilterSequestResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
