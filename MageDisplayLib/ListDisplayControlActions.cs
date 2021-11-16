using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// This partial class implements the context menu functions
    /// for the ListDisplayControl class
    /// </summary>
    public partial class ListDisplayControl
    {
        // Ignore Spelling: Mage

        /// <summary>
        /// List of names of all menu items created by this class
        /// </summary>
        private readonly List<string> mAllMenuItems = new();
        private readonly List<string> mSelectAllMenuItems = new();

        // Context Menu Methods

        /// <summary>
        /// Add context menu to control for saving and restoring contents of list display to/from a file
        /// </summary>
        private void SetupContextMenus(Control targetControl)
        {
            var toolStripItems = new List<ToolStripItem>();
            toolStripItems.AddRange(GetBasicHousekeepingMenuItems().ToArray());
            toolStripItems.Add(new ToolStripSeparator());
            toolStripItems.AddRange(GetCopyMenuItems());

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(toolStripItems.ToArray());
            contextMenu.Items.AddRange(GetPersistenceMenuItems().ToArray());
            targetControl.ContextMenuStrip = contextMenu;

            foreach (var menuItem in toolStripItems)
            {
                menuItem.Enabled = false;
                mAllMenuItems.Add(menuItem.Name);
            }

            mSelectAllMenuItems.Add("SelectAll");

            SelectionChanged += HandleSelectionChanged;
        }

        /// <summary>
        /// Insert new menu items ahead of any menu items
        /// currently in the context menu
        /// </summary>
        /// <param name="items"></param>
        [Obsolete("Unused")]
        // ReSharper disable once UnusedMember.Global
        public void InsertContextMenuItems(ToolStripItem[] items)
        {
            var currentMenuItems = new List<ToolStripItem>();
            foreach (ToolStripItem tsi in lvQueryResults.ContextMenuStrip.Items)
            {
                currentMenuItems.Add(tsi);
            }
            var newMenu = new ContextMenuStrip();
            newMenu.Items.AddRange(items);
            newMenu.Items.Add(new ToolStripSeparator());
            newMenu.Items.AddRange(currentMenuItems.ToArray());
            lvQueryResults.ContextMenuStrip = newMenu;
        }

        /// <summary>
        /// Append new menu items after existing menu items
        /// </summary>
        /// <param name="items"></param>
        public void AppendContextMenuItems(ToolStripItem[] items)
        {
            // lvQueryResults.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            lvQueryResults.ContextMenuStrip.Items.AddRange(items);
        }

        // List Persistence Methods

        private List<ToolStripItem> GetPersistenceMenuItems()
        {
            return new()
            {
                new ToolStripSeparator(),
                new ToolStripMenuItem("Save to file", null, HandleSaveListDisplay, "SaveToFile"),
                new ToolStripMenuItem("Reload from file", null, HandleReloadListDisplay, "ReloadFromFile")
            };
        }

        /// <summary>
        /// <para>
        /// Save contents of list display to file chosen by user
        /// </para>
        /// <para>
        /// Prompt user to choose file, create Mage pipeline to write it from
        /// the display list, and either send off as command to be executed by main program
        /// or execute directly
        /// </para>
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">(ignored)</param>
        private void HandleSaveListDisplay(object sender, EventArgs args)
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Save display to file"
            };
            saveDialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(saveDialog.FileName))
                return;

            try
            {
                IBaseModule source = new LVPipelineSource(this, DisplaySourceMode.All);
                var filePath = saveDialog.FileName;
                var pipeline = SaveListDisplay(source, filePath);
                pipeline.RunRoot(null);
            }
            catch (MageException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// <para>
        /// Restore contents of display list from file chosen by user.
        /// </para>
        /// <para>
        /// Prompt user to choose file, create Mage pipeline to read it into
        /// the display list, and either send off as command to be executed by main program
        /// or execute directly
        /// </para>
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">(ignored)</param>
        private void HandleReloadListDisplay(object sender, EventArgs args)
        {
            var fileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                var filePath = fileDialog.FileName;
                var fileName = Path.GetFileName(filePath);
                var title = string.Format("Reloaded File {0}", fileName);
                var sink = MakeSink(title, 15);
                var pipeline = ReloadListDisplay(sink, filePath);
                pipeline.RunRoot(null);
                OnAction?.Invoke(this, new MageCommandEventArgs("display_reloaded"));
            }
            catch (MageException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Build Mage pipeline to save contents of list display to file
        /// </summary>
        /// <param name="sourceObject">Mage module that can deliver contents of ListView on standard tabular input</param>
        /// <param name="filePath">File to save contents to</param>
        private static ProcessingPipeline SaveListDisplay(IBaseModule sourceObject, string filePath)
        {
            var writer = new DelimitedFileWriter
            {
                FilePath = filePath
            };

            return ProcessingPipeline.Assemble("SaveListDisplayPipeline", new Collection<object> { sourceObject, writer });
        }

        /// <summary>
        /// Build Mage pipeline to restore contents of list display from file
        /// </summary>
        /// <param name="sinkObject">Mage module that can deliver standard tabular input to ListView</param>
        /// <param name="filePath">File to reload list from</param>
        private static ProcessingPipeline ReloadListDisplay(ISinkModule sinkObject, string filePath)
        {
            var reader = new DelimitedFileReader
            {
                FilePath = filePath
            };

            return ProcessingPipeline.Assemble("RestoreListDisplayPipeline", new Collection<object> { reader, sinkObject });
        }

        /// <summary>
        /// Builds a set of new menu items that handle basic housekeeping
        /// (selection and deletion) of items in list
        /// </summary>
        private ToolStripItem[] GetBasicHousekeepingMenuItems()
        {
            var toolStripItems = new List<ToolStripItem>
            {
                new ToolStripMenuItem("Select All", null, HandleSelectAll, "SelectAll"),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Delete selected rows", null, HandleDeleteSelectedRows, "DeleteSelectedRows"),
                new ToolStripMenuItem("Delete all except selected rows", null, HandleDeleteNotSelectedRows,
                                      "DeleteNonSelectedRows")
            };
            return toolStripItems.ToArray();
        }

        /// <summary>
        /// Handlers for menu item events for basic context menu operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSelectAll(object sender, EventArgs e)
        {
            SelectAllRows();
        }
        private void HandleDeleteSelectedRows(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }
        private void HandleDeleteNotSelectedRows(object sender, EventArgs e)
        {
            DeleteNotSelectedItems();
        }

        // Copy Menus

        /// <summary>
        /// Builds as set of menu items for actions that copy contents of
        /// List to clipboard
        /// </summary>
        private ToolStripItem[] GetCopyMenuItems()
        {
            var toolStripItems = new List<ToolStripItem>
            {
                new ToolStripMenuItem("Copy selected rows", null, HandleCopyRows, "CopySelectedRows")
            };
            return toolStripItems.ToArray();
        }

        /// <summary>
        /// Handlers for copy menu item events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCopyRows(object sender, EventArgs e)
        {
            CopySelectedRows();
        }

        /// <summary>
        /// Copy list of values in specified column to the clipboard
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="delimiter"></param>
        private void CopyColumnList(string colName, string delimiter)
        {
            var sourceColumnIndex = PanelSupport.GetColumnIndex(lvQueryResults, colName);

            if (sourceColumnIndex == -1)
                return;

            var delimiterToUse = PanelSupport.GetColumnDataCopyDelimiter(delimiter);

            var copiedText = new StringBuilder(4096);
            var itemIndex = 0;
            foreach (ListViewItem objRow in lvQueryResults.SelectedItems)
            {
                if (itemIndex > 0)
                    copiedText.Append(delimiterToUse);

                copiedText.Append(objRow.SubItems[sourceColumnIndex].Text);
                itemIndex++;
            }

            Clipboard.SetText(copiedText.ToString());
        }

        /// <summary>
        /// Copies the selected rows to the clipboard
        /// </summary>
        private void CopySelectedRows()
        {
            // Copy the selected data to the clipboard
            // Include the column names if more than one row is selected (or if the ListView only contains one row)
            var copiedText = new StringBuilder(4096);
            if (lvQueryResults.Items.Count == 1 || lvQueryResults.SelectedItems.Count > 1)
            {
                // Populate strText with the column names
                for (var i = 0; i < lvQueryResults.Columns.Count; i++)
                {
                    if (i > 0)
                        copiedText.Append("\t");

                    copiedText.Append(lvQueryResults.Columns[i].Name);
                }
                copiedText.Append(Environment.NewLine);
            }

            var intRowIndex = 0;
            foreach (ListViewItem objRow in lvQueryResults.SelectedItems)
            {
                if (intRowIndex > 0)
                    copiedText.Append(Environment.NewLine);

                for (var i = 0; i < objRow.SubItems.Count; i++)
                {
                    if (i > 0)
                        copiedText.Append("\t");

                    copiedText.Append(PanelSupport.FixNull(objRow.SubItems[i].Text));
                }
                intRowIndex++;
            }
            Clipboard.SetText(copiedText.ToString());
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
            if (SelectedItemCount == 0)
            {
                List<string> lstAlwaysActive = null;

                if (ItemCount > 0)
                    lstAlwaysActive = mSelectAllMenuItems;

                AdjustMenuItemsFromNameList(mAllMenuItems, false, lstAlwaysActive);
            }
            else
            {
                AdjustMenuItemsFromNameList(mAllMenuItems, true, null);
            }
        }

        /// <summary>
        /// Set the enabled state of the menu items in the ListDisplay ListView
        /// (using name property of menu item) from the given list of menu item names.
        /// </summary>
        /// <param name="itemNames"></param>
        /// <param name="active"></param>
        /// <param name="lstAlwaysActive"></param>
        private void AdjustMenuItemsFromNameList(IEnumerable<string> itemNames, bool active, ICollection<string> lstAlwaysActive)
        {
            foreach (var name in itemNames)
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                foreach (var tsi in lvQueryResults.ContextMenuStrip.Items.Find(name, true))
                {
                    if (lstAlwaysActive != null && lstAlwaysActive.Contains(name))
                        tsi.Enabled = true;
                    else
                        tsi.Enabled = active;
                }
            }
        }
    }
}
