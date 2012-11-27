using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MageExtContentFilters;
using Mage;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters {

    public class MSGFDbExtractionFilter : ExtractionFilter {

		#region "Enums"
		public enum MSGFDBColumns {
			Peptide,
			Charge,
			MH,
			MSGF_SpecProb,
			PValueOrEValue,
			FDROrQValue,
			PepFDROrPepQValue,
			MSGFDB_SpecProbOrEValue,
			Rank_MSGFDB_SpecProbOrEValue
		}

		#endregion

        #region Member Variables

        // working copy of MSGFDB filter object
        private FilterMSGFDbResults mMSGFDbFilter = null;

        // indexes into the synopsis row field array
        private int peptideSequenceIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
		private int msgfDbSpecProbValueIndex = 0;		// MSGFDB_SpecProb for MSGFDB, MSGFDB_SpecEValue for MSGF+
		private int rankMSGFDbSpecProbIndex = -1;		// Rank_MSGFDB_SpecProb for MSGFDB, Rank_MSGFDB_SpecEValue for MSGF+

		private int pValueIndex = 0;					// PValue for MSGFDB,          EValue for MSGF+

        // Note that FDR and PepFDR may not be present
        private int FDRIndex = -1;						// FDR for MSGFDB,             QValue for MSGF+
		private int pepFDRIndex = -1;					// PepFDR for MSGFDB,          PepQValue for MSGF+

        private int msgfSpecProbIndex = -1;

        private MergeProteinData mProteinMerger = null;
        private bool mOutputAllProteins = false;
		
        #endregion

        #region Properties

        public FilterMSGFDbResults ResultChecker {
            get { return mMSGFDbFilter; }
            set { mMSGFDbFilter = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters() {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.Inspect);
            mOutputAllProteins = (mExtractionType.RType.ResultName == ResultType.MSGFDB_SYN_ALL_PROTEINS);
        }

        public override void Prepare() {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            mFilterResultsColumnName = "Passed_Filter";
            OutputColumnList = "Job, Passed_Filter|+|text, *, Cleavage_State|+|text, Terminus_State|+|text";
            base.HandleColumnDef(sender, args);

            ////List<MageColumnDef> cd = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            ////OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));

            PrecalculateFieldIndexes();
            if (mProteinMerger != null) {
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
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                ++mTotalRowsCounter;
                ReportProgress();

                bool accepted = false;
                object[] outRow = MapDataRow(args.Fields);

                if (!mOutputAllProteins) {
                    mProteinMerger.MergeFirstProtein(ref outRow);
                    accepted = CheckFilter(ref outRow);
                } else {
                    Collection<object[]> rows = null;

                    rows = mProteinMerger.MergeAllProteins(ref outRow);
                    if (rows == null) {
                        accepted = CheckFilter(ref outRow);
						OnWarningMessage(new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
                    } else {
                        for (int i = 0; i < rows.Count; i++) {
                            object[] row = rows[i];
                            accepted = CheckFilter(ref row);
                        }
                    }
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
				int rankMSGFDbSpecProb = GetColumnValue(ref vals, rankMSGFDbSpecProbIndex, -1);

				bool pass = mMSGFDbFilter.EvaluateMSGFDB(peptideSequence, chargeState, peptideMass, SpecProb, PValue, FDR, PepFDR, msgfSpecProb, rankMSGFDbSpecProb);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = ((pass) ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
            }

            if (accept) {
                mPassedRowsCounter++;
                OnDataRowAvailable(new MageDataEventArgs(vals));
            }

            return accept;
        }

		public static void DetermineFieldIndexes(Dictionary<string, int> columnHeaders, out Dictionary<MSGFDBColumns, int> dctColumnMapping)
		{
			int columnIndex;
			bool msgfPlus = false;

			dctColumnMapping = new Dictionary<MSGFDBColumns, int>();

			dctColumnMapping.Add(MSGFDBColumns.Peptide, GetColumnIndex(columnHeaders, "Peptide"));
			dctColumnMapping.Add(MSGFDBColumns.Charge, GetColumnIndex(columnHeaders, "Charge"));
			dctColumnMapping.Add(MSGFDBColumns.MH, GetColumnIndex(columnHeaders, "MH"));
			columnIndex = GetColumnIndex(columnHeaders, "MSGFDB_SpecProb");

			// MSGF+ has MSGFDB_SpecEValue instead of MSGFDB_SpecProb; need to check for this
			if (columnIndex < 0)
			{
				columnIndex = GetColumnIndex(columnHeaders, "MSGFDB_SpecEValue");
				if (columnIndex >= 0)
					msgfPlus = true;
			}
			else
			{
				msgfPlus = false;
			}

			dctColumnMapping.Add(MSGFDBColumns.MSGFDB_SpecProbOrEValue, columnIndex);

			// Note that FDR and PepFDR (QValue and PepQValue in MSGF+) may not be present in MSGFDB results
			if (msgfPlus)
			{
				dctColumnMapping.Add(MSGFDBColumns.PValueOrEValue, GetColumnIndex(columnHeaders, "EValue"));
				dctColumnMapping.Add(MSGFDBColumns.FDROrQValue, GetColumnIndex(columnHeaders, "QValue"));
				dctColumnMapping.Add(MSGFDBColumns.PepFDROrPepQValue, GetColumnIndex(columnHeaders, "PepQValue"));
				dctColumnMapping.Add(MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue, GetColumnIndex(columnHeaders, "Rank_MSGFDB_SpecEValue"));
			}
			else
			{
				dctColumnMapping.Add(MSGFDBColumns.PValueOrEValue, GetColumnIndex(columnHeaders, "PValue"));
				dctColumnMapping.Add(MSGFDBColumns.FDROrQValue, GetColumnIndex(columnHeaders, "FDR"));
				dctColumnMapping.Add(MSGFDBColumns.PepFDROrPepQValue, GetColumnIndex(columnHeaders, "PepFDR"));
				dctColumnMapping.Add(MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue, GetColumnIndex(columnHeaders, "Rank_MSGFDB_SpecProb"));
			}

			dctColumnMapping.Add(MSGFDBColumns.MSGF_SpecProb, GetColumnIndex(columnHeaders, "MSGF_SpecProb"));
		}

		public static int GetMSGFDBColumnIndex(Dictionary<MSGFDBColumns, int> columnPos, MSGFDBColumns columnName)
		{
			int value;

			if (columnPos.TryGetValue(columnName, out value))
				return value;
			else
				return -1;
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

			Dictionary<MSGFDBColumns, int> dctColumnMapping;
					
			DetermineFieldIndexes(columnPos, out dctColumnMapping);

			peptideSequenceIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Peptide);
			chargeStateIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Charge);
			peptideMassIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MH);
			
			msgfDbSpecProbValueIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MSGFDB_SpecProbOrEValue);
			rankMSGFDbSpecProbIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue);

			pValueIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.PValueOrEValue);
			
			// Note that FDR and PepFDR may not be present
			FDRIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.FDROrQValue);
			pepFDRIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.PepFDROrPepQValue);
			
			msgfSpecProbIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MSGF_SpecProb);
			
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
