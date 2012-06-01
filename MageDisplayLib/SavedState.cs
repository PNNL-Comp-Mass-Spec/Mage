using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.IO;
using System.Xml;

namespace MageDisplayLib {


    /// <summary>
    /// This class provides support for saving and restoring configuration files
    /// and the state of UI fields for UI component panels
    /// </summary>
    public class SavedState {

        // names and paths for configuration directory and files
        private static string mAppDataFolderName;
        private static string mDataDirectory;
        private static string mQueryDefFileName = "QueryDefinitions.xml";
        private static string mSavedStateFileName = "SavedState.xml";

        /// <summary>
        /// Name of folder that contains config/state files for application
        /// </summary>
        public static string AppDataFolderName {
            get { return mAppDataFolderName; }
            set { mAppDataFolderName = value; }
        }

        /// <summary>
        /// Path to folder that contains config folder
        /// </summary>
        public static string DataDirectory {
            get { return mDataDirectory; }
            set { mDataDirectory = value; }
        }

        /// <summary>
        /// path to saved state file
        /// </summary>
        public static string FilePath { get; set; }


        /// <summary>
        /// private constructor - no instances can be created
        /// </summary>
        private SavedState() {
        }

        /// <summary>
        /// Set global paths to reference where config files live
        /// and assure that valild folder exists and valid copies
        /// of the basic config files.
        /// </summary>
        public static void SetupConfigFiles(string configFolderName) {

            mAppDataFolderName = configFolderName;
            mDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), mAppDataFolderName);

            // make sure application data directory exists
            if (!Directory.Exists(mDataDirectory)) {
                Directory.CreateDirectory(mDataDirectory);
            }

            //Set default path for query definitions config file
            ModuleDiscovery.QueryDefinitionFileName = Path.Combine(mDataDirectory, mQueryDefFileName);
            if (!File.Exists(ModuleDiscovery.QueryDefinitionFileName)) {
                FileInfo ioQueryDef = new FileInfo(mQueryDefFileName);
                if (ioQueryDef.Exists) {
                    File.Copy(mQueryDefFileName, ModuleDiscovery.QueryDefinitionFileName);
                } else{
                    throw new FileNotFoundException("Query Definitions file not found; please copy " + ioQueryDef.Name + " from the appropriate subdirectory at \\\\floyd\\software\\Mage\\Exe_Only to " + ioQueryDef.Directory);
                }
            } else {
                FileInfo fiInfo = new FileInfo(mQueryDefFileName);
                FileInfo fcInfo = new FileInfo(ModuleDiscovery.QueryDefinitionFileName);
                if (fiInfo.LastWriteTimeUtc > fcInfo.LastWriteTimeUtc) {
                    File.Copy(mQueryDefFileName, ModuleDiscovery.QueryDefinitionFileName, true);
                }
            }

            // setup to save and restore settings for UI component panels
            SavedState.FilePath = Path.Combine(mDataDirectory, mSavedStateFileName);

            // tell modules where to look for loadable module DLLs
            FileInfo fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            ModuleDiscovery.ExternalModuleFolder = fi.DirectoryName;
        }

        /// <summary>
        /// Save the settings for parameter panels in the master list
        /// that are visible via their IModuleParameters interface
        /// </summary>
        /// <param name="panelList">List of UI component panels</param>
        public static void SaveParameters(Dictionary<string, IModuleParameters> panelList) {
            StringBuilder sb = new StringBuilder();

            // make XML header and opening root element
            sb.AppendLine("<?xml version='1.0' encoding='utf-8' ?>");
            sb.AppendLine("<parameters>");

            // step through panel list and add XML parameter defitions for each parameter
            // for each panel that has an IModuleParameters interface
            string lineFormat = "<parameter panel='{0}' name='{1}' value='{2}' />";
            foreach (KeyValuePair<string, IModuleParameters> panelDesc in panelList) {
                string paramPanel = panelDesc.Key;
                Dictionary<string, string> parms = panelDesc.Value.GetParameters();
                foreach (KeyValuePair<string, string> paramDesc in parms) {
                    string paramName = paramDesc.Key;
                    string paramValue = paramDesc.Value;
                    sb.AppendLine(string.Format(lineFormat, paramPanel, paramName, paramValue));
                }
            }
            // make closing XML root element
            sb.AppendLine("</parameters>");

            // dump XML to file
            StreamWriter mOutFile = new StreamWriter(FilePath);
            mOutFile.WriteLine(sb.ToString());
            mOutFile.Close();
        }

        /// <summary>
        /// Restore the settings for parameter panels in the master list
        /// that are visible via their IModuleParameters interface
        /// </summary>
        /// <param name="panelList">List of UI component panels</param>
        public static void RestoreSavedPanelParameters(Dictionary<string, IModuleParameters> panelList) {
            if (!File.Exists(SavedState.FilePath)) return;

            // get XML containing saved panel parameters
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);

            // get list of paramter nodes
            string xpath = ".//parameter";
            XmlNodeList parms = doc.SelectNodes(xpath);

            // for each panel in list, collect its parameters from the XML node list
            // and set them for the panel
            foreach (string listPanel in panelList.Keys) {
                Dictionary<string, string> parameterList = new Dictionary<string, string>();
                foreach (XmlNode parm in parms) {
                    string paramPanel = parm.Attributes["panel"].InnerText;
                    string paramName = parm.Attributes["name"].InnerText;
                    string paramValue = parm.Attributes["value"].InnerText;
                    if (paramPanel == listPanel) {
                        parameterList[paramName] = paramValue;
                    }
                }
                IModuleParameters panel = panelList[listPanel];
                panel.SetParameters(parameterList);
            }
        }
    }
}
