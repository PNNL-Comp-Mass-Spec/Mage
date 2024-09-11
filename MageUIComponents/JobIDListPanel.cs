using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class JobIDListPanel : UserControl, IModuleParameters
    {
        // Ignore Spelling: Ctrl

        public const string LIST_NAME_DATASET_ID = "Dataset_ID";

        public const string LIST_NAME_JOB = "Job";

        public event EventHandler<MageCommandEventArgs> OnAction;

        public string ListName { get; set; }

        public string Legend
        {
            get => LegendCtl.Text;
            set => LegendCtl.Text = value;
        }

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { ListName, JobListCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var paramDef in paramList)
            {
                if (paramDef.Key == ListName)
                {
                    JobListCtl.Text = paramDef.Value;
                }
            }
        }

        public JobIDListPanel()
        {
            InitializeComponent();
            ListName = "Job";
        }

        private void GetJobsCtl_Click(object sender, EventArgs e)
        {
            if (!ValidateJobNumbers())
                return;

            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Jobs"));
        }

        private void JobListCtl_Leave(object sender, EventArgs e)
        {
            JobListCtl.Text = PanelSupport.CleanUpDelimitedList(JobListCtl.Text);
        }

        private void JobListCtl_KeyDown(object sender, EventArgs e)
        {
            var args = (KeyEventArgs)e;

            if (args.Control && args.KeyCode == Keys.A)
            {
                // Ctrl+A pressed
                JobListCtl.SelectAll();
            }
        }

        private bool ValidateJobNumbers()
        {
            string itemDescription;
            string itemDescriptionCapitalized;

            if (ListName.Equals(LIST_NAME_DATASET_ID))
            {
                itemDescription = "dataset ID";
                itemDescriptionCapitalized = "Dataset ID";
            }
            else
            {
                itemDescription = "job number";
                itemDescriptionCapitalized = "Job Number";
            }

            if (string.IsNullOrWhiteSpace(JobListCtl.Text))
            {
                MessageBox.Show(string.Format("Please enter one or more {0}s", itemDescription), string.Format("Missing {0}(s)", itemDescriptionCapitalized),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            foreach (var jobNumber in JobListCtl.Text.Split(','))
            {
                if (string.IsNullOrWhiteSpace(jobNumber))
                    continue;

                if (int.TryParse(jobNumber, out _))
                    continue;

                MessageBox.Show(string.Format("Invalid {0} '{1}'; must be an integer", itemDescription, jobNumber.Trim()), string.Format("Invalid {0}", itemDescriptionCapitalized),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            return true;
        }
    }
}
