using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace Mage
{
    /// <summary>
    /// Supports discovery and dynamic loading of Mage pipeline modules
    /// with special support for filters and their associated parameter panels
    /// </summary>
    public sealed class ModuleDiscovery
    {
        // Ignore Spelling: Mage

        /// <summary>
        /// Static Constructor
        /// </summary>
        static ModuleDiscovery()
        {
            LoadableModuleFileNamePrefix = "MageExt";
            QueryDefinitionFileName = "QueryDefinitions.xml";
        }

        /// <summary>
        /// Directory path where this class will search for DLLs that contain loadable Mage modules
        /// (including content filter modules and associated parameter panels)
        /// </summary>
        public static string ExternalModuleDirectory { get; set; }

        /// <summary>
        /// Prefix that this class will require file names of DLLs to
        /// have in order to search them for loadable Mage modules
        /// </summary>
        public static string LoadableModuleFileNamePrefix { get; set; }

        /// <summary>
        /// Name (or full path) of predefined query file
        /// </summary>
        public static string QueryDefinitionFileName { get; set; }

        /// <summary>
        /// DMS server name to use instead of the name defined in the QueryDefinitions.xml file
        /// </summary>
        /// <remarks>
        /// This allows the user to update the .exe.config file with a specific server and database,
        /// without needing to also update the QueryDefinitions.xml file
        /// </remarks>
        [Obsolete("Retired in January 2023 since the SavedState class now compares connection info in the default QueryDefinitions.xml file to the copy in the user's AppData directory; if the info differs, the file in AppData is replace with the default file")]
        public static string DMSServerOverride;

        /// <summary>
        /// DMS database name to use instead of the name defined in the QueryDefinitions.xml file
        /// </summary>
        /// <remarks>
        /// This allows the user to update the .exe.config file with a specific server and database,
        /// without needing to also update the QueryDefinitions.xml file
        /// </remarks>
        [Obsolete("Retired in January 2023")]
        public static string DMSDatabaseOverride;

        // General discovery methods

        /// <summary>
        /// Looks for a class with the given class name in the canonical
        /// assemblies already loaded for the parent application and
        /// also searches assembly DLLs that are in the directory given
        /// by the ExternalModuleDirectory property and tagged with the
        /// file name prefix given by the LoadableModuleFileNamePrefix property.
        /// </summary>
        /// <param name="className">name of class to search for</param>
        /// <returns>.NET Type object</returns>
        public static Type GetModuleTypeFromClassName(string className)
        {
            // Is the module class in the executing assembly?
            var executingAssembly = Assembly.GetExecutingAssembly();

            var modType = GetClassTypeFromAssembly(className, executingAssembly);

            if (modType == null)
            {
                // Is the module class in the main assembly?
                var entryAssembly = Assembly.GetEntryAssembly();
                modType = GetClassTypeFromAssembly(className, entryAssembly);
            }

            if (modType == null)
            {
                // Is the module class in the assembly of the code that called us?
                var callingAssembly = Assembly.GetCallingAssembly();
                modType = GetClassTypeFromAssembly(className, callingAssembly);
            }

            if (modType == null)
            {
                // Is the module class found in a loadable assembly?
                if (ExternalModuleDirectory != null)
                {
                    var moduleDirectory = new DirectoryInfo(ExternalModuleDirectory);

                    foreach (var fi in moduleDirectory.GetFiles(LoadableModuleFileNamePrefix + "*.dll"))
                    {
                        var DLLName = fi.Name;
                        var path = Path.Combine(ExternalModuleDirectory, DLLName);
                        var af = Assembly.LoadFrom(path);
                        modType = GetClassTypeFromAssembly(className, af);
                        if (modType != null)
                            break; // We found it, don't keep looking
                    }
                }
            }
            return modType;
        }

        /// <summary>
        /// Look in the given assembly object for a class with the given name.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="assembly"></param>
        private static Type GetClassTypeFromAssembly(string className, Assembly assembly)
        {
            Type modType = null;
            if (assembly != null)
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.Name == className)
                    {
                        modType = t;
                        break;
                    }
                }
            }
            return modType;
        }

        /// <summary>
        /// List of attributes for filters and parameter panels
        /// </summary>
        private static Collection<MageAttribute> mFilterList = new();

        /// <summary>
        /// List of attributes for filters, and filter panels, indexed by ID
        /// </summary>
        private static readonly Dictionary<string, MageAttribute> mFilters = new();
        private static readonly Dictionary<string, MageAttribute> mPanels = new();

        /// <summary>
        /// List of attributes for filters, indexed by label
        /// </summary>
        private static readonly Dictionary<string, MageAttribute> mFiltersByLabel = new();

        /// <summary>
        /// Get list of filters
        /// </summary>
        public static IEnumerable<MageAttribute> Filters => new Collection<MageAttribute>(mFilters.Values.ToArray());

        /// <summary>
        /// Return class name for given filter label
        /// </summary>
        /// <param name="filterLabel"></param>
        public static string SelectedFilterClassName(string filterLabel)
        {
            if (mFiltersByLabel.ContainsKey(filterLabel))
            {
                return mFiltersByLabel[filterLabel].ModClassName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Find name of parameter panel associated with given filter label, if there is one
        /// </summary>
        /// <param name="filterLabel"></param>
        public static string GetParameterPanelForFilter(string filterLabel)
        {
            if (mFiltersByLabel.ContainsKey(filterLabel))
            {
                var id = mFiltersByLabel[filterLabel].ModID;
                if (mPanels.ContainsKey(id))
                {
                    return mPanels[id].ModClassName;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Discover filter modules and their associated parameter panels
        /// and set up the necessary internal properties, components, and variables
        /// </summary>
        public static void SetupFilters()
        {
            mFilterList.Clear();
            mFilterList = FindFilters();

            foreach (var item in mFilterList)
            {
                if (item.ModType == MageAttribute.FILTER_MODULE)
                {
                    mFilters.Add(item.ModID, item);
                    mFiltersByLabel.Add(item.ModLabel, item);
                }
                else if (item.ModType == MageAttribute.FILTER_PANEL_MODULE)
                {
                    mPanels.Add(item.ModID, item);
                }
            }
        }

        /// <summary>
        /// Find filter modules in main assembly and loadable assemblies
        /// and add to the internal master list
        /// </summary>
        public static Collection<MageAttribute> FindFilters()
        {
            // List to hold info about discovered filters
            var filterList = new Collection<MageAttribute>();

            // List to hold classes that we will look at
            var classesToExamine = new List<Type>();

            var entryAssembly = Assembly.GetEntryAssembly() ?? throw new NullReferenceException("Unable to determine the entry assembly");

            // Add classes from main assembly
            classesToExamine.AddRange(entryAssembly.GetTypes());

            // Get classes from loadable DLLs
            var di = new DirectoryInfo(ExternalModuleDirectory);
            var dllFiles = new List<FileInfo>();
            dllFiles.AddRange(di.GetFiles(LoadableModuleFileNamePrefix + "*.dll"));

            foreach (var fi in dllFiles)
            {
                var DLLName = fi.Name;
                var path = Path.Combine(ExternalModuleDirectory, DLLName);
                classesToExamine.AddRange(Assembly.LoadFrom(path).GetTypes());
            }

            // Look at each class in list to see if it is marked with
            // Mage attributes and examine them to find filter modules
            foreach (var modType in classesToExamine)
            {
                Console.WriteLine(modType.ToString());
                var customAttributes = modType.GetCustomAttributes(false);
                foreach (var candidateAttribute in customAttributes)
                {
                    if (candidateAttribute is MageAttribute newFilter)
                    {
                        newFilter.ModClassName = modType.Name;
                        filterList.Add(newFilter);
                    }
                }
            }
            return filterList;
        }

        /// <summary>
        /// Get XML definition for query with given name
        /// from external XML query definition file
        /// </summary>
        /// <param name="queryName">Query Name</param>
        public static string GetQueryXMLDef(string queryName)
        {
            var doc = new XmlDocument();
            doc.Load(QueryDefinitionFileName);

            // Find query node by name
            var xpath = string.Format(".//query[@name='{0}']", queryName);
            var queryNode = doc.SelectSingleNode(xpath);

            if (queryNode == null)
            {
                return string.Empty;
            }

            return queryNode.OuterXml;

            // Code used prior to January 2023 to override the DMS server name and database name, causing the connection info in the QueryDefinitions.xml file to be ignored

            /*
            if (string.IsNullOrWhiteSpace(DMSServerOverride) && string.IsNullOrWhiteSpace(DMSDatabaseOverride))
            {
                return queryNode.OuterXml;
            }

            return UpdateQueryXMLConnectionInfo(queryNode, DMSServerOverride, DMSDatabaseOverride);
            */
        }

        [Obsolete("Retired in January 2023")]
        private static string UpdateQueryXMLConnectionInfo(XmlNode queryNode, string dmsServerOverride, string dmsDatabaseOverride)
        {
            const string xpathConnection = "//queries/query/connection";
            var nodeList = queryNode.SelectNodes(xpathConnection);

            if (nodeList == null || nodeList.Count == 0)
                return queryNode.OuterXml;

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes == null)
                    continue;

                foreach (XmlAttribute candidateAttribute in node.Attributes)
                {
                    if (candidateAttribute.Name.Equals("server", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(dmsServerOverride))
                    {
                        candidateAttribute.Value = dmsServerOverride;
                    }

                    if (candidateAttribute.Name.Equals("database", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(dmsServerOverride))
                    {
                        candidateAttribute.Value = dmsDatabaseOverride;
                    }
                }
            }

            return queryNode.OuterXml;
        }
    }
}
