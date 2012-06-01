using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;


namespace MageDisplayLib {

    /// <summary>
    /// this is a pipeline module 
    /// that can serve the contents of a ListDisplayControl to standard tabular output
    /// 
    /// It is an adapter for making rows in a ListDisplayControl object
    /// available via Mage pipeline data source module connections
    /// </summary>
    public class LVPipelineSource : BaseModule {

        #region Member Variables

        // object whose data we are serving
        private ListDisplayControl myListControl = null;

        private List<MageColumnDef> mColumnDefs = new List<MageColumnDef>();

        private List<List<string>> RowBuffer = new List<List<string>>();

        #endregion

        #region Constructors

        /// <summary>
        /// construct a LVPipelineSource object 
        /// that can serve data rows from associated ListDisplayControl
        /// according to mode
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="mode"></param>
        public LVPipelineSource(ListDisplayControl lc, DisplaySourceMode mode) {
            myListControl = lc;
            if (lc.List.Items.Count == 0) {
                throw new MageException("There are no items to process");
            }
            if (mode == DisplaySourceMode.Selected && lc.List.SelectedItems.Count == 0) {
                throw new MageException("There are no items selected to process");
            }
            GetColumnDefs();
            GetRowsFromList(mode);
        }


        #endregion

        #region Properties

        /// <summary>
        /// set the module to stop execution
        /// </summary>
        public bool Stop { get { return Abort; } set { Abort = value; } }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// output each row in associated ListViewDisplayList object 
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state) {
            OutputListItems();
        }

        #endregion

        #region Private Functions

        private void GetColumnDefs() {
            foreach (MageColumnDef colDef in myListControl.ColumnDefs) {
                mColumnDefs.Add(colDef);
            }
        }

        private void GetRowsFromList(DisplaySourceMode mode) {
            switch (mode) {
                case DisplaySourceMode.All:
                    foreach (ListViewItem item in myListControl.List.Items) {
                        List<string> row = new List<string>();
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems) {
                            row.Add(subItem.Text);
                        }
                        RowBuffer.Add(row);
                    }
                    break;
                case DisplaySourceMode.Selected:
                    foreach (ListViewItem item in myListControl.List.SelectedItems) {
                        List<string> row = new List<string>();
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems) {
                            row.Add(subItem.Text);
                        }
                        RowBuffer.Add(row);
                    }
                    break;
            }
        }

        private void OutputListItems() {
            OnColumnDefAvailable(new MageColumnEventArgs(mColumnDefs.ToArray()));

            // output the rows from the list control according to current mode setting
            foreach (List<string> row in RowBuffer) {
                if (Abort) break;
                OnDataRowAvailable(new MageDataEventArgs(row.ToArray()));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        #endregion

    }
}
