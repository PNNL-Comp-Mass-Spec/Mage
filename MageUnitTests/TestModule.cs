using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace MageUnitTests {

    // Test mule for getting access to BaseModule stuff
    public class TestModule : BaseModule {

        #region Member Variables

        // An internal buffer for accumulating rows passed in via the standard tabular input handler
		protected List<string[]> SavedRows = new List<string[]>();
		protected List<string[]> SavedMappedRows = new List<string[]>();


        #endregion

        #region Properties

        public string Dummy { get; set; }

        // Get the column definitions
        public List<MageColumnDef> Columns { get { return InputColumnDefs; } }

        // Get rows that were accumumlated in the internal row buffer
		public List<string[]> Rows { get { return SavedRows; } }
		public List<string[]> MappedRows { get { return SavedMappedRows; } }

        public List<MageColumnDef> OutColDefs { get { return base.OutputColumnDefs; } }

        public Dictionary<string, int> OutColPos { get { return base.OutputColumnPos; } }

        public List<KeyValuePair<int, int>> OutToInPosMap { get { return OutputToInputColumnPosMap; } }

        //        public List<KeyValuePair<string, string>> InternalParameterList { get { return parameters; } }

        #endregion

        #region Constructors

        public TestModule() {
        }


        #endregion

        #region IBaseModule Members

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare() {
            // nothing to do here
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                SavedRows.Add(args.Fields);
                if (!string.IsNullOrEmpty(OutputColumnList)) {
                    SavedMappedRows.Add(MapDataRow(args.Fields));
                }
            }
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
        }

        #endregion


    }
}
