using System;
using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// MSGFDb Extraction Filter
    /// </summary>
    [Obsolete("Deprecated in 2024")]
    public class MSGFDbFHTExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Mage

        // Indexes into the synopsis row field array
        private int peptideSequenceIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        private int msgfDbSpecProbValueIndex;
        private int pValueIndex;

        // Note that FDR and PepFDR may not be present
        private int FDRIndex = -1;
        private int pepFDRIndex = -1;

        private int msgfSpecProbIndex = -1;

        public FilterMSGFDbResults ResultChecker { get; set; }

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mFilterResultsColumnName = "Passed_Filter";
            OutputColumnList = "Job, Passed_Filter|+|text, *";
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
                var chargeState = GetColumnValue(values, chargeStateIndex, 0);
                var peptideMass = GetColumnValue(values, peptideMassIndex, -1d);
                var SpecProb = GetColumnValue(values, msgfDbSpecProbValueIndex, -1d);
                var PValue = GetColumnValue(values, pValueIndex, -1d);
                var FDR = GetColumnValue(values, FDRIndex, -1d);
                var PepFDR = GetColumnValue(values, pepFDRIndex, -1d);
                var msgfSpecProb = GetColumnValue(values, msgfSpecProbIndex, -1d);
                const int rankMSGFDbSpecProb = 1;

                var pass = ResultChecker.EvaluateMSGFDB(peptideSequence, chargeState, peptideMass, SpecProb, PValue, FDR, PepFDR, msgfSpecProb, rankMSGFDbSpecProb);

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
            MSGFDbExtractionFilter.DetermineFieldIndexes(columnPos, out var columnMapping);

            peptideSequenceIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.Peptide);
            chargeStateIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.Charge);
            peptideMassIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MH);

            msgfDbSpecProbValueIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MSGFDB_SpecProbOrEValue);
            // rankMSGFDbSpecProbIndex = 1;

            pValueIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.PValueOrEValue);

            // Note that FDR and PepFDR may not be present
            FDRIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.FDROrQValue);
            pepFDRIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.PepFDROrPepQValue);

            msgfSpecProbIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(columnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MSGF_SpecProb);
        }

        /// <summary>
        /// Return an MSGFDB filter object that is preset with filter criteria
        /// that is obtained (my means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterMSGFDbResults MakeMSGFDbResultChecker(string FilterSetID)
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

            // Create new MS-GF+ filter object with retrieved filter criteria
            return new FilterMSGFDbResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
