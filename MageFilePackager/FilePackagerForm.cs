using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Mage;
using log4net;
using MageDisplayLib;
using System.Reflection;
using MageFilePackager.Properties;

namespace MageFilePackager
{

    public partial class FilePackagerForm : Form
    {

        #region Constants

        private const string TagJobIDs = "Job_ID_List";
        private const string TagJobIDsFromDatasets = "Jobs_From_Dataset_List";
        private const string TagDatasetList = "Datasets";
        private const string TagDatasetIDList = "Dataset_List";
        private const string TagDataPackageID = "Data_Package";
        private const string TagDataPackageDsIDs = "Data_Package_Datasets";
        private const string TagDataPackageDetails = "Data_Package_Details";

        private const string FileListLabelPrefix = "Files From ";

        #endregion

        #region Member Variables

        // current Mage pipeline that is running or has most recently run
        ProcessingPipeline _mCurrentPipeline;

        // current command that is being executed or has most recently been executed
        MageCommandEventArgs _mCurrentCmd;

        // object that sent the current command
        object _mCurrentCmdSender;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public FilePackagerForm()
        {
            _mCurrentCmdSender = null;
            _mCurrentCmd = null;
            _mCurrentPipeline = null;

            InitializeComponent();

            const bool isBetaVersion = true;
            SetFormTitle("2017-10-09", isBetaVersion);

            SetTags();

            SetAboutText();

            // These settings are loaded from file MagerConcatenator.exe.config
            // Typically gigasax and DMS5
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;

            txtServer.Text = "DMS Server: " + Globals.DMSServer;

            ModuleDiscovery.DMSServerOverride = Globals.DMSServer;
            ModuleDiscovery.DMSDatabaseOverride = Globals.DMSDatabase;

            try
            {
                // set up configuration folder and files
                SavedState.SetupConfigFiles("MageFilePackager");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // These settings are loaded from file MageFilePackager.exe.config
            // Typically gigasax and DMS5
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;

            ModuleDiscovery.DMSServerOverride = Globals.DMSServer;
            ModuleDiscovery.DMSDatabaseOverride = Globals.DMSDatabase;

            try
            {
                // set up configuration folder and files
                // Set log4net path and kick the logger into action
                var logFileName = Path.Combine(SavedState.DataDirectory, "log.txt");
                GlobalContext.Properties["LogName"] = logFileName;
                var traceLog = LogManager.GetLogger("TraceLog");
                traceLog.Info("Starting MageFilePackager");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // setup UI component panels
            SetupStatusPanel();
            SetupCommandHandler();

            filePackageMgmtPanel1.FileSourceList = FileListDisplayControl;
            filePackageMgmtPanel1.FileListLabelPrefix = FileListLabelPrefix;

            // setup context menus for list displays

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(JobListDisplayControl);

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(FileListDisplayControl);

            // Connect click events
            lblAboutLink.LinkClicked += LblAboutLinkLinkClicked;

            // connect callbacks for UI panels
            //--            FileProcessingPanel1.GetSelectedFileInfo += GetSelectedFileItem;
            //--            FileProcessingPanel1.GetSelectedOutputInfo += GetSelectedOutputItem;

            SetupFlexQueryPanels(); // must be done before restoring saved state
            try
            {
                // restore settings to UI component panels
                SavedState.RestoreSavedPanelParameters(PanelSupport.GetParameterPanelList(this));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring saved settings; will auto-delete SavedState.xml.  Message details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // Delete the SavedState.xml file
                try
                {
                    File.Delete(SavedState.FilePath);
                }
                catch (Exception ex2)
                {
                    // Ignore errors here
                    Console.WriteLine("Error deleting SavedState file: " + ex2.Message);
                }
            }
            AdjustInitialUIState();
        }

        private void SetAboutText()
        {
            txtAbout1.Text = "Mage File packager can build download package manifests by searching for files from DMS datasets, analysis jobs, and data packages.";
            txtAbout2.Text = "Written by Gary Kiebel in 2012 for the Department of Energy (PNNL, Richland, WA)";
            lblAboutLink.Text = "http://prismwiki.pnl.gov/wiki/Mage_Suite";
        }

        private void SetFormTitle(string programDate, bool beta)
        {
            var objVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var version = string.Format("{0}.{1}.{2}", objVersion.Major, objVersion.Minor, objVersion.Build);

            txtVersion.Text = "Version " + version;

            if (!string.IsNullOrEmpty(programDate))
            {
                if (beta)
                    txtVersion.Text += string.Format(" ({0}, beta)", programDate);
                else
                    txtVersion.Text += string.Format(" ({0})", programDate);
            }

            if (beta)
                Text += " (beta)";
        }

        /// <summary>
        /// Set labelling for UI panels
        /// </summary>
        private void SetTags()
        {
            JobListTabPage.Tag = TagJobIDs;
            JobsFromDatasetIDTabPage.Tag = TagJobIDsFromDatasets;
            DatasetTabPage.Tag = TagDatasetList;
            DatasetIDTabPage.Tag = TagDatasetIDList;
            DataPackageJobsTabPage.Tag = TagDataPackageID;
            DataPackageDatasetsTabPage.Tag = TagDataPackageDsIDs;
            DataPackageDetailsTabPage.Tag = TagDataPackageDetails;
        }

        #endregion

        #region Command Processing

        /// <summary>
        /// execute a command by building and running
        /// the appropriate pipeline (or cancelling
        /// the current pipeline activity)
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="command">Command to execute</param>
        public void DoCommand(object sender, MageCommandEventArgs command)
        {

            // remember who sent us the command
            _mCurrentCmdSender = sender;

            if (command.Action == "display_reloaded")
            {
                _mCurrentCmd = command;
                AdjusttPostCommndUIState(null);
                return;
            }
            // cancel the currently running pipeline
            if (command.Action == "cancel_operation" && _mCurrentPipeline != null && _mCurrentPipeline.Running)
            {
                _mCurrentPipeline.Cancel();
                return;
            }
            // don't allow another pipeline if one is currently running
            if (_mCurrentPipeline != null && _mCurrentPipeline.Running)
            {
                MessageBox.Show("Pipeline is already active", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // construct suitable Mage pipeline for the given command
            // and run that pipeline
            BuildAndRunPipeline(command);
        }

        /// <summary>
        /// Construnct and run a Mage pipeline for the given command
        /// </summary>
        /// <param name="command"></param>
        private void BuildAndRunPipeline(MageCommandEventArgs command)
        {
            var mode = (command.Mode == "selected") ? DisplaySourceMode.Selected : DisplaySourceMode.All;

            try
            {
                // build and run the pipeline appropriate to the command
                ISinkModule sink;
                string queryDefXML;
                switch (command.Action)
                {
                    case "get_entities_from_query":
                        queryDefXML = GetQueryDefinition(out var queryName);
                        if (string.IsNullOrEmpty(queryDefXML))
                        {
                            MessageBox.Show("Unknown query type '" + queryName + "'.  Your QueryDefinitions.xml file is out-of-date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        var queryParameters = GetRuntimeParmsForEntityQuery();
                        if (!ValidQueryParameters(queryName, queryParameters))
                        {
                            return;
                        }
                        sink = JobListDisplayControl.MakeSink(command.Mode, 15);
                        _mCurrentPipeline = Pipelines.MakeJobQueryPipeline(sink, queryDefXML, queryParameters);
                        break;

                    case "get_entities_from_flex_query":
                        queryDefXML = ModuleDiscovery.GetQueryXMLDef(command.Mode);
                        var builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML);
                        if (builder.HasPredicate)
                        {
                            var reader = new MSSQLReader(builder);
                            sink = JobListDisplayControl.MakeSink("Jobs", 15);
                            _mCurrentPipeline = ProcessingPipeline.Assemble("Get Jobs", reader, sink);
                        }
                        else
                        {
                            MessageBox.Show("You must define one or more search criteria before searching", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            _mCurrentPipeline = null;
                            return;
                        }
                        break;

                    case "get_files_from_entities":
                        var entityType = JobListDisplayControl.PageTitle;
                        var runtimeParms = GetRuntimeParmsForEntityFileType(entityType);
                        var source = new GVPipelineSource(JobListDisplayControl, mode);
                        sink = FileListDisplayControl.MakeSink("Files", 15);
                        _mCurrentPipeline = Pipelines.MakeFileListPipeline(source, sink, runtimeParms);
                        break;
                    default:
                        return;
                }
                if (_mCurrentPipeline != null)
                {
                    _mCurrentCmd = command;

                    // Clear any warnings
                    statusPanel1.ClearWarnings();

                    _mCurrentPipeline.OnStatusMessageUpdated += statusPanel1.HandleStatusMessageUpdated;
                    _mCurrentPipeline.OnWarningMessageUpdated += statusPanel1.HandleWarningMessageUpdated;
                    _mCurrentPipeline.OnRunCompleted += statusPanel1.HandleCompletionMessageUpdate;
                    _mCurrentPipeline.OnRunCompleted += HandlePipelineCompletion;
                    EnableCancel(true);
                    _mCurrentPipeline.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool ValidQueryParameters(string queryName, Dictionary<string, string> queryParameters)
        {
            var msg = string.Empty;
            var bFilterDefined = false;

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var entry in queryParameters)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value))
                {
                    if (entry.Value.Trim().Length > 0)
                    {
                        bFilterDefined = true;
                        break;
                    }
                }
            }

            if (!bFilterDefined)
            {

                switch (queryName)
                {
                    case TagJobIDs:
                        msg = "Job ID list cannot be empty";
                        break;
                    case TagJobIDsFromDatasets:
                        msg = "Dataset list cannot be empty";
                        break;
                    case TagDatasetIDList:
                        msg = "Dataset ID list cannot be empty";
                        break;
                    case TagDataPackageID:
                    case TagDataPackageDsIDs:
                        msg = "Please enter a data package ID";
                        break;
                    case TagDataPackageDetails:
                        msg = "Please enter one or more data package IDs";
                        break;
                    default:
                        msg = "You must define one or more search criteria before searching for jobs";
                        break;
                }
            }

            // FUTURE: validation for TagDataPackageDetails??
            if (string.IsNullOrEmpty(msg) && (queryName == TagJobIDs || queryName == TagJobIDsFromDatasets || queryName == TagDatasetIDList))
            {
                var cSepChars = new[] { ',', '\t' };

                var sWarning = queryName == TagJobIDs ? "Job number '" : "Use dataset IDs, not dataset names: '";

                // Validate that the job numbers or dataset IDs are all numeric
                foreach (var entry in queryParameters)
                {
                    var sValue = entry.Value.Replace(Environment.NewLine, ",");

                    var values = sValue.Split(cSepChars);

                    foreach (var datasetID in values)
                    {
                        if (!int.TryParse(datasetID, out _))
                        {
                            msg = sWarning + datasetID + "' is not numeric";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(msg))
                        break;
                }
            }

            if (string.IsNullOrEmpty(msg))
                return true;

            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }

        #endregion

        #region Functions for setting UI state

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            // initial labels for display list control panels
            JobListDisplayControl.PageTitle = "Entities";
            FileListDisplayControl.PageTitle = "Files";
            JobDatasetIDList1.Legend = "(Dataset IDs)";
            JobDatasetIDList1.ListName = "Dataset_ID";

            // disable certain UI component panels
            EntityFilePanel1.Enabled = false;
            EnableCancel(false);
        }

        /// <summary>
        /// Enable/Disable the cancel button
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableCancel(bool enabled)
        {
            statusPanel1.ShowCancel = enabled;
            statusPanel1.EnableCancel = enabled;
        }

        /// <summary>
        /// Adjust the labelling and status of various UI components
        /// (called when a command pipeline completes via cross-thread invoke from HandleStatusMessageUpdated)
        /// </summary>
        /// <param name="status"></param>
        private void AdjusttPostCommndUIState(object status)
        {
            if (_mCurrentCmd == null)
                return;

            EnableCancel(false);

            AdjustEntityFileTabLabels();
            AdjustListDisplayTitleFromColumDefs();
            AdjustFileListLabels();
            AdjustFileExtractionPanel();
            AdjustFileProcessingPanels();

            SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
        }

        /// <summary>
        /// Processing files is only possible when file list contains files,
        /// adjust the processing panels to inform user
        /// </summary>
        private void AdjustFileProcessingPanels()
        {
            //--            FileCopyPanel1.Enabled = FileListDisplayControl.ItemCount != 0;
        }

        /// <summary>
        /// Finding files for entities is only possible
        /// when there are entities in the entity list
        /// </summary>
        private void AdjustFileExtractionPanel()
        {
            var entityCount = JobListDisplayControl.ItemCount;
            EntityFilePanel1.Enabled = entityCount != 0;
        }

        /// <summary>
        /// Since the list of files can be derived from different sources,
        /// adjust the labelling to inform the user about which one was used
        /// </summary>
        private void AdjustFileListLabels()
        {
            switch (_mCurrentCmd.Action)
            {
                case "get_files_from_entities":
                    FileListDisplayControl.PageTitle = FileListLabelPrefix + JobListDisplayControl.PageTitle;
                    // FileCopyPanel1.ApplyPrefixToFileName = "Yes";
                    //--                    FileCopyPanel1.PrefixColumnName = GetBestPrefixIDColumnName(FileListDisplayControl.ColumnDefs);
                    break;
                    /*
                                    case "get_files_from_local_folder":
                                        FileListDisplayControl.PageTitle = FileListLabelPrefix + "Local Folder";
                                        break;
                                    case "get_files_from_local_manifest":
                                        FileListDisplayControl.PageTitle = FileListLabelPrefix + "Manifest";
                                        break;
                    */
            }
        }

        /// <summary>
        /// adjust tab label for file source actions according to entity type
        /// </summary>
        private void AdjustEntityFileTabLabels()
        {
            switch (_mCurrentCmd.Action)
            {
                case "get_entities_from_query":
                    GetEntityFilesTabPage.Text = string.Format("Get Files From {0}", JobListDisplayControl.PageTitle);
                    break;
            }
        }

        /// <summary>
        /// If the contents of a list display have been restored from file,
        /// make a guess at the type of information it contains
        /// according to the combination of columns it has,
        /// and set its title accordingly
        /// </summary>
        private void AdjustListDisplayTitleFromColumDefs()
        {
            if (_mCurrentCmd.Action == "reload_list_display" || _mCurrentCmd.Action == "display_reloaded")
            {
                if (_mCurrentCmdSender is IMageDisplayControl ldc)
                {
                    var type = ldc.PageTitle;
                    var colNames = ldc.ColumnNames;
                    if (colNames.Contains("Item"))
                    {
                        type = "Files";
                    }
                    else
                        if (colNames.Contains("Job"))
                    {
                        type = "Jobs";

                    }
                    else
                            if (colNames.Contains("Dataset_ID"))
                    {
                        type = "Datasets";
                    }
                    ldc.PageTitle = type;
                }
            }
        }

        /// <summary>
        /// control enable state of filter panel based on output tab choice
        /// </summary>
        private void EnableDisableOutputTabs()
        {
            //--
            //string mode = FilterOutputTabs.SelectedTab.Tag.ToString();
            //if (mode == "Copy_Files") {
            //    FileProcessingPanel1.Enabled = false;
            //} else {
            //    if (FolderDestinationPanel1.Enabled || SQLiteDestinationPanel1.Enabled) {
            //        FileProcessingPanel1.Enabled = true;
            //    }
            //}
        }

        #endregion

        #region Support functions for building runtime parameter lists from component panels


        /// <summary>
        /// Get XML definition for query with given name
        /// from external XML query definition file
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        private string GetQueryDefinition(out string queryName)
        {
            // Note: Tab page tag field contains name of query to look up in query def file
            Control queryPage = EntityListSourceTabs.SelectedTab;
            queryName = queryPage.Tag.ToString();

            // get XML query definitions from bin copy of file
            // not the one in AppData
            var doc = new XmlDocument();
            doc.Load("QueryDefinitions.xml");
            // find query node by name
            var xpath = string.Format(".//query[@name='{0}']", queryName);
            var queryNode = doc.SelectSingleNode(xpath);
            return queryNode?.OuterXml ?? "";
            //--            return ModuleDiscovery.GetQueryXMLDef(queryName);
        }

        private Dictionary<string, string> GetRuntimeParmsForEntityQuery()
        {
            Control queryPage = EntityListSourceTabs.SelectedTab;
            var panel = PanelSupport.GetParameterPanel(queryPage);
            return panel.GetParameters();
        }

        private Dictionary<string, string> GetRuntimeParmsForEntityFileType(string entityType)
        {
            var rp = new Dictionary<string, string> {
                             {"FileSelectors", EntityFilePanel1.FileSelectors},
                             {"FileSelectionMode", EntityFilePanel1.FileSelectionMode},
                             {"IncludeFilesOrFolders", EntityFilePanel1.IncludeFilesOrFolders},
                             {"SearchInSubfolders", EntityFilePanel1.SearchInSubfolders},
                             {"SubfolderSearchName", EntityFilePanel1.SubfolderSearchName},
                             {"SourceFolderColumnName", "Folder"},
                             {"FileColumnName", "Name"}
                         };
            switch (entityType)
            {
                case "Jobs":
                    rp.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Folder, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument, Storage_Path, Purged, Archive_Path");
                    break;
                case "Datasets":
                    rp.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Folder, Dataset, Dataset_ID, State, Instrument, Type, Storage_Path, Purged, Archive_Path");
                    break;
                default:
                    rp.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Folder, *");
                    break;
            }
            return rp;
        }

        #endregion

        #region Functions for handling status updates

        private delegate void CompletionStateUpdated(object status);
        private delegate void VoidFnDelegate();

        /// <summary>
        /// handle updating control enable status on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            CompletionStateUpdated csu = AdjusttPostCommndUIState;
            Invoke(csu, new object[] { null });

            if (args.Message.StartsWith(MSSQLReader.SQL_COMMAND_ERROR))
                MessageBox.Show(args.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
            VoidFnDelegate et = EnableDisableOutputTabs;
            Invoke(et);
        }

        private void LblAboutLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LaunchMageFileProcessorHelpPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LaunchMageFileProcessorHelpPage()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            lblAboutLink.LinkVisited = true;

            //Call the Process.Start method to open the default browser
            //with a URL:
            System.Diagnostics.Process.Start(lblAboutLink.Text);
        }

        #endregion

        #region Panel Support Functions

        /// <summary>
        /// set up status panel
        /// </summary>
        private void SetupStatusPanel()
        {
            statusPanel1.OwnerControl = this;
        }

        /// <summary>
        /// wire up the command event in panels that have it
        /// to the DoCommand event handler method
        /// </summary>
        private void SetupCommandHandler()
        {
            // get reference to the method that handles command events
            var methodInfo = GetType().GetMethod("DoCommand");
            Control subjectControl = this;

            PanelSupport.DiscoverAndConnectCommandHandlers(subjectControl, methodInfo);
        }

        /// <summary>
        /// Initialize parameters for flex query panels
        /// </summary>
        private void SetupFlexQueryPanels()
        {
            JobFlexQueryPanel.QueryName = "Job_Flex_Query";
            JobFlexQueryPanel.SetColumnPickList(new[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign", "Organism", "Organism DB", "Protein Collection List", "Protein Options", "Comment", "Results Folder", "Folder", "Dataset_Created", "Job_Finish", "Request_ID" });
            JobFlexQueryPanel.SetComparisionPickList(new[] { "ContainsText", "DoesNotContainText", "StartsWithText", "MatchesText", "MatchesTextOrBlank", "Equals", "NotEqual", "GreaterThan", "GreaterThanOrEqualTo", "LessThan", "LessThanOrEqualTo", "MostRecentWeeks", "LaterThan", "EarlierThan", "InList" });
        }

        #endregion

        #region Callback Functions for UI Panel Use

        #endregion

    }
}
