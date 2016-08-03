using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using System.Reflection;

namespace MageDisplayLib
{

    /// <summary>
    /// Utility functions for common tasks in support of user command UI panels
    /// </summary>
    public static class PanelSupport
    {

        /// <summary>
        /// Return control that implements Mage IModuleParameters interface
        /// that is contained in the given control or its descendent control hierarchy
        /// Note: Only return one if there are many (which current design approach should rule out)
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static IModuleParameters GetParameterPanel(Control ctrl)
        {
            var userControlList = new List<Control>();
            AddControlsToList(ctrl, typeof(UserControl), userControlList);

            foreach (var userControl in userControlList)
            {
                var control = userControl as IModuleParameters;
                if (control != null)
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
                { // panel defines the command event, wire it up
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
        private static void AddControlsToList(Control subjectControl, Type controlType, List<Control> controlList)
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
        /// get list for all panels that have parameters interface
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
        /// remove duplicates and missing values and return clean list
        /// </summary>
        /// <param name="rawList"></param>
        /// <returns></returns>
        public static string CleanUpDelimitedList(string rawList)
        {
            rawList = rawList.Replace("\t", ",");
            rawList = rawList.Replace(" ", ",");
            rawList = rawList.Replace(Environment.NewLine, ", ").TrimEnd(',', ' ');
            var cleanList = "";

            var items = new Dictionary<string, string>();
            foreach (var field in rawList.Split(','))
            {
                var s = field.Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    items[s] = "";
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
        /// <returns></returns>
        public static string FixNull(object value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        /// <summary>
        /// Get a delimiter appropriate for use when copying all of the data values in a column
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
    }

}
