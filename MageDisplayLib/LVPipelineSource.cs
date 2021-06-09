using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// This is a pipeline module
    /// that can serve the contents of a ListDisplayControl to standard tabular output
    ///
    /// It is an adapter for making rows in a ListDisplayControl object
    /// available via Mage pipeline data source module connections
    /// </summary>
    public class LVPipelineSource : BaseModule
    {
        // Ignore Spelling: Mage

        #region Member Variables

        // Object whose data we are serving
        private readonly ListDisplayControl myListControl;

        private readonly List<MageColumnDef> mColumnDefs = new();

        private readonly List<List<string>> RowBuffer = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a LVPipelineSource object
        /// that can serve data rows from associated ListDisplayControl
        /// according to mode
        /// </summary>
        /// <param name="lc"></param>
        /// <param name="mode"></param>
        public LVPipelineSource(ListDisplayControl lc, DisplaySourceMode mode)
        {
            myListControl = lc;
            if (lc.List.Items.Count == 0)
            {
                throw new MageException("There are no items to process");
            }
            if (mode == DisplaySourceMode.Selected && lc.List.SelectedItems.Count == 0)
            {
                throw new MageException("There are no items selected to process");
            }
            GetColumnDefs();
            GetRowsFromList(mode);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Set the module to stop execution
        /// </summary>
        public bool Stop
        {
            get => Abort;
            set => Abort = value;
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// Output each row in associated ListViewDisplayList object
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            OutputListItems();
        }

        #endregion

        #region Private Functions

        private void GetColumnDefs()
        {
            foreach (var colDef in myListControl.ColumnDefs)
            {
                mColumnDefs.Add(colDef);
            }
        }

        private void GetRowsFromList(DisplaySourceMode mode)
        {
            switch (mode)
            {
                case DisplaySourceMode.All:
                    foreach (ListViewItem item in myListControl.List.Items)
                    {
                        var row = new List<string>();
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                        {
                            row.Add(subItem.Text);
                        }
                        RowBuffer.Add(row);
                    }
                    break;
                case DisplaySourceMode.Selected:
                    foreach (ListViewItem item in myListControl.List.SelectedItems)
                    {
                        var row = new List<string>();
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                        {
                            row.Add(subItem.Text);
                        }
                        RowBuffer.Add(row);
                    }
                    break;
            }
        }

        private void OutputListItems()
        {
            OnColumnDefAvailable(new MageColumnEventArgs(mColumnDefs.ToArray()));

            // Output the rows from the list control according to current mode setting
            foreach (var row in RowBuffer)
            {
                if (Abort)
                    break;
                OnDataRowAvailable(new MageDataEventArgs(row.ToArray()));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        #endregion

    }
}
