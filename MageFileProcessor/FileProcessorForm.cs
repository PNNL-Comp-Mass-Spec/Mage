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

        #region Constants
        protected const string TAG_JOB_IDs = "Job_ID_List";
        protected const string TAG_JOB_IDs_FROM_DATASETS = "Jobs_From_Dataset_List";
        protected const string TAG_DATASET_LIST = "Datasets";
        protected const string TAG_DATASET_ID_LIST = "Dataset_List";
        protected const string TAG_DATASET_NAME_LIST = "Dataset_Name_List";
        protected const string TAG_DATA_PACKAGE_ID = "Data_Package";
        protected const string TAG_DATA_PACKAGE_DS_IDs = "Data_Package_Datasets";

        #endregion

        #region Member Variables

        /// <summary>
        /// Pipeline queue for running the multiple pipelines that make up the workflows for this module
        /// </summary>
        private PipelineQueue mPipelineQueue = new PipelineQueue();

        private string mFinalPipelineName = string.Empty;

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Used to determine which field names to use for source files when copying files or processing files</remarks>
        private string mFileSourcePipelineName = string.Empty;

        // Current command that is being executed or has most recently been executed
        MageCommandEventArgs mCurrentCmd;

        // Object that sent the current command
        object mCurrentCmdSender;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public FileProcessorForm()
        {
            InitializeComponent();

            const bool isBetaVersion = false;
            SetFormTitle("2018-09-21", isBetaVersion);

            SetTags();

            SetAboutText();

            // These settings are loaded from file MageFileProcessor.exe.config
            // Typically gigasax and DMS5
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;

            txtServer.Text = "DMS Server: " + Globals.DMSServer;

            ModuleDiscovery.DMSServerOverride = Globals.DMSServer;
            ModuleDiscovery.DMSDatabaseOverride = Globals.DMSDatabase;

            try
            {
                // Set up configuration folder and files
                SavedState.SetupConfigFiles("MageFileProcessor");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

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
            lblAboutLink.LinkClicked += lblAboutLink_LinkClicked;

            // Connect the pipeline queue to message handlers
            ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

            // Connect callbacks for UI panels
            FileProcessingPanel1.GetSelectedFileInfo += GetSelectedFileItem;
            FileProcessingPanel1.GetSelectedOutputInfo += GetSelectedOutputItem;

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
            txtAbout1.Text = "Mage File Processor can search for files associated with DMS analysis jobs or datasets, then copy those files to the local computer.  The files can optionally be combined into a single tab-delimited text file or a single SQLite database.";
            txtAbout2.Text = "Written by Gary Kiebel and Matthew Monroe in 2011 for the Department of Energy (PNNL, Richland, WA).";
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


        #endregion

        #region Command Processing

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
        /// Execute a command by building and running
        /// the appropriate pipeline (or cancelling
        /// the current pipeline activity)
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="command">Command to execute</param>
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
            if (command.Action == "cancel_operation" && mPipelineQueue != null && mPipelineQueue.IsRunning)
            {
                mPipelineQueue.Cancel();
                return;
            }

            // Don't allow another pipeline if one is currently running
            if (mPipelineQueue != null && mPipelineQueue.IsRunning)
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
                        sink = JobListDisplayControl.MakeSink(command.Mode, 15);

                        pipeline = Pipelines.MakeJobQueryPipeline(sink, queryDefXML, queryParameters);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        break;

                    case "get_entities_from_flex_query":
                        queryDefXML = ModuleDiscovery.GetQueryXMLDef(command.Mode);
                        var builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML);
                        if (!builder.HasPredicate)
                        {
                            MessageBox.Show("You must define one or more search criteria before searching", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        var reader = new MSSQLReader(builder);
                        sink = JobListDisplayControl.MakeSink("Jobs", 15);

                        pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, sink);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        break;

                    case "get_files_from_entities":
                        var entityType = JobListDisplayControl.PageTitle;
                        runtimeParams = GetRuntimeParamsForEntityFileType(entityType);
                        source = new GVPipelineSource(JobListDisplayControl, mode);
                        sink = FileListDisplayControl.MakeSink("Files", 15);

                        pipeline = Pipelines.MakeFileListPipeline(source, sink, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        mFileSourcePipelineName = pipeline.PipelineName;
                        break;

                    case "get_files_from_local_folder":
                        runtimeParams = GetRuntimeParamsForLocalFolder();
                        var sFolder = GetRuntimeParam(runtimeParams, "Folder");
                        if (!Directory.Exists(sFolder))
                        {
                            MessageBox.Show("Folder not found: " + sFolder, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        sink = FileListDisplayControl.MakeSink("Files", 15);

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
                        sink = FileListDisplayControl.MakeSink("Files", 15);

                        pipeline = Pipelines.MakePipelineToGetFilesFromManifest(sink, runtimeParams);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
                        mFileSourcePipelineName = pipeline.PipelineName;
                        break;

                    case "copy_files":
                        runtimeParams = GetRuntimeParamsForCopyFiles();
                        if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "OutputFolder")))
                        {
                            MessageBox.Show("Destination folder cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                                if (string.IsNullOrEmpty(GetRuntimeParam(runtimeParams, "OutputFolder")))
                                {
                                    MessageBox.Show("Destination folder cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            var bFilterDefined = false;

            foreach (var entry in queryParameters)
            {
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
                    case TAG_JOB_IDs:
                        msg = "Job ID list cannot be empty";
                        break;
                    case TAG_JOB_IDs_FROM_DATASETS:
                        msg = "Dataset list cannot be empty";
                        break;
                    case TAG_DATASET_ID_LIST:
                        msg = "Dataset ID list cannot be empty";
                        break;
                    case TAG_DATA_PACKAGE_ID:
                    case TAG_DATA_PACKAGE_DS_IDs:
                        msg = "Please enter a data package ID";
                        break;
                    default:
                        msg = "You must define one or more search criteria before searching for jobs";
                        break;
                }
            }

            if (string.IsNullOrEmpty(msg) && (queryName == TAG_JOB_IDs || queryName == TAG_JOB_IDs_FROM_DATASETS || queryName == TAG_DATASET_ID_LIST))
            {
                var cSepChars = new[] { ',', '\t' };
                string sWarning;

                if (queryName == TAG_JOB_IDs)
                    sWarning = "Job number '";
                else
                    sWarning = "Use dataset IDs, not dataset names: '";

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

        const string mFileListLabelPrefix = "Files From ";

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            // Initial labels for display list control panels
            JobListDisplayControl.PageTitle = "Entities";
            FileListDisplayControl.PageTitle = "Files";
            FileListDisplayControl.AutoSizeColumnWidths = true;

            JobDatasetIDList1.Legend = "(Dataset IDs)";
            JobDatasetIDList1.ListName = "Dataset_ID";

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
            if (entityCount == 0)
            {
                EntityFilePanel1.Enabled = false;
            }
            else
            {
                EntityFilePanel1.Enabled = true;
            }
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
                case "get_files_from_local_folder":
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + "Local Folder";
                    FileCopyPanel1.ApplyPrefixToFileName = "No";
                    FileCopyPanel1.PrefixColumnName = "";
                    break;
                case "get_files_from_local_manifest":
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + "Manifest";
                    FileCopyPanel1.ApplyPrefixToFileName = "No";
                    FileCopyPanel1.PrefixColumnName = "";
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

            if (!(mCurrentCmdSender is IMageDisplayControl))
            {
                return;
            }

            var ldc = (IMageDisplayControl)mCurrentCmdSender;
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

        #endregion

        #region Support functions for building runtime parameter lists from component panels

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
        /// <param name="runtimeParams">Runtime parameters</param>
        /// <param name="keyName">Parameter to find</param>
        /// <returns>The value for the parameter</returns>
        /// <remarks>Raises an exception if the runtimeParams dictionary does not have the desired key</remarks>
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

        private Dictionary<string, string> GetRuntimeParamsForLocalFolder()
        {
            var rp = new Dictionary<string, string>
            {
                {"FileNameFilter", LocalFolderPanel1.FileNameFilter},
                {"FileSelectionMode", LocalFolderPanel1.FileSelectionMode},
                {"Folder", LocalFolderPanel1.Folder},
                {"SearchInSubfolders", LocalFolderPanel1.SearchInSubfolders},
                {"SubfolderSearchName", LocalFolderPanel1.SubfolderSearchName}
            };
            return rp;
        }

        private Dictionary<string, string> GetRuntimeParamsForManifestFile()
        {
            var rp = new Dictionary<string, string>
            {
                {"ManifestFilePath", LocalManifestPanel1.ManifestFilePath}
            };
            return rp;
        }

        private Dictionary<string, string> GetRuntimeParamsForFileProcessing()
        {
            var rp = new Dictionary<string, string>
            {
                {"OutputFolder", FolderDestinationPanel1.OutputFolder},
                {"OutputFile", FolderDestinationPanel1.OutputFile},
                {"DatabaseName", SQLiteDestinationPanel1.DatabaseName},
                {"TableName", SQLiteDestinationPanel1.TableName},
                {"OutputMode", FilterOutputTabs.SelectedTab.Tag.ToString()},
                {"ManifestFileName", string.Format("Manifest_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now)}
            };

            if (mFileSourcePipelineName == Pipelines.PIPELINE_GET_LOCAL_FILES)
            {
                rp.Add("SourceFileColumnName", "File");
            }
            else
            {
                rp.Add("SourceFileColumnName", "Name");
            }

            return rp;
        }

        private Dictionary<string, string> GetRuntimeParamsForCopyFiles()
        {
            var rp = new Dictionary<string, string>
            {
                {"OutputFolder", FileCopyPanel1.OutputFolder},
                {"ManifestFileName", string.Format("Manifest_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now)},
                {"ApplyPrefixToFileName", FileCopyPanel1.ApplyPrefixToFileName},
                {"PrefixLeader", FileCopyPanel1.PrefixLeader},
                {"ColumnToUseForPrefix", FileCopyPanel1.PrefixColumnName},
                {"OverwriteExistingFiles", FileCopyPanel1.OverwriteExistingFiles},
                {"SourceFolderColumnName", "Folder"},
                {"ResolveCacheInfoFiles", FileCopyPanel1.ResolveCacheInfoFiles}
            };

            if (mFileSourcePipelineName == Pipelines.PIPELINE_GET_LOCAL_FILES)
            {
                rp.Add("SourceFileColumnName", "File");
                rp.Add("OutputColumnList", "File, *");
                rp.Add("OutputFileColumnName", "File");
            }
            else
            {
                rp.Add("SourceFileColumnName", "Name");
                rp.Add("OutputColumnList", "Name, *");
                rp.Add("OutputFileColumnName", "Name");
            }

            return rp;
        }

        private Dictionary<string, string> GetRuntimeParamsForEntityFileType(string entityType)
        {
            var rp = new Dictionary<string, string>
            {
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
                    rp.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Folder, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument");
                    break;
                case "Datasets":
                    rp.Add("OutputColumnList", "Item|+|text, Name|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_SIZE + "|+|text, " + FileListInfoBase.COLUMN_NAME_FILE_DATE + "|+|text, Folder, Dataset, Dataset_ID, Experiment, Campaign, State, Instrument, Created, Type, Comment");
                    break;
            }
            return rp;
        }

        /// <summary>
        /// Get "best" column name to use for naming prefix according to heuristic
        /// </summary>
        /// <returns></returns>
        private static string GetBestPrefixIDColumnName(IEnumerable<MageColumnDef> colDefs)
        {
            var IDColumnName = "";

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

        #endregion

        #region Functions for handling status updates

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

            if (sender is ProcessingPipeline pipeline)
            {
                if (pipeline.PipelineName == mFinalPipelineName)
                {

                    CompletionStateUpdated csu = AdjustPostCommandUIState;
                    Invoke(csu, new object[] { null });

                    // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
                    VoidFnDelegate et = EnableDisableOutputTabs;
                    Invoke(et);
                }
            }

            if (args.Message.StartsWith(MSSQLReader.SQL_COMMAND_ERROR))
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

        private void lblAboutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        #endregion

        #region Panel Support Functions

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
            JobFlexQueryPanel.SetColumnPickList(new[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign", "Organism", "Organism DB", "Protein Collection List", "Protein Options", "Comment", "Results Folder", "Folder", "Dataset_Created", "Job_Finish", "Request_ID" });
            JobFlexQueryPanel.SetComparisionPickList(new[] { "ContainsText", "DoesNotContainText", "StartsWithText", "MatchesText", "MatchesTextOrBlank", "Equals", "NotEqual", "GreaterThan", "GreaterThanOrEqualTo", "LessThan", "LessThanOrEqualTo", "MostRecentWeeks", "LaterThan", "EarlierThan", "InList" });
        }

        #endregion

        #region Callback Functions for UI Panel Use

        /// <summary>
        /// Get fields from selected item in file list
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetSelectedFileItem()
        {
            return FileListDisplayControl.SeletedItemFields;
        }

        private Dictionary<string, string> GetSelectedOutputItem()
        {
            if ("SQLite_Output" == FilterOutputTabs.SelectedTab.Tag.ToString())
            {
                return SQLiteDestinationPanel1.GetParameters();
            }

            return FolderDestinationPanel1.GetParameters();
        }

        #endregion
    }

}
