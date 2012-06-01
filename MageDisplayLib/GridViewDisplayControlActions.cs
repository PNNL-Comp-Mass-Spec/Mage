using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using System.IO;
using System.Collections.ObjectModel;

namespace MageDisplayLib {

    /// <summary>
    /// User control that provides a display list based on a DataGridView
    /// that can be used with Mage pipeline infrastructure
    /// </summary>
    public partial class GridViewDisplayControl {

        #region Constants
            const string MENU_SELECT_ALL = "SelectAll";
            const string MENU_SELECT_NONE = "SelectNone";
            const string MENU_DELETE_SELECTED = "DeleteSelectedRows";
            const string MENU_DELETE_NON_SELECTED = "DeleteNonSelectedRows";
        #endregion

        #region Member Variables

        /// <summary>
        /// this event is fired to sent command to external command handler(s)
        /// </summary>
        public event EventHandler<MageCommandEventArgs> OnAction;

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private List<string> mAllMenuItems = new List<string>();
        private List<string> mSelectAllOrNoneMenuItems = new List<string>();
        private List<string> mDeleteMenuItems = new List<string>();

        #endregion

        #region Context Menu Functions

        /// <summary>
        /// Add context menu to control for saving and restoring contents of list display to/from a file
        /// </summary>
        private void SetupContextMenus(Control targetControl) {
            List<ToolStripItem> mMyMenuItems = new List<ToolStripItem>();
            mMyMenuItems.AddRange(GetBasicHousekeepingMenuItems().ToArray());
            mMyMenuItems.Add(new ToolStripSeparator());
            mMyMenuItems.AddRange(GetCopyMenuItems());

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(mMyMenuItems.ToArray());
            contextMenu.Items.AddRange(GetPersistenceMenuItems().ToArray());
            targetControl.ContextMenuStrip = contextMenu;

            foreach (ToolStripItem tsmi in mMyMenuItems) {
                tsmi.Enabled = false;
                mAllMenuItems.Add(tsmi.Name);
            }

            mSelectAllOrNoneMenuItems.Clear();
            mSelectAllOrNoneMenuItems.Add(MENU_SELECT_ALL);
            mSelectAllOrNoneMenuItems.Add(MENU_SELECT_NONE);

            mDeleteMenuItems.Clear();
            mDeleteMenuItems.Add(MENU_DELETE_SELECTED);
            mDeleteMenuItems.Add(MENU_DELETE_NON_SELECTED);

            SelectionChanged += HandleSelectionChanged;
        }

        /// <summary>
        /// Insert new menu items ahead of any menu items
        /// currently in the context menu
        /// </summary>
        /// <param name="items"></param>
        public void InsertContextMenuItems(ToolStripItem[] items) {
            List<ToolStripItem> currentMenuItems = new List<ToolStripItem>();
            foreach (ToolStripItem tsi in gvQueryResults.ContextMenuStrip.Items) {
                currentMenuItems.Add(tsi);
            }
            ContextMenuStrip newMenu = new ContextMenuStrip();
            newMenu.Items.AddRange(items);
            newMenu.Items.Add(new ToolStripSeparator());
            newMenu.Items.AddRange(currentMenuItems.ToArray());
            gvQueryResults.ContextMenuStrip = newMenu;
        }

        /// <summary>
        /// Append new menu items after existing menu items
        /// </summary>
        /// <param name="items"></param>
        public void AppendContextMenuItems(ToolStripItem[] items) {
            //           gvQueryResults.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            gvQueryResults.ContextMenuStrip.Items.AddRange(items);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Will be wired up to receive "SelectionChanged" events
        /// from associated GridView so that display state 
        /// of menu items may be adjusted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleSelectionChanged(object sender, EventArgs args) {

            if (!gvQueryResults.LayoutSuspended) {
                // whole context menu enabled/disabled based on whether there are any rows selected or not
                if (SelectedItemCount == 0) {
                    System.Collections.Generic.List<string> lstAlwaysActive = null;

                    if (ItemCount > 0)
                        lstAlwaysActive = mSelectAllOrNoneMenuItems;

                    AdjustMenuItemsFromNameList(mAllMenuItems, false, lstAlwaysActive);

                } else {
                    AdjustMenuItemsFromNameList(mAllMenuItems, true, null);
                }
            }

        }

        /// <summary>
        /// Set the enabled state of the menu items in the ListDisplay ListView
        /// (using name property of menu item) from the given list of menu item names.
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="active"></param>
        /// <param name="lstAlwaysActive"></param>
        private void AdjustMenuItemsFromNameList(List<string> itemNames, bool active, System.Collections.Generic.List<string> lstAlwaysActive) {
            foreach (string name in itemNames) {
                if (!string.IsNullOrEmpty(name)) {
                    foreach (ToolStripItem tsi in gvQueryResults.ContextMenuStrip.Items.Find(name, true)) {
                        if (lstAlwaysActive != null && lstAlwaysActive.Contains(name))
                            tsi.Enabled = true;
                        else {
                            if (!gvQueryResults.AllowDelete && mDeleteMenuItems.Contains(name))
                                tsi.Enabled = false;
                            else
                                tsi.Enabled = active;
                        }
                    }
                }
            }
        }

        #endregion


        #region Basic Housekeeping Menus

        /// <summary>
        /// builds a set of new menu items that handle basic housekeeping
        /// (selection and deletion) of items in list
        /// </summary>
        /// <returns></returns>
        private ToolStripItem[] GetBasicHousekeepingMenuItems() {
            List<ToolStripItem> l = new List<ToolStripItem>();
            l.Add(new ToolStripMenuItem("Select All", null, HandleSelectAll, MENU_SELECT_ALL));
            l.Add(new ToolStripMenuItem("Select None", null, HandleClearSelection, MENU_SELECT_NONE));
            l.Add(new ToolStripSeparator());
            l.Add(new ToolStripMenuItem("Delete selected rows", null, HandleDeleteSelectedRows, MENU_DELETE_SELECTED));
            l.Add(new ToolStripMenuItem("Delete all except selected rows", null, HandleDeleteNotSelectedRows, MENU_DELETE_NON_SELECTED));
            return l.ToArray();
        }

        /// <summary>
        /// Handlers for menu item events for basic context menu operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSelectAll(object sender, EventArgs e) {
            SelectAllRows();
        }
        private void HandleClearSelection(object sender, EventArgs e) {
            ClearSelection();
        }
        private void HandleDeleteSelectedRows(object sender, EventArgs e) {
            DeleteSelectedItems();
        }
        private void HandleDeleteNotSelectedRows(object sender, EventArgs e) {
            DeleteNotSelectedItems();
        }

        #endregion


        #region List Persistence Functions

        private List<ToolStripItem> GetPersistenceMenuItems() {
            List<ToolStripItem> menuItems = new List<ToolStripItem>();
            menuItems.Add(new ToolStripSeparator());
            menuItems.Add(new ToolStripMenuItem("Save to file", null, HandleSaveListDisplay, "SaveToFile"));
            menuItems.Add(new ToolStripMenuItem("Reload from file", null, HandleReloadListDisplay, "ReloadFromFile"));
            return menuItems;
        }

        /// <summary>
        /// Save contents of list display to file chosen by user
        /// 
        /// Prompt user to choose file, create Mage pipline to write it from
        /// the display list, and either send off as command to be executed by main program
        /// or execute directly
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">(ignored)</param>
        private void HandleSaveListDisplay(object sender, EventArgs args) {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save display to file";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text (tab delimited)|*.txt";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "") {
                try {
                    IBaseModule source = new GVPipelineSource(this, "All");
                    string filePath = saveFileDialog1.FileName;
                    ProcessingPipeline pipeline = SaveListDisplay(source, filePath);
                    pipeline.RunRoot(null);
                } catch (MageException ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Restore contents of display list from file chosen by user.
        /// 
        /// Prompt user to choose file, create Mage pipline to read it into
        /// the display list, and either send off as command to be executed by main program
        /// or execute directly
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">(ignored)</param>
        private void HandleReloadListDisplay(object sender, EventArgs args) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = false;
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text (Tab delimited)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    string filePath = openFileDialog1.FileName;
                    string fileName = Path.GetFileName(filePath);
                    string title = string.Format("Reloaded File {0}", fileName);
                    ProcessingPipeline pipeline = ReloadListDisplay(this, filePath);
                    pipeline.RunRoot(null);
                    if (OnAction != null) OnAction(this, new MageCommandEventArgs("display_reloaded"));
                } catch (MageException ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Build Mage pipeline to save contents of list display to file
        /// </summary>
        /// <param name="sourceObject">Mage module that can deliver contents of ListView on standard tabular input</param>
        /// <param name="filePath">File to save contents to</param>
        /// <returns></returns>
        private static ProcessingPipeline SaveListDisplay(IBaseModule sourceObject, string filePath) {
            DelimitedFileWriter writer = new DelimitedFileWriter();
            writer.FilePath = filePath;
            string name = "SaveListDisplayPipeline";
            return ProcessingPipeline.Assemble(name, new Collection<object>() { sourceObject, writer });
        }

        /// <summary>
        /// Build Mage pipeline to restore contents of list display from file
        /// </summary>
        /// <param name="sinkObject">Mage module that can deliver standard tabular input to ListView</param>
        /// <param name="filePath">File to reload list from</param>
        /// <returns></returns>
        private static ProcessingPipeline ReloadListDisplay(ISinkModule sinkObject, string filePath) {
            DelimitedFileReader reader = new DelimitedFileReader();
            reader.FilePath = filePath;
            string name = "RestoreListDisplayPipeline";
            return ProcessingPipeline.Assemble(name, new Collection<object>() { reader, sinkObject });
        }

        #endregion


        #region Copy Menus

        /// <summary>
        /// Builds as set of menu items for actions that copy contents of
        /// list to clipboard
        /// </summary>
        /// <returns></returns>
        private ToolStripItem[] GetCopyMenuItems() {
            List<ToolStripItem> l = new List<ToolStripItem>();
            l.Add(new ToolStripMenuItem("Copy selected rows", null, HandleCopyRows, "CopySelectedRows"));
            return l.ToArray();
        }

        /// <summary>
        /// Handlers for copy menu item events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCopyRows(object sender, EventArgs e) {
            CopySelectedRows();
        }
        private void HandleListCopyColumn(object sender, EventArgs e) {
            CopyColumnList("Folder", null);
        }

        /// <summary>
        /// copy list of values in specified column to the clipboard
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="delimiter"></param>
        private void CopyColumnList(string colName, string delimiter) {
            int i = GetColumnIndex(colName);
            delimiter = (delimiter == null) ? delimiter = Environment.NewLine : delimiter + " ";
            if (i != -1) {
                string strText = "";
                string delim = "";
                foreach (DataGridViewRow objRow in gvQueryResults.SelectedRows) {
                    strText += delim + objRow.Cells[i].Value.ToString();
                    delim = delimiter;
                }
                Clipboard.SetText(strText);
            }
        }

        /// <summary>
        /// Copies the selected rows to the clipboard
        /// </summary>
        private void CopySelectedRows() {
            // Copy the selected data to the clipboard
            // Include the column names if more than one row is selected (or if the ListView only contains one row)
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(4096);

            if (gvQueryResults.Rows.Count == 1 | gvQueryResults.SelectedRows.Count > 1) {
                // Populate strText with the column names
                for (int i = 0; i < gvQueryResults.Columns.Count; i++) {
                    if (i == 0)
                        sbText.Append(gvQueryResults.Columns[i].Name);
                    else
                        sbText.Append("\t" + gvQueryResults.Columns[i].Name);
                }
                sbText.Append("\n");
            }
            int intRowIndex = 0;
            foreach (DataGridViewRow objRow in gvQueryResults.SelectedRows) {
                if (intRowIndex > 0)
                    sbText.Append("\n");
                for (int i = 0; i < objRow.Cells.Count; i++) {
                    if (i == 0)
                        sbText.Append(FixNull(objRow.Cells[i].Value));
                    else
                        sbText.Append("\t" + FixNull(objRow.Cells[i].Value));
                }
                intRowIndex++;
            }
            Clipboard.SetText(sbText.ToString());
        }

        private string FixNull(object value) {
            if (value == null)
                return string.Empty;
            else
                return value.ToString();
        }

        /// <summary>
        /// Return the index to the given column
        /// </summary>
        /// <param name="colName">Name of column to get index for</param>
        /// <returns>Position of column in item array</returns>
        private int GetColumnIndex(string colName) {
            int i = gvQueryResults.Columns[colName].Index;
            return i;
        }

        /// <summary>
        /// Get values in given column for currently selected items in display list
        /// </summary>
        /// <param name="colName">Column name to get values from</param>
        /// <returns>List of contents of column for each selected row</returns>
        public string[] GetItemList(string colName) {
            List<string> lst = new List<string>();
            int i = GetColumnIndex(colName);
            if (i != -1) {
                foreach (DataGridViewRow objRow in gvQueryResults.SelectedRows) {
                    lst.Add(objRow.Cells[i].Value.ToString());
                }
            }
            return lst.ToArray();
        }

        #endregion

    }
}
