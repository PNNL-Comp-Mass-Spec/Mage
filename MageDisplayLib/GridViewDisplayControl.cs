using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    public partial class GridViewDisplayControl : UserControl, IMageDisplayControl, ISinkModule, IModuleParameters
    {
        // Ignore Spelling: checkbox, Ctrl, deselecting, deselects, Mage, subitems

        /// <summary>
        /// Signals anyone interested that row selection has changed
        /// </summary>
        public event EventHandler<EventArgs> SelectionChanged;

        private readonly Dictionary<string, string> mParameters = new();

        #region IModuleParameters Members

        /// <summary>
        /// Gets settings for this panel
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters()
        {
            mParameters["ShiftClickMode"] = chkShiftClickMode.Checked.ToString();
            return mParameters;
        }

        /// <summary>
        /// Sets settings for this panel
        /// </summary>
        /// <returns></returns>
        public void SetParameters(Dictionary<string, string> paramList)
        {
            if (paramList.TryGetValue("ShiftClickMode", out var value))
            {
                if (bool.TryParse(value, out var isChecked))
                    chkShiftClickMode.Checked = isChecked;
            }
        }

        #endregion

        #region Member Variables

        /// <summary>
        /// Stash the Mage column defs we received on STI
        /// in case we need to output them later
        /// </summary>
        private Collection<MageColumnDef> mColumnDefs;

        private bool mAllowDisableShiftClickMode = true;

        #endregion

        #region Properties

        /// <summary>
        /// Defines whether or not checkbox "Use Shift+Click, Ctrl+Click" is visible
        /// </summary>
        public bool AllowDisableShiftClickMode
        {
            get => mAllowDisableShiftClickMode;
            set
            {
                mAllowDisableShiftClickMode = value;
                if (value)
                {
                    chkShiftClickMode.Visible = true;
                }
                else
                {
                    chkShiftClickMode.Visible = false;
                    chkShiftClickMode.Checked = true;
                }
            }
        }

        /// <summary>
        /// Set to True to automatically set column widths after populating the grid
        /// </summary>
        public bool AutoSizeColumnWidths { get; set; }

        /// <summary>
        /// Defines whether or not multiple items can be selected in the data grid view
        /// </summary>
        public bool MultiSelect
        {
            get => gvQueryResults.MultiSelect;
            set
            {
                gvQueryResults.MultiSelect = value;
                if (value)
                {
                    chkShiftClickMode.Visible = mAllowDisableShiftClickMode;
                }
                else
                {
                    AllowDisableShiftClickMode = false;
                }
            }
        }

        /// <summary>
        /// Supplemental information about rows in list display (typically number of rows)
        /// </summary>
        public string Notice
        {
            get => lblNotice.Text;
            set => lblNotice.Text = value;
        }

        /// <summary>
        /// Title field in header above list display rows
        /// </summary>
        public string PageTitle
        {
            get => lblPageTitle.Text;
            set => lblPageTitle.Text = value;
        }

        /// <summary>
        /// Show or hide the header panel
        /// </summary>
        public bool HeaderVisible
        {
            get => panel1.Visible;
            set => panel1.Visible = value;
        }

        /// <summary>
        /// Get reference to internal DataGridView control
        /// </summary>
        public MyDataGridView List => gvQueryResults;

        /// <summary>
        /// Number of items currently in display
        /// </summary>
        public int ItemCount => gvQueryResults.Rows.Count;

        /// <summary>
        /// Number of selected items currently in display
        /// </summary>
        public int SelectedItemCount => gvQueryResults.SelectedRows.Count;

        /// <summary>
        /// Get the complete list of the selected rows
        /// </summary>
        /// <remarks>Returns the values for each row as a list of strings</remarks>
        public List<List<string>> SelectedItemRows
        {
            get
            {
                var lstSelectedRows = new List<List<string>>();

                gvQueryResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                foreach (DataGridViewRow item in gvQueryResults.SelectedRows)
                {
                    var rowValues = new List<string>();

                    for (var i = 0; i < item.Cells.Count; i++)
                    {
                        rowValues.Add(item.Cells[i].Value.ToString());
                    }
                    lstSelectedRows.Add(rowValues);
                }

                return lstSelectedRows;
            }
        }

        /// <summary>
        /// Get the complete list of the selected rows
        /// </summary>
        /// <remarks>Returns the values for each row as a dictionary where keys are the column header and values are the value</remarks>
        public List<Dictionary<string, string>> SelectedItemRowsDictionaryList
        {
            get
            {
                var lstSelectedRows = new List<Dictionary<string, string>>();

                gvQueryResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                foreach (DataGridViewRow item in gvQueryResults.SelectedRows)
                {
                    var rowValues = new Dictionary<string, string>();

                    for (var i = 0; i < ColumnDefs.Count; i++)
                    {
                        var colName = ColumnDefs[i].Name;
                        var fieldVal = item.Cells[i].Value.ToString();
                        rowValues.Add(colName, fieldVal);
                    }
                    lstSelectedRows.Add(rowValues);
                }

                return lstSelectedRows;
            }
        }

        /// <summary>
        /// Get contents of first selected row as key/value pairs
        /// where key is column name and value is contents of column
        /// </summary>
        public Dictionary<string, string> SelectedItemFields
        {
            get
            {
                var fields = new Dictionary<string, string>();
                if (gvQueryResults.SelectedRows.Count > 0)
                {
                    var item = gvQueryResults.SelectedRows[0];
                    for (var i = 0; i < ColumnDefs.Count; i++)
                    {
                        var colName = ColumnDefs[i].Name;
                        var fieldVal = item.Cells[i].Value.ToString();
                        fields.Add(colName, fieldVal);
                    }
                }
                return fields;
            }
        }

        /// <summary>
        /// Get collection of column names
        /// </summary>
        public Collection<string> ColumnNames
        {
            get
            {
                var names = new Collection<string>();
                foreach (var colDef in ColumnDefs)
                {
                    names.Add(colDef.Name);
                }
                return names;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new GridViewDisplayControl object
        /// </summary>
        public GridViewDisplayControl()
        {
            InitializeComponent();

            ItemBlockSize = 100;

            SetupContextMenus(gvQueryResults);

            gvQueryResults.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            gvQueryResults.RowHeadersVisible = false;
            gvQueryResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // gvQueryResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            // gvQueryResults.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            // gvQueryResults.GridColor = Color.Black;
            gvQueryResults.AllowUserToAddRows = false;
            gvQueryResults.AllowUserToDeleteRows = false;
            gvQueryResults.AllowUserToResizeColumns = true;
            gvQueryResults.ReadOnly = true;

            AllowDisableShiftClickMode = true;
            gvQueryResults.ShiftClickSelect = true;
            MultiSelect = true;

            // Get and Set the Current Cell
            // gvQueryResults.CurrentCell.RowIndex;
            // gvQueryResults.CurrentCell.ColumnIndex;
            // gvQueryResults.CurrentCell = this.gvQueryResults[1,0]; // column 1, Row 0.

        }

        #endregion

        #region Functions for inter-thread UI updating

        /// <summary>
        /// Delegates for inter-thread access to DataGridView control
        /// </summary>
        private delegate void DisplayColumnCallback(Collection<MageColumnDef> colDefs);
        private delegate void DisplayRowBlockCallback(Collection<string[]> rows);

        /// <summary>
        /// Set up columns for DataGridView control
        /// </summary>
        /// <param name="colDefs"></param>
        private void HandleDisplayColumnSetup(Collection<MageColumnDef> colDefs)
        {
            gvQueryResults.ColumnCount = colDefs.Count;
            for (var i = 0; i < colDefs.Count; i++)
            {
                gvQueryResults.Columns[i].Name = colDefs[i].Name;
            }
        }

        /// <summary>
        /// Add a block of rows to DataGridView control
        /// </summary>
        /// <param name="rows"></param>
        private void HandleDisplayRowBlock(Collection<string[]> rows)
        {
            if (rows != null)
            {
                if (gvQueryResults.Columns.Count > 0)
                {
                    foreach (var currentRow in rows)
                    {
                        gvQueryResults.Rows.Add(currentRow);
                    }
                    UpdateNoticeFld(".");
                }
                else
                {
                    UpdateNoticeFld("Cannot add row: no columns are defined");
                }
            }
            else
            {
                UpdateNoticeFld("");
                if (AutoSizeColumnWidths)
                    AutoSizeColumnWidthsNow();
            }
        }

        /// <summary>
        /// Called as target of invoke operation to
        /// actually update the notice filed
        /// </summary>
        /// <param name="text"></param>
        private void UpdateNoticeFld(string text)
        {
            if (text != null && text == ".")
            {
                if (lblNotice.Text.Length > 64)
                {
                    lblNotice.Text = lblNotice.Text.Replace(".", "");
                }
                lblNotice.Text = "." + lblNotice.Text;
                lblNotice.Update();
            }
            else
            {
                var strStatus = gvQueryResults.Rows.Count + " row";
                if (gvQueryResults.Rows.Count != 1)
                {
                    strStatus += "s";
                }
                if (!string.IsNullOrEmpty(text))
                {
                    strStatus = text + "; " + strStatus;
                }
                lblNotice.Text = strStatus;
            }
        }

        #endregion

        #region List Maintenence

        /// <summary>
        /// Empty the current display contents
        /// </summary>
        public void Clear()
        {
            gvQueryResults.Rows.Clear();
            gvQueryResults.Columns.Clear();
            gvQueryResults.Update();
            lblNotice.Text = string.Empty;
        }

        /// <summary>
        /// De-selects all of the rows in the display
        /// </summary>
        public void ClearSelection()
        {
            gvQueryResults.ClearSelection();
        }

        /// <summary>
        /// Select all rows in display
        /// </summary>
        public void SelectAllRows()
        {
            gvQueryResults.SelectAll();
        }

        /// <summary>
        /// Remove the currently selected items from the display list
        /// </summary>
        public void DeleteSelectedItems()
        {
            gvQueryResults.DeleteSelectedItems();
            UpdateNoticeFieldWithRowInfo();
        }

        /// <summary>
        /// Remove the items from the display list that are currently not selected
        /// </summary>
        public void DeleteNotSelectedItems()
        {
            gvQueryResults.DeleteNotSelectedItems();
            UpdateNoticeFieldWithRowInfo();
        }

        #endregion

        #region Control Handlers

        // Note that a right click context menu provides for Select All, Select None, copy selected rows, etc.
        // See file GridViewDisplayControlActions.cs

        // Toggles support for Shift+Click selection mode in gvQueryResults
        private void chkShiftClickSelect_CheckedChanged(object sender, EventArgs e)
        {
            gvQueryResults.ShiftClickSelect = chkShiftClickMode.Checked;
        }

        /// <summary>
        /// Update notice when number of selected rows changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvQueryResults_SelectionChanged(object sender, EventArgs e)
        {
            UpdateNoticeFieldWithRowInfo();
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Update notice field with number of rows and selection
        /// </summary>
        private void UpdateNoticeFieldWithRowInfo()
        {
            var s = (SelectedItemCount > 0) ? SelectedItemCount.ToString() + "/" : "";
            var i = ItemCount.ToString();
            var r = (ItemCount == 1) ? " row" : " rows";
            lblNotice.Text = s + i + r;
        }

        #endregion

        //--------------------------------------------------------
        // Mage standard tabular input (ISinkModule interface)
        //--------------------------------------------------------

        #region STI Member Variables

        /// <summary>
        /// Internal buffer to accumulate data rows from standard tabular input
        /// </summary>
        private readonly Collection<string[]> mRowBuffer = new();

        #endregion

        #region STI Properties

        /// <summary>
        /// Number of data rows in an item block
        /// </summary>
        public int ItemBlockSize { get; set; }

        #endregion

        #region STI ISinkModule Members

        /// <summary>
        /// Get Mage column definitions
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs => mColumnDefs;

        /// <summary>
        /// Handle a column definition event on standard tabular input stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mColumnDefs = new Collection<MageColumnDef>(args.ColumnDefs);
            DisplayColumnCallback csd = HandleDisplayColumnSetup;
            Invoke(csd, args.ColumnDefs);
        }

        /// <summary>
        /// Handle a data row received event on standard tabular input stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            var endOfData = !args.DataAvailable;
            if (args.DataAvailable)
            {
                mRowBuffer.Add(args.Fields);
            }
            if (mRowBuffer.Count == ItemBlockSize || endOfData)
            {
                DisplayRowBlockCallback drc = HandleDisplayRowBlock;
                Invoke(drc, mRowBuffer);
                mRowBuffer.Clear();
            }
            if (endOfData)
            {
                DisplayRowBlockCallback drc = HandleDisplayRowBlock;
                Invoke(drc, new object[] { null });
            }
        }

        #endregion

        #region Compatibility with ListDisplayControl

        /// <summary>
        /// Return suitable ISinkModule reference to this object
        /// </summary>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink()
        {
            const int BLOCK_SIZE = 10;
            return MakeSink(BLOCK_SIZE);
        }

        /// <summary>
        /// Return suitable ISinkModule reference to this object
        /// </summary>
        /// <param name="title"></param>
        /// <param name="blkSz"></param>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink(string title, int blkSz)
        {
            PageTitle = title;
            return MakeSink(blkSz);
        }

        /// <summary>
        /// Return suitable ISinkModule reference to this object
        /// </summary>
        /// <param name="blkSz"></param>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink(int blkSz)
        {
            Clear();
            return this;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Automatically resize the column widths
        /// </summary>
        public void AutoSizeColumnWidthsNow()
        {
            foreach (DataGridViewColumn item in gvQueryResults.Columns)
            {
                item.DataGridView.AutoResizeColumns();
            }
        }

        /// <summary>
        /// Move the first currently selected item up or down in the list
        /// </summary>
        /// <param name="moveUp"></param>
        public void MoveListItem(bool moveUp)
        {
            string cache;
            var lv = gvQueryResults;

            var selIdx = lv.SelectedRows[0].Index;
            if (moveUp)
            {
                // Ignore move up of row(0)
                if (selIdx == 0)
                    return;

                // Move the subitems for the previous row
                // to cache to make room for the selected row
                for (var i = 0; i < lv.Rows[selIdx].Cells.Count; i++)
                {
                    cache = lv.Rows[selIdx - 1].Cells[i].Value.ToString();
                    lv.Rows[selIdx - 1].Cells[i].Value =
                      lv.Rows[selIdx].Cells[i].Value.ToString();
                    lv.Rows[selIdx].Cells[i].Value = cache;
                }
                lv.Rows[selIdx - 1].Selected = true;
                lv.Rows[selIdx].Selected = false;
            }
            else
            {
                // Ignore move down of last item
                if (selIdx == lv.Rows.Count - 1)
                    return;

                // Move the Cells for the next row
                // to cache so we can move the selected row down
                for (var i = 0; i < lv.Rows[selIdx].Cells.Count; i++)
                {
                    cache = lv.Rows[selIdx + 1].Cells[i].Value.ToString();
                    lv.Rows[selIdx + 1].Cells[i].Value =
                      lv.Rows[selIdx].Cells[i].Value.ToString();
                    lv.Rows[selIdx].Cells[i].Value = cache;
                }
                lv.Rows[selIdx + 1].Selected = true;
                lv.Rows[selIdx].Selected = false;
            }
            lv.Refresh();
            lv.Focus();
        }

        #endregion

        /// <summary>
        /// Displays data in a customizable grid
        /// Rows can be selected/deselected by clicking on the row; Ctrl+Click is not required (or supported)
        /// </summary>
        public class MyDataGridView : DataGridView
        {
            private readonly Hashtable myHashTable = new();

            /// <summary>
            /// Defines whether or not user can delete rows using the right-click menu
            /// </summary>
            public bool AllowDelete { get; set; } = true;

            /// <summary>
            /// Toggles whether or not Shift+Click is supported
            /// When Shift+Click is supported, then persistence of row selection is disabled
            /// </summary>
            public bool ShiftClickSelect { get; set; }

            /// <summary>
            /// True when SuspendLayout() is en effect for this data grid
            /// </summary>
            public bool LayoutSuspended { get; private set; }

            /// <summary>
            /// Clears the current selection by unselecting all selected cells.
            /// </summary>
            public new void ClearSelection()
            {
                base.ClearSelection();

                myHashTable.Clear();
            }

            /// <summary>
            /// Remove the items from the display list that are currently not selected
            /// </summary>
            public void DeleteNotSelectedItems()
            {
                if (AllowDelete)
                {
                    var toRemove = new List<DataGridViewRow>();

                    SuspendLayout();
                    LayoutSuspended = true;

                    foreach (DataGridViewRow item in Rows)
                    {
                        if (!item.Selected && !item.IsNewRow)
                        {
                            toRemove.Add(item);
                        }
                    }
                    foreach (var item in toRemove)
                    {
                        Rows.Remove(item);
                    }

                    LayoutSuspended = false;
                    ResumeLayout();

                    Update();
                }
            }

            /// <summary>
            /// Remove the currently selected items from the display list
            /// </summary>
            public void DeleteSelectedItems()
            {
                if (SelectedRows.Count > 0 && AllowDelete)
                {
                    SuspendLayout();
                    LayoutSuspended = true;

                    var toRemove = new List<DataGridViewRow>(SelectedRows.Count);

                    // Cache the rows that are selected
                    foreach (DataGridViewRow item in SelectedRows)
                    {
                        toRemove.Add(item);
                    }

                    // Remove each row
                    foreach (var item in toRemove)
                    {
                        Rows.Remove(item);
                    }

                    LayoutSuspended = false;
                    ResumeLayout();

                    Update();
                }
            }

            /// <summary>
            /// Highlights the rows that are tracked in myHashTable
            /// </summary>
            protected void HighlightSelectedRows()
            {
                // Select all rows clicked
                foreach (DataGridViewRow SelectedRow in myHashTable.Values)
                {
                    SelectedRow.Selected = true;
                }
            }

            /// <summary>
            /// Selects all the cells in the System.Windows.Forms.DataGridView.
            /// </summary>
            public new void SelectAll()
            {
                if (MultiSelect)
                {
                    SuspendLayout();

                    base.SelectAll();

                    myHashTable.Clear();

                    foreach (DataGridViewRow SelectedRow in Rows)
                    {
                        myHashTable.Add(SelectedRow.Index.ToString(), SelectedRow);
                    }

                    HighlightSelectedRows();

                    ResumeLayout();
                }
            }

            /// <summary>
            /// Updates myHashTable to track the currently selected rows
            /// </summary>
            protected void SynchronizeHashTableWithSelectedRows()
            {
                myHashTable.Clear();

                foreach (DataGridViewRow SelectedRow in SelectedRows)
                {
                    myHashTable.Add(SelectedRow.Index.ToString(), SelectedRow);
                }
            }

            /// <summary>
            /// Initializes a new instance of the MageDisplayLib.MyDataGridView class.
            /// </summary>
            public MyDataGridView()
            {
                AllowUserToAddRows = false;
                AllowUserToDeleteRows = false;
                AllowUserToResizeColumns = true;
                AllowUserToOrderColumns = false;
                ReadOnly = true;
            }

            /// <summary>
            /// Raises the System.Windows.Forms.DataGridView.RowsAdded event
            /// Updates myHashTable with the currently selected rows
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
            {
                base.OnRowsAdded(e);
                SynchronizeHashTableWithSelectedRows();
            }

            /// <summary>
            /// Raises the System.Windows.Forms.DataGridView.RowsRemoved event.
            /// Updates myHashTable with the currently selected rows
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
            {
                base.OnRowsRemoved(e);

                // Make sure myHashTable is up-to-date
                SynchronizeHashTableWithSelectedRows();
            }

            /// <summary>
            /// Selects/deselects the row in which the mouse was clicked
            /// </summary>
            /// <param name="e"></param>
            protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
            {
                if (ShiftClickSelect || !MultiSelect || e.Button != MouseButtons.Left)
                {
                    // Use the default behavior for click, Shift+Click, Ctrl+Click, or click of middle or right button.
                    base.OnCellMouseDown(e);
                    SynchronizeHashTableWithSelectedRows();
                }
                else
                {
                    if ((e.RowIndex >= 0) && (e.RowIndex < Rows.Count) && (e.ColumnIndex >= 0) && (e.ColumnIndex < Columns.Count))
                    {
                        // A cell was clicked
                        // Do not call the base class .OnCellMouseDown() event

                        var s = e.RowIndex.ToString();

                        if (myHashTable.ContainsKey(s))
                        {
                            // If the row is in the hashtable, remove it
                            myHashTable.Remove(s);
                            Rows[e.RowIndex].Selected = false;
                        }
                        else
                        {
                            // Otherwise, insert it into the hashtable
                            Rows[e.RowIndex].Selected = false;

                            myHashTable.Add(s, Rows[e.RowIndex]);
                        }

                        SuspendLayout();
                        HighlightSelectedRows();
                        ResumeLayout();
                    }
                    else
                    {
                        // User likely clicked on the header row
                        // Call the base-class method
                        base.OnCellMouseDown(e);
                    }
                }
            }
        }
    }
}
