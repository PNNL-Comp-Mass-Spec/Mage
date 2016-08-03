using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using MageDisplayLib;

namespace MageFilePackager
{

    /// <summary>
    /// Builds an extension to the context menu for a GridViewDisplayControl object,
    /// and provides the code to handle the extension functions.
    /// </summary>
    class GridViewDisplayActions
    {

        #region Member Variables

        /// <summary>
        /// The particular GridViewDisplayControl object that this object is attached to
        /// and supplies context menu items for
        /// </summary>
        private readonly GridViewDisplayControl _displayUserControl;

        /// <summary>
        /// The DataGridView control contained by the attached GridViewDisplayControl
        /// (broken out as separate reference for convenience)
        /// </summary>
        private readonly DataGridView _displayView;

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private readonly List<string> _allMenuItems = new List<string>();

        /// <summary>
        /// Lists of names of menu items 
        /// that are sensitive to presence of certain columns in list display
        /// </summary>
        private readonly List<string> _folderSensitiveMenuItems = new List<string>();
        private readonly List<string> _jobSensitiveMenuItems = new List<string>();
        private readonly List<string> _datasetSensitiveMenuItems = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// Can't instantiate this class without an associated GridViewDisplayControl object
        /// </summary>
        /// <param name="listDisplay"></param>
        public GridViewDisplayActions(GridViewDisplayControl listDisplay)
        {
            _displayUserControl = listDisplay;
            _displayView = _displayUserControl.List;
            _displayUserControl.SelectionChanged += HandleSelectionChanged;
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

            _displayUserControl.AppendContextMenuItems(mMyMenuItems.ToArray());

            foreach (var tsmi in mMyMenuItems)
            {
                tsmi.Enabled = false;
                _allMenuItems.Add(tsmi.Name);
            }
        }

        #endregion

        #region Column Functions

        /// <summary>
        /// Return the index to the given column
        /// </summary>
        /// <param name="colName">Name of column to get index for</param>
        /// <returns>Position of column in item array, 0 if _displayView.Columns does not contains colName</returns>
        private int GetColumnIndex(string colName)
        {
            var dataGridViewColumn = _displayView.Columns[colName];
            if (dataGridViewColumn == null)
                return 0;

            return dataGridViewColumn.Index;
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
                foreach (DataGridViewRow objRow in _displayView.SelectedRows)
                {
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
        private IEnumerable<ToolStripItem> GetWebActionMenuItems()
        {
            var tsmil = new List<ToolStripItem>();
            var webmi = new ToolStripMenuItem("Open DMS web page") { Name = "WebPageSubmenu" };
            tsmil.Add(webmi);

            var tsmi = new ToolStripMenuItem("Job detail", null, HandleJobWebAction, "JobDetailWebPage");
            _jobSensitiveMenuItems.Add(tsmi.Name);
            webmi.DropDownItems.Add(tsmi);

            tsmi = new ToolStripMenuItem("Dataset detail", null, HandleDatasetWebAction, "DatasetDetailWebPage");
            _datasetSensitiveMenuItems.Add(tsmi.Name);
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
            if (_displayView.SelectedRows.Count == 0)
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
                Process.Start(url + itemList[0]);
            }
        }

        #endregion

        #region Windows Explorer Folder Menu Actions

        private IEnumerable<ToolStripItem> GetFolderMenuItems()
        {
            var l = new List<ToolStripItem>();
            var tsmi = new ToolStripMenuItem("Open Folder", null, HandleFolderAction, "OpenFolder");
            _folderSensitiveMenuItems.Add(tsmi.Name);
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
            if (_displayView.SelectedRows.Count == 0)
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
                Process.Start("explorer.exe", filePath);
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

            // whole context menu enabled/disabled based on whether there are any rows selected or not
            if (_displayUserControl.SelectedItemCount == 0)
            {
                AdjustMenuItemsFromNameList(_allMenuItems, false);
            }
            else
            {
                AdjustMenuItemsFromNameList(_allMenuItems, true);

                // enable/disable selected menu items based on presence
                // of certain columns in rows
                AdjustMenuItemsFromNameList(_folderSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(_jobSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(_datasetSensitiveMenuItems, false);
                //
                foreach (var colDef in _displayUserControl.ColumnDefs)
                {
                    switch (colDef.Name)
                    {
                        case "Job":
                            AdjustMenuItemsFromNameList(_jobSensitiveMenuItems, true);
                            break;
                        case "Dataset":
                            AdjustMenuItemsFromNameList(_datasetSensitiveMenuItems, true);
                            break;
                        case "Folder":
                            AdjustMenuItemsFromNameList(_folderSensitiveMenuItems, true);
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
        private void AdjustMenuItemsFromNameList(IEnumerable<string> itemNames, bool active)
        {
            foreach (var name in itemNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var tsi in _displayView.ContextMenuStrip.Items.Find(name, true))
                    {
                        tsi.Enabled = active;
                    }
                }
            }
        }

        #endregion
    }
}
