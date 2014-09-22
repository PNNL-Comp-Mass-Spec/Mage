using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using System.Collections.ObjectModel;

namespace MageDisplayLib {

    public partial class GridViewDisplayControl : UserControl, IMageDisplayControl, ISinkModule, IModuleParameters {

        /// <summary>
        /// Signals anyone interested that row selection has changed
        /// </summary>
        public event EventHandler<EventArgs> SelectionChanged;

        Dictionary<string, string> mParameters = new Dictionary<string, string>();

        #region IModuleParameters Members

        /// <summary>
        /// Gets settings for this panel
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters() {
            mParameters["ShiftClickMode"] = chkShiftClickMode.Checked.ToString();
            return mParameters;
        }

        /// <summary>
        /// Sets settings for this panel
        /// </summary>
        /// <returns></returns>
        public void SetParameters(Dictionary<string, string> paramList) {
            bool bChecked;
            string sValue;

            if (paramList.TryGetValue("ShiftClickMode", out sValue)) {
                if (bool.TryParse(paramList["ShiftClickMode"], out bChecked))
                     chkShiftClickMode.Checked = bChecked;
            }
        }


		#endregion

		#region Member Variables

		/// <summary>
		/// stash the Mage column defs we received on STI
		/// in case we need to output them later
		/// </summary>
		private Collection<MageColumnDef> mColumnDefs = null;

		private bool mAllowDisableShiftClickMode = true;

        #endregion
		
        #region Properties

		/// <summary>
		/// Defines whether or not checkbox "Use Shift+Click, Ctrl+Click" is visible
		/// </summary>
		public bool AllowDisableShiftClickMode {
			get {
				return mAllowDisableShiftClickMode;
			}
			set {
				mAllowDisableShiftClickMode = value;
				if (value) {
					chkShiftClickMode.Visible = true;
				} else {
					chkShiftClickMode.Visible = false;
					chkShiftClickMode.Checked = true;
				}
			}
		}

        /// <summary>
        /// Defines whether or not multiple items can be selected in the data grid view 
        /// </summary>
        public bool MultiSelect {
            get { 
                return gvQueryResults.MultiSelect; 
            }
            set { 
                gvQueryResults.MultiSelect = value; 
                if (value) {
					chkShiftClickMode.Visible = mAllowDisableShiftClickMode;
                } else {
					this.AllowDisableShiftClickMode = false;
                }
            }
        }

        /// <summary>
        /// Supplemental information about rows in list display (typicall number of rows)
        /// </summary>
        public string Notice { get { return this.lblNotice.Text; } set { this.lblNotice.Text = value; } }

        /// <summary>
        /// Title field in header above list display rows
        /// </summary>
        public string PageTitle {
            get { return this.lblPageTitle.Text; }
            set { this.lblPageTitle.Text = value; }
        }

        /// <summary>
        /// Show or hide the header panel
        /// </summary>
        public bool HeaderVisible { get { return panel1.Visible; } set { panel1.Visible = value; } }

        /// <summary>
        /// get reference to internal DataGridView control
        /// </summary>
        public MyDataGridView List { get { return gvQueryResults; } }

        /// <summary>
        /// Number of items currently in display
        /// </summary>
        public int ItemCount {
            get {
                return gvQueryResults.Rows.Count;
            }
        }

        /// <summary>
        /// Number of selected items currently in display
        /// </summary>
        public int SelectedItemCount {
            get {
                return gvQueryResults.SelectedRows.Count;
            }
        }

        /// <summary>
        /// Get contents of first selected row as key/value pairs
        /// where key is column name and value is contents of column
        /// </summary>
        public Dictionary<string, string> SeletedItemFields {
            get {
                Dictionary<string, string> fields = new Dictionary<string, string>();
                if (gvQueryResults.SelectedRows.Count > 0) {
                    DataGridViewRow item = gvQueryResults.SelectedRows[0];
                    for (int i = 0; i < ColumnDefs.Count; i++) {
                        string colName = ColumnDefs[i].Name;
                        string fieldVal = item.Cells[i].Value.ToString();
                        fields.Add(colName, fieldVal);
                    }
                }
                return fields;
            }
        }

        /// <summary>
        /// Get collection of columnn names
        /// </summary>
        public Collection<string> ColumnNames {
            get {
                Collection<string> names = new Collection<string>();
                foreach (MageColumnDef colDef in ColumnDefs) {
                    names.Add(colDef.Name);
                }
                return names;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new GridViewDisplayControl object
        /// </summary>
        public GridViewDisplayControl() {
            InitializeComponent();

            ItemBlockSize = 100;

            SetupContextMenus(gvQueryResults);

            gvQueryResults.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            gvQueryResults.RowHeadersVisible = false;
            gvQueryResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //gvQueryResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            //gvQueryResults.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            //gvQueryResults.GridColor = Color.Black;
            gvQueryResults.AllowUserToAddRows = false;
            gvQueryResults.AllowUserToDeleteRows = false;
            gvQueryResults.AllowUserToResizeColumns = true;
            gvQueryResults.ReadOnly = true;

			this.AllowDisableShiftClickMode = true;
			this.MultiSelect = true;

            //Get and Set the Current Cell 
            ////gvQueryResults.CurrentCell.RowIndex;
            ////gvQueryResults.CurrentCell.ColumnIndex;
            ////gvQueryResults.CurrentCell = this.gvQueryResults[1,0]; // column 1, Row 0. 

        }

        #endregion

        #region Functions for inter-thread UI updating

        /// <summary>
        /// delegates for inter-thread access to DataGridView control
        /// </summary>
        private delegate void DisplayColumnCallback(Collection<MageColumnDef> colDefs);
		private delegate void DisplayRowBlockCallback(Collection<string[]> rows);

        /// <summary>
        /// Set up columns for DataGridView control
        /// </summary>
        /// <param name="colDefs"></param>
        private void HandleDisplayColumnSetup(Collection<MageColumnDef> colDefs) {
            gvQueryResults.ColumnCount = colDefs.Count;
            for (int i = 0; i < colDefs.Count; i++) {
                gvQueryResults.Columns[i].Name = colDefs[i].Name;
            }
        }

        /// <summary>
        /// Add a block of rows to DataGridView control
        /// </summary>
        /// <param name="rows"></param>
		private void HandleDisplayRowBlock(Collection<string[]> rows)
		{
            if (rows != null) {
                if (gvQueryResults.Columns.Count > 0) {
					foreach (string[] row in rows)
					{
                        gvQueryResults.Rows.Add(row);
                    }
                    UpdateNoticeFld(".");
                } else {
                    UpdateNoticeFld("Cannot add row: no columns are defined");
                }
            } else {
                UpdateNoticeFld("");
            }
        }

        /// <summary>
        /// Called as target of invoke operation to 
        /// actually update the notice filed
        /// </summary>
        /// <param name="text"></param>
        private void UpdateNoticeFld(string text) {
            if (text != null && text == ".") {
                if (lblNotice.Text.Length > 64) {
                    lblNotice.Text = lblNotice.Text.Replace(".", "");
                }
                lblNotice.Text = "." + lblNotice.Text;
                lblNotice.Update();
            } else {
                string strStatus;
                strStatus = gvQueryResults.Rows.Count.ToString() + " row";
                if (gvQueryResults.Rows.Count != 1) {
                    strStatus += "s";
                }
                if (text != null && text.Length > 0) {
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
        public void Clear() {
            gvQueryResults.Rows.Clear();
            gvQueryResults.Columns.Clear();
            gvQueryResults.Update();
            lblNotice.Text = "";
        }

        /// <summary>
        /// De-selects all of the rows in the display
        /// </summary>
        public void ClearSelection() {
            gvQueryResults.ClearSelection();
        }

        /// <summary>
        /// Select all rows in display
        /// </summary>
        public void SelectAllRows() {
            gvQueryResults.SelectAll();
        }

        /// <summary>
        /// Remove the currently selected items from the display list
        /// </summary>
        public void DeleteSelectedItems() {
            gvQueryResults.DeleteSelectedItems();
            UpdateNoticeFieldWithRowInfo();
        }

        /// <summary>
        /// Remove the items from the display list that are currently not selected
        /// </summary>
        public void DeleteNotSelectedItems() {
            gvQueryResults.DeleteNotSelectedItems();           
            UpdateNoticeFieldWithRowInfo();
        }

        #endregion

        #region Control Handlers

        // Note that a right click context menu provides for Select All, Select None, copy selected rows, etc.
        // See file GridViewDisplayControlActions.cs

        // Toggles support for Shift+Click selection mode in gvQueryResults
        private void chkShiftClickSelect_CheckedChanged(object sender, EventArgs e) {
            gvQueryResults.ShiftClickSelect = chkShiftClickMode.Checked;
        }

        /// <summary>
        /// Update notice when number of selected rows changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvQueryResults_SelectionChanged(object sender, EventArgs e) {
            UpdateNoticeFieldWithRowInfo();
            if (SelectionChanged != null) {
                SelectionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Update notice field with number of rows and selection
        /// </summary>
        private void UpdateNoticeFieldWithRowInfo() {
            string s = (SelectedItemCount > 0) ? SelectedItemCount.ToString() + "/" : "";
            string i = ItemCount.ToString();
            string r = (ItemCount == 1) ? " row" : " rows";
            lblNotice.Text = s + i + r;
        }

        #endregion

        //--------------------------------------------------------
        // Mage standard tabular input (ISinkModule interface)
        //--------------------------------------------------------

        #region STI Member Variables

        /// <summary>
        /// internal buffer to accumulate data rows from standard tabular input
        /// </summary>
		private Collection<string[]> mRowBuffer = new Collection<string[]>();


        #endregion

        #region STI Properties

        /// <summary>
        /// number of data rows in an item block
        /// </summary>
        public int ItemBlockSize { get; set; }

        #endregion

        #region STI ISinkModule Members

        /// <summary>
        /// Get Mage column definitions
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs { get { return mColumnDefs; } }

        /// <summary>
        /// Handle a column definition event on standard tabular input stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args) {
            mColumnDefs = new Collection<MageColumnDef>(args.ColumnDefs);
            DisplayColumnCallback csd = HandleDisplayColumnSetup;
            Invoke(csd, new object[] { args.ColumnDefs });
        }

        /// <summary>
        /// Handle a data row received event on standard tabular input stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args) {
            bool endOfData = !args.DataAvailable;
            if (args.DataAvailable) {
                mRowBuffer.Add(args.Fields);
            }
            if (mRowBuffer.Count == ItemBlockSize || endOfData) {
                DisplayRowBlockCallback drc = HandleDisplayRowBlock;
                Invoke(drc, new object[] { mRowBuffer });
                mRowBuffer.Clear();
            }
            if (endOfData) {
                DisplayRowBlockCallback drc = HandleDisplayRowBlock;
                Invoke(drc, new object[] { null });
            }
        }

        #endregion

        #region Compatibility with ListDisplayControl

        /// <summary>
        /// return suitable ISinkModule reference to this object
        /// </summary>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink() {
            return MakeSink(10);
        }

        /// <summary>
        /// return suitable ISinkModule reference to this object
        /// </summary>
        /// <param name="title"></param>
        /// <param name="blkSz"></param>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink(string title, int blkSz) {
            PageTitle = title;
            return MakeSink(blkSz);
        }

        /// <summary>
        /// return suitable ISinkModule reference to this object
        /// </summary>
        /// <param name="blkSz"></param>
        /// <returns>ISinkModule reference</returns>
        public ISinkModule MakeSink(int blkSz) {
            Clear();
            return this;
        }


        #endregion

        #region Move Items

        /// <summary>
        /// Move the first currently selected item up or down in the list
        /// </summary>
        /// <param name="moveUp"></param>
        public void MoveListItem(bool moveUp) {
            string cache;
            int selIdx;
            MyDataGridView lv = gvQueryResults;

            selIdx = lv.SelectedRows[0].Index;
            if (moveUp) {
                // ignore moveup of row(0)
                if (selIdx == 0)
                    return;

                // move the subitems for the previous row
                // to cache to make room for the selected row
                for (int i = 0; i < lv.Rows[selIdx].Cells.Count; i++) {
                    cache = lv.Rows[selIdx - 1].Cells[i].Value.ToString();
                    lv.Rows[selIdx - 1].Cells[i].Value =
                      lv.Rows[selIdx].Cells[i].Value.ToString();
                    lv.Rows[selIdx].Cells[i].Value = cache;
                }
                lv.Rows[selIdx - 1].Selected = true;
                lv.Rows[selIdx].Selected = false;
            } else {
                // ignore movedown of last item
                if (selIdx == lv.Rows.Count - 1)
                    return;
                // move the Cells for the next row
                // to cache so we can move the selected row down
                for (int i = 0; i < lv.Rows[selIdx].Cells.Count; i++) {
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
        public class MyDataGridView : System.Windows.Forms.DataGridView
	    {

            private System.Collections.Hashtable myHashTable = new System.Collections.Hashtable();
            string s;
            bool mLayoutSuspended = false;
            bool mAllowDelete = true;

            /// <summary>
            /// Defines whether or not user can delete rows using the right-click menu
            /// </summary>
            public bool AllowDelete { get { return mAllowDelete; } set { mAllowDelete = value; } }

            /// <summary>
            /// Toggles whether or not Shift+Click is supported
            /// When Shift+Click is supported, then persistence of row selection is disabled
            /// </summary>
            public bool ShiftClickSelect { get; set; }

            /// <summary>
            /// True when SuspendLayout() is en effect for this data grid
            /// </summary>
            public bool LayoutSuspended {
                get { return mLayoutSuspended; }
            }

            /// <summary>
            /// Clears the current selection by unselecting all selected cells.
            /// </summary>
            new public void ClearSelection() {
                base.ClearSelection();

                myHashTable.Clear();
            }

            /// <summary>
            /// Remove the items from the display list that are currently not selected
            /// </summary>
            /// 
            public void DeleteNotSelectedItems() {
                if (this.AllowDelete) {
                    List<DataGridViewRow> toRemove = new List<DataGridViewRow>();

                    this.SuspendLayout();
                    mLayoutSuspended = true;

                    foreach (DataGridViewRow item in this.Rows) {
                        if (!item.Selected && !item.IsNewRow) {
                            toRemove.Add(item);
                        }
                    }
                    foreach (DataGridViewRow item in toRemove) {
                        this.Rows.Remove(item);
                    }

                    mLayoutSuspended = false;
                    this.ResumeLayout();

                    this.Update();
                }
            }

            /// <summary>
            /// Remove the currently selected items from the display list
            /// </summary>           
            public void DeleteSelectedItems() {

                if (this.SelectedRows.Count > 0 && this.AllowDelete) {
                    this.SuspendLayout();
                    mLayoutSuspended = true;

                    List<DataGridViewRow> toRemove = new List<DataGridViewRow>(this.SelectedRows.Count);

                    // Cache the rows that are selected
                    foreach (DataGridViewRow item in this.SelectedRows) {
                        toRemove.Add(item);
                    }

                    // Remove each row
                    foreach (DataGridViewRow item in toRemove) {
                        this.Rows.Remove(item);
                    }

                    mLayoutSuspended = false;
                    this.ResumeLayout();

                    this.Update();
                }
                
            }

            /// <summary>
            /// Highlights the rows that are tracked in myHashTable
            /// </summary>
            protected void HighlightSelectedRows() {

                // Select all rows clicked
                foreach (System.Windows.Forms.DataGridViewRow SelectedRow in myHashTable.Values) {
                    SelectedRow.Selected = true;
                }

            }

            /// <summary>
            /// Selects all the cells in the System.Windows.Forms.DataGridView.
            /// </summary>
            new public void SelectAll() {
                if (this.MultiSelect) {
                    this.SuspendLayout();

                    base.SelectAll();

                    myHashTable.Clear();

                    foreach (System.Windows.Forms.DataGridViewRow SelectedRow in this.Rows) {
                        myHashTable.Add(SelectedRow.Index.ToString(), SelectedRow);
                    }

                    HighlightSelectedRows();

                    this.ResumeLayout();
                }
            }

            /// <summary>
            /// Updates myHashTable to track the currently selected rows
            /// </summary>
            protected void SynchronizeHashTableWithSelectedRows() {

                myHashTable.Clear();

                foreach (System.Windows.Forms.DataGridViewRow SelectedRow in this.SelectedRows) {
                    myHashTable.Add(SelectedRow.Index.ToString(), SelectedRow);
                }

            }

            /// <summary>
            /// Initializes a new instance of the MageDisplayLib.MyDataGridView class.
            /// </summary>
            public MyDataGridView() {
                this.AllowUserToAddRows = false;
                this.AllowUserToDeleteRows = false;
                this.AllowUserToResizeColumns = true;
                this.AllowUserToOrderColumns = false;
                this.ReadOnly = true;

            }

            /// <summary>
            ///  Raises the System.Windows.Forms.DataGridView.RowsAdded event
            ///  Updates myHashTable with the currently selected rows
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e) {
                base.OnRowsAdded(e);
                SynchronizeHashTableWithSelectedRows();
            }

            /// <summary>
            /// Raises the System.Windows.Forms.DataGridView.RowsRemoved event.
            /// ///  Updates myHashTable with the currently selected rows
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e) {
                base.OnRowsRemoved(e);

                // Make sure myHashTable is up-to-date
                SynchronizeHashTableWithSelectedRows();
            }

            /// <summary>
            /// Selects/deselects the row in which the mouse was clicked
            /// </summary>
            /// <param name="e"></param>
            protected override void OnCellMouseDown(System.Windows.Forms.DataGridViewCellMouseEventArgs e) {
                if (ShiftClickSelect || !this.MultiSelect || e.Button != System.Windows.Forms.MouseButtons.Left) {
                    // Use the default behavior for click, Shift+Click, Ctrl+Click, or click of middle or right button.
                    base.OnCellMouseDown(e);
                    SynchronizeHashTableWithSelectedRows();
                } else {
					if ((e.RowIndex >= 0) && (e.RowIndex < this.Rows.Count) && (e.ColumnIndex >= 0) && (e.ColumnIndex < this.Columns.Count)) {
						// A cell was clicked
						// Do not call the base class .OnCellMouseDown() event

						s = e.RowIndex.ToString();

						if (myHashTable.ContainsKey(s)) {
							//If the row is in the hashtable, remove it
							myHashTable.Remove(s);
							this.Rows[e.RowIndex].Selected = false;
						} else {
							// Otherwise, insert it into the hashtable
							this.Rows[e.RowIndex].Selected = false;

							myHashTable.Add(s, this.Rows[e.RowIndex]);
						}

						this.SuspendLayout();
						this.HighlightSelectedRows();
						this.ResumeLayout();
					} else {
						// User likely clicked on the header row
						// Call the base-class method
						base.OnCellMouseDown(e);
					}
                }

            }

        }

    }
}
