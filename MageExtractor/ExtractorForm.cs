using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Mage;
using MageDisplayLib;
using MageExtExtractionFilters;
using MageExtractor.Properties;
using PRISM.Logging;

namespace MageExtractor
{
    public partial class ExtractorForm : Form
    {
        // Ignore Spelling: cmd, mage, workflows

        protected const string TAG_JOB_IDs = "Job_ID_List";
        protected const string TAG_JOB_IDs_FROM_DATASETS = "Jobs_From_Dataset_List";
        protected const string TAG_DATA_PACKAGE_ID = "Data_Package";

        /// <summary>
        /// Pipeline queue for running the multiple pipelines that make up the workflows for this module
        /// </summary>
        private PipelineQueue mPipelineQueue = new();

        /// <summary>
        /// The parameters for the slated extraction
        /// </summary>
        private ExtractionType mExtractionParams;

        /// <summary>
        /// Where extracted results will be delivered
        /// </summary>
        private DestinationType mDestination;

        /// <summary>
        /// Keeps track of the target directories to which files were saved; used by ClearTempFiles
        /// </summary>
        private readonly SortedSet<string> mOutputDirectoryPaths = new();

        private string mFinalPipelineName = string.Empty;

        public ExtractorForm()
        {
            InitializeComponent();

            const bool isBetaVersion = false;
            SetFormTitle("2021-10-12", isBetaVersion);

            SetTags();

            SetAboutText();

            // These settings are loaded from file MageExtractor.exe.config
            // Typically gigasax and DMS5
            Globals.DMSServer = Settings.Default.DMSServer;
            Globals.DMSDatabase = Settings.Default.DMSDatabase;

            txtServer.Text = "DMS Server: " + Globals.DMSServer;

            ModuleDiscovery.DMSServerOverride = Globals.DMSServer;
            ModuleDiscovery.DMSDatabaseOverride = Globals.DMSDatabase;

            try
            {
                // Set up configuration directory and files
                SavedState.SetupConfigFiles("MageExtractor");
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
                FileLogger.WriteLog(BaseLogger.LogLevels.INFO, "Starting Mage Extractor");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            ConnectPanelsToCommandHandlers();

            // Connect click events
            lblAboutLink.LinkClicked += AboutLink_LinkClicked;

            // Connect the pipeline queue to message handlers
            ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

            DisableCancelButton();

            SetupFlexQueryPanels(); // Must be done before restoring saved state

            try
            {
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
        }

        private void SetAboutText()
        {
            txtAbout1.Text = "Mage Extractor can extract MS/MS search results from SEQUEST, X!Tandem, Inspect, or MSGF+ analysis jobs and combine the results into a single tab-delimited text file or a single SQLite database.";
            txtAbout2.Text = "Written by Gary Kiebel and Matthew Monroe in 2011 for the Department of Energy (PNNL, Richland, WA)";
            lblAboutLink.Text = "http://prismwiki.pnl.gov/wiki/Mage_Extractor";
        }

        private void SetFormTitle(string programDate, bool beta)
        {
            var objVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
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
            DataPackageTabPage.Tag = TAG_DATA_PACKAGE_ID;
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

        private void ConnectPanelsToCommandHandlers()
        {
            statusPanel1.OnAction += HandleCancelCommand;
            JobSimpleQueryPanel.OnAction += HandleJobQueryCommand;
            JobIDListPanel1.OnAction += HandleJobQueryCommand;
            JobDatasetIDList1.OnAction += HandleJobQueryCommand;
            JobDataPackagePanel1.OnAction += HandleJobQueryCommand;
            JobFlexQueryPanel.OnAction += HandleJobFlexQueryCommand;
            extractionSettingsPanel1.OnAction += HandleExtractionCommand;
        }

        /// <summary>
        /// Disable and hide the cancel button
        /// </summary>
        private void DisableCancelButton()
        {
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

        private void HandleJobFlexQueryCommand(object sender, MageCommandEventArgs command)
        {
            if (GetJobList(command.Mode))
            {
                SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
            }
        }

        private void HandleJobQueryCommand(object sender, MageCommandEventArgs command)
        {
            var queryName = EntityListSourceTabs.SelectedTab.Tag.ToString();
            var queryTemplate = ModuleDiscovery.GetQueryXMLDef(queryName);

            if (sender is not IModuleParameters paramSource)
            {
                return;
            }

            var queryParameters = paramSource.GetParameters();

            if (!ValidQueryParameters(queryName, queryParameters))
            {
                return;
            }

            GetJobList(queryTemplate, queryParameters);
            SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
        }

        private void HandleExtractionCommand(object sender, MageCommandEventArgs command)
        {
            try
            {
                mExtractionParams = GetExtractionParameters();
                mDestination = GetDestinationParameters();

                if (mDestination == null)
                    return;

                if (!CheckForJobsToProcess()) return;

                var mode = (command.Mode == "all") ? DisplaySourceMode.All : DisplaySourceMode.Selected;
                var msg = ExtractionPipelines.CheckJobResultType(new GVPipelineSource(JobListDisplayCtl, mode), "Tool", mExtractionParams);
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!DestinationType.VerifyDestinationOptionsWithUser(mDestination)) return;

                // Validate the MSGF and FDR cutoffs
                if (!ValidateThreshold(mExtractionParams.MSGFCutoff, "MSGF SpecProb Cutoff"))
                    return;

                try
                {
                    if (!mOutputDirectoryPaths.Contains(mDestination.ContainerPath))
                        mOutputDirectoryPaths.Add(mDestination.ContainerPath);

                    BaseModule jobList = new GVPipelineSource(JobListDisplayCtl, mode);
                    ExtractFileContents(jobList);
                    SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error extracting results: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing extraction: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool ValidateThreshold(string cutoff, string cutoffDescription)
        {
            if (!string.Equals(cutoff, ExtractionFilter.ALL_PASS_CUTOFF, StringComparison.OrdinalIgnoreCase))
            {
                if (cutoff.EndsWith("%"))
                {
                    // FDR Cutoff, like 5%
                    cutoff = cutoff.Substring(0, cutoff.Length - 1);
                }

                if (!double.TryParse(cutoff, out var result))
                {
                    result = -1;
                }
                else
                {
                    if (result < 0 || result > 1)
                        result = -1;
                }

                if (result < 0)
                {
                    var msg = "Invalid value specified for " + cutoffDescription + ": '" + cutoff + "'; must either be 'All Pass' or a number between 0 and 1";
                    MessageBox.Show(msg, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }

        private void HandleCancelCommand(object sender, MageCommandEventArgs command)
        {
            mPipelineQueue.Cancel();
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
                msg = queryName switch
                {
                    TAG_JOB_IDs => "Job ID list cannot be empty",
                    TAG_JOB_IDs_FROM_DATASETS => "Dataset ID list cannot be empty",
                    TAG_DATA_PACKAGE_ID => "Please enter a data package ID",
                    _ => "You must define one or more search criteria before searching for jobs"
                };
            }

            if (string.IsNullOrEmpty(msg) && (queryName == TAG_JOB_IDs || queryName == TAG_JOB_IDs_FROM_DATASETS))
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

        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            statusPanel1.HandleCompletionMessageUpdate(this, new MageStatusEventArgs(args.Message));
            Console.WriteLine(args.Message);

            if (sender is ProcessingPipeline pipeline)
            {
                if (pipeline.PipelineName == mFinalPipelineName)
                {
                    // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
                    VoidFnDelegate dc = DisableCancelButton;
                    Invoke(dc);
                }
            }

            if (args.Message.StartsWith(SQLReader.SQL_COMMAND_ERROR))
                MessageBox.Show(args.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void HandlePipelineQueueUpdate(object sender, MageStatusEventArgs args)
        {
        }

        private void HandlePipelineQueueCompletion(object sender, MageStatusEventArgs args)
        {
        }

        private void AboutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LaunchMageExtractorHelpPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link that was clicked: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LaunchMageExtractorHelpPage()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            lblAboutLink.LinkVisited = true;

            // Call the Process.Start method to open the default browser
            // with a URL:
            System.Diagnostics.Process.Start(lblAboutLink.Text);
        }

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
        /// Build and run Mage pipeline queue to extract contents of results files
        /// for jobs given in jobList according to parameters defined in mExtractionParams
        /// and deliver output according to mDestination.  Also create metadata file for jobList.
        /// </summary>
        /// <param name="jobList"></param>
        private void ExtractFileContents(BaseModule jobList)
        {
            mPipelineQueue = ExtractionPipelines.MakePipelineQueueToExtractFromJobList(jobList, mExtractionParams, mDestination);
            foreach (var p in mPipelineQueue.Pipelines.ToArray())
            {
                ConnectPipelineToStatusDisplay(p);
                mFinalPipelineName = p.PipelineName;
            }
            ConnectPipelineQueueToStatusDisplay(mPipelineQueue);
            EnableCancel(true);
            mPipelineQueue.Run();
        }

        /// <summary>
        /// Build and run Mage pipeline to populate main list display with jobs
        /// </summary>
        /// <param name="queryName">Query name to use</param>
        private bool GetJobList(string queryName)
        {
            var result = false;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef(queryName);
            var builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML);
            if (builder.HasPredicate)
            {
                result = true;
                var reader = new SQLReader(builder);
                var pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, JobListDisplayCtl);
                ConnectPipelineToStatusDisplay(pipeline);
                JobListDisplayCtl.Clear();

                EnableCancel(true);
                mFinalPipelineName = pipeline.PipelineName;

                mPipelineQueue.Run(pipeline);
            }
            else
            {
                MessageBox.Show("You must define one or more search criteria before searching for jobs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return result;
        }

        /// <summary>
        /// Build and run Mage pipeline to populate main list display with jobs
        /// </summary>
        /// <param name="queryTemplate">XML query template (typically from QueryDefinitions.xml) </param>
        /// <param name="queryParameters">Key/Value pairs (column/value)</param>
        private void GetJobList(string queryTemplate, Dictionary<string, string> queryParameters)
        {
            var reader = new SQLReader(queryTemplate, queryParameters);
            JobListDisplayCtl.Clear();
            var pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, JobListDisplayCtl);
            ConnectPipelineToStatusDisplay(pipeline);

            EnableCancel(true);
            mFinalPipelineName = pipeline.PipelineName;

            mPipelineQueue.Run(pipeline);
        }

        /// <summary>
        /// Set the given content extraction module's destination parameters
        /// from the user's UI choices
        /// </summary>
        private DestinationType GetDestinationParameters()
        {
            DestinationType dest;
            switch (FilterOutputTabs.SelectedTab.Tag.ToString())
            {
                case "File_Output":
                    if (string.IsNullOrEmpty(FolderDestinationPanel1.OutputDirectory))
                    {
                        MessageBox.Show("Destination directory cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }

                    if (string.IsNullOrEmpty(FolderDestinationPanel1.OutputFile))
                    {
                        MessageBox.Show("Destination file cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }
                    dest = new DestinationType("File_Output", FolderDestinationPanel1.OutputDirectory, FolderDestinationPanel1.OutputFile);
                    break;

                case "SQLite_Output":
                    if (string.IsNullOrEmpty(SQLiteDestinationPanel1.DatabaseName))
                    {
                        MessageBox.Show("SQLite Database path cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }

                    if (string.IsNullOrEmpty(SQLiteDestinationPanel1.TableName))
                    {
                        MessageBox.Show("SQLite destination table name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }
                    dest = new DestinationType("SQLite_Output", SQLiteDestinationPanel1.DatabaseName, SQLiteDestinationPanel1.TableName);
                    break;

                default:
                    MessageBox.Show("Programming bug in GetDestinationParameters: control FilterOutputTabs has an unrecognized tab with tag " + FilterOutputTabs.SelectedTab.Tag, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
            }
            return dest;
        }

        /// <summary>
        ///Get extraction parameters
        /// </summary>
        private ExtractionType GetExtractionParameters()
        {
            return new()
            {
                RType = extractionSettingsPanel1.ResultTypeDescription,
                ResultFilterSetID = extractionSettingsPanel1.ResultFilterSetID,
                KeepAllResults = extractionSettingsPanel1.KeepAllResults,
                MSGFCutoff = extractionSettingsPanel1.MSGFCutoff
            };
        }

        /// <summary>
        /// Can't process empty list of jobs
        /// </summary>
        private bool CheckForJobsToProcess()
        {
            var ok = true;
            if (JobListDisplayCtl.List.Rows.Count == 0)
            {
                MessageBox.Show("There are no jobs to process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ok = false;
            }
            return ok;
        }

        private void ClearMageTempFiles(string directoryPath)
        {
            try
            {
                var targetDirectory = new DirectoryInfo(Path.Combine(directoryPath, FileProcessingBase.MAGE_TEMP_FILES_DIRECTORY));
                if (targetDirectory.Exists)
                {
                    targetDirectory.Delete(true);
                }
                else
                {
                    var fiFileCheck = new FileInfo(directoryPath);
                    if (fiFileCheck.Exists && fiFileCheck.Directory != null)
                    {
                        ClearMageTempFiles(fiFileCheck.Directory.FullName);
                    }
                }
            }
            catch
            {
                // Ignore errors here
            }
        }

        private void ClearTempFiles(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!mOutputDirectoryPaths.Contains(Path.GetTempPath()))
                    mOutputDirectoryPaths.Add(Path.GetTempPath());

                foreach (var directoryPath in mOutputDirectoryPaths)
                {
                    ClearMageTempFiles(directoryPath);
                }
            }
            catch
            {
                // Ignore errors here
            }
        }
    }
}
