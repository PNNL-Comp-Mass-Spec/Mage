using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// <para>
    /// This is a pipeline module
    /// that can serve the contents of a GridViewDisplayControl to standard tabular output
    ///</para>
    /// <para>
    /// It is an adapter for making rows in a GridViewDisplayControl object
    /// available via Mage pipeline data source module connections
    /// </para>
    /// </summary>
    public class GVPipelineSource : BaseModule
    {
        // Ignore Spelling: Mage

        // Data grid view display whose data we are serving
        private GridViewDisplayControl myListControl;

        /// <summary>
        /// Whether or not we are outputting all the rows
        /// in our associated display grid view or only
        /// the currently selected rows
        /// </summary>
        private readonly DisplaySourceMode mInputMode;

        /// <summary>
        /// Mage column definitions
        /// </summary>
        private List<MageColumnDef> mColumnDefs = new();

        /// <summary>
        /// Internal buffer for cell contents from our associated GridViewDisplayControl
        /// </summary>
        private readonly List<string[]> mRowBuffer = new();

        /// <summary>
        /// Construct a new GVPipelineSource object
        /// that will serve data rows from given GridViewDisplayControl
        /// </summary>
        /// <param name="gv">GVPipelineSource object</param>
        /// <param name="mode">selected or all</param>
        public GVPipelineSource(GridViewDisplayControl gv, DisplaySourceMode mode)
        {
            mInputMode = mode;
            Initialize(gv);
        }

        /// <summary>
        /// Construct a new GVPipelineSource object
        /// that will serve data rows from given GridViewDisplayControl
        /// </summary>
        /// <param name="gv">GVPipelineSource object</param>
        /// <param name="mode">"Selected" or "All"</param>
        public GVPipelineSource(GridViewDisplayControl gv, string mode)
        {
            if (mode == "All")
            {
                mInputMode = DisplaySourceMode.All;
            }
            else
            {
                mInputMode = DisplaySourceMode.Selected;
            }
            Initialize(gv);
        }

        /// <summary>
        /// Get column definition and data rows into internal buffers
        /// so that this module can be run by non-UI thread
        /// </summary>
        private void Initialize(GridViewDisplayControl gv)
        {
            myListControl = gv;
            GetColumnDefs();
            GetRowsFromList();
        }

        /// <summary>
        /// Set this module to stop executing
        /// </summary>
        public bool Stop
        {
            get => Abort;
            set => Abort = value;
        }

        /// <summary>
        /// Output each row in associated GridViewDisplayList object
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            OutputListItems();
        }

        /// <summary>
        /// Return an array of objects from given DataGridView row cell contents
        /// </summary>
        /// <param name="row"></param>
        private static string[] GetOutputRowFromGridRow(DataGridViewRow row)
        {
            var n = row.Cells.Count;
            var vals = new string[n];
            for (var i = 0; i < n; i++)
            {
                if (row.Cells[i].Value == null)
                    vals[i] = string.Empty;
                else
                    vals[i] = row.Cells[i].Value.ToString();
            }
            return vals;
        }

        /// <summary>
        /// Get Mage column definitions from the GridViewDisplay
        /// into our internal buffers
        /// </summary>
        private void GetColumnDefs()
        {
            if (myListControl.ColumnDefs == null)
                mColumnDefs = new List<MageColumnDef>();
            else
                mColumnDefs = new List<MageColumnDef>(myListControl.ColumnDefs);
        }

        /// <summary>
        /// Get data rows from the GridViewDisplay
        /// into our internal buffers
        /// </summary>
        private void GetRowsFromList()
        {
            switch (mInputMode)
            {
                case DisplaySourceMode.All:
                    var allRows = myListControl.List.Rows;
                    foreach (DataGridViewRow row in allRows)
                    {
                        var vals = GetOutputRowFromGridRow(row);
                        mRowBuffer.Add(vals);
                    }
                    break;
                case DisplaySourceMode.Selected:
                    var selRows = myListControl.List.SelectedRows;
                    foreach (DataGridViewRow row in selRows)
                    {
                        var vals = GetOutputRowFromGridRow(row);
                        mRowBuffer.Add(vals);
                    }
                    break;
            }
        }

        /// <summary>
        /// Deliver the contents of our internal buffers
        /// to standard tabular output
        /// </summary>
        private void OutputListItems()
        {
            OnColumnDefAvailable(new MageColumnEventArgs(mColumnDefs.ToArray()));

            // output the rows from the list control according to current mode setting
            foreach (var row in mRowBuffer)
            {
                if (Abort)
                    break;
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }
    }
}
