using System.Collections.Generic;
using Mage;

namespace MageUnitTests
{

    /// <summary>
    /// Test harness for getting access to BaseModule stuff
    /// </summary>
    public class TestModule : BaseModule
    {

        #region Member Variables

        // An internal buffer for accumulating rows passed in via the standard tabular input handler
        protected readonly List<string[]> SavedRows = new List<string[]>();
        protected readonly List<string[]> SavedMappedRows = new List<string[]>();

        #endregion

        #region Properties

        /// <summary>
        /// This property name is used by Unit Test SetModuleParameterTest
        /// </summary>
        public string Dummy { get; set; }

        // Get the column definitions
        public List<MageColumnDef> Columns => InputColumnDefs;

        // Get rows that were accumulated in the internal row buffer
        public List<string[]> Rows => SavedRows;

        public List<string[]> MappedRows => SavedMappedRows;

        public List<MageColumnDef> OutColDefs => OutputColumnDefs;

        public Dictionary<string, int> OutColPos => OutputColumnPos;

        public List<KeyValuePair<int, int>> OutToInPosMap => OutputToInputColumnPosMap;

        // public List<KeyValuePair<string, string>> InternalParameterList { get { return parameters; } }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                SavedRows.Add(args.Fields);
                if (!string.IsNullOrEmpty(OutputColumnList))
                {
                    SavedMappedRows.Add(MapDataRow(args.Fields));
                }
            }
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
        }

        #endregion


    }
}
