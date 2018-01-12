using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MageDisplayLib;

namespace MageFileProcessor
{

    /// <summary>
    /// Builds an extension to the context menu for a ListDisplayControl object,
    /// and provides the code to handle the extension functions.
    /// </summary>
    class ListDisplayActions
    {

        #region Member Variables

        /// <summary>
        /// The particular ListDisplayControl object that this object is attached to
        /// and supplies context menu items for
        /// </summary>
        private readonly ListDisplayControl mListDisplay;

        /// <summary>
        /// The ListView control contained by the attached ListDisplayControl
        /// (broken out as separate reference for convenience)
        /// </summary>
        private readonly ListView mListView;

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private readonly List<string> mAllMenuItems = new List<string>();

        /// <summary>
        /// Lists of names of menu items
        /// that are sensitive to presence of certain columns in list display
        /// </summary>
        private readonly List<string> mFolderSensitiveMenuItems = new List<string>();
        private readonly List<string> mJobSensitiveMenuItems = new List<string>();
        private readonly List<string> mDatasetSensitiveMenuItems = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// Can't instantiate this class without an associated ListDisplayControl object
        /// </summary>
        /// <param name="listDisplay"></param>
        public ListDisplayActions(ListDisplayControl listDisplay)
        {
            mListDisplay = listDisplay;
            mListView = mListDisplay.List;
            mListDisplay.SelectionChanged += HandleSelectionChanged;
            SetupContextMenus();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Create the context menu for the display list
        /// </summary>
        private void SetupContextMenus()
        {

            var mMyMenuItems = new List<ToolStripItem> { new ToolStripSeparator() };
            mMyMenuItems.AddRange(GetFolderMenuItems().ToArray());
            mMyMenuItems.AddRange(GetWebActionMenuItems().ToArray());

            mListDisplay.AppendContextMenuItems(mMyMenuItems.ToArray());

            foreach (var tsmi in mMyMenuItems)
            {
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
        private int GetColumnIndex(string colName)
        {
            var i = mListView.Columns.IndexOfKey(colName);
            return i;
        }

        /// <summary>
        /// Get values in given column for currently selected items in display list
        /// </summary>
        /// <param name="colName">Column name to get values from</param>
        /// <returns>List of contents of column for each selected row</returns>
        private string[] GetItemList(string colName)
        {
            var lst = new List<string>();
            var i = GetColumnIndex(colName);
            if (i != -1)
            {
                foreach (ListViewItem objRow in mListView.SelectedItems)
                {
                    lst.Add(objRow.SubItems[i].Text);
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
        private ToolStripItem[] GetWebActionMenuItems()
        {
            var tsmil = new List<ToolStripItem>();
            var webmi = new ToolStripMenuItem("Open DMS web page") { Name = "WebPageSubmenu" };
            tsmil.Add(webmi);

            var tsmi = new ToolStripMenuItem("Job detail", null, HandleJobWebAction, "JobDetailWebPage");
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
        private void HandleJobWebAction(object sender, EventArgs e)
        {
            LaunchWebBrowser("http://dms2.pnl.gov/analysis_job/show/", "Job");
        }
        private void HandleDatasetWebAction(object sender, EventArgs e)
        {
            LaunchWebBrowser("http://dms2.pnl.gov/dataset/show/", "Dataset");
        }

        /// <summary>
        /// Launch the default web browser with URL
        /// </summary>
        /// <param name="url">Base URL</param>
        /// <param name="columnName">column to get trailing URL segment from</param>
        private void LaunchWebBrowser(string url, string columnName)
        {
            var itemList = GetItemList(columnName);
            if (mListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("No rows selected");
            }
            else
                if (itemList.Length == 0)
            {
                MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
            }
            else
            {
                System.Diagnostics.Process.Start(url + itemList[0]);
            }
        }

        #endregion

        #region Windows Explorer Folder Menu Actions

        private ToolStripItem[] GetFolderMenuItems()
        {
            var l = new List<ToolStripItem>();
            var tsmi = new ToolStripMenuItem("Open Folder", null, HandleFolderAction, "OpenFolder");
            mFolderSensitiveMenuItems.Add(tsmi.Name);
            l.Add(tsmi);
            return l.ToArray();
        }

        /// <summary>
        /// Handle menu item event for opening Windows Explorer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFolderAction(object sender, EventArgs e)
        {
            OpenWindowsExplorer("Folder");
        }

        /// <summary>
        /// Opens a Windows Explorer window with contents of given column
        /// as file path to open
        /// </summary>
        /// <param name="columnName"></param>
        private void OpenWindowsExplorer(string columnName)
        {
            var itemList = GetItemList(columnName);
            if (mListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("No rows selected");
            }
            else
                if (itemList.Length == 0)
            {
                MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
            }
            else
            {
                var filePath = itemList[0];
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
        private void HandleSelectionChanged(object sender, EventArgs args)
        {

            // Whole context menu enabled/disabled based on whether there are any rows selected or not
            if (mListDisplay.SelectedItemCount == 0)
            {
                AdjustMenuItemsFromNameList(mAllMenuItems, false);
            }
            else
            {
                AdjustMenuItemsFromNameList(mAllMenuItems, true);

                // Enable/disable selected menu items based on presence
                // of certain columns in rows
                AdjustMenuItemsFromNameList(mFolderSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mJobSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mDatasetSensitiveMenuItems, false);

                foreach (var colDef in mListDisplay.ColumnDefs)
                {
                    switch (colDef.Name)
                    {
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
        /// Set the enabled state of the menu items in the ListDisplay ListView
        /// (using name property of menu item) from the given list of menu item names.
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="active"></param>
        private void AdjustMenuItemsFromNameList(List<string> itemNames, bool active)
        {
            foreach (var name in itemNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var tsi in mListView.ContextMenuStrip.Items.Find(name, true))
                    {
                        tsi.Enabled = active;
                    }
                }
            }
        }

        #endregion
    }
}
