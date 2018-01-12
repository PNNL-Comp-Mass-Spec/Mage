using Mage;

namespace MageExtContentFilters
{

    /// <summary>
    /// Filter X!Tandem results using the FilterSetID defined by the base class
    /// </summary>
    /// <remarks>
    /// This class will be auto-discovered by the ModuleDiscovery class in Mage
    /// The list of auto-discovered filters is then used to populate the gridview on form FilterSelectionForm.cs
    /// </remarks>
    [MageAttribute("Filter", "XTFilter", "XT filter", "Uses filter criteria defined in DMS")]
    class XTFilter : ContentFilter
    {

        #region Member Variables

        // Working copy of SEQUEST filter object
        private FilterXTResults mXTFilter;

        // Indexes into the synopsis row field array
        private int peptideSequenceIndex;
        private int delCN2ValueIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        private int hyperScoreValueIndex;
        private int logEValueIndex;
        private int msgfSpecProbIndex = -1;

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
            SetupXTFilter();
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
        /// The position index for each column has been precalculated by
        /// PrecalculateFieldIndexes() at startup.
        /// </summary>
        /// <param name="fields">Row, as array of fields</param>
        /// <returns>Whether or not row should be included in output</returns>
        protected override bool CheckFilter(ref string[] fields)
        {
            var peptideSequence = GetColumnValue(fields, peptideSequenceIndex, string.Empty);
            var hyperScoreValue = GetColumnValue(fields, hyperScoreValueIndex, -1d);
            var logEValue = GetColumnValue(fields, logEValueIndex, -1d);
            var delCN2Value = GetColumnValue(fields, delCN2ValueIndex, -1d);
            var chargeState = GetColumnValue(fields, chargeStateIndex, -1);
            var peptideMass = GetColumnValue(fields, peptideMassIndex, -1d);
            var msgfSpecProb = GetColumnValue(fields, msgfSpecProbIndex, -1d);

            var accepted = mXTFilter.EvaluateXTandem(peptideSequence, hyperScoreValue, logEValue, delCN2Value, chargeState, peptideMass, msgfSpecProb);

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
        private void SetupXTFilter()
        {

            // Create Mage module to query DMS (typically on gigasax)
            var reader = new MSSQLReader
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
            mXTFilter = new FilterXTResults(filterCriteria.Rows, FilterSetID);
        }

        /// <summary>
        /// Set up indexes into row fields array based on column name
        /// </summary>
        private void PrecalculateFieldIndexes()
        {
            peptideSequenceIndex = GetColumnIndex(InputColumnPos, "Peptide_Sequence");
            hyperScoreValueIndex = GetColumnIndex(InputColumnPos, "Peptide_Hyperscore");
            logEValueIndex = GetColumnIndex(InputColumnPos, "Peptide_Expectation_Value_Log(e)");
            delCN2ValueIndex = GetColumnIndex(InputColumnPos, "DeltaCn2");
            chargeStateIndex = GetColumnIndex(InputColumnPos, "Charge");
            peptideMassIndex = GetColumnIndex(InputColumnPos, "Peptide_MH");
            msgfSpecProbIndex = GetColumnIndex(InputColumnPos, "MSGF_SpecProb");
        }
        /*
        Result_ID
        Group_ID
        Scan
        Charge
        Peptide_MH
        Peptide_Hyperscore
        Peptide_Expectation_Value_Log(e)
        Multiple_Protein_Count
        Peptide_Sequence
        DeltaCn2
        y_score
        y_ions
        b_score
        b_ions
        Delta_Mass
        Peptide_Intensity_Log(I)
        */
    }
}
