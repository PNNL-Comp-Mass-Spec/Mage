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
        /// Directory path where this class will search for DLLs that contain loadable Mage modules
        /// </summary>
        [Obsolete("Use ExternalModuleDirectory")]
        public static string ExternalModuleFolder {
            get => ExternalModuleDirectory;
            set => ExternalModuleDirectory = value;
        }

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
        public static string DMSServerOverride;

        /// <summary>
        /// DMS database name to use instead of the name defined in the QueryDefinitions.xml file
        /// </summary>
        /// <remarks>
        /// This allows the user to update the .exe.config file with a specific server and database,
        /// without needing to also update the QueryDefinitions.xml file
        /// </remarks>
        public static string DMSDatabaseOverride;

        // General discovery methods

        /// <summary>
        /// Looks for a class with the given class name in the canonical
        /// assemblies already loaded for the parent application and
        /// also searches assembly DLLs that are in the directory given
        /// by the ExternalModuleDirectory property and tagged with the
        /// file name prefix given by the LoadableModuleFileNamePrefix property.
        ///
        /// Returns a .Net Type object suitable for further examination or
        /// instantiation by the caller.
        /// </summary>
        /// <param name="ClassName">name of class to search for</param>
        /// <returns></returns>
        public static Type GetModuleTypeFromClassName(string ClassName)
        {
            // Is the module class in the executing assembly?
            var ae = Assembly.GetExecutingAssembly(); // GetType().Assembly;
                                                      // modType = ae.GetType(ClassName); // should work, but doesn't
                                                      // string ne = ae.GetName().Name;
                                                      // modType = Type.GetType(ne + "." + ClassName); // does work, but do it the long way for consistency
            var modType = GetClassTypeFromAssembly(ClassName, ae);

            if (modType == null)
            {
                // Is the module class in the main assembly?
                var aa = Assembly.GetEntryAssembly();
                modType = GetClassTypeFromAssembly(ClassName, aa);
            }
            if (modType == null)
            {
                // Is the module class in the assembly of the code that called us?
                var ac = Assembly.GetCallingAssembly(); // GetType().Assembly;
                // string nc = ac.GetName().Name;
                // modType = Type.GetType(nc + "." + ClassName); // should work, but doesn't
                modType = GetClassTypeFromAssembly(ClassName, ac);
            }
            if (modType == null)
            {
                // Is the module class found in a loadable assembly?
                if (ExternalModuleDirectory != null)
                {
                    var di = new DirectoryInfo(ExternalModuleDirectory);
                    var dllFiles = di.GetFiles(LoadableModuleFileNamePrefix + "*.dll");
                    foreach (var fi in dllFiles)
                    {
                        var DLLName = fi.Name;
                        var path = Path.Combine(ExternalModuleDirectory, DLLName);
                        var af = Assembly.LoadFrom(path);
                        modType = GetClassTypeFromAssembly(ClassName, af);
                        if (modType != null)
                            break; // We found it, don't keep looking
                    } // foreach
                }
            }
            return modType;
        }

        /// <summary>
        /// Look in the given assembly object for a class with the given name.
        ///
        /// We should have been able to use assembly.GetType("className")
        /// instead of doing this function, but it doesn't seem to work.  This is the work-around
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static Type GetClassTypeFromAssembly(string ClassName, Assembly assembly)
        {
            Type modType = null;
            if (assembly != null)
            {
                var ts = assembly.GetTypes();
                foreach (var t in ts)
                {
                    if (t.Name == ClassName)
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
        /// Get list of filter labels (for display)
        /// </summary>
        [Obsolete("Unused")]
        public static Collection<string> FilterLabels
        {
            get
            {
                var labels = new Collection<string>();
                foreach (var ma in mFilters.Values)
                {
                    labels.Add(ma.ModLabel);
                }
                return labels;
            }
        }

        /// <summary>
        /// Get list of filters
        /// </summary>
        public static IEnumerable<MageAttribute> Filters => new Collection<MageAttribute>(mFilters.Values.ToArray());

        /// <summary>
        /// Get Mage attributes for filters
        /// </summary>
        [Obsolete("Unused")]
        public static MageAttribute GetFilterAttributes(string filterName)
        {
            return mFilters[filterName];
        }

        /// <summary>
        /// Return class name for given filter label
        /// </summary>
        /// <param name="filterLabel"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static string GetParameterPanelForFilter(string filterLabel)
        {
            if (mFiltersByLabel.ContainsKey(filterLabel))
            {
                var ID = mFiltersByLabel[filterLabel].ModID;
                if (mPanels.ContainsKey(ID))
                {
                    return mPanels[ID].ModClassName;
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
            foreach (var ma in mFilterList)
            {
                if (ma.ModType == "Filter")
                {
                    mFilters.Add(ma.ModID, ma);
                    mFiltersByLabel.Add(ma.ModLabel, ma);
                }
                else if (ma.ModType == "FilterPanel")
                {
                    mPanels.Add(ma.ModID, ma);
                }
            }
        }

        /// <summary>
        /// Find filter modules in main assembly and loadable assemblies
        /// and add to the internal master list
        /// </summary>
        /// <returns></returns>
        public static Collection<MageAttribute> FindFilters()
        {
            // List to hold info about discovered filters
            var filterList = new Collection<MageAttribute>();

            // List to hold classes that we will look at
            var classesToExamine = new List<Type>();

            // Add classes from main assembly
            classesToExamine.AddRange(Assembly.GetEntryAssembly().GetTypes());

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
        /// <returns></returns>
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

            if (string.IsNullOrWhiteSpace(DMSServerOverride) && string.IsNullOrWhiteSpace(DMSDatabaseOverride))
            {
                var nodeXML = queryNode.OuterXml;
                return nodeXML;
            }

            return UpdateQueryXMLConnectionInfo(queryNode, DMSServerOverride, DMSDatabaseOverride);
        }

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
