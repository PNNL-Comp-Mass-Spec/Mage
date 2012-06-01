using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.Windows.Forms;

namespace MageDisplayLib {

    /// <summary>
    /// this is a pipeline module 
    /// that can serve the contents of a GridViewDisplayControl to standard tabular output
    /// 
    /// It is an adapter for making rows in a GridViewDisplayControl object
    /// available via Mage pipeline data source module connections
    /// </summary>

    public class GVPipelineSource : BaseModule {

        #region Member Variables

        // Data grid view display whose data we are serving
        private GridViewDisplayControl myListControl = null;

        /// <summary>
        /// whether or not we are outputing all the rows
        /// in our associated display grid view or only
        /// the currently selected rows
        /// </summary>
        private DisplaySourceMode mInputMode = DisplaySourceMode.All;

        /// <summary>
        /// </summary>
        private List<MageColumnDef> mColumnDefs = new List<MageColumnDef>();

        /// <summary>
        /// internal buffer for cell contents from our associated GridViewDisplayControl
        /// </summary>
        private List<object[]> mRowBuffer = new List<object[]>();

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new GVPipelineSource object
        /// that will serve data rows from given GridViewDisplayControl
        /// </summary>
        /// <param name="gv">GVPipelineSource object</param>
        /// <param name="mode">selected or all</param>
        public GVPipelineSource(GridViewDisplayControl gv, DisplaySourceMode mode) {
            mInputMode = mode;
            Initialize(gv);
        }

        /// <summary>
        /// construct a new GVPipelineSource object
        /// that will serve data rows from given GridViewDisplayControl
        /// </summary>
        /// <param name="gv">GVPipelineSource object</param>
        /// <param name="mode">"Selected" or "All"</param>
        public GVPipelineSource(GridViewDisplayControl gv, string mode) {
            if (mode == "All") {
                mInputMode = DisplaySourceMode.All;
            } else {
                mInputMode = DisplaySourceMode.Selected;
            }
            Initialize(gv);
        }

        /// <summary>
        /// Get column definition and data rows into internal buffers
        /// so that this module can be run by non-UI thread
        /// </summary>
        private void Initialize(GridViewDisplayControl gv) {
            myListControl = gv;
            GetColumnDefs();
            GetRowsFromList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// set this module to stop executing
        /// </summary>
        public bool Stop { get { return Abort; } set { Abort = value; } }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// output each row in associated GridViewDisplayList object 
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state) {
            OutputListItems();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// return an array of objects from given DataGridView row cell contents
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static object[] GetOutputRowFromGridRow(DataGridViewRow row) {
            int n = row.Cells.Count;
            object[] vals = new object[n];
            for (int i = 0; i < n; i++) {
                vals[i] = row.Cells[i].Value;
            }
            return vals;
        }

        /// <summary>
        /// get Mage column definitions from the GridViewDisplay
        /// into our internal bufferes
        /// </summary>
        private void GetColumnDefs() {
            mColumnDefs = new List<MageColumnDef>(myListControl.ColumnDefs);
        }

        /// <summary>
        /// get data rows from the GridViewDisplay
        /// into our internal buffers
        /// </summary>
        private void GetRowsFromList() {
            switch (mInputMode) {
                case DisplaySourceMode.All:
                    DataGridViewRowCollection allRows = myListControl.List.Rows;
                    foreach (DataGridViewRow row in allRows) {
                        object[] vals = GetOutputRowFromGridRow(row);
                        mRowBuffer.Add(vals);
                    }
                    break;
                case DisplaySourceMode.Selected:
                    DataGridViewSelectedRowCollection selRows = myListControl.List.SelectedRows;
                    foreach (DataGridViewRow row in selRows) {
                        object[] vals = GetOutputRowFromGridRow(row);
                        mRowBuffer.Add(vals);
                    }
                    break;
            }
        }

        /// <summary>
        /// Deliver the contents of our internal buffers
        /// to standard tabular output
        /// </summary>
        private void OutputListItems() {
            OnColumnDefAvailable(new MageColumnEventArgs(mColumnDefs.ToArray()));

            // output the rows from the list control according to current mode setting
            foreach (object[] row in mRowBuffer) {
                if (Abort) break;
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        #endregion
    }

}
