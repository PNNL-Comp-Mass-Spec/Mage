using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageFileProcessor
{
    /// <summary>
    /// Builds an extension to the context menu for a GridViewDisplayControl object,
    /// and provides the code to handle the extension functions.
    /// </summary>
    internal class GridViewDisplayActions
    {
        // Ignore Spelling: cbdms

        public string BaseDmsUrl
        {
            get
            {
                return Globals.DMSServer.ToLower() switch
                {
                    "cbdms" => "http://cbdmsweb.pnl.gov",
                    _ => "https://dms2.pnl.gov"       // Includes gigasax
                };
            }
        }

        /// <summary>
        /// The particular GridViewDisplayControl object that this object is attached to
        /// and supplies context menu items for
        /// </summary>
        private readonly GridViewDisplayControl mDisplayUserControl;

        /// <summary>
        /// The DataGridView control contained by the attached GridViewDisplayControl
        /// (broken out as separate reference for convenience)
        /// </summary>
        private readonly DataGridView mDisplayView;

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private readonly List<string> mAllMenuItems = new();

        /// <summary>
        /// Lists of names of menu items
        /// that are sensitive to presence of certain columns in list display
        /// </summary>
        private readonly List<string> mDirectorySensitiveMenuItems = new();
        private readonly List<string> mJobSensitiveMenuItems = new();
        private readonly List<string> mDatasetSensitiveMenuItems = new();

        /// <summary>
        /// Constructor
        /// Can't instantiate this class without an associated GridViewDisplayControl object
        /// </summary>
        /// <param name="listDisplay"></param>
        public GridViewDisplayActions(GridViewDisplayControl listDisplay)
        {
            mDisplayUserControl = listDisplay;
            mDisplayView = mDisplayUserControl.List;
            mDisplayUserControl.SelectionChanged += HandleSelectionChanged;
            SetupContextMenus();
        }

        /// <summary>
        /// Create the context menu for the display list
        /// </summary>
        private void SetupContextMenus()
        {
            var toolStripItems = new List<ToolStripItem> { new ToolStripSeparator() };
            toolStripItems.AddRange(GetDirectoryMenuItems().ToArray());
            toolStripItems.AddRange(GetWebActionMenuItems().ToArray());

            mDisplayUserControl.AppendContextMenuItems(toolStripItems.ToArray());

            foreach (var menuItem in toolStripItems)
            {
                menuItem.Enabled = false;
                mAllMenuItems.Add(menuItem.Name);
            }
        }

        // Web Action Menus

        /// <summary>
        /// Build set of menu items for opening web pages
        /// </summary>
        /// <returns>Menu items</returns>
        private ToolStripItem[] GetWebActionMenuItems()
        {
            var toolStripItems = new List<ToolStripItem>();

            var webPageMenuItem = new ToolStripMenuItem("Open DMS web page") { Name = "WebPageSubmenu" };
            toolStripItems.Add(webPageMenuItem);

            var jobDetailMenuItem = new ToolStripMenuItem("Job detail", null, HandleJobWebAction, "JobDetailWebPage");
            mJobSensitiveMenuItems.Add(jobDetailMenuItem.Name);
            webPageMenuItem.DropDownItems.Add(jobDetailMenuItem);

            var datasetDetailMenuItem = new ToolStripMenuItem("Dataset detail", null, HandleDatasetWebAction, "DatasetDetailWebPage");
            mDatasetSensitiveMenuItems.Add(datasetDetailMenuItem.Name);
            webPageMenuItem.DropDownItems.Add(datasetDetailMenuItem);

            return toolStripItems.ToArray();
        }

        /// <summary>
        /// Handles menu item event to open web browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleJobWebAction(object sender, EventArgs e)
        {
            // Example URL: https://dms2.pnl.gov/analysis_job/show/1302557
            LaunchWebBrowser(BaseDmsUrl + "/analysis_job/show/", "Job");
        }
        private void HandleDatasetWebAction(object sender, EventArgs e)
        {
            // Example URL: https://dms2.pnl.gov/dataset/show/QC_Shew_16_01_2_11Apr16_Pippin_16-03-05
            LaunchWebBrowser(BaseDmsUrl + "/dataset/show/", "Dataset");
        }

        /// <summary>
        /// Launch the default web browser with URL
        /// </summary>
        /// <param name="url">Base URL</param>
        /// <param name="columnName">column to get trailing URL segment from</param>
        private void LaunchWebBrowser(string url, string columnName)
        {
            PanelSupport.LaunchWebBrowser(mDisplayView, url, columnName);
        }

        // Windows Explorer Directory Menu Actions

        private ToolStripItem[] GetDirectoryMenuItems()
        {
            var toolStripItems = new List<ToolStripItem>();

            var openDirectoryMenuItem = new ToolStripMenuItem("Open Directory", null, HandleDirectoryAction, "OpenDirectory");
            mDirectorySensitiveMenuItems.Add(openDirectoryMenuItem.Name);
            toolStripItems.Add(openDirectoryMenuItem);

            return toolStripItems.ToArray();
        }

        /// <summary>
        /// Handle menu item event for opening Windows Explorer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDirectoryAction(object sender, EventArgs e)
        {
            OpenWindowsExplorer("Directory");
        }

        /// <summary>
        /// Opens a Windows Explorer window with contents of given column
        /// as file path to open
        /// </summary>
        /// <param name="columnName"></param>
        private void OpenWindowsExplorer(string columnName)
        {
            PanelSupport.OpenWindowsExplorer(mDisplayView, columnName);
        }

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
            if (mDisplayUserControl.SelectedItemCount == 0)
            {
                AdjustMenuItemsFromNameList(mAllMenuItems, false);
            }
            else
            {
                AdjustMenuItemsFromNameList(mAllMenuItems, true);

                // Enable/disable selected menu items based on presence
                // of certain columns in rows
                AdjustMenuItemsFromNameList(mDirectorySensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mJobSensitiveMenuItems, false);
                AdjustMenuItemsFromNameList(mDatasetSensitiveMenuItems, false);

                foreach (var colDef in mDisplayUserControl.ColumnDefs)
                {
                    switch (colDef.Name)
                    {
                        case "Job":
                            AdjustMenuItemsFromNameList(mJobSensitiveMenuItems, true);
                            break;
                        case "Dataset":
                            AdjustMenuItemsFromNameList(mDatasetSensitiveMenuItems, true);
                            break;
                        case "Directory":
                        case "Folder":
                            AdjustMenuItemsFromNameList(mDirectorySensitiveMenuItems, true);
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
        private void AdjustMenuItemsFromNameList(List<string> itemNames, bool active)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var name in itemNames)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                foreach (var tsi in mDisplayView.ContextMenuStrip.Items.Find(name, true))
                {
                    tsi.Enabled = active;
                }
            }
        }
    }
}
