using Mage;

namespace MageExtContentFilters
{
    /// <summary>
    /// Filter Sequest results using the FilterSetID defined by the base class
    /// </summary>
    /// <remarks>
    /// This class will be auto-discovered by the ModuleDiscovery class in Mage
    /// The list of auto-discovered filters is then used to populate the grid view on form FilterSelectionForm.cs
    /// </remarks>
    [MageAttribute("Filter", "SEQUEST", "SEQUEST filter", "Uses filter criteria defined in DMS")]
    class SequestFilter : ContentFilter
    {
        #region Member Variables

        /// <summary>
        /// Working copy of SEQUEST filter object
        /// </summary>
        private FilterSequestResults mSeqFilter;

        /// <summary>
        /// Indexes into the synopsis row field array
        /// </summary>
        private int peptideSequenceIndex;
        private int xCorrValueIndex;
        private int delCNValueIndex;
        private int delCN2ValueIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        // private int spectrumCountIndex = 0;
        // private int discriminantScoreIndex = 0;
        // private int NETAbsoluteDifferenceIndex = 0;
        private int cleavageStateIndex;
        private int msgfSpecProbIndex;
        private int rankXcIndex;

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the filter set from DMS to use
        /// </summary>
        public string FilterSetID { get; set; }

        #endregion

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();
            SetupSequestFilter();
        }

        /// <summary>
        /// This is called when all the field column definitions
        /// have been read from standard tabular input
        /// </summary>
        protected override void ColumnDefsFinished()
        {
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
            var peptideSequence = GetColumnValue(fields, peptideSequenceIndex, string.Empty);
            var xCorrValue = GetColumnValue(fields, xCorrValueIndex, -1d);
            var delCNValue = GetColumnValue(fields, delCNValueIndex, -1d);
            var delCN2Value = GetColumnValue(fields, delCN2ValueIndex, -1d);
            var chargeState = GetColumnValue(fields, chargeStateIndex, -1);
            var peptideMass = GetColumnValue(fields, peptideMassIndex, -1d);
            var cleavageState = GetColumnValue(fields, cleavageStateIndex, -1);
            var msgfSpecProb = GetColumnValue(fields, msgfSpecProbIndex, -1d);
            var rankXc = GetColumnValue(fields, rankXcIndex, -1);

            var spectrumCount = -1;
            double discriminantScore = -1;
            double NETAbsoluteDifference = -1;

            var accepted = mSeqFilter.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, spectrumCount, discriminantScore, NETAbsoluteDifference, cleavageState, msgfSpecProb, rankXc);

            if (accepted && OutputColumnDefs != null)
            {
                var outRow = MapDataRow(fields);
                fields = outRow;
            }
            return accepted;
        }

        /// <summary>
        /// Setup a Sequest filter object with a set of filter criteria
        /// that is obtained for the given FilterSetID from DMS
        /// my means of a Mage pipeline
        /// </summary>
        private void SetupSequestFilter()
        {
            // Create Mage module to query DMS (typically on gigasax)
            var reader = new SQLReader
            {
                Database = Globals.DMSDatabase,
                Server = Globals.DMSServer,
                SQLText =
                    string.Format(
                        "SELECT Filter_Criteria_Group_ID, Criterion_Name, Criterion_Comparison, Criterion_Value FROM V_Mage_Filter_Set_Criteria WHERE Filter_Set_ID = {0}",
                        FilterSetID)
            };

            // Create Mage module to receive query results
            var filterCriteria = new SimpleSink();

            // Build pipeline and run it
            var pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // Create new Sequest filter object with retrieved filter criteria
            mSeqFilter = new FilterSequestResults(filterCriteria.Rows, FilterSetID);
        }

        /// <summary>
        /// Set up indexes into row fields array based on column name
        /// </summary>
        private void PrecalculateFieldIndexes()
        {
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
