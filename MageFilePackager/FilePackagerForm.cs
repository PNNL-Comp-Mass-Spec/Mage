﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Mage;
using MageDisplayLib;
using System.Reflection;
using MageFilePackager.Properties;
using PRISM.Logging;
using MageUIComponents;

namespace MageFilePackager
{
    public partial class FilePackagerForm : Form
    {
        // Ignore Spelling: Mage

        private const string TagJobIDs = "Job_ID_List";
        private const string TagJobIDsFromDatasets = "Jobs_From_Dataset_List";
        private const string TagDatasetList = "Datasets";
        private const string TagDatasetIDList = "Dataset_List";
        private const string TagDataPackageID = "Data_Package";
        private const string TagDataPackageDsIDs = "Data_Package_Datasets";
        private const string TagDataPackageDetails = "Data_Package_Details";

        private const string FileListLabelPrefix = "Files From ";

        // Current Mage pipeline that is running or has most recently run
        private ProcessingPipeline mCurrentPipeline;

        // Current command that is being executed or has most recently been executed
        private MageCommandEventArgs mCurrentCmd;

        // Object that sent the current command
        private object mCurrentCmdSender;

        /// <summary>
        /// Constructor
        /// </summary>
        public FilePackagerForm()
        {
            mCurrentCmdSender = null;
            mCurrentCmd = null;
            mCurrentPipeline = null;

            InitializeComponent();

            SetFormTitle();

            SetTags();

            SetAboutText();

            // These settings are loaded from file MageFilePackager.exe.config
            // Typically prismdb2.emsl.pnl.gov and dms
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;
            Globals.DMSUser = Settings.Default.DMSUser;
            Globals.DMSUserPassword = Settings.Default.DMSUserPassword;

            try
            {
                // Set up configuration directory and files
                SavedState.SetupConfigFiles("MageFilePackager");

                if (SavedState.GetDatabaseConnectionInfo(out var dmsServer, out var dmsDatabase, out var dmsUser, out var dmsUserPassword))
                {
                    Globals.DMSServer = dmsServer;
                    Globals.DMSDatabase = dmsDatabase;
                    Globals.DMSUser = dmsUser;
                    Globals.DMSUserPassword = dmsUserPassword;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            txtServer.Text = "DMS Server: " + Globals.DMSServer;

            try
            {
                // Configure logging
                var logFilePath = Path.Combine(SavedState.DataDirectory, "log.txt");
                const bool appendDateToBaseName = false;
                FileLogger.ChangeLogFileBaseName(logFilePath, appendDateToBaseName);
                FileLogger.WriteLog(BaseLogger.LogLevels.INFO, "Starting MageFilePackager");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Setup UI component panels
            SetupStatusPanel();
            SetupCommandHandler();

            filePackageMgmtPanel1.FileSourceList = FileListDisplayControl;
            filePackageMgmtPanel1.FileListLabelPrefix = FileListLabelPrefix;

            // Setup context menus for list displays

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(JobListDisplayControl);

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(FileListDisplayControl);

            // Connect click events
            lblAboutLink.LinkClicked += LblAboutLinkLinkClicked;

            // Connect callbacks for UI panels
            //--            FileProcessingPanel1.GetSelectedFileInfo += GetSelectedFileItem;
            //--            FileProcessingPanel1.GetSelectedOutputInfo += GetSelectedOutputItem;

            SetupFlexQueryPanels(); // Must be done before restoring saved state
            try
            {
                // Restore settings to UI component panels
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

        private void SetFormTitle()
        {
            var objVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var version = string.Format("{0}.{1}.{2}", objVersion.Major, objVersion.Minor, objVersion.Build);

            txtVersion.Text = string.Format("Version {0} ({1})", version, Globals.PROGRAM_DATE_SHORT);
        }

        /// <summary>
        /// Set labeling for UI panels
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

        // Command Processing

        /// <summary>
        /// Execute a command by building and running the appropriate pipeline
        /// (alternatively, cancel a running pipeline)
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="command">Command to execute</param>
        // ReSharper disable once UnusedMember.Global
        public void DoCommand(object sender, MageCommandEventArgs command)
        {
            // Remember who sent us the command
            mCurrentCmdSender = sender;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (command.Action == "display_reloaded")
            {
                mCurrentCmd = command;
                AdjustPostCommandUIState(null);
                return;
            }

            // Cancel the currently running pipeline
            if (command.Action == "cancel_operation" && mCurrentPipeline?.Running == true)
            {
                mCurrentPipeline.Cancel();
                return;
            }

            // Don't allow another pipeline if one is currently running
            if (mCurrentPipeline?.Running == true)
            {
                MessageBox.Show("Pipeline is already active", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Construct suitable Mage pipeline for the given command
            // and run that pipeline
            BuildAndRunPipeline(command);
        }

        /// <summary>
        /// Construct and run a Mage pipeline for the given command
        /// </summary>
        /// <param name="command"></param>
        private void BuildAndRunPipeline(MageCommandEventArgs command)
        {
            var mode = (command.Mode == "selected") ? DisplaySourceMode.Selected : DisplaySourceMode.All;

            try
            {
                // Build and run the pipeline appropriate to the command
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
                        var queryParameters = GetRuntimeParamsForEntityQuery();
                        if (!ValidQueryParameters(queryName, queryParameters))
                        {
                            return;
                        }
                        sink = JobListDisplayControl.MakeSink(command.Mode);
                        mCurrentPipeline = Pipelines.MakeJobQueryPipeline(sink, queryDefXML, queryParameters);

                        break;

                    case "get_entities_from_flex_query":
                        queryDefXML = ModuleDiscovery.GetQueryXMLDef(command.Mode);

                        var builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML, Globals.PostgresDMS);

                        if (builder.HasPredicate)
                        {
                            var reader = new SQLReader(builder);
                            sink = JobListDisplayControl.MakeSink("Jobs");
                            mCurrentPipeline = ProcessingPipeline.Assemble("Get Jobs", reader, sink);
                        }
                        else
                        {
                            MessageBox.Show("You must define one or more search criteria before searching", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            mCurrentPipeline = null;
                            return;
                        }

                        break;

                    case "get_files_from_entities":
                        var entityType = JobListDisplayControl.PageTitle;
                        var runtimeParams = GetRuntimeParamsForEntityFileType(entityType);
                        var source = new GVPipelineSource(JobListDisplayControl, mode);
                        sink = FileListDisplayControl.MakeSink("Files");
                        mCurrentPipeline = Pipelines.MakeFileListPipeline(source, sink, runtimeParams);
                        break;

                    default:
                        return;
                }

                if (mCurrentPipeline != null)
                {
                    mCurrentCmd = command;

                    // Clear any warnings
                    statusPanel1.ClearWarnings();

                    mCurrentPipeline.OnStatusMessageUpdated += statusPanel1.HandleStatusMessageUpdated;
                    mCurrentPipeline.OnWarningMessageUpdated += statusPanel1.HandleWarningMessageUpdated;
                    mCurrentPipeline.OnRunCompleted += statusPanel1.HandleCompletionMessageUpdate;
                    mCurrentPipeline.OnRunCompleted += HandlePipelineCompletion;
                    EnableCancel(true);
                    mCurrentPipeline.Run();
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
            var filterDefined = false;

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var entry in queryParameters)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value))
                {
                    if (entry.Value.Trim().Length > 0)
                    {
                        filterDefined = true;
                        break;
                    }
                }
            }

            if (!filterDefined)
            {
                msg = queryName switch
                {
                    TagJobIDs => "Job ID list cannot be empty",
                    TagJobIDsFromDatasets => "Dataset list cannot be empty",
                    TagDatasetIDList => "Dataset ID list cannot be empty",
                    TagDataPackageID or TagDataPackageDsIDs => "Please enter a data package ID",
                    TagDataPackageDetails => "Please enter one or more data package IDs",
                    _ => "You must define one or more search criteria before searching for jobs",
                };
            }

            // FUTURE: validation for TagDataPackageDetails??
            if (string.IsNullOrEmpty(msg) && queryName is TagJobIDs or TagJobIDsFromDatasets or TagDatasetIDList)
            {
                var sepChars = new[] { ',', '\t' };

                var warning = queryName == TagJobIDs ? "Job number '" : "Use dataset IDs, not dataset names: '";

                // Validate that the job numbers or dataset IDs are all numeric
                foreach (var entry in queryParameters)
                {
                    var entityList = entry.Value.Replace(Environment.NewLine, ",");

                    foreach (var datasetID in entityList.Split(sepChars))
                    {
                        if (!int.TryParse(datasetID, out _))
                        {
                            msg = warning + datasetID + "' is not numeric";
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

        // Methods for setting UI state

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            // Initial labels for display list control panels
            JobListDisplayControl.PageTitle = "Entities";
            FileListDisplayControl.PageTitle = "Files";
            JobDatasetIDList1.Legend = "(Dataset IDs)";
            JobDatasetIDList1.ListName = JobIDListPanel.LIST_NAME_DATASET_ID;

            // Disable certain UI component panels
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
        /// Adjust the labeling and status of various UI components
        /// (called when a command pipeline completes via cross-thread invoke from HandleStatusMessageUpdated)
        /// </summary>
        /// <param name="status"></param>
        private void AdjustPostCommandUIState(object status)
        {
            if (mCurrentCmd == null)
                return;

            EnableCancel(false);

            AdjustEntityFileTabLabels();
            AdjustListDisplayTitleFromColumnDefs();
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
        /// adjust the labeling to inform the user about which one was used
        /// </summary>
        private void AdjustFileListLabels()
        {
            switch (mCurrentCmd.Action)
            {
                case "get_files_from_entities":
                    FileListDisplayControl.PageTitle = FileListLabelPrefix + JobListDisplayControl.PageTitle;
                    // FileCopyPanel1.ApplyPrefixToFileName = "Yes";
                    //--                    FileCopyPanel1.PrefixColumnName = GetBestPrefixIDColumnName(FileListDisplayControl.ColumnDefs);
                    break;
                    /*
                                    case "get_files_from_local_directory":
                                        FileListDisplayControl.PageTitle = FileListLabelPrefix + "Local Directory";
                                        break;
                                    case "get_files_from_local_manifest":
                                        FileListDisplayControl.PageTitle = FileListLabelPrefix + "Manifest";
                                        break;
                    */
            }
        }

        /// <summary>
        /// Adjust tab label for file source actions according to entity type
        /// </summary>
        private void AdjustEntityFileTabLabels()
        {
            switch (mCurrentCmd.Action)
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
        private void AdjustListDisplayTitleFromColumnDefs()
        {
            if (mCurrentCmd.Action is "reload_list_display" or "display_reloaded" &&
                mCurrentCmdSender is IMageDisplayControl ldc)
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

        /// <summary>
        /// Control enable state of filter panel based on output tab choice
        /// </summary>
        private void EnableDisableOutputTabs()
        {

            // string mode = FilterOutputTabs.SelectedTab.Tag.ToString();
            // if (mode == "Copy_Files") {
            //     FileProcessingPanel1.Enabled = false;
            // } else {
            //     if (FolderDestinationPanel1.Enabled || SQLiteDestinationPanel1.Enabled) {
            //         FileProcessingPanel1.Enabled = true;
            //     }
            // }
        }

        // Support methods for building runtime parameter lists from component panels

        /// <summary>
        /// Get XML definition for query with given name
        /// from external XML query definition file
        /// </summary>
        /// <param name="queryName"></param>
        private string GetQueryDefinition(out string queryName)
        {
            // Note: Tab page tag field contains name of query to look up in query def file
            Control queryPage = EntityListSourceTabs.SelectedTab;
            queryName = queryPage.Tag.ToString();

            // Get XML query definitions from the .xml file in the same directory as the .exe
            // (do not use the one in AppData)
            var doc = new XmlDocument();
            doc.Load("QueryDefinitions.xml");

            // Find query node by name
            var xpath = string.Format(".//query[@name='{0}']", queryName);
            var queryNode = doc.SelectSingleNode(xpath);
            return queryNode?.OuterXml ?? string.Empty;
            //--            return ModuleDiscovery.GetQueryXMLDef(queryName);
        }

        private Dictionary<string, string> GetRuntimeParamsForEntityQuery()
        {
            Control queryPage = EntityListSourceTabs.SelectedTab;
            var panel = PanelSupport.GetParameterPanel(queryPage);
            return panel.GetParameters();
        }

        private Dictionary<string, string> GetRuntimeParamsForEntityFileType(string entityType)
        {
            var runtimeParams = new Dictionary<string, string> {
                             {"FileSelectors", EntityFilePanel1.FileSelectors},
                             {"FileSelectionMode", EntityFilePanel1.FileSelectionMode},
                             {"IncludeFilesOrDirectories", EntityFilePanel1.IncludeFilesOrDirectories},
                             {"SearchInSubdirectories", EntityFilePanel1.SearchInSubdirectories},
                             {"SubdirectorySearchName", EntityFilePanel1.SubdirectorySearchName},
                             {"SourceDirectoryColumnName", "Directory"},
                             {"FileColumnName", "Name"}
                         };

            switch (entityType)
            {
                case "Jobs":
                    runtimeParams.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Directory, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument, Storage_Path, Purged, Archive_Path");
                    break;

                case "Datasets":
                    runtimeParams.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Directory, Dataset, Dataset_ID, State, Instrument, Type, Storage_Path, Purged, Archive_Path");
                    break;

                default:
                    runtimeParams.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Directory, *");
                    break;
            }

            return runtimeParams;
        }

        // Methods for handling status updates

        private delegate void CompletionStateUpdated(object status);
        private delegate void VoidFnDelegate();

        /// <summary>
        /// Handle updating control enable status on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            CompletionStateUpdated csu = AdjustPostCommandUIState;
            Invoke(csu, [null]);

            if (args.Message.StartsWith(SQLReader.SQL_COMMAND_ERROR))
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

            // Call the Process.Start method to open the default browser
            // with a URL:
            System.Diagnostics.Process.Start(lblAboutLink.Text);
        }

        // Panel Support Methods

        /// <summary>
        /// Set up status panel
        /// </summary>
        private void SetupStatusPanel()
        {
            statusPanel1.OwnerControl = this;
        }

        /// <summary>
        /// Wire up the command event in panels that have it
        /// to the DoCommand event handler method
        /// </summary>
        private void SetupCommandHandler()
        {
            // Get reference to the method that handles command events
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

            // Note that method GetSQLBuilder in class FlexQueryPanel will replace spaces with underscores for the names in this list
            JobFlexQueryPanel.SetColumnPickList(["Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign", "Organism", "Organism DB", "Protein Collection List", "Protein Options", "Comment", "Results Folder", "Folder", "Dataset_Created", "Job_Finish", "Request_ID"]);

            JobFlexQueryPanel.SetComparisonPickList(["ContainsText", "DoesNotContainText", "StartsWithText", "MatchesText", "MatchesTextOrBlank", "Equals", "NotEqual", "GreaterThan", "GreaterThanOrEqualTo", "LessThan", "LessThanOrEqualTo", "MostRecentWeeks", "LaterThan", "EarlierThan", "InList"]);
        }

        // Callback methods for UI Panel Use

    }
}
