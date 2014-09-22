using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters {

    /// <summary>
    /// Sequest Extraction Filter
    /// </summary>
    public class SequestExtractionFilter : ExtractionFilter {

        #region Member Variables

        // working copy of SEQUEST hit checker object
        private FilterSequestResults mSeqFilter = null;

        // indexes into the synopsis row field array
        private int peptideSequenceIndex = 0;
        private int xCorrValueIndex = 0;
        private int delCNValueIndex = 0;
        private int delCN2ValueIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
        private int cleavageStateIndex = 0;
        private int msgfSpecProbIndex = 0;
		private int rankXCIndex = 0;

        #endregion

        #region Properties

        public FilterSequestResults ResultChecker {
            get { return mSeqFilter; }
            set { mSeqFilter = value; }
        }

        #endregion

        #region Initialization

        public override void Prepare() {
            base.Prepare();
        }

        #endregion

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            mFilterResultsColumnName = "Passed_Filter";
			OutputColumnList = "Job,Passed_Filter|+|text, *";
            base.HandleColumnDef(sender, args);
            
            ////List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            ////OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
            PrecalculateFieldIndexes();
        }

        /// <summary>
        /// Process a single result record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {

                bool accepted = false;
				string[] outRow = MapDataRow(args.Fields);

                accepted = CheckFilter(ref outRow);

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

        /// <summary>
        /// Evaluate result against filter (if one is active)
        /// and annotate the appropriate column in the result (if one is specified)
        /// and pass on result if it passed the filter
        /// </summary>
        /// <param name="outRow"></param>
        /// <returns></returns>
		protected bool CheckFilter(ref string[] vals)
		{
            bool accept = true;
            if (mSeqFilter == null) {
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            } else {
                string peptideSequence = GetColumnValue(vals, peptideSequenceIndex, "");
				double xCorrValue = GetColumnValue(vals, xCorrValueIndex, -1d);
				double delCNValue = GetColumnValue(vals, delCNValueIndex, -1d);
				double delCN2Value = GetColumnValue(vals, delCN2ValueIndex, -1d);
				int chargeState = GetColumnValue(vals, chargeStateIndex, -1);
				double peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
				int cleavageState = GetColumnValue(vals, cleavageStateIndex, -1);
				double msgfSpecProb = GetColumnValue(vals, msgfSpecProbIndex, -1d);
				int rankXC = GetColumnValue(vals, rankXCIndex, -1);

                // Legacy columns; no longer used
                int spectrumCount = -1;
                double discriminantScore = -1;
                double NETAbsoluteDifference = -1;

				bool pass = mSeqFilter.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, spectrumCount, discriminantScore, NETAbsoluteDifference, cleavageState, msgfSpecProb, rankXC);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = ((pass) ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
            }
            return accept;
        }

        /// <summary>
        /// set up indexes into row fields array based on column name
        /// (saves time when referencing result columns later)
        /// </summary>
        private void PrecalculateFieldIndexes() {
            if (string.IsNullOrEmpty(OutputColumnList)) {
                PrecalculateFieldIndexes(InputColumnPos);
            } else {
                PrecalculateFieldIndexes(OutputColumnPos);
            }
        }

        private void PrecalculateFieldIndexes(Dictionary<string, int> columnPos) {
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
        public static FilterSequestResults MakeSequestResultChecker(string FilterSetID) {
        
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            Dictionary<string, string> runtimeParms = new Dictionary<string, string>() { { "Filter_Set_ID", FilterSetID } };
            MSSQLReader reader = new MSSQLReader(queryDefXML, runtimeParms);

            // create Mage module to receive query results
            SimpleSink filterCriteria = new SimpleSink();

            // build pipeline and run it
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // create new Sequest filter object with retrieved filter criteria
            return new FilterSequestResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
