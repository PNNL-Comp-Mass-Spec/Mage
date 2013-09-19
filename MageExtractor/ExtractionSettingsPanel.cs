using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using MageExtExtractionFilters;

namespace MageExtractor {

    public partial class ExtractionSettingsPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        private Dictionary<string, ResultType> mResultTypes = ResultType.TypeList;

        #endregion

        #region Properties

        public ResultType ResultTypeDescription {
            get {
                return mResultTypes[ResultTypeName]; 
            }
        }

        public string ResultTypeName {
            get { return ResultTypeNameCtl.Text; }
            set { ResultTypeNameCtl.Text = value; }
        }

        public string KeepAllResults {
            get { return (KeepResultsCtl.Checked) ? "Yes" : "No"; }
            set { KeepResultsCtl.Checked = (value == "Yes") ? true : false; }
        }

        public string ResultFilterSetID {
            get { return FilterSetIDCtl.Text; }
            set { FilterSetIDCtl.Text = value; }
        }

        public string MSGFCutoff {
            get { return MSGFCutoffCtl.Text; }
            set { MSGFCutoffCtl.Text = value; }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "ResultType",   ResultTypeName},
                { "KeepAllResults",   KeepAllResults},
                { "ResultFilter",   ResultFilterSetID},
                { "MSGFCutoff",   MSGFCutoff},
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "ResultType":
                        ResultTypeName = paramDef.Value;
                        break;
                    case "KeepAllResults":
                        KeepAllResults = paramDef.Value;
                        break;
                    case "ResultFilter":
                        ResultFilterSetID = paramDef.Value;
                        break;
                    case "MSGFCutoff":
                        MSGFCutoff = paramDef.Value;
                        break;
                }
            }
        }

        #endregion


        public ExtractionSettingsPanel() {
            InitializeComponent();

			int indexToSelect = 0;
            foreach (string resultType in mResultTypes.Keys) {
                ResultTypeNameCtl.Items.Add(resultType);

				if (resultType == ResultType.MSGFDB_SYN_ALL_PROTEINS)
					indexToSelect = ResultTypeNameCtl.Items.Count - 1;
            }

            if (string.IsNullOrEmpty(ResultTypeNameCtl.Text)) {
				ResultTypeNameCtl.SelectedIndex = indexToSelect;				
            }

            MSGFCutoffCtl.Items.AddRange(new string[] { "All Pass", "1E-8", "1E-9", "5E-9", "1E-10", "5E-10", "1E-11" });
            MSGFCutoffCtl.Text = "1E-10";
        }

        private void ExtractFromSelectedBtn_Click(object sender, EventArgs e) {
            if (OnAction != null) {
				try
				{
					MageCommandEventArgs command = new MageCommandEventArgs();
					command.Mode = "selected";
					command.Action = "extract_results";
					OnAction(this, command);
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show("Error extracting results: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
            }

        }

        private void ExtractFromAllBtn_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                MageCommandEventArgs command = new MageCommandEventArgs();
                command.Mode = "all";
                command.Action = "extract_results";
                OnAction(this, command);
            }

        }

        private void SelectFilterBtn_Click(object sender, EventArgs e) {
            ResultsFilterSelector selectionForm = new ResultsFilterSelector();
			selectionForm.FilterSetIDToSelect = ResultFilterSetID;			
            if (selectionForm.ShowDialog() == DialogResult.OK) {
                Dictionary<string, string> parms = selectionForm.GetParameters();
                ResultFilterSetID = parms["FilterSetID"];
            }

        }

        private void ClearFilterBtn_Click(object sender, EventArgs e) {
            ResultFilterSetID = "All Pass";
        }

    }
}
