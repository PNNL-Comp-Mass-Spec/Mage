using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageExtExtractionFilters;

namespace MageExtractor
{
    public partial class ExtractionSettingsPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        private readonly Dictionary<string, ResultType> mResultTypes = ResultType.TypeList;

        public ResultType ResultTypeDescription => mResultTypes[ResultTypeName];

        public string ResultTypeName
        {
            get => ResultTypeNameCtl.Text;
            set => ResultTypeNameCtl.Text = value;
        }

        public string KeepAllResults
        {
            get => KeepResultsCtl.Checked ? "Yes" : "No";
            set => KeepResultsCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        public string ResultFilterSetID
        {
            get => FilterSetIDCtl.Text;
            set => FilterSetIDCtl.Text = value;
        }

        public string MSGFCutoff
        {
            get => MSGFCutoffCtl.Text;
            set => MSGFCutoffCtl.Text = value;
        }

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "ResultType",   ResultTypeName},
                { "KeepAllResults",   KeepAllResults},
                { "ResultFilter",   ResultFilterSetID},
                { "MSGFCutoff",   MSGFCutoff}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                try
                {
                    switch (paramDef.Key)
                    {
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
                catch
                {
                    // Ignore errors setting this parameter
                }
            }
        }

        public ExtractionSettingsPanel()
        {
            InitializeComponent();

            var indexToSelect = 0;
            foreach (var resultType in mResultTypes.Keys)
            {
                ResultTypeNameCtl.Items.Add(resultType);

                if (resultType == ResultType.MSGFDB_SYN_ALL_PROTEINS)
                    indexToSelect = ResultTypeNameCtl.Items.Count - 1;
            }

            if (string.IsNullOrEmpty(ResultTypeNameCtl.Text))
            {
                ResultTypeNameCtl.SelectedIndex = indexToSelect;
            }

            MSGFCutoffCtl.Items.AddRange(new object[] {ExtractionFilter.ALL_PASS_CUTOFF, "1E-8", "1E-9", "5E-9", "1E-10", "5E-10", "1E-11" });
            MSGFCutoffCtl.Text = "1E-10";
        }

        private void ExtractFromSelectedBtn_Click(object sender, EventArgs e)
        {
            if (OnAction != null)
            {
                try
                {
                    var command = new MageCommandEventArgs
                    {
                        Mode = "selected",
                        Action = "extract_results"
                    };
                    OnAction(this, command);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error extracting results: " + ex.Message + "; " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void ExtractFromAllBtn_Click(object sender, EventArgs e)
        {
            if (OnAction != null)
            {
                var command = new MageCommandEventArgs
                {
                    Mode = "all",
                    Action = "extract_results"
                };
                OnAction(this, command);
            }
        }

        private void SelectFilterBtn_Click(object sender, EventArgs e)
        {
            var selectionForm = new ResultsFilterSelector
            {
                FilterSetIDToSelect = ResultFilterSetID
            };

            // Show, then quickly hide the form
            // Required because HandlePipelineCompletion in ResultsFilterSelector will invoke UpdateSelectedFilterSetID
            // and that raises an exception saying "Invoke or BeginInvoke cannot be called on a control until the window handle has been created"
            // if we don't first .Show() the form

            selectionForm.Show();
            selectionForm.Hide();
            selectionForm.InitializeFilterSetList();

            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                var parms = selectionForm.GetParameters();
                ResultFilterSetID = parms["FilterSetID"];
            }
        }

        private void ClearFilterBtn_Click(object sender, EventArgs e)
        {
            ResultFilterSetID = ExtractionFilter.ALL_PASS_CUTOFF;
        }

        private void ResultTypeNameCtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            fraMSGFCutoff.Visible = !ResultTypeNameCtl.Text.StartsWith("MSPathFinder");
        }
    }
}
