using System.Collections.Generic;
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
			Rank_MSGFDB_SpecProbOrEValue,
			Scan,
			Protein
		}

		#endregion

        #region Member Variables

        // working copy of MSGFDB filter object
        private FilterMSGFDbResults mMSGFDbFilter = null;

        // indexes into the synopsis row field array
		private int scanNumberIndex = 0;
        private int peptideSequenceIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
		private int proteinIndex = 0;
		private int msgfDbSpecProbValueIndex = 0;		// MSGFDB_SpecProb for MSGFDB, MSGFDB_SpecEValue for MSGF+
		private int rankMSGFDbSpecProbIndex = -1;		// Rank_MSGFDB_SpecProb for MSGFDB, Rank_MSGFDB_SpecEValue for MSGF+

		private int pValueIndex = 0;					// PValue for MSGFDB,          EValue for MSGF+

        // Note that FDR and PepFDR may not be present
        private int FDRIndex = -1;						// FDR for MSGFDB,             QValue for MSGF+
		private int pepFDRIndex = -1;					// PepFDR for MSGFDB,          PepQValue for MSGF+

        private int msgfSpecProbIndex = -1;

        private MergeProteinData mProteinMerger = null;
        private bool mOutputAllProteins = false;
		private SortedSet<string> mDataWrittenRowTags;

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
			mDataWrittenRowTags = new SortedSet<string>();
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

                bool accepted = false;
				string[] outRow = MapDataRow(args.Fields);

                if (!mOutputAllProteins) {
                    mProteinMerger.MergeFirstProtein(ref outRow);

					string sScanChargePeptide = CreateRowTag(outRow, includeProtein: false);
					if (mDataWrittenRowTags.Contains(sScanChargePeptide))
						accepted = false;
					else
					{
						mDataWrittenRowTags.Add(sScanChargePeptide);
						accepted = CheckFilter(ref outRow);
					}
                } else {
					Collection<string[]> rows = null;
	                bool matchFound;

                    rows = mProteinMerger.MergeAllProteins(ref outRow, out matchFound);
                    if (rows == null) {
						// Either the peptide only maps to one protein, or the ProteinMerger did not find a match for the row
						string sScanChargePeptideProtein = CreateRowTag(outRow, includeProtein: true);
						if (mDataWrittenRowTags.Contains(sScanChargePeptideProtein))
							accepted = false;
						else
						{
							mDataWrittenRowTags.Add(sScanChargePeptideProtein);
							accepted = CheckFilter(ref outRow);
							if (!matchFound)
								OnWarningMessage(new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
						}
                    } else {
                        for (int i = 0; i < rows.Count; i++) {
							string[] row = rows[i];

							string sScanChargePeptideProtein = CreateRowTag(row, includeProtein: true);
							if (mDataWrittenRowTags.Contains(sScanChargePeptideProtein))
								accepted = false;
							else
							{
								mDataWrittenRowTags.Add(sScanChargePeptideProtein);
								accepted = CheckFilter(ref row);
							}
                        }
                    }
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
            if (mMSGFDbFilter == null) {
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            } else {
                string peptideSequence = GetColumnValue(vals, peptideSequenceIndex, "");
                int chargeState = GetColumnValue(vals, chargeStateIndex, 0);
                double peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
                double SpecProb = GetColumnValue(vals, msgfDbSpecProbValueIndex, -1d);
                double PValue = GetColumnValue(vals, pValueIndex, -1d);
                double FDR = GetColumnValue(vals, FDRIndex, -1d);
                double PepFDR = GetColumnValue(vals, pepFDRIndex, -1d);
                double msgfSpecProb = GetColumnValue(vals, msgfSpecProbIndex, -1d);
				int rankMSGFDbSpecProb = GetColumnValue(vals, rankMSGFDbSpecProbIndex, -1);

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

		protected string CreateRowTag(string[] vals, bool includeProtein)
		{
			int scanNumber = GetColumnValue(vals, scanNumberIndex, -1);
			int chargeState = GetColumnValue(vals, chargeStateIndex, 0);
			string peptideSequence = GetColumnValue(vals, peptideSequenceIndex, string.Empty);
			string proteinName = GetColumnValue(vals, proteinIndex, string.Empty);

			if (includeProtein)
			{
				// Even though we are tracking proteins, we need to remove prefix and suffix residues to avoid reporting the same peptide mapped to the same protein in the same scan twice in the output file
				return scanNumber + "_" + chargeState + "_" + RemovePrefixAndSuffixResidues(peptideSequence) + "_" + proteinName;
			}
			else
			{
				peptideSequence = RemovePrefixAndSuffixResidues(peptideSequence);
				return scanNumber + "_" + chargeState + "_" + peptideSequence;
			}

		}

		public static void DetermineFieldIndexes(Dictionary<string, int> columnHeaders, out Dictionary<MSGFDBColumns, int> dctColumnMapping)
		{
			int columnIndex;
			bool msgfPlus = false;

			dctColumnMapping = new Dictionary<MSGFDBColumns, int>();

			dctColumnMapping.Add(MSGFDBColumns.Scan, GetColumnIndex(columnHeaders, "Scan"));
			dctColumnMapping.Add(MSGFDBColumns.Peptide, GetColumnIndex(columnHeaders, "Peptide"));
			dctColumnMapping.Add(MSGFDBColumns.Charge, GetColumnIndex(columnHeaders, "Charge"));
			dctColumnMapping.Add(MSGFDBColumns.MH, GetColumnIndex(columnHeaders, "MH"));
			dctColumnMapping.Add(MSGFDBColumns.Protein, GetColumnIndex(columnHeaders, "Protein"));

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

			scanNumberIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Scan);
			peptideSequenceIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Peptide);
			chargeStateIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Charge);
			peptideMassIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MH);
			proteinIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Protein);

			msgfDbSpecProbValueIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MSGFDB_SpecProbOrEValue);
			rankMSGFDbSpecProbIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue);

			pValueIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.PValueOrEValue);
			
			// Note that FDR and PepFDR may not be present
			FDRIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.FDROrQValue);
			pepFDRIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.PepFDROrPepQValue);
			
			msgfSpecProbIndex = GetMSGFDBColumnIndex(dctColumnMapping, MSGFDBColumns.MSGF_SpecProb);
			
        }

		protected string RemovePrefixAndSuffixResidues(string peptide)
		{
			int periodIndex1;
			int periodIndex2;

			if (peptide.StartsWith("..") && peptide.Length > 2)
				peptide = '.' + peptide.Substring(2);

			if (peptide.EndsWith("..") && peptide.Length > 2)
				peptide = peptide.Substring(0, peptide.Length - 2) + '.';

			periodIndex1 = peptide.IndexOf('.');
			periodIndex2 = peptide.IndexOf('.', periodIndex1 + 1);

			// See if strSequenceIn contains two periods
			if (periodIndex1 > -1 && periodIndex2 > -1 && periodIndex2 > periodIndex1 + 1)
			{
				// Sequence contains two periods with letters between the periods, 
				// For example, A.BCDEFGHIJK.L or ABCD.BCDEFGHIJK.L
				// Extract out the text between the periods
				peptide = peptide.Substring(periodIndex1 + 1, periodIndex2 - periodIndex1 - 1);
			} 

			return peptide;
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
