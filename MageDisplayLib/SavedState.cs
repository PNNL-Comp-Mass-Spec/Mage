using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// This class provides support for saving and restoring configuration files
    /// and the state of UI fields for UI component panels
    /// </summary>
    public static class SavedState
    {
        // Ignore Spelling: floyd, Mage, utf

        private const string QUERY_DEFS_FILE = "QueryDefinitions.xml";

        private const string SAVED_STATE_FILE = "SavedState.xml";

        /// <summary>
        /// Path to directory that contains config data
        /// </summary>
        public static string DataDirectory { get; set; }

        /// <summary>
        /// Path to saved state file
        /// </summary>
        public static string FilePath { get; set; }

        /// <summary>
        /// Set global paths to reference where config files live
        /// and assure that valid directory exists and valid copies
        /// of the basic config files.
        /// </summary>
        public static void SetupConfigFiles(string configDirectoryName)
        {
            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), configDirectoryName);

            // Make sure application data directory exists
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            // Set default path for query definitions config file
            ModuleDiscovery.QueryDefinitionFileName = Path.Combine(DataDirectory, QUERY_DEFS_FILE);
            if (!File.Exists(ModuleDiscovery.QueryDefinitionFileName))
            {
                var ioQueryDef = new FileInfo(QUERY_DEFS_FILE);
                if (ioQueryDef.Exists)
                {
                    File.Copy(QUERY_DEFS_FILE, ModuleDiscovery.QueryDefinitionFileName);
                }
                else
                {
                    throw new FileNotFoundException("Query Definitions file not found; please copy " + ioQueryDef.Name + " from the appropriate subdirectory at \\\\floyd\\software\\Mage\\Exe_Only to " + ioQueryDef.Directory);
                }
            }
            else
            {
                var fiInfo = new FileInfo(QUERY_DEFS_FILE);
                var fcInfo = new FileInfo(ModuleDiscovery.QueryDefinitionFileName);
                if (fiInfo.LastWriteTimeUtc > fcInfo.LastWriteTimeUtc)
                {
                    File.Copy(QUERY_DEFS_FILE, ModuleDiscovery.QueryDefinitionFileName, true);
                }
            }

            // Setup to save and restore settings for UI component panels
            FilePath = Path.Combine(DataDirectory, SAVED_STATE_FILE);

            // Tell modules where to look for loadable module DLLs
            var fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            ModuleDiscovery.ExternalModuleDirectory = fi.DirectoryName;
        }

        /// <summary>
        /// Save the settings for parameter panels in the master list
        /// that are visible via their IModuleParameters interface
        /// </summary>
        /// <param name="panelList">List of UI component panels</param>
        public static void SaveParameters(Dictionary<string, IModuleParameters> panelList)
        {
            var sb = new StringBuilder();

            // Make XML header and opening root element
            sb.AppendLine("<?xml version='1.0' encoding='utf-8' ?>");
            sb.AppendLine("<parameters>");

            // Step through panel list and add XML parameter definitions for each parameter
            // for each panel that has an IModuleParameters interface
            const string LINE_FORMAT = "<parameter panel='{0}' name='{1}' value='{2}' />";

            foreach (var panelDesc in panelList)
            {
                var paramPanel = panelDesc.Key;

                foreach (var paramDesc in panelDesc.Value.GetParameters())
                {
                    var paramName = paramDesc.Key;
                    var paramValue = paramDesc.Value;
                    sb.AppendFormat(LINE_FORMAT, paramPanel, paramName, paramValue).AppendLine();
                }
            }
            // make closing XML root element
            sb.AppendLine("</parameters>");

            // Dump XML to file
            var mOutFile = new StreamWriter(FilePath);
            mOutFile.WriteLine(sb.ToString());
            mOutFile.Close();
        }

        /// <summary>
        /// Restore the settings for parameter panels in the master list
        /// that are visible via their IModuleParameters interface
        /// </summary>
        /// <param name="panelList">List of UI component panels</param>
        public static void RestoreSavedPanelParameters(Dictionary<string, IModuleParameters> panelList)
        {
            if (!File.Exists(FilePath))
                return;

            // Get XML containing saved panel parameters
            var doc = new XmlDocument();
            doc.Load(FilePath);

            // Get list of parameter nodes
            var parms = doc.SelectNodes(".//parameter");

            // For each panel in list, collect its parameters from the XML node list
            // and set them for the panel
            foreach (var listPanel in panelList.Keys)
            {
                var parameterList = new Dictionary<string, string>();
                if (parms != null)
                {
                    foreach (XmlNode parameter in parms)
                    {
                        if (parameter.Attributes == null)
                            continue;

                        var paramPanel = parameter.Attributes["panel"].InnerText;
                        var paramName = parameter.Attributes["name"].InnerText;
                        var paramValue = parameter.Attributes["value"].InnerText;

                        if (paramPanel == listPanel)
                        {
                            parameterList[paramName] = paramValue;
                        }
                    }
                }

                var panel = panelList[listPanel];
                panel.SetParameters(parameterList);
            }
        }
    }
}
