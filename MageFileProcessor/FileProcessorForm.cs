using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;
using MageFileProcessor.Properties;
using MageUIComponents;
using PRISM.Logging;

namespace MageFileProcessor
{
    public partial class FileProcessorForm : Form
    {
        // Ignore Spelling: Mage, workflows

        protected const string TAG_JOB_IDs = "Job_ID_List";
        protected const string TAG_JOB_IDs_FROM_DATASETS = "Jobs_From_Dataset_List";
        protected const string TAG_DATASET_LIST = "Datasets";
        protected const string TAG_DATASET_ID_LIST = "Dataset_List";
        protected const string TAG_DATASET_NAME_LIST = "Dataset_Name_List";
        protected const string TAG_DATA_PACKAGE_ID = "Data_Package";
        protected const string TAG_DATA_PACKAGE_DS_IDs = "Data_Package_Datasets";

        /// <summary>
        /// Pipeline queue for running the multiple pipelines that make up the workflows for this module
        /// </summary>
        private PipelineQueue mPipelineQueue = new();

        private string mFinalPipelineName = string.Empty;

        /// <summary>
        /// Source pipeline name
        /// </summary>
        /// <remarks>Used to determine which field names to use for source files when copying files or processing files</remarks>
        private string mFileSourcePipelineName = string.Empty;

        // Current command that is being executed or has most recently been executed
        private MageCommandEventArgs mCurrentCmd;

        // Object that sent the current command
        private object mCurrentCmdSender;

        /// <summary>
        /// Constructor
        /// </summary>
        public FileProcessorForm()
        {
            InitializeComponent();

            SetFormTitle();

            SetTags();

            SetAboutText();

            // These settings are loaded from file MageFileProcessor.exe.config
            // The global values will be updated when the QueryDefinitions.xml file is read
            Globals.DMSServer = Settings.Default.DMSServer;             // Default: prismdb2.emsl.pnl.gov
            Globals.DMSDatabase = Settings.Default.DMSDatabase;         // Default: dms
            Globals.DMSUser = Settings.Default.DMSUser;                 // Default: dmsreader
            Globals.DMSUserPassword = Settings.Default.DMSUserPassword;

            try
            {
                // Set up configuration directory and files
                SavedState.SetupConfigFiles("MageFileProcessor");

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
                FileLogger.WriteLog(BaseLogger.LogLevels.INFO, "Starting Mage File Processor");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Setup UI component panels
            SetupStatusPanel();
            SetupCommandHandler();
            SetupFilterSelectionListForFileProcessor();
            SetupColumnMapping();

            // Setup context menus for list displays

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(JobListDisplayControl);

            // ReSharper disable once ObjectCreationAsStatement
            new GridViewDisplayActions(FileListDisplayControl);

            // Connect click events
            lblAboutLink.LinkClicked += AboutLink_LinkClicked;

            // Connect the pipeline queue to message handlers
            ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

            // Connect callbacks for UI panels
            FileProcessingPanel1.GetSelectedFileInfo += GetSelectedFileItem;
            FileProcessingPanel1.GetSelectedOutputInfo += GetSelectedOutputItem;

            // This must be called before restoring saved state
            SetupFlexQueryPanels();

            // In addition, assure that the ListName for JobDatasetIDList1 is Dataset_ID
            JobDatasetIDList1.Legend = "(Dataset IDs)";
            JobDatasetIDList1.ListName = JobIDListPanel.LIST_NAME_DATASET_ID;

            try
            {
                // Restore settings to UI component panels
                SavedState.RestoreSavedPanelParameters(PanelSupport.GetParameterPanelList(this));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring saved settings; will auto-delete SavedState.xml. Message details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

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
            txtAbout1.Text = "Mage File Processor can search for files associated with DMS analysis jobs or datasets, then copy those files to the local computer.  The files can optionally be combined into a single tab-delimited text file or a single SQLite database.";
            txtAbout2.Text = "Written by Gary Kiebel and Matthew Monroe in 2011 for the Department of Energy (PNNL, Richland, WA).";
            lblAboutLink.Text = "http://prismwiki.pnl.gov/wiki/Mage_Suite";
        }

        private void SetFormTitle()
        {
            var objVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var version = string.Format("{0}.{1}.{2}", objVersion.Major, objVersion.Minor, objVersion.Build);

            txtVersion.Text = string.Format("Version {0} ({1})", version, Globals.PROGRAM_DATE_SHORT);
        }

        private void SetTags()
        {
            JobListTabPage.Tag = TAG_JOB_IDs;
            JobsFromDatasetIDTabPage.Tag = TAG_JOB_IDs_FROM_DATASETS;
            DatasetTabPage.Tag = TAG_DATASET_LIST;
            DatasetIDTabPage.Tag = TAG_DATASET_ID_LIST;
            DatasetNameTabPage.Tag = TAG_DATASET_NAME_LIST;
            DataPackageJobsTabPage.Tag = TAG_DATA_PACKAGE_ID;
            DataPackageDatasetsTabPage.Tag = TAG_DATA_PACKAGE_DS_IDs;
        }

        // Command Processing

        private void ConnectPipelineToStatusDisplay(ProcessingPipeline pipeline)
        {
            // Clear any warnings
            statusPanel1.ClearWarnings();

            pipeline.OnStatusMessageUpdated += HandlePipelineUpdate;
            pipeline.OnWarningMessageUpdated += HandlePipelineWarning;
            pipeline.OnRunCompleted += HandlePipelineCompletion;
        }

        private void ConnectPipelineQueueToStatusDisplay(PipelineQueue pipelineQueue)
        {
            // Clear any warnings
            statusPanel1.ClearWarnings();

            pipelineQueue.OnRunCompleted += HandlePipelineQueueCompletion;
            pipelineQueue.OnPipelineStarted += HandlePipelineQueueUpdate;
        }

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

            if (command.Action == "display_reloaded")
            {
                mCurrentCmd = command;
                AdjustPostCommandUIState(null);
                return;
            }

            // Cancel the currently running pipeline
            if (command.Action == "cancel_operation" && mPipelineQueue?.IsRunning == true)
            {
                mPipelineQueue.Cancel();
                return;
            }

            // Don't allow another pipeline if one is currently running
            if (mPipelineQueue?.IsRunning == true)
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

            mPipelineQueue.Pipelines.Clear();

            try
            {
                // Build and run the pipeline appropriate to the command
                Dictionary<string, string> runtimeParams;
                GVPipelineSource source;
                ISinkModule sink;
                string queryDefXML;
                ProcessingPipeline pipeline;

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

                        pipeline = Pipelines.MakeJobQueryPipeline(sink, queryDefXML, queryParameters);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;

                        break;

                    case "get_entities_from_flex_query":
                        queryDefXML = ModuleDiscovery.GetQueryXMLDef(command.Mode);

                        var builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML, Globals.PostgresDMS);

                        if (!builder.HasPredicate)
                        {
                            MessageBox.Show("You must define one or more search criteria before searching", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        var reader = new SQLReader(builder);
                        sink = JobListDisplayControl.MakeSink("Jobs");

                        pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, sink);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;

                        break;

                    case "get_files_from_entities":
                        var entityType = JobListDisplayControl.PageTitle;

                        runtimeParams = GetRuntimeParamsForEntityFileType(entityType);

                        source = new GVPipelineSource(JobListDisplayControl, mode);
                        sink = FileListDisplayControl.MakeSink("Files");

                        pipeline = Pipelines.MakeFileListPipeline(source, sink, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        mFileSourcePipelineName = pipeline.PipelineName;

                        break;

                    case "get_files_from_local_directory":
                        runtimeParams = GetRuntimeParamsForLocalDirectory();

                        var directoryPath = GetRuntimeParam(runtimeParams, "Directory");

                        if (!Directory.Exists(directoryPath))
                        {
                            MessageBox.Show("Directory not found: " + directoryPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        sink = FileListDisplayControl.MakeSink("Files");

                        pipeline = Pipelines.MakePipelineToGetLocalFileList(sink, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        mFileSourcePipelineName = pipeline.PipelineName;

                        break;

                    case "get_files_from_local_manifest":
                        runtimeParams = GetRuntimeParamsForManifestFile();

                        var sFile = GetRuntimeParam(runtimeParams, "ManifestFilePath");

                        if (!File.Exists(sFile))
                        {
                            MessageBox.Show("Manifest file not found: " + sFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        sink = FileListDisplayControl.MakeSink("Files");

                        pipeline = Pipelines.MakePipelineToGetFilesFromManifest(sink, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        mFileSourcePipelineName = pipeline.PipelineName;

                        break;

                    case "copy_files":
                        runtimeParams = GetRuntimeParamsForCopyFiles();

                        if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "OutputDirectory")))
                        {
                            MessageBox.Show("Destination directory cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        source = new GVPipelineSource(FileListDisplayControl, mode);

                        pipeline = Pipelines.MakeFileCopyPipeline(source, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        break;

                    case "process_file_contents":
                        var filterParams = FileProcessingPanel1.GetParameters();

                        runtimeParams = GetRuntimeParamsForFileProcessing();

                        switch (FilterOutputTabs.SelectedTab.Tag.ToString())
                        {
                            case "File_Output":
                                if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "OutputDirectory")))
                                {
                                    MessageBox.Show("Destination directory cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }

                                if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "OutputFile")))
                                {
                                    MessageBox.Show("Destination file cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }

                                break;

                            case "SQLite_Output":
                                if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "DatabaseName")))
                                {
                                    MessageBox.Show("SQLite Database path cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }

                                if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "TableName")))
                                {
                                    MessageBox.Show("SQLite destination table name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }

                                break;

                            default:
                                MessageBox.Show("Programming bug in BuildAndRunPipeline: control FilterOutputTabs has an unrecognized tab with tag " + FilterOutputTabs.SelectedTab.Tag, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                        }

                        source = new GVPipelineSource(FileListDisplayControl, mode);

                        mPipelineQueue = Pipelines.MakePipelineQueueToPreProcessThenFilterFiles(source, runtimeParams, filterParams);
                        mFinalPipelineName = mPipelineQueue.Pipelines.Last().PipelineName;

                        break;

                    default:
                        return;
                }

                if (mPipelineQueue.Pipelines.Count == 0)
                {
                    MessageBox.Show(string.Format("Could not build pipeline for '{0}' operation", command.Action), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    mCurrentCmd = command;

                    // Clear any warnings
                    statusPanel1.ClearWarnings();

                    foreach (var p in mPipelineQueue.Pipelines.ToArray())
                    {
                        ConnectPipelineToStatusDisplay(p);
                        mFinalPipelineName = p.PipelineName;
                    }

                    ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

                    EnableCancel(true);
                    mPipelineQueue.Run();
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

            foreach (var entry in queryParameters)
            {
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
                    TAG_JOB_IDs => "Job ID list cannot be empty",
                    TAG_JOB_IDs_FROM_DATASETS => "Dataset list cannot be empty",
                    TAG_DATASET_ID_LIST => "Dataset ID list cannot be empty",
                    TAG_DATA_PACKAGE_ID or TAG_DATA_PACKAGE_DS_IDs => "Please enter a data package ID",
                    _ => "You must define one or more search criteria before searching for jobs",
                };
            }

            if (string.IsNullOrEmpty(msg) && queryName is TAG_JOB_IDs or TAG_JOB_IDs_FROM_DATASETS or TAG_DATASET_ID_LIST)
            {
                var sepChars = new[] { ',', '\t' };

                var warning = queryName == TAG_JOB_IDs ? "Job number '" : "Use dataset IDs, not dataset names: '";

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

        private const string mFileListLabelPrefix = "Files From ";

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            // Initial labels for display list control panels
            JobListDisplayControl.PageTitle = "Entities";
            FileListDisplayControl.PageTitle = "Files";
            FileListDisplayControl.AutoSizeColumnWidths = true;

            // Disable certain UI component panels
            EntityFilePanel1.Enabled = false;
            FileProcessingPanel1.Enabled = false;
            FileCopyPanel1.Enabled = false;
            FolderDestinationPanel1.Enabled = false;
            SQLiteDestinationPanel1.Enabled = false;

            EnableDisableOutputTabs();
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
            var fileCount = FileListDisplayControl.ItemCount;
            if (fileCount == 0)
            {
                FileCopyPanel1.Enabled = false;
                FolderDestinationPanel1.Enabled = false;
                SQLiteDestinationPanel1.Enabled = false;
            }
            else
            {
                FileCopyPanel1.Enabled = true;
                FolderDestinationPanel1.Enabled = true;
                SQLiteDestinationPanel1.Enabled = true;
            }
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
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + JobListDisplayControl.PageTitle;
                    // FileCopyPanel1.ApplyPrefixToFileName = "Yes";
                    FileCopyPanel1.PrefixColumnName = GetBestPrefixIDColumnName(FileListDisplayControl.ColumnDefs);
                    break;

                case "get_files_from_local_directory":
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + "Local Directory";
                    FileCopyPanel1.ApplyPrefixToFileName = "No";
                    FileCopyPanel1.PrefixColumnName = string.Empty;
                    break;

                case "get_files_from_local_manifest":
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + "Manifest";
                    FileCopyPanel1.ApplyPrefixToFileName = "No";
                    FileCopyPanel1.PrefixColumnName = string.Empty;
                    break;
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
            if (mCurrentCmd.Action != "reload_list_display" && mCurrentCmd.Action != "display_reloaded")
            {
                return;
            }

            if (mCurrentCmdSender is not IMageDisplayControl ldc)
            {
                return;
            }

            var type = ldc.PageTitle;

            var colNames = ldc.ColumnNames;
            if (colNames.Contains("Item"))
            {
                type = "Files";
            }
            else
            {
                if (colNames.Contains("Job"))
                {
                    type = "Jobs";
                }
                else if (colNames.Contains("Dataset_ID"))
                {
                    type = "Datasets";
                }
            }

            ldc.PageTitle = type;
        }

        /// <summary>
        /// Control enable state of filter panel based on output tab choice
        /// </summary>
        private void EnableDisableOutputTabs()
        {
            var mode = FilterOutputTabs.SelectedTab.Tag.ToString();
            if (mode == "Copy_Files")
            {
                FileProcessingPanel1.Enabled = false;
                // FileProcessingPanel1.Visible = false;
                // panel3.Height = 180;
            }
            else
            {
                if (FolderDestinationPanel1.Enabled || SQLiteDestinationPanel1.Enabled)
                {
                    FileProcessingPanel1.Enabled = true;
                    // FileProcessingPanel1.Visible = true;
                    // panel3.Height = 280;
                }
            }
        }

        private void FilterOutputTabs_Selected(object sender, TabControlEventArgs e)
        {
            EnableDisableOutputTabs();
        }

        // Methods for building runtime parameter lists from component panels

        private string GetQueryDefinition(out string queryName)
        {
            // Note: Tab page tag field contains name of query to look up in query def file
            Control queryPage = EntityListSourceTabs.SelectedTab;
            queryName = queryPage.Tag.ToString();
            return ModuleDiscovery.GetQueryXMLDef(queryName);
        }

        /// <summary>
        /// Return the value for the given runtime parameter
        /// </summary>
        /// <remarks>Raises an exception if the runtimeParams dictionary does not have the desired key</remarks>
        /// <param name="runtimeParams">Runtime parameters</param>
        /// <param name="keyName">Parameter to find</param>
        /// <returns>The value for the parameter</returns>
        private string GetRuntimeParam(IReadOnlyDictionary<string, string> runtimeParams, string keyName)
        {
            if (!runtimeParams.ContainsKey(keyName))
                throw new Exception("runtimeParams does not contain key " + keyName);

            return runtimeParams[keyName];
        }

        private Dictionary<string, string> GetRuntimeParamsForEntityQuery()
        {
            Control queryPage = EntityListSourceTabs.SelectedTab;
            var panel = PanelSupport.GetParameterPanel(queryPage);
            if (panel == null)
                return new Dictionary<string, string>();

            return panel.GetParameters();
        }

        private Dictionary<string, string> GetRuntimeParamsForLocalDirectory()
        {
            return new Dictionary<string, string>
            {
                {"FileNameFilter", LocalFolderPanel1.FileNameFilter},
                {"FileSelectionMode", LocalFolderPanel1.FileSelectionMode},
                {"Directory", LocalFolderPanel1.Directory},
                {"SearchInSubdirectories", LocalFolderPanel1.SearchInSubdirectories},
                {"SubdirectorySearchName", LocalFolderPanel1.SubdirectorySearchName}
            };
        }

        private Dictionary<string, string> GetRuntimeParamsForManifestFile()
        {
            return new Dictionary<string, string>
            {
                {"ManifestFilePath", LocalManifestPanel1.ManifestFilePath}
            };
        }

        private Dictionary<string, string> GetRuntimeParamsForFileProcessing()
        {
            var runtimeParams = new Dictionary<string, string>
            {
                {"OutputDirectory", FolderDestinationPanel1.OutputDirectory},
                {"OutputFile", FolderDestinationPanel1.OutputFile},
                {"DatabaseName", SQLiteDestinationPanel1.DatabaseName},
                {"TableName", SQLiteDestinationPanel1.TableName},
                {"OutputMode", FilterOutputTabs.SelectedTab.Tag.ToString()},
                {"ManifestFileName", string.Format("Manifest_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now)}
            };

            if (mFileSourcePipelineName == Pipelines.PIPELINE_GET_LOCAL_FILES)
            {
                runtimeParams.Add("SourceFileColumnName", "File");
            }
            else
            {
                runtimeParams.Add("SourceFileColumnName", "Name");
            }

            return runtimeParams;
        }

        private Dictionary<string, string> GetRuntimeParamsForCopyFiles()
        {
            var runtimeParams = new Dictionary<string, string>
            {
                {"OutputDirectory", FileCopyPanel1.OutputDirectory},
                {"ManifestFileName", string.Format("Manifest_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now)},
                {"ApplyPrefixToFileName", FileCopyPanel1.ApplyPrefixToFileName},
                {"PrefixLeader", FileCopyPanel1.PrefixLeader},
                {"ColumnToUseForPrefix", FileCopyPanel1.PrefixColumnName},
                {"OverwriteExistingFiles", FileCopyPanel1.OverwriteExistingFiles},
                {"SourceDirectoryColumnName", "Directory"},
                {"ResolveCacheInfoFiles", FileCopyPanel1.ResolveCacheInfoFiles}
            };

            if (mFileSourcePipelineName == Pipelines.PIPELINE_GET_LOCAL_FILES)
            {
                runtimeParams.Add("SourceFileColumnName", "File");
                runtimeParams.Add("OutputColumnList", "File, *");
                runtimeParams.Add("OutputFileColumnName", "File");
            }
            else
            {
                runtimeParams.Add("SourceFileColumnName", "Name");
                runtimeParams.Add("OutputColumnList", "Name, *");
                runtimeParams.Add("OutputFileColumnName", "Name");
            }

            return runtimeParams;
        }

        private Dictionary<string, string> GetRuntimeParamsForEntityFileType(string entityType)
        {
            var runtimeParams = new Dictionary<string, string>
            {
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
                    // Columns Directory, Job, Dataset, etc. correspond to views V_Mage_Analysis_Jobs and V_Mage_Data_Package_Analysis_Jobs
                    runtimeParams.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Directory, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument");
                    break;

                case "Datasets":
                    // Columns  Directory, Dataset, Dataset_ID, etc. correspond to views V_Mage_Dataset_List and V_Mage_Data_Package_Datasets
                    runtimeParams.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Directory, Dataset, Dataset_ID, Experiment, Campaign, State, Instrument, Created, Dataset_Type, Comment");
                    break;
            }

            return runtimeParams;
        }

        /// <summary>
        /// Get "best" column name to use for naming prefix according to heuristic
        /// </summary>
        private static string GetBestPrefixIDColumnName(IEnumerable<MageColumnDef> colDefs)
        {
            var IDColumnName = string.Empty;

            // Define list of potential candidate names in order of precedence
            var candidateIDColumnNames = new Dictionary<string, bool>
            {
                { "Job", false }, { "Dataset_ID", false }, { "Dataset", false }
            };

            // Go through actual column names and make the potential candidates
            foreach (var colDef in colDefs)
            {
                if (candidateIDColumnNames.ContainsKey(colDef.Name))
                {
                    candidateIDColumnNames[colDef.Name] = true;
                }
            }

            // Use the highest precedence marked potential candidate
            foreach (var candidateIDColName in candidateIDColumnNames)
            {
                if (candidateIDColName.Value)
                {
                    IDColumnName = candidateIDColName.Key;
                    break;
                }
            }
            return IDColumnName;
        }

        // Methods for handling status updates

        private delegate void CompletionStateUpdated(object status);
        private delegate void VoidFnDelegate();

        private void HandlePipelineUpdate(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleStatusMessageUpdated(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine(args.Message);
        }

        private void HandlePipelineWarning(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleWarningMessageUpdated(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine("Warning: " + args.Message);
        }

        /// <summary>
        /// Handle updating control enable status on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleCompletionMessageUpdate(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine(args.Message);

            if (sender is ProcessingPipeline pipeline && pipeline.PipelineName == mFinalPipelineName)
            {
                CompletionStateUpdated csu = AdjustPostCommandUIState;

                // The following is equivalent to:
                // Invoke(csu, new object[] { null });
                Invoke(csu, [null]);

                // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
                VoidFnDelegate et = EnableDisableOutputTabs;
                Invoke(et);
            }

            if (args.Message.StartsWith(SQLReader.SQL_COMMAND_ERROR))
                MessageBox.Show(args.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void HandlePipelineQueueUpdate(object sender, MageStatusEventArgs args)
        {
            // Console.WriteLine("PipelineQueueUpdate: " + args.Message);
        }

        private void HandlePipelineQueueCompletion(object sender, MageStatusEventArgs args)
        {
            // Console.WriteLine("PipelineQueueCompletion: " + args.Message);
        }

        private void AboutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
            Process.Start(lblAboutLink.Text);
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
        /// Set up filter selection list for file processing panel
        /// </summary>
        private static void SetupFilterSelectionListForFileProcessor()
        {
            ModuleDiscovery.SetupFilters();
        }

        /// <summary>
        /// Setup path to column mapping config file for selection forms
        /// </summary>
        private void SetupColumnMapping()
        {
            const string columnMappingFileName = "ColumnMapping.txt";
            var path = Path.Combine(SavedState.DataDirectory, columnMappingFileName);  // "ColumnMappingConfig.db")
            if (!File.Exists(path))
            {
                File.Copy(columnMappingFileName, path);
            }
            ColumnMapSelectionForm.MappingConfigFilePath = path;
            ColumnMappingForm.MappingConfigFilePath = path;
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

        // Callback Methods for UI Panel Use

        /// <summary>
        /// Get fields from selected item in file list
        /// </summary>
        private Dictionary<string, string> GetSelectedFileItem()
        {
            return FileListDisplayControl.SelectedItemFields;
        }

        private Dictionary<string, string> GetSelectedOutputItem()
        {
            if (FilterOutputTabs.SelectedTab.Tag.ToString().Equals("SQLite_Output"))
            {
                return SQLiteDestinationPanel1.GetParameters();
            }

            return FolderDestinationPanel1.GetParameters();
        }
    }
}
