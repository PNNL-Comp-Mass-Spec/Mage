using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using Mage;
using MageDisplayLib;

namespace MageConcatenator
{
    public partial class MageConcatenator : Form
    {
        #region Member Variables

        public const string PROGRAM_DATE = "October 23, 2014";

        /// <summary>
        /// Pipeline queue for running the multiple pipelines that make up the workflows for this module
        /// </summary>
        private readonly PipelineQueue mPipelineQueue = new PipelineQueue();

        private string mFinalPipelineName = string.Empty;

        private Timer mFileInfoUpdater;
        private bool mFileInfoUpdateRequired;

        // current command that is being executed or has most recently been executed
        MageCommandEventArgs mCurrentCmd;

        // object that sent the current command
        object mCurrentCmdSender;

        #endregion

        #region Initialization

        public MageConcatenator()
        {
            InitializeComponent();

            try
            {
                // set up configuration folder and files
                SavedState.SetupConfigFiles("MageConcatenator");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            try
            {
                // set up configuration folder and files
                // Set log4net path and kick the logger into action
                string LogFileName = Path.Combine(SavedState.DataDirectory, "log.txt");
                log4net.GlobalContext.Properties["LogName"] = LogFileName;
                ILog traceLog = LogManager.GetLogger("TraceLog");
                traceLog.Info("Starting");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // setup UI component panels
            SetupStatusPanel();
            SetupCommandHandler();
            SetupFilterSelectionListForFileProcessor();

            // setup context menus for list displays
            new GridViewDisplayActions(FileListDisplayControl);

            // Connect the pipeline queue to message handlers
            ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

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
        /// execute a command by building and running 
        /// the appropriate pipeline (or cancelling
        /// the current pipeline activity)
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="command">Command to execute</param>
        public void DoCommand(object sender, MageCommandEventArgs command)
        {

            // remember who sent us the command
            mCurrentCmdSender = sender;

            if (command.Action == "display_reloaded")
            {
                mCurrentCmd = command;
                AdjusttPostCommndUIState(null);
                return;
            }

            // cancel the currently running pipeline
            if (command.Action == "cancel_operation" && mPipelineQueue != null && mPipelineQueue.IsRunning)
            {
                mPipelineQueue.Cancel();
                return;
            }

            // don't allow another pipeline if one is currently running
            if (mPipelineQueue != null && mPipelineQueue.IsRunning)
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
            DisplaySourceMode mode = (command.Mode == "selected") ? DisplaySourceMode.Selected : DisplaySourceMode.All;

            mPipelineQueue.Pipelines.Clear();

            try
            {
                // build and run the pipeline appropriate to the command
                Dictionary<string, string> runtimeParms;
                GVPipelineSource source;
                ISinkModule sink;
                string queryDefXML;
                ProcessingPipeline pipeline;

                switch (command.Action)
                {
                    case "get_files_from_local_folder":
                        runtimeParms = GetRuntimeParmsForLocalFolder();
                        string sFolder = runtimeParms["Folder"];
                        if (!Directory.Exists(sFolder))
                        {
                            MessageBox.Show("Folder not found: " + sFolder, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        sink = FileListDisplayControl.MakeSink("Files", 15);

                        pipeline = Pipelines.MakePipelineToGetLocalFileList(sink, runtimeParms);
                        mPipelineQueue.Pipelines.Enqueue(pipeline);
                        mFinalPipelineName = pipeline.PipelineName;
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

                    foreach (ProcessingPipeline p in mPipelineQueue.Pipelines.ToArray())
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

        #endregion

        #region File Grocessing Routines

        private void CombineFiles(bool processAllFiles)
        {

            try
            {
                var runtimeParms = GetRuntimeParmsForFileProcessing();

                if (string.IsNullOrWhiteSpace(runtimeParms["OutputFolder"]))
                {
                    MessageBox.Show("Destination folder cannot be empty", "Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(runtimeParms["OutputFile"]))
                {
                    MessageBox.Show("Destination file cannot be empty", "Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    return;
                }

                string targetFilePath = Path.Combine(runtimeParms["OutputFolder"], runtimeParms["OutputFile"]);

                if (processAllFiles)
                {
                    FileListDisplayControl.SelectAllRows();
                }

                // Construct the list of the file paths to concatenate
                var lstFilePaths = new List<string>();

                foreach (var selectedFileRow in FileListDisplayControl.SelectedItemRowsDictionaryList)
                {

                    string sourceFilePath = Path.Combine(selectedFileRow["Folder"], selectedFileRow["File"]);

                    // Make sure the target file is not in the source file list
                    if (System.String.Compare(sourceFilePath, targetFilePath, System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Auto-rename the target file
                        var fiTargetFile = new FileInfo(targetFilePath);
                        if (fiTargetFile.Directory == null)
                        {
                             MessageBox.Show("Error in CombineFiles: cannot determine the parent folder path for the target file, " + targetFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        var dtTimestamp = "_combined_" + DateTime.Now.ToString("HH:mm:ss").Replace(":", "_");
                        targetFilePath = Path.GetFileNameWithoutExtension(targetFilePath) + dtTimestamp +
                                         Path.GetExtension(targetFilePath);

                        targetFilePath = Path.Combine(fiTargetFile.Directory.FullName, targetFilePath);

                    }
                    lstFilePaths.Add(sourceFilePath);
                }

                if (lstFilePaths.Count == 0)
                {
                    statusPanel1.HandleStatusMessageUpdated(this, new MageStatusEventArgs("No files are selected; nothing to do"));
                    return;
                }
                

                // Clear any warnings
                statusPanel1.ClearWarnings();
                EnableCancel(true);

                var fileCombiner = new clsFileCombiner();
                fileCombiner.OnStatusUpdate += HandlePipelineUpdate;
                fileCombiner.OnError += HandlePipelineWarning;
                fileCombiner.OnWarning += HandlePipelineWarning;
                fileCombiner.OnRunCompleted += HandlePipelineCompletion;

                var success = fileCombiner.CombineFiles(lstFilePaths, targetFilePath);
                
                if (success)
                    AdjusttPostCommndUIState(null);
                else 
                    EnableCancel(false);

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in CombineFiles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        /// <summary>
        /// Obtains the data from the File List control
        /// </summary>
        /// <returns>Dictionary where keys are file paths and values are the file information for each file</returns>
        private List<clsFileInfo> GetSelectedFiles()
        {
            var lstSelectedFiles = new List<clsFileInfo>();
            var lstFilePaths = new SortedSet<string>();

            foreach (var selectedFileRow in FileListDisplayControl.SelectedItemRowsDictionaryList)
            {
                string folderPath;
                string filename;
                string dateModified;
                string fileSizeKB;

                selectedFileRow.TryGetValue("Folder", out folderPath);
                selectedFileRow.TryGetValue("File", out filename);
                selectedFileRow.TryGetValue("File_Size_KB", out fileSizeKB);
                selectedFileRow.TryGetValue("File_Date", out dateModified);

                if (string.IsNullOrWhiteSpace(folderPath))
                    folderPath = string.Empty;

                if (string.IsNullOrWhiteSpace(filename))
                {
                    continue;
                }

                var fullPath = Path.Combine(folderPath, filename);
                if (lstFilePaths.Contains(fullPath))
                {
                    continue;
                }

                var fileInfo = new clsFileInfo(folderPath, filename)
                {
                    DateModified = dateModified,
                    SizeKB = fileSizeKB
                };

                lstSelectedFiles.Add(fileInfo);
                lstFilePaths.Add(fullPath);
            }

            return lstSelectedFiles;
        }

        private void UpdateFileInformation()
        {
            try
            {

                FileListDisplayControl.SelectAllRows();

                // Construct the list of the file paths in the FileListDisplay
                var dctSelectedFiles = GetSelectedFiles();

                if (dctSelectedFiles.Count == 0)
                    return;

                var fileCombiner = new clsFileCombiner();

                var success = fileCombiner.UpdateFileRowColCounts(dctSelectedFiles);

                if (success)
                {
                    FileListDisplayControl.Clear();

                    var colDefs = new List<MageColumnDef>
                    {
                        new MageColumnDef("File", "text", "128"),
                        new MageColumnDef("File_Size_KB", "float", "32"),
                        new MageColumnDef("File_Date", "string", "128"),
                        new MageColumnDef("Folder", "string", "255"),
                        new MageColumnDef("Rows", "string", "32"),
                        new MageColumnDef("Columns", "string", "32")
                    };

                    var colDefArgs = new MageColumnEventArgs(colDefs);
                    FileListDisplayControl.HandleColumnDef(this, colDefArgs);

                    foreach (var currentFile in dctSelectedFiles)
                    {
                        var data = new string[colDefs.Count];
                        data[0] = currentFile.Name;
                        data[1] = currentFile.SizeKB;
                        data[2] = currentFile.DateModified;
                        data[3] = currentFile.FolderPath;

                        if (currentFile.Rows > clsFileCombiner.MAX_ROWS_TO_TRACK)
                            data[4] = clsFileCombiner.MAX_ROWS_TO_TRACK + "+";
                        else
                            data[4] = currentFile.Rows.ToString("#,##0");

                        data[5] = currentFile.Columns.ToString("0");

                        var dataRowArgs = new MageDataEventArgs(data);

                        FileListDisplayControl.HandleDataRow(this, dataRowArgs);
                    }

                    FileListDisplayControl.HandleDataRow(this, new MageDataEventArgs(null));
                    FileListDisplayControl.SelectAllRows();

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in UpdateFileInformation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        #endregion

        #region Functions for setting UI state

        const string mFileListLabelPrefix = "Files From ";

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            // initial labels for display list control panels
            FileListDisplayControl.PageTitle = "Files";
            FileListDisplayControl.AutoSizeColumnWidths = true;

            // disable certain UI component panels 
            FolderDestinationPanel1.Enabled = false;

            EnableCancel(false);

            mFileInfoUpdater = new Timer
            {
                Interval = 100,
                Enabled = true
            };
            mFileInfoUpdater.Tick += mFileInfoUpdater_Tick;
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
            if (mCurrentCmd == null) return;

            EnableCancel(false);

            AdjustFileListLabels();
            AdjustFileProcessingPanels();

            SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
        }

        /// <summary>
        /// Processing files is only possible when file list contains files,
        /// adjust the processing panels to inform user
        /// </summary>
        private void AdjustFileProcessingPanels()
        {
            int fileCount = FileListDisplayControl.ItemCount;
            if (fileCount == 0)
            {
                FolderDestinationPanel1.Enabled = false;
            }
            else
            {
                FolderDestinationPanel1.Enabled = true;
            }
        }
        /// <summary>
        /// Since the list of files can be derived from different sources,
        /// adjust the labelling to inform the user about which one was used
        /// </summary>
        private void AdjustFileListLabels()
        {
            switch (mCurrentCmd.Action)
            {
                case "get_files_from_local_folder":
                    FileListDisplayControl.PageTitle = mFileListLabelPrefix + "Local Folder";
                    break;
            }
        }

        private Dictionary<string, string> GetRuntimeParmsForLocalFolder()
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
        private Dictionary<string, string> GetRuntimeParmsForFileProcessing()
        {
            var rp = new Dictionary<string, string>
			{
			    {"OutputFolder", FolderDestinationPanel1.OutputFolder},
			    {"OutputFile", FolderDestinationPanel1.OutputFile},
			    {"OutputMode", "File_Output"},
			    {"ManifestFileName", string.Format("Manifest_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now)},
			    {"SourceFileColumnName", "File"}
			};

            return rp;
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
        /// handle updating control enable status on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleCompletionMessageUpdate(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine(args.Message);

            var pipeline = sender as ProcessingPipeline;
            if (pipeline != null)
            {
                if (pipeline.PipelineName == mFinalPipelineName)
                {

                    CompletionStateUpdated csu = AdjusttPostCommndUIState;
                    Invoke(csu, new object[] { null });

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
            mFileInfoUpdateRequired = true;
        }


        private void mFileInfoUpdater_Tick(object sender, EventArgs e)
        {
            if (mFileInfoUpdateRequired)
            {
                mFileInfoUpdateRequired = false;
                UpdateFileInformation();
            }
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
            MethodInfo methodInfo = this.GetType().GetMethod("DoCommand");
            Control subjectControl = this;

            PanelSupport.DiscoverAndConnectCommandHandlers(subjectControl, methodInfo);
        }

        /// <summary>
        /// set up filter selection list for file processing panel
        /// </summary>
        private static void SetupFilterSelectionListForFileProcessor()
        {
            ModuleDiscovery.SetupFilters();
        }

        #endregion

        #region Button Events

        private void cmdAbout_Click(object sender, EventArgs e)
        {
            string message = "Written by Matthew Monroe for the Department of Energy.  This is version " +
                             System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (" + PROGRAM_DATE + ")";
            MessageBox.Show(message, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProcessAllFilesCtl_Click(object sender, EventArgs e)
        {
            CombineFiles(true);
        }

        private void ProcessSelectedFilesCtl_Click(object sender, EventArgs e)
        {
            CombineFiles(false);
        }

        #endregion
     
    }
}
