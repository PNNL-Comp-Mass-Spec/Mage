using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MageDisplayLib;
using Mage;

namespace MageFileProcessor {

    /// <summary>
    /// Builds an extension to the context menu for a GridViewDisplayControl object,
    /// and provides the code to handle the extension functions.
    /// </summary>
    class GridViewDisplayActions {

        #region Member Variables

        /// <summary>
        /// The particular GridViewDisplayControl object that this object is attached to
        /// and supplies context menu items for
        /// </summary>
        private GridViewDisplayControl mDisplayUserControl = null;

        /// <summary>
        /// The DataGridView control contained by the attached GridViewDisplayControl
        /// (broken out as separate reference for convenience)
        /// </summary>
        private DataGridView mDisplayView = null;

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private List<string> mAllMenuItems = new List<string>();

        /// <summary>
        /// Lists of names of menu items 
        /// that are sensitive to presence of certain columns in list display
        /// </summary>
        private List<string> mFolderSensitiveMenuItems = new List<string>();
        private List<string> mJobSensitiveMenuItems = new List<string>();
        private List<string> mDatasetSensitiveMenuItems = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// Can't instantiate this class without an associated GridViewDisplayControl object
        /// </summary>
        /// <param name="listDisplay"></param>
        public GridViewDisplayActions(GridViewDisplayControl listDisplay) {
            mDisplayUserControl = listDisplay;
            mDisplayView = mDisplayUserControl.List;
            mDisplayUserControl.SelectionChanged += HandleSelectionChanged;
            SetupContextMenus();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Create the context menu for the display list 
        /// </summary>
        private void SetupContextMenus() {

            List<ToolStripItem> mMyMenuItems = new List<ToolStripItem>();
            mMyMenuItems.Add(new ToolStripSeparator());
            mMyMenuItems.AddRange(GetFolderMenuItems().ToArray());
            mMyMenuItems.AddRange(GetWebActionMenuItems().ToArray());

            mDisplayUserControl.AppendContextMenuItems(mMyMenuItems.ToArray());

            foreach (ToolStripItem tsmi in mMyMenuItems) {
                tsmi.Enabled = false;
                mAllMenuItems.Add(tsmi.Name);
            }
        }

        #endregion

        #region Column Functions

        /// <summary>
        /// Return the index to the given column
        /// </summary>
        /// <param name="colName">Name of column to get index for</param>
        /// <returns>Position of column in item array</returns>
        private int GetColumnIndex(string colName) {
            int i = mDisplayView.Columns[colName].Index;
            return i;
        }

        /// <summary>
        /// Get values in given column for currently selected items in display list
        /// </summary>
        /// <param name="colName">Column name to get values from</param>
        /// <returns>List of contents of column for each selected row</returns>
        private string[] GetItemList(string colName) {
            List<string> lst = new List<string>();
            int i = GetColumnIndex(colName);
            if (i != -1) {
                foreach (DataGridViewRow objRow in mDisplayView.SelectedRows) {
                    lst.Add(objRow.Cells[i].Value.ToString());
                }
            }
            return lst.ToArray();
        }

        #endregion

        #region Web Action Menus

        /// <summary>
        /// Build set of menu items for opening web pages
        /// </summary>
        /// <returns>Menu items</returns>
        private ToolStripItem[] GetWebActionMenuItems() {
            List<ToolStripItem> tsmil = new List<ToolStripItem>();
            ToolStripMenuItem tsmi = null;
            ToolStripMenuItem webmi = new ToolStripMenuItem("Open DMS web page");
            webmi.Name = "WebPageSubmenu";
            tsmil.Add(webmi);

            tsmi = new ToolStripMenuItem("Job detail", null, HandleJobWebAction, "JobDetailWebPage");
            mJobSensitiveMenuItems.Add(tsmi.Name);
            webmi.DropDownItems.Add(tsmi);

            tsmi = new ToolStripMenuItem("Dataset detail", null, HandleDatasetWebAction, "DatasetDetailWebPage");
            mDatasetSensitiveMenuItems.Add(tsmi.Name);
            webmi.DropDownItems.Add(tsmi);
            return tsmil.ToArray();
        }

        /// <summary>
        /// Handles menu item event to open web browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleJobWebAction(object sender, EventArgs e) {
            LaunchWebBrowser("http://dms2.pnl.gov/analysis_job/show/", "Job");
        }
        private void HandleDatasetWebAction(object sender, EventArgs e) {
            LaunchWebBrowser("http://dms2.pnl.gov/dataset/show/", "Dataset");
        }

        /// <summary>
        /// Launch the default web browser with URL
        /// </summary>
        /// <param name="url">Base URL</param>
        /// <param name="columnName">column to get trailing URL segment from</param>
        private void LaunchWebBrowser(string url, string columnName) {
            string[] itemList = GetItemList(columnName);
            if (mDisplayView.SelectedRows.Count == 0) {
                MessageBox.Show("No rows selected");
            } else
                if (itemList.Length == 0) {
                    MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
                } else {
                    System.Diagnostics.Process.Start(url + itemList[0]);
                }
        }

        #endregion

        #region Windows Explorer Folder Menu Actions

        private ToolStripItem[] GetFolderMenuItems() {
            List<ToolStripItem> l = new List<ToolStripItem>();
            ToolStripMenuItem tsmi = new ToolStripMenuItem("Open Folder", null, HandleFolderAction, "OpenFolder");
            mFolderSensitiveMenuItems.Add(tsmi.Name);
            l.Add(tsmi);
            return l.ToArray();
        }

        /// <summary>
        /// Handle menu item event for opening Windows Explorer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFolderAction(object sender, EventArgs e) {
            OpenWindowsExplorer("Folder");
        }

        /// <summary>
        /// Opens a Windows Explorer window with contents of given column
        /// as file path to open
        /// </summary>
        /// <param name="columnName"></param>
        private void OpenWindowsExplorer(string columnName) {
            string[] itemList = GetItemList(columnName);
            if (mDisplayView.SelectedRows.Count == 0) {
                MessageBox.Show("No rows selected");
            } else
                if (itemList.Length == 0) {
                    MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
                } else {
                    string filePath = itemList[0];
                    System.Diagnostics.Process.Start("explorer.exe", filePath);
                }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Will be wired up to receive "SelectionChanged" events
        /// from associated ListDisplay so that display state 
        /// of menu items may be adjusted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleSelectionChanged(object sender, EventArgs args) {

            // whole context menu enabled/disabled based on whether there are any rows selected or not
            if (mDisplayUserControl.SelectedItemCount == 0) {
                AdjustMenuItemsFromNameList(mAllMenuItems, false);
            } else {
                AdjustMenuItemsFromNameList(mAllMenuItems, true);

                // enable/disable selected menu items based on presence
                // of certain columns in rows
                AdjustMenuItemsFromNameList(mFolderSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mJobSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mDatasetSensitiveMenuItems, false);
                //
                foreach (MageColumnDef colDef in mDisplayUserControl.ColumnDefs) {
                    switch (colDef.Name) {
                        case "Job":
                            AdjustMenuItemsFromNameList(mJobSensitiveMenuItems, true);
                            break;
                        case "Dataset":
                            AdjustMenuItemsFromNameList(mDatasetSensitiveMenuItems, true);
                            break;
                        case "Folder":
                            AdjustMenuItemsFromNameList(mFolderSensitiveMenuItems, true);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Set the enabled state of the menu items in the ListDisplay DataGridView
        /// (using name property of menu item) from the given list of menu item names.
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="active"></param>
        private void AdjustMenuItemsFromNameList(List<string> itemNames, bool active) {
            foreach (string name in itemNames) {
                if (!string.IsNullOrEmpty(name)) {
                    foreach (ToolStripItem tsi in mDisplayView.ContextMenuStrip.Items.Find(name, true)) {
                        tsi.Enabled = active;
                    }
                }
            }
        }

        #endregion
    }
}
