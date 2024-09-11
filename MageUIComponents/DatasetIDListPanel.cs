using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class DatasetIDListPanel : UserControl, IModuleParameters
    {
        // Ignore Spelling: Ctrl, Mage

        public event EventHandler<MageCommandEventArgs> OnAction;

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "Dataset_ID", DatasetListCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "Dataset_ID":
                        DatasetListCtl.Text = paramDef.Value;
                        break;
                }
            }
        }

        public DatasetIDListPanel()
        {
            InitializeComponent();
        }

        private void GetDatasetsCtl_Click(object sender, EventArgs e)
        {
            if (!ValidateDatasetIDs())
                return;

            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Datasets"));
        }

        private void DatasetListCtl_Leave(object sender, EventArgs e)
        {
            DatasetListCtl.Text = PanelSupport.CleanUpDelimitedList(DatasetListCtl.Text);
        }

        private void DatasetListCtl_KeyDown(object sender, EventArgs e)
        {
            var args = (KeyEventArgs)e;

            if (args.Control && args.KeyCode == Keys.A)
            {
                // Ctrl+A pressed
                DatasetListCtl.SelectAll();
            }
        }

        private bool ValidateDatasetIDs()
        {
            if (string.IsNullOrWhiteSpace(DatasetListCtl.Text))
            {
                MessageBox.Show("Please enter one or more dataset IDs", "Missing Dataset ID(s)",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            foreach (var datasetID in DatasetListCtl.Text.Split(','))
            {
                if (string.IsNullOrWhiteSpace(datasetID))
                    continue;

                if (int.TryParse(datasetID, out _))
                    continue;

                MessageBox.Show(string.Format("Invalid dataset ID '{0}'; must be an integer", datasetID.Trim()), "Invalid Dataset ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            return true;
        }
    }
}
