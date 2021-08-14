using System.Collections.Generic;

namespace Mage
{
    /// <summary>
    /// This Mage filter builds a crosstab from the input data flow
    /// and outputs it when input data flow is complete
    /// </summary>
    public class CrosstabFilter : BaseModule
    {
        // Indexes to critical columns
        private int mEntityIdx = -1;
        private int mEntityIDIdx = -1;
        private int mFactorIdx = -1;
        private int mValueIdx = -1;

        // Master list of entities to build crosstab against.
        // It is a dictionary of entity ID/entity name key/value pairs
        private readonly Dictionary<string, string> mEntityList = new();

        // Accumulator list of factors
        // It is a dictionary, keyed by entity ID, of dictionaries of name/value pairs for the crosstab fields
        private readonly Dictionary<string, Dictionary<string, string>> mFactorList = new();

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

        /// <summary>
        /// Construct a new Mage crosstab filter module
        /// </summary>
        public CrosstabFilter()
        {
            EntityNameCol = "Dataset";
            EntityIDCol = "Dataset_ID";
            FactorNameCol = "Factor";
            FactorValueCol = "Value";
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        ///
        /// Handle the column definitions
        /// Just call the base class, and then
        /// precalculate the indexes to the critical columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            mEntityIdx = InputColumnPos[EntityNameCol];
            mEntityIDIdx = InputColumnPos[EntityIDCol];
            mFactorIdx = InputColumnPos[FactorNameCol];
            mValueIdx = InputColumnPos[FactorValueCol];
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        ///
        /// Accumulate the data fields into the crosstab buffers
        /// and then output the accumulated results when the input
        /// data stream completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                RememberFactorData(args);
            }
            else
            {
                OutputColumnDefinitions();
                OutputDataRows();
            }
        }

        /// <summary>
        /// Pull data of of the input fields and add it to
        /// accumulating crosstab buffers
        /// </summary>
        /// <param name="args"></param>
        private void RememberFactorData(MageDataEventArgs args)
        {
            // Get values of critical fields using pre-calculated indexes
            var entityName = args.Fields[mEntityIdx];
            var entityID = args.Fields[mEntityIDIdx];
            var factor = args.Fields[mFactorIdx];
            var value = args.Fields[mValueIdx];

            // Make sure entity ID (and name) are in master list
            mEntityList[entityID] = entityName;

            // Create dictionary to accumulate entity id /factor value pairs
            // for the factor
            if (!mFactorList.ContainsKey(factor))
            {
                mFactorList[factor] = new Dictionary<string, string>();
            }
            mFactorList[factor].Add(entityID, value);
        }

        /// <summary>
        /// Go through the accumulated crosstab information and
        /// output it as rows.
        /// </summary>
        private void OutputDataRows()
        {
            // Iterate over all the entity IDs in the master list
            // and create an output row for each one
            foreach (var entityID in mEntityList.Keys)
            {
                // Create list to hold output row fields
                // Add entity name and entity ID fields
                var outputRow = new List<string>
                {
                    entityID,
                    mEntityList[entityID]
                };

                // Add entity name and entity ID fields

                // Add fields for all factors
                // and set field values for factors that the entity has vales for
                foreach (var factor in mFactorList.Keys)
                {
                    string fieldValue;
                    if (mFactorList[factor].ContainsKey(entityID))
                    {
                        fieldValue = mFactorList[factor][entityID];
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }

                    outputRow.Add(fieldValue);
                }
                OnDataRowAvailable(new MageDataEventArgs(outputRow.ToArray()));
            }

            // Inform our subscribers that all data has been sent
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        /// <summary>
        /// Output the column definition for the output crosstab rows
        /// </summary>
        private void OutputColumnDefinitions()
        {
            // Start with empty column definition list
            // Add the entity name and id columns
            var outCols = new List<MageColumnDef>
            {
                InputColumnDefs[mEntityIDIdx],
                InputColumnDefs[mEntityIdx]
            };

            // Add a column for each factor
            foreach (var fac in mFactorList.Keys)
            {
                outCols.Add(new MageColumnDef(fac, "text", "15"));
            }

            OnColumnDefAvailable(new MageColumnEventArgs(outCols.ToArray()));
        }
    }
}
