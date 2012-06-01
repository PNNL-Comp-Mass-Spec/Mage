using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Mage;
using log4net;
using MageDisplayLib;
using System.Collections.ObjectModel;
using System.Reflection;
using MageUIComponents;

namespace MageMetadataProcessor {

    public partial class MetadataProcessorForm : Form {

        #region Member Variables

        // current Mage pipeline that is running or has most recently run
        ProcessingPipeline mCurrentPipeline = null;

        // current command that is being executed or has most recently been executed
        MageCommandEventArgs mCurrentCmd = null;

        // object that sent the current command
        object mCurrentCmdSender = null;

        //private static readonly ILog traceLog = LogManager.GetLogger("TraceLog");
        private ILog traceLog; //= LogManager.GetLogger("TraceLog");

        #endregion

        #region Initialization

        public MetadataProcessorForm() {
            InitializeComponent();

            try {
                // set up configuration folder and files
                SavedState.SetupConfigFiles("MageMetadataProcessor");
            } catch (Exception ex){
                System.Windows.Forms.MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // tell modules where to look for loadable module DLLs
            FileInfo fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
            ModuleDiscovery.ExternalModuleFolder = fi.DirectoryName;

            //Set log4net path
            string LogFileName = Path.Combine(SavedState.DataDirectory, "log.txt");
            log4net.GlobalContext.Properties["LogName"] = LogFileName;

            traceLog = LogManager.GetLogger("TraceLog");
            // kick the logger into action
            traceLog.Info("Starting");


            // setup UI component panels
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
        /// execute a command by building and running 
        /// the appropriate pipeline (or cancelling
        /// the current pipeline activity)
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="command">Command to execute</param>
        public void DoCommand(object sender, MageCommandEventArgs command) {

            // remember who sent us the command
            mCurrentCmdSender = sender;

            // cancel the currently running pipeline
            if (command.Action == "cancel_operation" && mCurrentPipeline != null && mCurrentPipeline.Running) {
                mCurrentPipeline.Cancel();
                return;
            }
            // don't allow another pipeline if one is currently running
            if (mCurrentPipeline != null && mCurrentPipeline.Running) {
                MessageBox.Show("Pipeline is already active");
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
        private void BuildAndRunPipeline(MageCommandEventArgs command) {
            mCurrentPipeline = null;
            DisplaySourceMode mode = (command.Mode == "selected") ? DisplaySourceMode.Selected : DisplaySourceMode.All;
            GVPipelineSource source = null;
            ISinkModule display = null;
            string server = "gigasax";
            string database = "DMS5";
            string sql = "";

            switch (command.Action) {
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
                    sql = "SELECT COUNT(*) AS Rows FROM V_Custom_Factors_List_Report WHERE (Dataset LIKE '%{0}%')";
                    sql = string.Format(sql, datasetFactorsPanel1.DatasetName);
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "factor_list":
                    sql = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE (Dataset LIKE '%{0}%')";
                    sql = string.Format(sql, datasetFactorsPanel1.DatasetName);
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeRawQueryPipeline(display, server, database, sql);
                    break;
                case "factor_crosstab":
                    sql = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE (Dataset LIKE '%{0}%')";
                    sql = string.Format(sql, datasetFactorsPanel1.DatasetName);
                    display = gridViewDisplayControl1.MakeSink("Metadata", 25);
                    mCurrentPipeline = MakeCrosstabQueryPipeline(display, server, database, sql);
                    break;
                case "save_to_db":
                    source = new GVPipelineSource(gridViewDisplayControl1, mode);
                    string filePath = sqLiteDBPanel1.DBFilePath;
                    string tableName = sqLiteDBPanel1.TableName;
                    string outputColumnList = sqLiteDBPanel1.OutputColumnList;
                    mCurrentPipeline = MakeSaveToDBPipeline(source, filePath, tableName, outputColumnList);
                    break;
                default:
                    break;
            }
            try {
                if (mCurrentPipeline == null) {
                    MessageBox.Show(string.Format("Could not build pipeline for '{0}' operation", command.Action));
                } else {
                    mCurrentCmd = command;
                    mCurrentPipeline.OnStatusMessageUpdated += statusPanel1.HandleStatusMessageUpdated;
                    mCurrentPipeline.OnRunCompleted += statusPanel1.HandleCompletionMessageUpdate;
                    mCurrentPipeline.OnRunCompleted += HandlePipelineCompletion;
                    EnableCancel(true);
                    mCurrentPipeline.Run();
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        #region Functions for setting UI state

        /// <summary>
        /// Set initial conditions for display components
        /// </summary>
        private void AdjustInitialUIState() {
            rawQueryPanel1.RawSQL = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset LIKE 'sarc_ms%'";
            sqLiteDBPanel1.DBFilePath = @"C:\Data\Junk\metadata.db";
            sqLiteDBPanel1.TableName = "factors";
            EnableCancel(false);
        }

        /// <summary>
        /// Enable/Disable the cancel button
        /// </summary>
        /// <param name="enabled"></param>
        private void EnableCancel(bool enabled) {
            statusPanel1.EnableCancel = enabled;
        }

        private void AdjusttPostCommndUIState(object status) {
            if (mCurrentCmd == null) return;

        }

        #endregion

        #region Functions for handling status updates

        private delegate void CompletionStateUpdated(object status);

        /// <summary>
        /// handle the status completion message from the currently running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args) {
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
        /// <returns></returns>
        private static ProcessingPipeline MakeSaveToDBPipeline(IBaseModule sourceObject, string filePath, string tableName, string outputColumnList) {
            SQLiteWriter writer = new SQLiteWriter();
            writer.DbPath = filePath;
            writer.TableName = tableName;

            ProcessingPipeline pipeline = null;
            if (string.IsNullOrEmpty(outputColumnList)) {
                pipeline = ProcessingPipeline.Assemble("SaveListDisplayPipeline", sourceObject, writer);
            } else {
                NullFilter filter = new NullFilter();
                filter.OutputColumnList = outputColumnList;
                pipeline = ProcessingPipeline.Assemble("SaveListDisplayPipeline", sourceObject, filter, writer);
            }
            return pipeline;
        }

        private ProcessingPipeline MakeRawQueryPipeline(ISinkModule display, string server, string database, string sql) {
            MSSQLReader reader = new MSSQLReader();
            reader.Server = server;
            reader.Database = database;
            reader.SQLText = sql;

            return ProcessingPipeline.Assemble("MetadataPreview", reader, display);
        }

        private ProcessingPipeline MakeCrosstabQueryPipeline(ISinkModule display, string server, string database, string sql) {
            MSSQLReader reader = new MSSQLReader();
            reader.Server = server;
            reader.Database = database;
            reader.SQLText = sql;

            CrosstabFilter filter = new CrosstabFilter();
            filter.EntityNameCol = "Dataset";
            filter.EntityIDCol = "Dataset_ID";
            filter.FactorNameCol = "Factor";
            filter.FactorValueCol = "Value";

            return ProcessingPipeline.Assemble("MetadataPreview", reader, filter, display);
        }

        #endregion

        #region Panel Support Functions

        /// <summary>
        /// set up status panel
        /// </summary>
        private void SetupStatusPanel() {
            statusPanel1.OwnerControl = this;
        }

        /// <summary>
        /// wire up the command event in panels that have it
        /// to the DoCommand event handler method
        /// </summary>
        private void SetupCommandHandler() {
            // get reference to the method that handles command events
            MethodInfo methodInfo = this.GetType().GetMethod("DoCommand");
            Control subjectControl = this;

            PanelSupport.DiscoverAndConnectCommandHandlers(subjectControl, methodInfo);
        }

        /// <summary>
        /// setup path to column mapping config file for selection forms
        /// </summary>
        private void SetupColumnMapping() {
            ColumnMapSelectionForm.MappingConfigFilePath = Path.Combine(SavedState.DataDirectory, "ColumnMappingConfig.db");
        }

        #endregion

    }
}
