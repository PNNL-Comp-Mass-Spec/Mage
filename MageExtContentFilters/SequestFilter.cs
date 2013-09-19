using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace MageExtContentFilters {

    /// <summary>
    /// Filter Sequest results using the FilterSetID defined by the base class
    /// </summary>
	/// <remarks>
	/// This class will be auto-discovered by the ModuleDiscovery class in Mage
	/// The list of auto-discovered filters is then used to populate the gridview on form FilterSelectionForm.cs
	/// </remarks>
    [MageAttribute("Filter", "SEQUEST", "SEQUEST filter", "Uses filter criteria defined in DMS")]
    class SequestFilter : ContentFilter {

        #region Member Variables

        // working copy of SEQUEST filter object
        private FilterSequestResults mSeqFilter = null;

        // indexes into the synopsis row field array
        private int peptideSequenceIndex = 0;
        private int xCorrValueIndex = 0;
        private int delCNValueIndex = 0;
        private int delCN2ValueIndex = 0;
        private int chargeStateIndex = 0;
        private int peptideMassIndex = 0;
        ////private int spectrumCountIndex = 0;
        ////private int discriminantScoreIndex = 0;
        ////private int NETAbsoluteDifferenceIndex = 0;
        private int cleavageStateIndex = 0;
		private int msgfSpecProbIndex = 0;
		private int rankXcIndex = 0;

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the filter set from DMS to use
        /// </summary>
        public string FilterSetID { get; set; }

        #endregion

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare() {
            base.Prepare();
            SetupSequestFilter();
        }

        /// <summary>
        /// this is called when all the field column definitions 
        /// have been read from standard tabular input
        /// </summary>
        protected override void ColumnDefsFinished() {
            PrecalculateFieldIndexes();
        }

        /// <summary>
        /// This is called for each row that is being subjected to filtering.
        /// The fields array contains value of each column for the row.
        /// The position index for each column has been precalculted by 
        /// PrecalculateFieldIndexes() at startup.
        /// </summary>
        /// <param name="fields">Row, as array of fields</param>
        /// <returns>Whether or not row should be included in output</returns>
		protected override bool CheckFilter(ref string[] fields)
		{
            bool accepted = false;
         
			string peptideSequence = GetColumnValue(ref fields, peptideSequenceIndex, string.Empty);
			double xCorrValue = GetColumnValue(ref fields, xCorrValueIndex, -1d);
			double delCNValue = GetColumnValue(ref fields, delCNValueIndex, -1d);
			double delCN2Value = GetColumnValue(ref fields, delCN2ValueIndex, -1d);
			int chargeState = GetColumnValue(ref fields, chargeStateIndex, -1);
			double peptideMass = GetColumnValue(ref fields, peptideMassIndex, -1d);
			int cleavageState = GetColumnValue(ref fields, cleavageStateIndex, -1);
			double msgfSpecProb = GetColumnValue(ref fields, msgfSpecProbIndex, -1d);
			int rankXc = GetColumnValue(ref fields, rankXcIndex, -1);

			int spectrumCount = -1;
			double discriminantScore = -1;
			double NETAbsoluteDifference = -1;

			accepted = mSeqFilter.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, spectrumCount, discriminantScore, NETAbsoluteDifference, cleavageState, msgfSpecProb, rankXc);

            if (accepted && OutputColumnDefs != null) {
				string[] outRow = MapDataRow(fields);
                fields = outRow;
            }
            return accepted;
        }

        /// <summary>
        /// Setup a Sequest filter object with a set of filter criteria
        /// that is obtained for the given FilterSetID from DMS
        /// my means of a Mage pipeline
        /// </summary>
        private void SetupSequestFilter() {

            // create Mage module to query DMS
            MSSQLReader reader = new MSSQLReader();
            reader.Database = "DMS5";
            reader.Server = "gigasax";
            reader.SQLText = string.Format("SELECT Filter_Criteria_Group_ID, Criterion_Name, Criterion_Comparison, Criterion_Value FROM V_Mage_Filter_Set_Criteria WHERE Filter_Set_ID = {0}", FilterSetID);

            // create Mage module to receive query results
            SimpleSink filterCriteria = new SimpleSink();

            // build pipeline and run it
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // create new Sequest filter object with retrieved filter criteria
            mSeqFilter = new FilterSequestResults(filterCriteria.Rows, FilterSetID);
        }

        /// <summary>
        /// set up indexes into row fields array based on column name
        /// </summary>
        private void PrecalculateFieldIndexes() {
            peptideSequenceIndex = GetColumnIndex(InputColumnPos, "Peptide");
            xCorrValueIndex = GetColumnIndex(InputColumnPos, "XCorr");
            delCNValueIndex = GetColumnIndex(InputColumnPos, "DelCn");
            delCN2ValueIndex = GetColumnIndex(InputColumnPos, "DelCn2");
            chargeStateIndex = GetColumnIndex(InputColumnPos, "ChargeState");
            peptideMassIndex = GetColumnIndex(InputColumnPos, "MH");
            cleavageStateIndex = GetColumnIndex(InputColumnPos, "NumTrypticEnds");
			msgfSpecProbIndex = GetColumnIndex(InputColumnPos, "MSGF_SpecProb");
			rankXcIndex = GetColumnIndex(InputColumnPos, "RankXc");
			
        }

    }

}
