using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MageExtContentFilters;
using Mage;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters {

    public class MSGFDbExtractionFilter : ExtractionFilter {

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
                
                bool pass = mMSGFDbFilter.EvaluateMSGFDB(peptideSequence, chargeState, peptideMass, SpecProb, PValue, FDR, PepFDR, msgfSpecProb);

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
            peptideSequenceIndex = GetColumnIndex(columnPos, "Peptide");
            chargeStateIndex = GetColumnIndex(columnPos, "Charge");
            peptideMassIndex = GetColumnIndex(columnPos, "MH");
            msgfDbSpecProbValueIndex = GetColumnIndex(columnPos, "MSGFDB_SpecProb");
            pValueIndex = GetColumnIndex(columnPos, "PValue");
            // Note that FDR and PepFDR may not be present
            FDRIndex = GetColumnIndex(columnPos, "FDR");
            pepFDRIndex = GetColumnIndex(columnPos, "PepFDR");
            msgfSpecProbIndex = GetColumnIndex(columnPos, "MSGF_SpecProb");        
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
