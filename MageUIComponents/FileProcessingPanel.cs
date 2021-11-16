using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class FileProcessingPanel : UserControl
    {
        public FileProcessingPanel()
        {
            InitializeComponent();
            FilterParametersCtl.Enabled = false;
        }

        /// <summary>
        /// Event for issuing commands
        /// </summary>
        public event EventHandler<MageCommandEventArgs> OnAction;

        /// <summary>
        /// Delegate for getting information about file to extract column information from
        /// </summary>
        public Func<Dictionary<string, string>> GetSelectedFileInfo;

        // Delegate for getting information about output file/database
        public Func<Dictionary<string, string>> GetSelectedOutputInfo;

        /// <summary>
        /// Stores parameter sets for filters, keyed by filter name.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> mParameters = new();

        /// <summary>
        /// Output column list from seleted column mapping (if any)
        /// </summary>
        private string mOutputColumnList = string.Empty;

        /// <summary>
        /// Return class name for selected filter
        /// </summary>
        public string SelectedFilterClassName => ModuleDiscovery.SelectedFilterClassName(FilterSelectionCtl.Text);

        /// <summary>
        /// Get a merged set of parameters consisting of the canonical parameters
        /// plus any specific parameters for the currently selected filter
        /// (added by filter's parameter panel)
        /// </summary>
        public Dictionary<string, string> GetParameters()
        {
            // If there is a parameter set for the currently selected filter
            // use it as starting point for parameter
            // Otherwise start with blank collection.
            Dictionary<string, string> p;
            if (mParameters.ContainsKey(FilterSelectionCtl.Text))
            {
                p = mParameters[FilterSelectionCtl.Text];
            }
            else
            {
                p = new Dictionary<string, string>();
            }

            // Add the canonical parameters for the processing panel itself
            p["SelectedFilterClassName"] = SelectedFilterClassName;
            p["OutputColumnList"] = mOutputColumnList;
            return p;
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            // mParameters[FilterSelectionCtl.Text] = paramList;
            // FUTURE: set individual controls from items in the list
        }



        /// <summary>
        /// Issue command to process file contents
        /// for selected items in list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessSelectedFilesCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("process_file_contents", "selected"));
        }

        /// <summary>
        /// Issue command to process file contents
        /// for all items in list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessAllFilesCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("process_file_contents", "all"));
        }

        // Support methods

        /// <summary>
        /// See if there is a parameter panel associated with the currently selected filter
        /// and, if there is one, present it to the user and save its returned parameter values
        /// </summary>
        private void GetFilterParams()
        {
            var FilterLabel = FilterSelectionCtl.Text;
            var panelName = ModuleDiscovery.GetParameterPanelForFilter(FilterLabel);
            if (!string.IsNullOrEmpty(panelName))
            {
                // Create an instance of the parameter panel
                var modType = ModuleDiscovery.GetModuleTypeFromClassName(panelName);
                var paramForm = (Form)Activator.CreateInstance(modType);

                // Need reference that lets us access its parameters
                var iPar = (IModuleParameters)paramForm;

                // Initialize its current parameter values
                if (mParameters.ContainsKey(FilterLabel))
                {
                    iPar.SetParameters(mParameters[FilterLabel]);
                }
                // Popup the parameter panel and save its parameter values
                if (paramForm.ShowDialog() == DialogResult.OK)
                {
                    mParameters[FilterLabel] = iPar.GetParameters();
                }
            }
        }

        // (if there is such a panel)
        /// <summary>
        /// Bring up parameter panel associated with currently selected filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterParametersCtl_Click(object sender, EventArgs e)
        {
            GetFilterParams();
        }

        /// <summary>
        /// Enable and disable the button that brings up parameter panel
        /// that is associated with currently selected filter
        /// according to whether or not such a panel exists
        /// </summary>
        private void AdjustFilterParameterAccessButton()
        {
            var panelName = ModuleDiscovery.GetParameterPanelForFilter(FilterSelectionCtl.Text);
            if (!string.IsNullOrEmpty(panelName))
            {
                FilterParametersCtl.Enabled = true;
                var modType = ModuleDiscovery.GetModuleTypeFromClassName(panelName);
                var paramForm = (Form)Activator.CreateInstance(modType);
                var iPar = (IModuleParameters)paramForm;
                mParameters[FilterSelectionCtl.Text] = iPar.GetParameters();
            }
            else
            {
                FilterParametersCtl.Enabled = false;
            }
        }

        // Column mapping methods

        /// <summary>
        /// Bring up column mapping selection list and allow user to choose one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectColMapBtn_Click(object sender, EventArgs e)
        {
            var selectionForm = new ColumnMapSelectionForm { ColumnMapToSelect = ColumnMapSelectionCtl.Text };
            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                ColumnMapSelectionCtl.Text = selectionForm.ColumnMapping;
                mOutputColumnList = selectionForm.OutputColumnList;
            }
        }

        /// <summary>
        /// Bring up form for editing column mappings.
        /// Also handle user's selection of a column mapping to use (if any)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditColumnMappingBtn_Click(object sender, EventArgs e)
        {
            var editingForm = new ColumnMappingForm
            {
                InputFileInfo = GetSelectedFileInfo(),
                OutputInfo = GetSelectedOutputInfo()
            };
            if (editingForm.ShowDialog() == DialogResult.OK)
            {
                ColumnMapSelectionCtl.Text = editingForm.ColumnMapping;
                mOutputColumnList = editingForm.OutputColumnList;
            }
        }

        /// <summary>
        /// Reset column mapping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearColMapBtn_Click(object sender, EventArgs e)
        {
            ColumnMapSelectionCtl.Text = "(automatic)";
            mOutputColumnList = string.Empty;
        }

        /// <summary>
        /// Bring up filter selection dialog and allow user to choose one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFilterBtn_Click(object sender, EventArgs e)
        {
            var selectionForm = new FilterSelectionForm { FilterNameToSelect = FilterSelectionCtl.Text };
            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                FilterSelectionCtl.Text = selectionForm.FilterName;
            }
        }

        /// <summary>
        /// Filter selection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSelectionCtl_TextChanged(object sender, EventArgs e)
        {
            AdjustFilterParameterAccessButton();
        }

        /// <summary>
        /// Reset filter selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFilterBtn_Click(object sender, EventArgs e)
        {
            FilterSelectionCtl.Text = "All Pass";
        }
    }
}
