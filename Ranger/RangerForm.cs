using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;
using RangerLib;

namespace Ranger {
    public partial class RangerForm : Form {

        private readonly List<IModuleParameters> mParamPanels = new List<IModuleParameters>();

        public RangerForm() {
            InitializeComponent();
            InitializeParameterSubpanels();

            try {
                // Set up configuration folder and files
                SavedState.SetupConfigFiles("MageRanger");
            } catch (Exception ex) {
                MessageBox.Show("Error loading settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            try {
                // Restore settings to UI component panels
                SavedState.RestoreSavedPanelParameters(PanelSupport.GetParameterPanelList(this));
            } catch (Exception ex) {
                MessageBox.Show("Error restoring saved settings; will auto-delete SavedState.xml.  Message details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // Delete the SavedState.xml file
                try {
                    System.IO.File.Delete(SavedState.FilePath);
                } catch (Exception ex2) {
                    // Ignore errors here
                    Console.WriteLine("Error deleting SavedState file: " + ex2.Message);
                }
            }

            StatusCtl.Text = "";
        }

        private void InitializeParameterSubpanels() {
            // Make list of parameter subpanels for convenient access
            mParamPanels.Add(incrementalParameterSubPanel1);
            mParamPanels.Add(incrementalParameterSubPanel2);
            mParamPanels.Add(incrementalParameterSubPanel3);
            mParamPanels.Add(incrementalParameterSubPanel4);
            mParamPanels.Add(incrementalParameterSubPanel5);
            mParamPanels.Add(incrementalParameterSubPanel6);
            mParamPanels.Add(incrementalParameterLitSubPanel1);
            mParamPanels.Add(incrementalParameterLitSubPanel2);

            // Preset parameter subpanels
            incrementalParameterSubPanel1.SetValues("ChargeState", "1", "5", "1", true);
            incrementalParameterSubPanel2.SetValues("CleavageType", "0", "2", "1", true);
            incrementalParameterSubPanel3.SetValues("Xcorr", "0", "5", "0.1", false);
            incrementalParameterSubPanel4.SetValues("DelCN2", "0", "0.5", "0.01", false);
            incrementalParameterSubPanel5.SetValues("PPM", "0", "6", "0.5", false);
            incrementalParameterSubPanel6.SetValues("", "", "", "", false);

            incrementalParameterLitSubPanel1.SetParameters(new Dictionary<string, string>() {
                { "ParamName", "MSGF_Cutoff" },
                { "Operator", "=" },
                { "Active", "Off" },
                { "IncrementList", "1E-9, 5E-9, 1E-10, 5E-10"}
            });
            incrementalParameterLitSubPanel2.SetParameters(new Dictionary<string, string>() {
                { "ParamName", "" },
                { "Operator", "=" },
                { "Active", "Off" },
                { "IncrementList", ""}
            });
        }

        private void SaveBtn_Click(object sender, EventArgs e) {
            StatusCtl.Text = "Processing...";
            var ran = BuildAndRunPipeline();
            if (ran) {
                SavedState.SaveParameters(PanelSupport.GetParameterPanelList(this));
            } else {
                StatusCtl.Text = "Canceled";
            }
        }

        private bool BuildAndRunPipeline() {
            var ran = false;
            // Make new pipeline to generate parameter table
            var ptg = new ParamTableGenerator();

            // Populate pipeline with specs for each parameter to be generated
            foreach (var iP in mParamPanels) {
                var p = iP.GetParameters();
                if (p["Active"] == "On") {
                    ptg.AddParamColumn(p);
                }
            }

            // Setup pipeline output
            switch (tabControl1.SelectedTab.Tag.ToString()) {
                case "SaveToFile":
                    // Create module to write to file
                    ptg.FilePath = simpleFilePanel1.FilePath;
                    break;
                case "SaveToSQLite":
                    ptg.DBPath = simpleSQLitePanel1.FilePath;
                    ptg.TableName = simpleSQLitePanel1.TableName;
                    break;
            }

            // Inform user of how many rows will be generated and
            // allow for confirmation or cancellation
            var prompt = string.Format("This will generate {0} rows", ptg.GeneratedParameterCount);
            var r = MessageBox.Show(prompt, "Confirm save", MessageBoxButtons.OKCancel);
            if (r == DialogResult.OK) {
                // Connect pipeline and run it
                ran = true;
                var pipeline = ptg.GetPipeline();
                pipeline.OnStatusMessageUpdated += HandleStatusMessageUpdated;
                pipeline.OnRunCompleted += HandlePipelineCompletion;
                pipeline.Run();
            }
            return ran;
        }



        #region Functions for handling status updates

        private delegate void MessageHandler(string message);
        private delegate void CompletionStateUpdated(object status);

        /// <summary>
        /// Handle the status update messages from the currently running pipeline
        /// </summary>
        /// <param name="sender">(ignored))</param>
        /// <param name="args">(ignored)</param>
        private void HandleStatusMessageUpdated(object sender, MageStatusEventArgs args) {
            // The current pipleline will call this function from its own thread
            // We need to do the cross-thread thing to update the GUI
            MessageHandler ncb = SetStatusMessage;
            Invoke(ncb, args.Message);
        }

        /// <summary>
        /// Handle the status completion message from the currently running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args) {
            // Pipeline didn't blow up, make nice reassuring message
            if (string.IsNullOrEmpty(args.Message)) {
                args.Message = "Process completed normally";
            }

            // The current pipleline will call this function from its own thread
            // We need to do the cross-thread thing to update the GUI
            MessageHandler ncb = SetStatusMessage;
            Invoke(ncb, args.Message);
        }

        // This is targeted by the cross-thread invoke from HandleStatusMessageUpdated
        // and update the message status display
        private void SetStatusMessage(string Message) {
            StatusCtl.Text = Message;
        }

        #endregion
    }
}
