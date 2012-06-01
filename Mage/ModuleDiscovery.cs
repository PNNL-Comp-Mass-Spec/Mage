using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace Mage {

    /// <summary>
    /// Supports discovery and dynamic loading of Mage pipeline modules
    /// with special support for filters and their associated parameter panels
    /// </summary>
    public sealed class ModuleDiscovery {

        private ModuleDiscovery() {
        }

        // static constructor
        static ModuleDiscovery() {
            LoadableModuleFileNamePrefix = "MageExt";
            QueryDefinitionFileName = "QueryDefinitions.xml";
        }

        #region Member Variables

        #endregion

        #region Properties

        /// <summary>
        /// Folder path where this class will search for DLLs that contain
        /// loabable Mage modules (including content filter modules and associated parameter panels)
        /// </summary>
        public static string ExternalModuleFolder { get; set; }

        /// <summary>
        /// Prefix that this class will require file names of DLLs to 
        /// have in order to search them for loadable Mage modules
        /// </summary>
        public static string LoadableModuleFileNamePrefix { get; set; }

        /// <summary>
        /// Name of predefined query file
        /// </summary>
        public static string QueryDefinitionFileName { get; set; }

        #endregion

        #region General Discovery Functions

        /// <summary>
        /// Looks for a class with the given class name in the canonical
        /// assemblies already loaded for the parent application and
        /// also searches assembly DLLs that are in the folder given 
        /// by the ExternalModuleFolder property and tagged with the
        /// file name prefix given by the LoadableModuleFileNamePrefix property.
        /// 
        /// Returns a .Net Type object suitable for further examination or
        /// instantiation by the caller.
        /// </summary>
        /// <param name="ClassName">name of class to search for</param>
        /// <returns></returns>
        public static Type GetModuleTypeFromClassName(string ClassName) {
            Type modType = null;
            if (modType == null) {
                // is the module class in the executing assembly?
                Assembly ae = Assembly.GetExecutingAssembly(); //GetType().Assembly;
                //modType = ae.GetType(ClassName); // should work, but doesn't
                //string ne = ae.GetName().Name;
                //modType = Type.GetType(ne + "." + ClassName); // does work, but do it the long way for consistency
                modType = GetClassTypeFromAssembly(ClassName, ae);
            }
            if (modType == null) {
                // is the module class in the main assembly?
                Assembly aa = Assembly.GetEntryAssembly();
                modType = GetClassTypeFromAssembly(ClassName, aa);
            }
            if (modType == null) {
                // is the module class in the assembly of the code that called us?
                Assembly ac = Assembly.GetCallingAssembly(); //GetType().Assembly;
                //string nc = ac.GetName().Name;
                // modType = Type.GetType(nc + "." + ClassName); // should work, but doesn't
                modType = GetClassTypeFromAssembly(ClassName, ac);
            }
            if (modType == null) {
                // is the module class found in a loadable assembly?
                if (ExternalModuleFolder != null) {
                    DirectoryInfo di = new DirectoryInfo(ExternalModuleFolder);
                    FileInfo[] dllFiles = di.GetFiles(LoadableModuleFileNamePrefix + "*.dll");
                    foreach (FileInfo fi in dllFiles) {
                        string DLLName = fi.Name;
                        string path = Path.Combine(ExternalModuleFolder, DLLName);
                        Assembly af = Assembly.LoadFrom(path);
                        modType = GetClassTypeFromAssembly(ClassName, af);
                        if (modType != null) break; // we found it, don't keep looking
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
        private static Type GetClassTypeFromAssembly(string ClassName, Assembly assembly) {
            Type modType = null;
            if (assembly != null) {
                Type[] ts = assembly.GetTypes();
                foreach (Type t in ts) {
                    if (t.Name == ClassName) {
                        modType = t;
                        break;
                    }
                }
            }
            return modType;
        }

        #endregion

        #region Filter Discovery support

        /// <summary>
        /// list of attributes for filters and parameter panels
        /// </summary>
        private static Collection<MageAttribute> mFilterList = new Collection<MageAttribute>();

        /// <summary>
        /// list of attributes for filters, and filter panels, indexed by ID 
        /// </summary>
        private static Dictionary<string, MageAttribute> mFilters = new Dictionary<string, MageAttribute>();
        private static Dictionary<string, MageAttribute> mPanels = new Dictionary<string, MageAttribute>();

        /// <summary>
        /// list of attributes for filters, indexed by label
        /// </summary>
        private static Dictionary<string, MageAttribute> mFiltersByLabel = new Dictionary<string, MageAttribute>();

        /// <summary>
        /// get list of filter labels (for display)
        /// </summary>
        public static Collection<string> FilterLabels {
            get {
                Collection<string> labels = new Collection<string>();
                foreach (MageAttribute ma in mFilters.Values) {
                    labels.Add(ma.ModLabel);
                }
                return labels;
            }
        }

        /// <summary>
        /// Get list of filters
        /// </summary>
        public static Collection<MageAttribute> Filters {
            get {
                return new Collection<MageAttribute>(mFilters.Values.ToArray());
            }
        }

        /// <summary>
        /// Get Mage attributes for filters
        /// </summary>
        public static MageAttribute GetFilterAttributes(string filterName) {
            return mFilters[filterName];
        }

        /// <summary>
        /// return class name for given filter label
        /// </summary>
        /// <param name="filterLabel"></param>
        /// <returns></returns>
        public static string SelectedFilterClassName(string filterLabel) {
            string sel = "";
            if (mFiltersByLabel.ContainsKey(filterLabel)) {
                sel = mFiltersByLabel[filterLabel].ModClassName;
            }
            return sel;
        }

        /// <summary>
        /// find name of parameter panel associated with given filter label, if there is one
        /// </summary>
        /// <param name="filterLabel"></param>
        /// <returns></returns>
        public static string GetParameterPanelForFilter(string filterLabel) {
            string panelClass = "";
            if (mFiltersByLabel.ContainsKey(filterLabel)) {
                string ID = mFiltersByLabel[filterLabel].ModID;
                if (mPanels.ContainsKey(ID)) {
                    panelClass = mPanels[ID].ModClassName;
                }
            }
            return panelClass;
        }

        /// <summary>
        /// discover filter modules and their associated parameter panels
        /// and set up the necessary internal properties, components, and variables
        /// </summary>
        public static void SetupFilters() {
            mFilterList.Clear();
            mFilterList = ModuleDiscovery.FindFilters();
            foreach (MageAttribute ma in mFilterList) {
                if (ma.ModType == "Filter") {
                    mFilters.Add(ma.ModID, ma);
                    mFiltersByLabel.Add(ma.ModLabel, ma);
                } else if (ma.ModType == "FilterPanel") {
                    mPanels.Add(ma.ModID, ma);
                }
            }
        }

        /// <summary>
        /// find filter modules in main assembly and loadable assemblies
        /// and add to the internal master list
        /// </summary>
        /// <returns></returns>
        public static Collection<MageAttribute> FindFilters() {
            // list to hold info about discovered filters
            Collection<MageAttribute> filterList = new Collection<MageAttribute>();

            // list to hold classes that we will look at
            List<Type> classesToExamine = new List<Type>();

            // add classes from main assembly
            classesToExamine.AddRange(Assembly.GetEntryAssembly().GetTypes());

            // get classes from loadable DLLs
            DirectoryInfo di = new DirectoryInfo(ExternalModuleFolder);
            List<FileInfo> dllFiles = new List<FileInfo>();
            dllFiles.AddRange(di.GetFiles(LoadableModuleFileNamePrefix + "*.dll"));
            foreach (FileInfo fi in dllFiles) {
                string DLLName = fi.Name;
                string path = Path.Combine(ExternalModuleFolder, DLLName);
                classesToExamine.AddRange(Assembly.LoadFrom(path).GetTypes());
            }

            // look at each class in list to see if it is marked with
            // Mage attributes and examine them to find filter modules
            foreach (Type modType in classesToExamine) {
                Console.WriteLine(modType.ToString());
                object[] atrbs = modType.GetCustomAttributes(false);
                foreach (object obj in atrbs) {
                    if (obj.GetType() == typeof(MageAttribute)) {
                        MageAttribute ma = (MageAttribute)obj;
                        ma.ModClassName = modType.Name;
                        filterList.Add(ma);
                    }
                }
            }
            return filterList;
        }

        #endregion

        #region Predefined Query Definition support

        /// <summary>
        /// Get XML definition for query with given name
        /// from external XML query defintion file
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        public static string GetQueryXMLDef(string queryName) {
            XmlDocument doc = new XmlDocument();
            doc.Load(QueryDefinitionFileName);

            // find query node by name
            string xpath = string.Format(".//query[@name='{0}']", queryName);
            XmlNode queryNode = doc.SelectSingleNode(xpath);
            return (queryNode != null) ? queryNode.OuterXml : "";
        }


        #endregion

    }
}
