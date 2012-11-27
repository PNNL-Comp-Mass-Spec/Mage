using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters {

    /// <summary>
    /// MSGFDb Extraction Filter
    /// </summary>
    public class MSGFDbFHTExtractionFilter : ExtractionFilter {

        #region Member Variables

        // working copy of MSGFDB filter object
        private FilterMSGFDbResults mMSGFDbFilter = null;

        // indexes into the synopsis row field array
        private int peptideSequenceIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
        private int msgfDbSpecProbValueIndex = 0;
        private int pValueIndex = 0;

        // Note that FDR and PepFDR may not be present
        private int FDRIndex = -1;
        private int pepFDRIndex = -1;

        private int msgfSpecProbIndex = -1;

        #endregion

        #region Properties

        public FilterMSGFDbResults ResultChecker {
            get { return mMSGFDbFilter; }
            set { mMSGFDbFilter = value; }
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
            OutputColumnList = "Job, Passed_Filter|+|text, *";
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
                ++mTotalRowsCounter;
                ReportProgress();

                bool accepted = false;
                object[] outRow = MapDataRow(args.Fields);

                accepted = CheckFilter(ref outRow);
                
                if (accepted) {
                    mPassedRowsCounter++;
                    OnDataRowAvailable(new MageDataEventArgs(outRow));
                }                
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
        protected bool CheckFilter(ref object[] vals) {
            bool accept = true;
            if (mMSGFDbFilter == null) {
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            } else {
                string peptideSequence = GetColumnValue(ref vals, peptideSequenceIndex, "");
                int chargeState = GetColumnValue(ref vals, chargeStateIndex, 0);
                double peptideMass = GetColumnValue(ref vals, peptideMassIndex, -1d);
                double SpecProb = GetColumnValue(ref vals, msgfDbSpecProbValueIndex, -1d);
                double PValue = GetColumnValue(ref vals, pValueIndex, -1d);
                double FDR = GetColumnValue(ref vals, FDRIndex, -1d);
                double PepFDR = GetColumnValue(ref vals, pepFDRIndex, -1d);
                double msgfSpecProb = GetColumnValue(ref vals, msgfSpecProbIndex, -1d);
				int rankMSGFDbSpecProb = 1;

				bool pass = mMSGFDbFilter.EvaluateMSGFDB(peptideSequence, chargeState, peptideMass, SpecProb, PValue, FDR, PepFDR, msgfSpecProb, rankMSGFDbSpecProb);

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
			Dictionary<MSGFDbExtractionFilter.MSGFDBColumns, int> dctColumnMapping;

			MSGFDbExtractionFilter.DetermineFieldIndexes(columnPos, out dctColumnMapping);

			peptideSequenceIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.Peptide);
			chargeStateIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.Charge);
			peptideMassIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MH);

			msgfDbSpecProbValueIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MSGFDB_SpecProbOrEValue);
			// rankMSGFDbSpecProbIndex = 1;

			pValueIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.PValueOrEValue);

			// Note that FDR and PepFDR may not be present
			FDRIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.FDROrQValue);
			pepFDRIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.PepFDROrPepQValue);

			msgfSpecProbIndex = MSGFDbExtractionFilter.GetMSGFDBColumnIndex(dctColumnMapping, MSGFDbExtractionFilter.MSGFDBColumns.MSGF_SpecProb);
        }

        /// <summary>
        /// Return an MSGFDB filter object that is preset with filter criteria
        /// that is obtained (my means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterMSGFDbResults MakeMSGFDbResultChecker(string FilterSetID) {

            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            Dictionary<string, string> runtimeParms = new Dictionary<string, string>() { { "Filter_Set_ID", FilterSetID } };
            MSSQLReader reader = new MSSQLReader(queryDefXML, runtimeParms);

            // create Mage module to receive query results
            SimpleSink filterCriteria = new SimpleSink();

            // build pipeline and run it
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // create new Sequest filter object with retrieved filter criteria
            return new FilterMSGFDbResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
