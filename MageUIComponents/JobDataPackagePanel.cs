using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class JobDataPackagePanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        protected bool mShowGetDatasets = true;
        protected bool mShowGetJobs = true;

        public bool ShowGetDatasets
        {
            get => mShowGetDatasets;
            set
            {
                mShowGetDatasets = value;
                GetDatasetsCtl.Visible = value;

                UpdateButtonPosition();
            }
        }

        public bool ShowGetJobs
        {
            get => mShowGetJobs;
            set
            {
                mShowGetJobs = value;
                GetJobsCtl.Visible = value;

                UpdateButtonPosition();
            }
        }

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "Data_Package_ID", DataPackageIDCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "Data_Package_ID":
                        DataPackageIDCtl.Text = paramDef.Value;
                        break;
                }
            }
        }

        public JobDataPackagePanel()
        {
            InitializeComponent();
        }

        private void GetJobsCtl_Click(object sender, EventArgs e)
        {
            if (!ValidateDataPackageID())
                return;

            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Jobs"));
        }

        private void GetDatasetsCtl_Click(object sender, EventArgs e)
        {
            if (!ValidateDataPackageID())
                return;

            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Datasets"));
        }

        private void UpdateButtonPosition()
        {
            var pLocation = GetJobsCtl.Location;
            if (ShowGetJobs)
            {
                // Position the Get Datasets button 29 points above the Get Jobs button
                pLocation.Y -= 29;
            }
            else
            {
                // Position the Get Datasets at the same spot as the Get Jobs button
                pLocation.Y += 0;
            }
            GetDatasetsCtl.Location = pLocation;
        }

        private bool ValidateDataPackageID()
        {
            var dataPackageID = DataPackageIDCtl.Text;

            if (string.IsNullOrWhiteSpace(dataPackageID))
            {
                MessageBox.Show("Please enter a data package ID", "Missing Data Package ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            if (int.TryParse(dataPackageID, out _))
                return true;

            MessageBox.Show(string.Format("Invalid data package ID '{0}'; must be an integer", dataPackageID.Trim()), "Invalid Data Package ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

            return false;
        }
    }
}
