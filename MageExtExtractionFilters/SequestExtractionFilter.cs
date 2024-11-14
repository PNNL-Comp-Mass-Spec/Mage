using System;
using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Sequest Extraction Filter
    /// </summary>
    [Obsolete("Deprecated in 2024")]
    public class SequestExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Mage, Sequest

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

        public FilterSequestResults ResultChecker { get; set; }

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

            // List<MageColumnDef> columnDefs = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            // OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
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
        /// <param name="values"></param>
        protected bool CheckFilter(string[] values)
        {
            var accept = true;
            if (ResultChecker == null)
            {
                if (mFilterResultsColIdx >= 0)
                {
                    values[mFilterResultsColIdx] = "Not Checked";
                }
            }
            else
            {
                var peptideSequence = GetColumnValue(values, peptideSequenceIndex, "");
                var xCorrValue = GetColumnValue(values, xCorrValueIndex, -1d);
                var delCNValue = GetColumnValue(values, delCNValueIndex, -1d);
                var delCN2Value = GetColumnValue(values, delCN2ValueIndex, -1d);
                var chargeState = GetColumnValue(values, chargeStateIndex, -1);
                var peptideMass = GetColumnValue(values, peptideMassIndex, -1d);
                var cleavageState = GetColumnValue(values, cleavageStateIndex, -1);
                var msgfSpecProb = GetColumnValue(values, msgfSpecProbIndex, -1d);
                var rankXC = GetColumnValue(values, rankXCIndex, -1);

                // Legacy columns; no longer used
                const int spectrumCount = -1;
                const double discriminantScore = -1;
                const double NETAbsoluteDifference = -1;

                var pass = ResultChecker.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, spectrumCount, discriminantScore, NETAbsoluteDifference, cleavageState, msgfSpecProb, rankXC);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0)
                {
                    values[mFilterResultsColIdx] = (pass ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
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
        public static FilterSequestResults MakeSequestResultChecker(string FilterSetID)
        {
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            var runtimeParms = new Dictionary<string, string>
            {
                { "Filter_Set_ID", FilterSetID }
            };

            var reader = new SQLReader(queryDefXML, runtimeParms);

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
