﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class JobIDListPanel : UserControl, IModuleParameters
    {
        // Ignore Spelling: Ctrl

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
    }
}
