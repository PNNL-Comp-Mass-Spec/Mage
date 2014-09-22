using System.Collections.Generic;
using MageExtContentFilters;
using Mage;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters {

    public class XTandemExtractionFilter : ExtractionFilter {

        #region Member Variables

        // working copy of SEQUEST filter object
        private FilterXTResults mXTFilter = null;

        // indexes into the synopsis row field array
        private int peptideSequenceIndex = 0;
        private int delCN2ValueIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
        private int hyperScoreValueIndex = 0;
        private int logEValueIndex = 0;
        private int msgfSpecProbIndex = 0;

        private MergeProteinData mProteinMerger = null;
        private bool mOutputAllProteins = false;


        #endregion

        #region Properties

        public FilterXTResults ResultChecker {
            get { return mXTFilter; }
            set { mXTFilter = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters() {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.XTandem);
            mOutputAllProteins = (mExtractionType.RType.ResultName == ResultType.XTANDEM_ALL_PROTEINS);
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
            OutputColumnList = "Job, Result_ID, Passed_Filter|+|text, *, Cleavage_State|+|text, Terminus_State|+|text, Protein_Name|+|text, Protein_Expectation_Value_Log(e)|+|double, Protein_Intensity_Log(I)|+|double";
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
                    accepted = CheckFilter(ref outRow);
                } else {
					Collection<string[]> rows = null;
	                bool matchFound;
					rows = mProteinMerger.MergeAllProteins(ref outRow, out matchFound);
                    if (rows == null) {
                        accepted = CheckFilter(ref outRow);
						if (!matchFound)
							OnWarningMessage(new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
                    } else {
                        for (int i = 0; i < rows.Count; i++) {
							string[] row = rows[i];
                            accepted = CheckFilter(ref row);
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
            if (mXTFilter == null) {
                if (mFilterResultsColIdx >= 0) {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            } else {
				string peptideSequence = GetColumnValue(vals, peptideSequenceIndex, "");
				double delCN2Value = GetColumnValue(vals, delCN2ValueIndex, -1d);
				double hyperScoreValue = GetColumnValue(vals, hyperScoreValueIndex, -1d);
				double logEValue = GetColumnValue(vals, logEValueIndex, -1d);
				int chargeState = GetColumnValue(vals, chargeStateIndex, -1);
				double peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
				double msgfSpecProb = GetColumnValue(vals, msgfSpecProbIndex, -1d);

                bool pass = mXTFilter.EvaluateXTandem(peptideSequence, hyperScoreValue, logEValue, delCN2Value, chargeState, peptideMass, msgfSpecProb);

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
        public static FilterXTResults MakeXTandemResultChecker(string FilterSetID) {

            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            Dictionary<string, string> runtimeParms = new Dictionary<string, string>() { { "Filter_Set_ID", FilterSetID } };
            MSSQLReader reader = new MSSQLReader(queryDefXML, runtimeParms);

            // create Mage module to receive query results
            SimpleSink filterCriteria = new SimpleSink();

            // build pipeline and run it
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // create new Sequest filter object with retrieved filter criteria
            return new FilterXTResults(filterCriteria.Rows, FilterSetID);
        }

    }
}
