using System.Collections.Generic;

namespace Mage {

    /// <summary>
    /// This Mage filter builds a crosstab from the input data flow
    /// and outputs it when input data flow is complete
    /// </summary>
    public class CrosstabFilter : BaseModule {

        #region Member Variables

        // indexes to critical columns
        private int mEntityIdx = -1;
        private int mEntityIDIdx = -1;
        private int mFactorIdx = -1;
        private int mValueIdx = -1;

        // Master list of entities to build crosstab against.
        // It is a dictionary of entity ID/entity name key/value pairs
        private readonly Dictionary<string, string> mEntityList = new Dictionary<string, string>();

        // Accumulator list of factors
        // It is a dictionary, keyed by entity ID, of dictionares of name/value pairs for the crosstab fields
        private readonly Dictionary<string, Dictionary<string, string>> mFactorList = new Dictionary<string, Dictionary<string, string>>();

        #endregion

        #region Properties

        /// <summary>
        /// Name of the input column that contains the name of the entity to build crosstab for
        /// </summary>
        public string EntityNameCol { get; set; }

        /// <summary>
        /// Name of the input column that contains the ID of the entity to build crosstab for
        /// </summary>
        public string EntityIDCol { get; set; }

        /// <summary>
        /// Name of the input column that contains the name of the crosstab item
        /// </summary>
        public string FactorNameCol { get; set; }

        /// <summary>
        /// Name of the input column that contains the value of the crosstab item
        /// </summary>
        public string FactorValueCol { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage crosstab filter module
        /// </summary>
        public CrosstabFilter() {
            EntityNameCol = "Dataset";
            EntityIDCol = "Dataset_ID";
            FactorNameCol = "Factor";
            FactorValueCol = "Value";
        }

        #endregion

        #region BaseModule Overrides

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        ///
        /// Handle the column definitions
        /// Just call the base class, and then 
        /// precalculate the indexes to the critical columns
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            mEntityIdx = InputColumnPos[EntityNameCol];
            mEntityIDIdx = InputColumnPos[EntityIDCol];
            mFactorIdx = InputColumnPos[FactorNameCol];
            mValueIdx = InputColumnPos[FactorValueCol];
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// 
        /// Accumulate the data fields into the crosstab buffers
        /// and then output the accumulated results when the input
        /// data stream completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                RememberFactorData(args);
            } else {
                OutputColumnDefinitions();
                OutputDataRows();
            }
        }

        #endregion

        #region Data Utitilites

        /// <summary>
        /// Pull data of of the input fields and add it to 
        /// accumulating crosstab buffers
        /// </summary>
        /// <param name="args"></param>
        private void RememberFactorData(MageDataEventArgs args) {

            // get values of critical fields using precalculated indexes
            string entityName = args.Fields[mEntityIdx];
            string entityID = args.Fields[mEntityIDIdx];
            string factor = args.Fields[mFactorIdx];
            string value = args.Fields[mValueIdx];

            // make sure entity ID (and name) are in master list
            mEntityList[entityID] = entityName;

            // create dictionary to accumulate entity id /factor value pairs
            // for the factor
            if (!mFactorList.ContainsKey(factor)) {
                mFactorList[factor] = new Dictionary<string, string>();
            }
            mFactorList[factor].Add(entityID, value);
        }

        /// <summary>
        /// Go through the accumulated crosstab information and
        /// output it as rows.
        /// </summary>
        private void OutputDataRows() {
            // iterate over all the entity IDs in the master list
            // and create an output row for each one
            foreach (string entityID in mEntityList.Keys) {

                // create list to hold output row fields
				// add entity name and entity ID fields
                var outputRow = new List<string>
                {
	                entityID,
	                mEntityList[entityID]
                };

                // add entity name and entity ID fields

	            // add fields for all factors
                // and set field values for factors that the entity has vales for
                foreach (string factor in mFactorList.Keys) {
                    string fieldValue = "";
                    if (mFactorList[factor].ContainsKey(entityID)) {
                        fieldValue = mFactorList[factor][entityID];
                    }
                    outputRow.Add(fieldValue);
                }
                OnDataRowAvailable(new MageDataEventArgs(outputRow.ToArray()));
            }
            // inform our subscribers that all data has been sent
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        /// <summary>
        /// Output the column definition for the output crosstab rows
        /// </summary>
        private void OutputColumnDefinitions() {

            // start with empty column definition list
			// add the entity name and id columns
            var outCols = new List<MageColumnDef>
            {
	            InputColumnDefs[mEntityIDIdx],
	            InputColumnDefs[mEntityIdx]
            };


	        // add a column for each factor
            foreach (string fac in mFactorList.Keys) {
                outCols.Add(new MageColumnDef(fac, "text", "15"));
            }

            OnColumnDefAvailable(new MageColumnEventArgs(outCols.ToArray()));
        }

        #endregion
    }
}
