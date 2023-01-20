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
            // Example data directory path:
            // C:\Users\D3L243\AppData\Roaming\MageFileProcessor

            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), configDirectoryName);

            // Make sure application data directory exists
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            // Set default path for query definition config file
            ModuleDiscovery.QueryDefinitionFileName = Path.Combine(DataDirectory, QUERY_DEFS_FILE);

            // This is the query definition config file in the same directory as the .exe
            var referenceQueryDefFile = new FileInfo(QUERY_DEFS_FILE);

            // This is the query definition config file in the user's application data directory
            var userQueryDefFile = new FileInfo(ModuleDiscovery.QueryDefinitionFileName);

            if (!userQueryDefFile.Exists)
            {
                if (referenceQueryDefFile.Exists)
                {
                    File.Copy(QUERY_DEFS_FILE, ModuleDiscovery.QueryDefinitionFileName);
                }
                else
                {
                    throw new FileNotFoundException("Query Definition file not found; please copy " + referenceQueryDefFile.Name + " from the appropriate subdirectory at \\\\floyd\\software\\Mage\\Exe_Only to " + referenceQueryDefFile.Directory);
                }
            }
            else
            {
                bool copyRequired;

                if (referenceQueryDefFile.Exists)
                {
                    if (referenceQueryDefFile.LastWriteTimeUtc > userQueryDefFile.LastWriteTimeUtc)
                    {
                        copyRequired = true;
                    }
                    else
                    {
                        copyRequired = !MatchingConnectionInfo(referenceQueryDefFile, userQueryDefFile);
                    }
                }
                else
                {
                    copyRequired = false;
                }

                if (copyRequired)
                {
                    File.Copy(referenceQueryDefFile.FullName, userQueryDefFile.FullName, true);
                }
            }

            // Setup to save and restore settings for UI component panels
            FilePath = Path.Combine(DataDirectory, SAVED_STATE_FILE);

            // Tell modules where to look for loadable module DLLs
            var programExecutable = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            ModuleDiscovery.ExternalModuleDirectory = programExecutable.DirectoryName;
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

        /// <summary>
        /// Read the queries in a query definition file and return a dictionary with each query's name and connection info
        /// </summary>
        /// <param name="filePath">Query definition file</param>
        /// <returns>Dictionary where keys are query names and values are instances of class ConnectionInfo</returns>
        private static Dictionary<string, ConnectionInfo> LoadQueryDefinitionFile(string filePath)
        {
            var queries = new Dictionary<string, ConnectionInfo>();

            XmlDocument xmlDoc;
            using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
            }

            // Use an XPath query to select all of the query nodes
            var nodes = xmlDoc.SelectNodes("//queries/query");

            if (nodes == null)
                return queries;

            foreach (XmlNode query in nodes)
            {
                // Example node XML:
                // <query name='Jobs'>
                //     <connection server='prismdb1' database='dms' postgres='true' user='d3l243' /

                if (query.Attributes == null)
                {
                    continue;
                }

                var queryName = query.Attributes["name"].Value;

                if (queries.ContainsKey(queryName))
                {
                    // The same query is defined twice; ignore the duplicate
                    continue;
                }

                var connectionInfo = query.SelectSingleNode("connection");

                if (connectionInfo?.Attributes == null)
                {
                    continue;
                }

                var server = TryGetAttribute(connectionInfo, "server", string.Empty);
                var database = TryGetAttribute(connectionInfo, "database", string.Empty);
                var postgres = TryGetAttribute(connectionInfo, "postgres", false);
                var user = TryGetAttribute(connectionInfo, "user", string.Empty);

                var connection = new ConnectionInfo(server, database, user)
                {
                    Postgres = postgres
                };

                queries.Add(queryName, connection);
            }

            return queries;
        }

        /// <summary>
        /// Compare the connection info for each query in the query definition files
        /// </summary>
        /// <param name="sourceFile">Source query definition file (typically in the directory with the .exe)</param>
        /// <param name="targetFile">Target query definition file (typically in C:\Users\Username\AppData\Roaming\MageFileProcessor)</param>
        /// <returns>True if the connection info matches for all of the queries, false if any do not match</returns>
        private static bool MatchingConnectionInfo(FileSystemInfo sourceFile, FileSystemInfo targetFile)
        {
            var sourceQueries = LoadQueryDefinitionFile(sourceFile.FullName);

            var targetQueries = LoadQueryDefinitionFile(targetFile.FullName);

            return MatchingConnectionInfo(sourceQueries, targetQueries);
        }

        /// <summary>
        /// Compare the connection info for each query in the dictionaries
        /// </summary>
        /// <param name="sourceQueries">Dictionary where keys are query names and values are instances of class ConnectionInfo</param>
        /// <param name="targetQueries">Dictionary where keys are query names and values are instances of class ConnectionInfo</param>
        /// <returns></returns>
        private static bool MatchingConnectionInfo(IReadOnlyDictionary<string, ConnectionInfo> sourceQueries, IReadOnlyDictionary<string, ConnectionInfo> targetQueries)
        {
            foreach (var sourceQuery in sourceQueries)
            {
                if (!targetQueries.TryGetValue(sourceQuery.Key, out var targetConnectionInfo))
                {
                    // The source file has a query that the target file does not have
                    return false;
                }

                if (!MatchingConnectionInfo(sourceQuery.Value, targetConnectionInfo))
                {
                    // Connection details differ
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the connection info
        /// </summary>
        /// <param name="sourceConnectionInfo"></param>
        /// <param name="targetConnectionInfo"></param>
        /// <returns>True if both refer to the same server, database, and user (which could be an empty string)</returns>
        private static bool MatchingConnectionInfo(ConnectionInfo sourceConnectionInfo, ConnectionInfo targetConnectionInfo)
        {
            return sourceConnectionInfo.Postgres == targetConnectionInfo.Postgres &&
                   (sourceConnectionInfo.Server ?? string.Empty) == (targetConnectionInfo.Server ?? string.Empty) &&
                   (sourceConnectionInfo.Database ?? string.Empty) == (targetConnectionInfo.Database ?? string.Empty) &&
                   (sourceConnectionInfo.User ?? string.Empty) == (targetConnectionInfo.User ?? string.Empty);
        }

        /// <summary>
        /// Retrieve the string value for the given XML attribute
        /// </summary>
        /// <param name="node">XML node</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Value if found, or defaultValue if the named attribute does not exist</returns>
        private static string TryGetAttribute(XmlNode node, string attributeName, string defaultValue)
        {
            if (node.Attributes == null)
                return defaultValue;

            var value = node.Attributes.GetNamedItem(attributeName);

            return value == null ? defaultValue : value.Value;
        }

        /// <summary>
        /// Retrieve the boolean value for the given XML attribute
        /// </summary>
        /// <param name="node">XML node</param>
        /// <param name="attributeName">>Attribute name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Value if found, or defaultValue if the named attribute does not exist</returns>
        private static bool TryGetAttribute(XmlNode node, string attributeName, bool defaultValue)
        {
            var postgres = TryGetAttribute(node, attributeName, string.Empty);

            return bool.TryParse(postgres, out var isPostgres) ? isPostgres : defaultValue;
        }
    }
}
