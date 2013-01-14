using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using System.IO;
using log4net;
using MageDisplayLib;
using MageExtExtractionFilters;
using MageUIComponents;
using System.Collections.ObjectModel;

namespace MageExtractor {

	public partial class ExtractorForm : Form {

		#region Constants

		protected const string TAG_JOB_IDs = "Job_ID_List";
		protected const string TAG_JOB_IDs_FROM_DATASETS = "Jobs_From_Dataset_List";
		protected const string TAG_DATA_PACKAGE_ID = "Data_Package";

		#endregion

		#region Member Variables

		private ILog traceLog;

		/// <summary>
		/// Pipeline queue for running the multiple pipelines that make up the workflows for this module
		/// </summary>
		private PipelineQueue mPipelineQueue = new PipelineQueue();

		/// <summary>
		/// The parameters for the slated extraction
		/// </summary>
		private ExtractionType mExtractionParms = null;

		/// <summary>
		/// Where extracted results will be delivered
		/// </summary>
		private DestinationType mDestination = null;

		private string mFinalPipelineName = string.Empty;

		#endregion

		#region Initialization

		public ExtractorForm() {
			InitializeComponent();

			bool isBetaVersion = false;
			SetFormTitle("2013-01-14", isBetaVersion);

			SetTags();

			SetAboutText();

			try {
				// set up configuration folder and files
				SavedState.SetupConfigFiles("MageExtractor");
			} catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			try {
				// Set log4net path and kick the logger into action
				string LogFileName = Path.Combine(SavedState.DataDirectory, "log.txt");
				log4net.GlobalContext.Properties["LogName"] = LogFileName;
				traceLog = LogManager.GetLogger("TraceLog");
				traceLog.Info("Starting");
			} catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show("Error instantiating trace log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			ConnectPanelsToCommandHandlers();

			// Connect click events
			this.lblAboutLink.LinkClicked += lblAboutLink_LinkClicked;

			// connect the pipeline queue to message handlers
			ConnectPipelineQueueToStatusDisplay(mPipelineQueue);

			DisableCancelButton();

			SetupFlexQueryPanels(); // must be done before restoring saved state

			try {
				SavedState.RestoreSavedPanelParameters(PanelSupport.GetParameterPanelList(this));
			} catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show("Error restoring saved settings; will auto-delete SavedState.xml.  Message details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				// Delete the SavedState.xml file
				try {
					System.IO.File.Delete(SavedState.FilePath);
				} catch (Exception ex2) {
					// Ignore errors here
					Console.WriteLine("Error deleting SavedState file: " + ex2.Message);
				}				
			}
		}

		private void SetAboutText() {
			txtAbout1.Text = "Mage Extractor can extract MS/MS search results from SEQUEST, X!Tandem, Inspect, or MSFBDB analysis jobs and combine the results into a single tab-delimited text file or a single SQLite database.";
			txtAbout2.Text = "Written by Gary Kiebel and Matthew Monroe in 2011 for the Department of Energy (PNNL, Richland, WA)";
			lblAboutLink.Text = "http://prismwiki.pnl.gov/wiki/Mage_Extractor";
		}

		private void SetFormTitle(string programDate, bool beta) {
			System.Version objVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			string version = string.Format("{0}.{1}.{2}", objVersion.Major, objVersion.Minor, objVersion.Build);

			txtVersion.Text = "Version " + version;

			if (!string.IsNullOrEmpty(programDate)) {
				if (beta)
					txtVersion.Text += string.Format(" ( {0}, beta)", programDate);
				else
					txtVersion.Text += string.Format(" ( {0})", programDate);
			}

			if (beta)
				this.Text += " (beta)";

		}

		private void SetTags() {
			this.JobListTabPage.Tag = TAG_JOB_IDs;
			this.JobsFromDatasetIDTabPage.Tag = TAG_JOB_IDs_FROM_DATASETS;
			this.DataPackageTabPage.Tag = TAG_DATA_PACKAGE_ID;
		}

		/// <summary>
		/// Initialize parameters for flex query panels
		/// </summary>
		private void SetupFlexQueryPanels() {
			JobFlexQueryPanel.QueryName = "Job_Flex_Query";
			JobFlexQueryPanel.SetColumnPickList(new string[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign", "Organism", "Organism DB", "Protein Collection List", "Protein Options", "Comment", "Results Folder", "Folder", "Dataset_Created", "Job_Finish" });
			JobFlexQueryPanel.SetComparisionPickList(new string[] { "ContainsText", "DoesNotContainText", "StartsWithText", "MatchesText", "MatchesTextOrBlank", "Equals", "NotEqual", "GreaterThan", "GreaterThanOrEqualTo", "LessThan", "LessThanOrEqualTo", "MostRecentWeeks", "LaterThan", "EarlierThan", "InList" });
		}

		#endregion

		#region Panel Commands

		private void ConnectPanelsToCommandHandlers() {
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
		private void DisableCancelButton() {
			EnableCancel(false);
		}

		/// <summary>
		/// Enable/Disable the cancel button
		/// </summary>
		/// <param name="enabled"></param>
		private void EnableCancel(bool enabled) {
			statusPanel1.ShowCancel = enabled;
			statusPanel1.EnableCancel = enabled;
		}

		private void HandleJobFlexQueryCommand(object sender, MageCommandEventArgs command) {
			if (GetJobList(command.Mode)) {
				SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
			}
		}

		private void HandleJobQueryCommand(object sender, MageCommandEventArgs command) {
			string queryName = EntityListSourceTabs.SelectedTab.Tag.ToString();
			string queryTemplate = ModuleDiscovery.GetQueryXMLDef(queryName);

			IModuleParameters paramSource = sender as IModuleParameters;
			Dictionary<string, string> queryParameters = paramSource.GetParameters();

			if (!ValidQueryParameters(queryName, queryParameters)) {
				return;
			}

			GetJobList(queryTemplate, queryParameters);
			SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
		}

		private void HandleExtractionCommand(object sender, MageCommandEventArgs command) {	
			try
			{
				mExtractionParms = GetExtractionParameters();
				mDestination = GetDestinationParameters();

				if (!CheckForJobsToProcess()) return;

				DisplaySourceMode mode = (command.Mode == "all") ? DisplaySourceMode.All : DisplaySourceMode.Selected;
				string msg = ExtractionPipelines.CheckJobResultType(new GVPipelineSource(JobListDisplayCtl, mode), "Tool", mExtractionParms);
				if (!string.IsNullOrEmpty(msg))
				{
					MessageBox.Show(msg, "Invalid Seletion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				if (!DestinationType.VerifyDestinationOptionsWithUser(mDestination)) return;

				// Validate mExtractionParms.MSGFCutoff
				if (mExtractionParms.MSGFCutoff.ToLower() != "All Pass".ToLower())
				{
					double result = -1;
					if (!double.TryParse(mExtractionParms.MSGFCutoff, out result))
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
						msg = "Invalid value specified for MSGF Cutoff: '" + mExtractionParms.MSGFCutoff + "'; must either be 'All Pass' or a number between 0 and 1";
						MessageBox.Show(msg, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}

				try
				{
					BaseModule jobList = new GVPipelineSource(JobListDisplayCtl, mode);
					ExtractFileContents(jobList);
					SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show("Error extracting results: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}

			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Error initializing extraction: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

	
		}

		private void HandleCancelCommand(object sender, MageCommandEventArgs command) {
			mPipelineQueue.Cancel();
		}

		private bool ValidQueryParameters(string queryName, Dictionary<string, string> queryParameters) {
			string msg = string.Empty;
			bool bFilterDefined = false;

			foreach (KeyValuePair<string, string> entry in queryParameters) {
				if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Value)) {
					if (entry.Value.Trim().Length > 0) {
						bFilterDefined = true;
						break;
					}
				}
			}

			if (!bFilterDefined) {

				switch (queryName) {
					case TAG_JOB_IDs:
						msg = "Job ID list cannot be empty";
						break;
					case TAG_JOB_IDs_FROM_DATASETS:
						msg = "Dataset ID list cannot be empty";
						break;
					case TAG_DATA_PACKAGE_ID:
						msg = "Please enter a data package ID";
						break;
					default:
						msg = "You must define one or more search criteria before searching for jobs";
						break;
				}
			}

			if (string.IsNullOrEmpty(msg) && (queryName == TAG_JOB_IDs || queryName == TAG_JOB_IDs_FROM_DATASETS)) {
				char[] cSepChars = new char[] { ',', '\t' };
				string sWarning;

				if (queryName == TAG_JOB_IDs)
					sWarning = "Job number '";
				else
					sWarning = "Use dataset IDs, not dataset names: '";

				// Validate that the job numbers or dataset IDs are all numeric
				string sValue;
				int iValue;
				foreach (KeyValuePair<string, string> entry in queryParameters) {
					sValue = entry.Value.Replace(Environment.NewLine, ",");

					string[] values = sValue.Split(cSepChars);

					foreach (string datasetID in values) {
						if (!int.TryParse(datasetID, out iValue)) {
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
			else {
				System.Windows.Forms.MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

		}

		#endregion

		#region Pipeline and Queue Update Message Handlers

		private delegate void VoidFnDelegate();

		private void HandlePipelineUpdate(object sender, MageStatusEventArgs args) {
			statusPanel1.HandleStatusMessageUpdated(this, new MageStatusEventArgs(args.Message));
			Console.WriteLine(args.Message);
		}

		private void HandlePipelineWarning(object sender, MageStatusEventArgs args) {
			statusPanel1.HandleWarningMessageUpdated(this, new MageStatusEventArgs(args.Message));
			Console.WriteLine("Warning: " + args.Message);
		}

		private void HandlePipelineCompletion(object sender, MageStatusEventArgs args) {
			statusPanel1.HandleCompletionMessageUpdate(this, new MageStatusEventArgs(args.Message));
			Console.WriteLine(args.Message);

			if (sender is Mage.ProcessingPipeline) {
				Mage.ProcessingPipeline pipeline = (Mage.ProcessingPipeline)sender;
				if (pipeline.PipelineName == mFinalPipelineName) {
					// Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
					VoidFnDelegate dc = DisableCancelButton;
					Invoke(dc);
				}
			}

			if (args.Message.StartsWith(Mage.MSSQLReader.SQL_COMMAND_ERROR))
				System.Windows.Forms.MessageBox.Show(args.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			//// If instructed to load results into Access, we could do that now
			//if (true)
			//    LoadDataIntoAccess(mPipelineQueue, mDestination);

		}

		private void HandlePipelineQueueUpdate(object sender, MageStatusEventArgs args) {
		}

		private void HandlePipelineQueueCompletion(object sender, MageStatusEventArgs args) {
		}

		private void lblAboutLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) {
			try {
				LaunchMageExtractorHelpPage();
			} catch (Exception ex) {
				MessageBox.Show("Unable to open link that was clicked: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void LaunchMageExtractorHelpPage() {
			// Change the color of the link text by setting LinkVisited 
			// to true.
			lblAboutLink.LinkVisited = true;

			//Call the Process.Start method to open the default browser 
			//with a URL:
			System.Diagnostics.Process.Start(lblAboutLink.Text);
		}

		#endregion

		#region Pipelines

		private void ConnectPipelineToStatusDisplay(ProcessingPipeline pipeline) {
			// Clear any warnings
			statusPanel1.ClearWarnings();

			pipeline.OnStatusMessageUpdated += HandlePipelineUpdate;
			pipeline.OnWarningMessageUpdated += HandlePipelineWarning;
			pipeline.OnRunCompleted += HandlePipelineCompletion;
		}

		private void ConnectPipelineQueueToStatusDisplay(PipelineQueue pipelineQueue) {
			// Clear any warnings
			statusPanel1.ClearWarnings();

			pipelineQueue.OnRunCompleted += HandlePipelineQueueCompletion;
			pipelineQueue.OnPipelineStarted += HandlePipelineQueueUpdate;

		}

		/// <summary>
		/// Build and run Mage pipeline queue to extract contents of results files
		/// for jobs given in jobList according to parameters defined in mExtractionParms 
		/// and deliver output according to mDestination.  Also create metadata file for jobList.
		/// </summary>
		/// <param name="jobList"></param>
		private void ExtractFileContents(BaseModule jobList) {
			mPipelineQueue = ExtractionPipelines.MakePipelineQueueToExtractFromJobList(jobList, mExtractionParms, mDestination);
			foreach (ProcessingPipeline p in mPipelineQueue.Pipelines.ToArray()) {
				ConnectPipelineToStatusDisplay(p);
				mFinalPipelineName = p.PipelineName;
			}
			ConnectPipelineQueueToStatusDisplay(mPipelineQueue);
			EnableCancel(true);
			mPipelineQueue.Run();
		}

		/// <summary>
		/// build and run Mage pipeline to populate main list display with jobs
		/// </summary>
		/// <param name="queryTemplate">XML query template (typically from QueryDefinitions.xml) </param>
		/// <param name="queryItems">List of predicate items in delimited string format (rel|col|cmp|val)</param>
		private bool GetJobList(string queryName) {
			bool result = false;
			string queryDefXML = ModuleDiscovery.GetQueryXMLDef(queryName);
			SQLBuilder builder = JobFlexQueryPanel.GetSQLBuilder(queryDefXML);
			if (builder.HasPredicate) {
				result = true;
				MSSQLReader reader = new MSSQLReader(builder);
				ProcessingPipeline pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, JobListDisplayCtl);
				ConnectPipelineToStatusDisplay(pipeline);
				JobListDisplayCtl.Clear();

				EnableCancel(true);
				mFinalPipelineName = pipeline.PipelineName;

				mPipelineQueue.Run(pipeline);
			} else {
				System.Windows.Forms.MessageBox.Show("You must define one or more search criteria before searching for jobs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			return result;
		}

		/// <summary>
		/// build and run Mage pipeline to populate main list display with jobs
		/// </summary>
		/// <param name="queryTemplate">XML query template (typically from QueryDefinitions.xml) </param>
		/// <param name="queryParameters">Key/Value pairs (column/value)</param>
		private void GetJobList(string queryTemplate, Dictionary<string, string> queryParameters) {
			MSSQLReader reader = new MSSQLReader(queryTemplate, queryParameters);
			JobListDisplayCtl.Clear();
			ProcessingPipeline pipeline = ProcessingPipeline.Assemble("Get Jobs", reader, JobListDisplayCtl);
			ConnectPipelineToStatusDisplay(pipeline);

			EnableCancel(true);
			mFinalPipelineName = pipeline.PipelineName;

			mPipelineQueue.Run(pipeline);
		}

		#endregion

		#region Setup Extraction Parameter Objects From UI

		/// <summary>
		/// Set the given content extraction module's destination parameters
		/// from the user's UI choices
		/// </summary>
		/// <param name="extractor"></param>
		private DestinationType GetDestinationParameters() {
			DestinationType dest = null;
			switch (FilterOutputTabs.SelectedTab.Tag.ToString()) {
				case "File_Output":
					dest = new DestinationType("File_Output", FolderDestinationPanel1.OutputFolder, FolderDestinationPanel1.OutputFile);
					break;
				case "SQLite_Output":
					dest = new DestinationType("SQLite_Output", SQLiteDestinationPanel1.DatabaseName, SQLiteDestinationPanel1.TableName);
					break;
			}
			return dest;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private ExtractionType GetExtractionParameters() {
			ExtractionType et = new ExtractionType();
			et.RType = extractionSettingsPanel1.ResultTypeDescription;
			et.ResultFilterSetID = extractionSettingsPanel1.ResultFilterSetID;
			et.KeepAllResults = extractionSettingsPanel1.KeepAllResults;
			et.MSGFCutoff = extractionSettingsPanel1.MSGFCutoff;
			return et;
		}

		//private bool LoadDataIntoAccess(PipelineQueue oPipelineQueue, DestinationType oDestination) {

		//    try 
		//    {  

		//        using (System.Data.OleDb.OleDbConnection conn =
		//            new System.Data.OleDb.OleDbConnection(@"provider=microsoft.jet.oledb.4.0;data source=F:\temp\Database1.mdb"))  
		//        {  
		//            conn.Open();  
		//            using (System.Data.OleDb.OleDbCommand cmd = conn.CreateCommand())  
		//            {  
		//                try 
		//                {  
		//                    cmd.CommandText = "drop table MyLocalTable";  
		//                    cmd.ExecuteNonQuery();  
		//                }  
		//                catch (Exception)   
		//                {   
		//                    // Don't care if target table exists, going to copy into it.  
		//                }

		//                cmd.CommandText = @"SELECT * INTO MyLocalTable FROM [Text;FMT=Delimited(,);HDR=YES;IMEX=2;DATABASE=F:\Temp].[QC_Shew_XTandem.txt]";  
		//                cmd.CommandType = CommandType.Text;  
		//                cmd.ExecuteNonQuery();  
		//            }  
		//        }  
		//    }  
		//    catch (Exception ex)  
		//    {  
		//        MessageBox.Show(ex.Message);  
		//    }


		//    return true;
		//}

		#endregion

		#region Misc

		/// <summary>
		/// Can't process empty list of jobs
		/// </summary>
		/// <returns></returns>
		private bool CheckForJobsToProcess() {
			bool ok = true;
			if (JobListDisplayCtl.List.Rows.Count == 0) {
				MessageBox.Show("There are no jobs to process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ok = false;
			}
			return ok;
		}


		#endregion

	}
}
