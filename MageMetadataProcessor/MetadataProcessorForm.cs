using System;
using System.IO;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;
using MageMetadataProcessor.Properties;
using MageUIComponents;
using PRISM.Logging;

namespace MageMetadataProcessor
{

    public partial class MetadataProcessorForm : Form
    {

        #region Member Variables

        // Current Mage pipeline that is running or has most recently run
        ProcessingPipeline mCurrentPipeline;

        // Current command that is being executed or has most recently been executed
        MageCommandEventArgs mCurrentCmd;

        // Object that sent the current command
        private object mCurrentCmdSender;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataProcessorForm()
        {
            InitializeComponent();

            // These settings are loaded from file MageMetadataProcessor.exe.config
            // Typically gigasax and DMS5
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;

            ModuleDiscovery.DMSServerOverride = Globals.DMSServer;
            ModuleDiscovery.DMSDatabaseOverride = Globals.DMSDatabase;

            try
            {
                // Set up configuration folder and files
                SavedState.SetupConfigFiles("MageMetadataProcessor");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Tell modules where to look for loadable module DLLs
            var fi = new FileInfo(Application.ExecutablePath);
            ModuleDiscovery.ExternalModuleFolder = fi.DirectoryName;

            // Configure logging
            var logFilePath = Path.Combine(SavedState.DataDirectory, "log.txt");
            const bool appendDateToBaseName = false;
            FileLogger.ChangeLogFileBaseName(logFilePath, appendDateToBaseName);
            FileLogger.WriteLog(BaseLogger.LogLevels.INFO, "Starting Mage Metadata Processor");

            ProcessingPipeline.AppendDateToLogFileName = FileLogger.AppendDateToBaseFileName;
            ProcessingPipeline.LogFilePath = logFilePath;

            // Setup UI component panels
            SetupCommandHandler();
            SetupColumnMapping();
            SetupStatusPanel();

            // restore settings to UI component panels
            ////SavedState.FilePath = Path.Combine(mDataDirectory, "SavedState.xml");
            ////SavedState.RestoreSavedPanelParameters(GetParameterPanelList());

            AdjustInitialUIState();
        }

        #endregion

        #region Command Processing

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

            // Cancel the currently running pipeline
            if (command.Action == "cancel_operation" && mCurrentPipeline != null && mCurrentPipeline.Running)
            {
                mCurrentPipeline.Cancel();
                return;
            }

            // Don't allow another pipeline if one is currently running
            if (mCurrentPipeline != null && mCurrentPipeline.Running)
            {
                MessageBox.Show("Pipeline is already active");
                return;
            }

            // Construct suitable Mage pipeline for the given command
            // and run that pipeline
            BuildAndRunPipeline(command);
        }

        /// <summary>
        /// Construnct and run a Mage pipeline for the given command
        /// </summary>
        /// <param name="command"></param>
        private void BuildAndRunPipeline(MageCommandEventArgs command)
        {
            mCurrentPipeline = null;
            var mode = (command.Mode == "selected") ? DisplaySourceMode.Selected : DisplaySourceMode.All;
            ISinkModule display;

            // Typically gigasax and DMS5
            var server = Globals.DMSServer;
            var database = Globals.DMSDatabase;
            string sql;

            switch (command.Action)
            {
                case "basic_read":
                    sql = rawQueryPanel1.RawSQL;
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "dataset_metadata":
                    sql = "SELECT * FROM V_Mage_Dataset_Factor_Metadata WHERE Dataset LIKE '%{0}%'";
                    sql = string.Format(sql, datasetFactorsPanel1.DatasetName);
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "factor_count":
                    sql = GetDatasetSql("COUNT(*) AS Rows");
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "factor_list":
                    sql = GetDatasetSql("Dataset, Dataset_ID, Factor, Value");
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "factor_crosstab":
                    sql = GetDatasetSql("Dataset, Dataset_ID, Factor, Value");
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeCrosstabQueryPipeline(display, server, database, sql);
                    break;
                case "save_to_db":
                    var source = new GVPipelineSource(gridViewDisplayControl1, mode);
                    var filePath = sqLiteDBPanel1.DBFilePath;
                    var tableName = sqLiteDBPanel1.TableName;
                    var outputColumnList = sqLiteDBPanel1.OutputColumnList;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        MessageBox.Show("Database file path cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (string.IsNullOrEmpty(tableName))
                    {
                        MessageBox.Show("Database table name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    mCurrentPipeline = MakeSaveToDBPipeline(source, filePath, tableName, outputColumnList);
                    break;
            }
            try
            {
                if (mCurrentPipeline == null)
                {
                    MessageBox.Show(string.Format("Could not build pipeline for '{0}' operation", command.Action));
                }
                else
                {
                    mCurrentCmd = command;
                    mCurrentPipeline.OnStatusMessageUpdated += statusPanel1.HandleStatusMessageUpdated;
                    mCurrentPipeline.OnRunCompleted += statusPanel1.HandleCompletionMessageUpdate;
                    mCurrentPipeline.OnRunCompleted += HandlePipelineCompletion;
                    EnableCancel(true);
                    mCurrentPipeline.Run();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        private string GetDatasetSql(string cols)
        {
            var sql = "SELECT " + cols + " FROM V_Custom_Factors_List_Report ";
            var datasetName = datasetFactorsPanel1.DatasetName;
            var dataPackageNumber = datasetFactorsPanel1.DataPackageNumber;
            var filter = " WHERE ";
            if (!string.IsNullOrEmpty(datasetName))
            {
                sql += filter + " Dataset LIKE '%" + datasetName + "%' ";
                filter = " AND ";
            }
            if (!string.IsNullOrEmpty(dataPackageNumber))
            {
                sql += filter + "Dataset IN ( SELECT Dataset FROM S_V_Data_Package_Datasets_Export WHERE Data_Package_ID = " + dataPackageNumber + " )";
            }
            return sql;
        }

        #region Functions for setting UI state

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState()
        {
            rawQueryPanel1.RawSQL = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset LIKE 'sarc_ms%'";
            sqLiteDBPanel1.DBFilePath = @"C:\Data\Junk\metadata.db";
            sqLiteDBPanel1.TableName = "factors";
            EnableCancel(false);
        }

        /// <summary>
        /// Enable/Disable the cancel button
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableCancel(bool enabled)
        {
            statusPanel1.EnableCancel = enabled;
        }

        private void AdjusttPostCommndUIState(object status)
        {
            EnableCancel(false);
        }

        #endregion

        #region Functions for handling status updates

        private delegate void CompletionStateUpdated(object status);

        /// <summary>
        /// Handle the status completion message from the currently running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleCompletionMessageUpdate(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine(args.Message);

            CompletionStateUpdated csu = AdjusttPostCommndUIState;
            Invoke(csu, new object[] { null });
        }

        #endregion


        #region Mage Pipelines

        /// <summary>
        /// Build Mage pipeline to save contents of list display to SQLite database
        /// </summary>
        /// <param name="sourceObject">Mage module that can deliver contents of ListView on standard tabular input</param>
        /// <param name="filePath">File to save contents to</param>
        /// <param name="tableName"></param>
        /// <param name="outputColumnList"></param>
        /// <returns></returns>
        private static ProcessingPipeline MakeSaveToDBPipeline(IBaseModule sourceObject, string filePath, string tableName, string outputColumnList)
        {
            var writer = new SQLiteWriter
            {
                DbPath = filePath,
                TableName = tableName
            };

            ProcessingPipeline pipeline;
            if (string.IsNullOrEmpty(outputColumnList))
            {
                pipeline = ProcessingPipeline.Assemble("SaveListDisplayPipeline", sourceObject, writer);
            }
            else
            {
                var filter = new NullFilter {OutputColumnList = outputColumnList};
                pipeline = ProcessingPipeline.Assemble("SaveListDisplayPipeline", sourceObject, filter, writer);
            }
            return pipeline;
        }

        private ProcessingPipeline MakeRawQueryPipeline(ISinkModule display, string server, string database, string sql)
        {
            var reader = new MSSQLReader
            {
                Server = server,
                Database = database,
                SQLText = sql
            };

            return ProcessingPipeline.Assemble("MetadataPreview", reader, display);
        }

        private ProcessingPipeline MakeCrosstabQueryPipeline(ISinkModule display, string server, string database, string sql)
        {
            var reader = new MSSQLReader
            {
                Server = server,
                Database = database,
                SQLText = sql
            };

            var filter = new CrosstabFilter
            {
                EntityNameCol = "Dataset",
                EntityIDCol = "Dataset_ID",
                FactorNameCol = "Factor",
                FactorValueCol = "Value"
            };

            return ProcessingPipeline.Assemble("MetadataPreview", reader, filter, display);
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
            var methodInfo = this.GetType().GetMethod("DoCommand");
            Control subjectControl = this;

            PanelSupport.DiscoverAndConnectCommandHandlers(subjectControl, methodInfo);
        }

        /// <summary>
        /// Setup path to column mapping config file for selection forms
        /// </summary>
        private void SetupColumnMapping()
        {
            ColumnMapSelectionForm.MappingConfigFilePath = Path.Combine(SavedState.DataDirectory, "ColumnMappingConfig.db");
        }

        #endregion

    }
}
