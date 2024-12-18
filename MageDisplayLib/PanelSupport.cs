﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// Utility functions for common tasks in support of user command UI panels
    /// </summary>
    public static class PanelSupport
    {
        // Ignore Spelling: Mage

        /// <summary>
        /// Return the control that implements the Mage IModuleParameters interface and is contained in the given control or its descendant control hierarchy
        /// </summary>
        /// <remarks>Only return the first control found (in the current design approach, each panel should only have one control)</remarks>
        /// <param name="controlToExamine"></param>
        public static IModuleParameters GetParameterPanel(Control controlToExamine)
        {
            var userControlList = new List<Control>();
            AddControlsToList(controlToExamine, typeof(UserControl), userControlList);

            foreach (var userControl in userControlList)
            {
                if (userControl is IModuleParameters control)
                {
                    return control;
                }
            }

            return null;
        }

        /// <summary>
        /// Walk down the control hierarchy of the subjectControl and look for
        /// UserControl controls that have "OnAction" event, and wire it up
        /// to the method contained in methodInfo.
        /// </summary>
        /// <param name="subjectControl"></param>
        /// <param name="methodInfo"></param>
        public static void DiscoverAndConnectCommandHandlers(Control subjectControl, MethodInfo methodInfo)
        {
            // go through list of user controls looking for those that have command event "OnAction"
            // and wire them to the command handler function
            var userPanelList = new List<Control>();
            AddControlsToList(subjectControl, typeof(UserControl), userPanelList);
            foreach (var panel in userPanelList)
            {
                var eventInfo = panel.GetType().GetEvent("OnAction");
                if (eventInfo != null)
                { // Panel defines the command event, wire it up
                    var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, subjectControl, methodInfo);
                    eventInfo.AddEventHandler(panel, handler);
                }
            }
        }

        /// <summary>
        /// Build list of controls of the specified type
        /// that are contained in the given control or its children
        /// (uses recursion to walk down the descendant hierarchy)
        /// </summary>
        /// <param name="subjectControl">given control to examine</param>
        /// <param name="controlType">specified type of control to collect into list</param>
        /// <param name="controlList">list to which to add controls of specified type</param>
        private static void AddControlsToList(Control subjectControl, Type controlType, ICollection<Control> controlList)
        {
            foreach (Control ctrl in subjectControl.Controls)
            {
                if (controlType.IsInstanceOfType(ctrl))
                {
                    controlList.Add(ctrl);
                }
                AddControlsToList(ctrl, controlType, controlList);
            }
        }

        /// <summary>
        /// Get list for all panels that have parameters interface
        /// (this list is primarily used for saving and restoring panel parameters)
        /// </summary>
        public static Dictionary<string, IModuleParameters> GetParameterPanelList(Control subjectControl)
        {
            var outputList = new Dictionary<string, IModuleParameters>();
            var paramPanelList = new List<Control>();
            AddControlsToList(subjectControl, typeof(IModuleParameters), paramPanelList);
            foreach (var ctrl in paramPanelList)
            {
                outputList.Add(ctrl.Name, (IModuleParameters)ctrl);
            }
            return outputList;
        }

        /// <summary>
        /// Remove duplicates and missing values and return clean list
        /// </summary>
        /// <param name="rawList"></param>
        public static string CleanUpDelimitedList(string rawList)
        {
            rawList = rawList.Replace("\t", ",");
            rawList = rawList.Replace(" ", ",");
            rawList = rawList.Replace(Environment.NewLine, ", ").TrimEnd(',', ' ');
            var cleanList = string.Empty;

            var items = new Dictionary<string, string>();
            foreach (var field in rawList.Split(','))
            {
                var s = field.Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    items[s] = string.Empty;
                }
            }
            if (items.Count > 0)
            {
                cleanList = string.Join(", ", items.Keys);
            }
            return cleanList;
        }

        /// <summary>
        /// Convert the value to a string, treating null as an empty string
        /// </summary>
        /// <param name="value"></param>
        public static string FixNull(object value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        /// <summary>
        /// Get a delimiter appropriate for use when copying the data values in a column
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns>Newline if delimiter is null empty; tab if delimiter is tab, and delimiter plus space otherwise</returns>
        public static string GetColumnDataCopyDelimiter(string delimiter)
        {
            string delimiterToUse;
            if (string.IsNullOrEmpty(delimiter))
            {
                delimiterToUse = Environment.NewLine;
            }
            else
            {
                if (delimiter.StartsWith("\t") || delimiter.EndsWith(" "))
                    delimiterToUse = delimiter;
                else
                    delimiterToUse = delimiter + " ";
            }

            return delimiterToUse;
        }

        /// <summary>
        /// Return the index to the given column
        /// </summary>
        /// <param name="displayView">DataGrid view</param>
        /// <param name="colName">Name of column to get index for</param>
        /// <returns>Position of column in item array; -1 if not found</returns>
        public static int GetColumnIndex(DataGridView displayView, string colName)
        {
            var columnDef = displayView.Columns[colName];
            if (columnDef == null)
            {
                return -1;
            }

            return columnDef.Index;
        }

        /// <summary>
        /// Return the index to the given column
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="colName">Name of column to get index for</param>
        /// <returns>Position of column in item array</returns>
        public static int GetColumnIndex(ListView listView, string colName)
        {
            var i = listView.Columns.IndexOfKey(colName);
            return i;
        }

        /// <summary>
        /// Get values in given column for currently selected items in display list
        /// </summary>
        /// <param name="displayView">DataGrid View</param>
        /// <param name="colName">Column name to get values from</param>
        /// <returns>List of contents of column for each selected row</returns>
        public static List<string> GetItemList(DataGridView displayView, string colName)
        {
            var items = new List<string>();
            var i = GetColumnIndex(displayView, colName);
            if (i >= 0)
            {
                foreach (DataGridViewRow objRow in displayView.SelectedRows)
                {
                    items.Add(objRow.Cells[i].Value.ToString());
                }
            }
            return items;
        }

        /// <summary>
        /// Get values in given column for currently selected items in display list
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="colName">Column name to get values from</param>
        /// <returns>List of contents of column for each selected row</returns>
        public static List<string> GetItemList(ListView listView, string colName)
        {
            var items = new List<string>();
            var i = GetColumnIndex(listView, colName);
            if (i >= 0)
            {
                foreach (ListViewItem objRow in listView.SelectedItems)
                {
                    items.Add(objRow.SubItems[i].Text);
                }
            }
            return items;
        }

        /// <summary>
        /// Launch the default web browser with URL
        /// </summary>
        /// <param name="displayView">DataGrid View</param>
        /// <param name="url">Base URL</param>
        /// <param name="columnName">column to get trailing URL segment from</param>
        public static void LaunchWebBrowser(DataGridView displayView, string url, string columnName)
        {
            if (displayView.SelectedRows.Count == 0)
            {
                MessageBox.Show("No rows selected");
                return;
            }

            var itemList = GetItemList(displayView, columnName);

            TryLaunchWebBrowser(url, columnName, itemList);
        }

        /// <summary>
        /// Launch the default web browser with URL
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="url">Base URL</param>
        /// <param name="columnName">column to get trailing URL segment from</param>
        public static void LaunchWebBrowser(ListView listView, string url, string columnName)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("No rows selected");
                return;
            }

            var itemList = GetItemList(listView, columnName);

            TryLaunchWebBrowser(url, columnName, itemList);
        }

        /// <summary>
        /// Opens a Windows Explorer window with contents of given column
        /// as file path to open
        /// </summary>
        /// <param name="displayView">DataGrid View</param>
        /// <param name="columnName"></param>
        public static void OpenWindowsExplorer(DataGridView displayView, string columnName)
        {
            if (displayView.SelectedRows.Count == 0)
            {
                MessageBox.Show("No rows selected");
                return;
            }

            var itemList = GetItemList(displayView, columnName);

            if (itemList.Count == 0 && columnName.Equals("Directory", StringComparison.OrdinalIgnoreCase))
            {
                var alternateItemList = GetItemList(displayView, "Folder");
                itemList.AddRange(alternateItemList);
            }

            TryOpenWindowsExplorer(columnName, itemList);
        }

        /// <summary>
        /// Opens a Windows Explorer window with contents of given column
        /// as file path to open
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="columnName"></param>
        public static void OpenWindowsExplorer(ListView listView, string columnName)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("No rows selected");
                return;
            }

            var itemList = GetItemList(listView, columnName);

            if (itemList.Count == 0 && columnName.Equals("Directory", StringComparison.OrdinalIgnoreCase))
            {
                var alternateItemList = GetItemList(listView, "Folder");
                itemList.AddRange(alternateItemList);
            }

            TryOpenWindowsExplorer(columnName, itemList);
        }

        private static void TryLaunchWebBrowser(string url, string columnName, IReadOnlyList<string> itemList)
        {
            if (itemList.Count == 0)
            {
                MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
            }
            else
            {
                Process.Start(url + itemList[0]);
            }
        }

        private static void TryOpenWindowsExplorer(string columnName, IReadOnlyList<string> itemList)
        {
            if (itemList.Count == 0)
            {
                MessageBox.Show(string.Format("Column '{0}' not present in row", columnName));
            }
            else
            {
                var directoryPath = itemList[0];
                Process.Start("explorer.exe", directoryPath);
            }
        }
    }
}
