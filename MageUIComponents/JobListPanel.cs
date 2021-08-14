using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class JobListPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        public JobListPanel()
        {
            InitializeComponent();
        }



        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "Dataset", DatasetCtl.Text },
                { "Tool", ToolCtl.Text },
                { "Settings_File", SettingsFileCtl.Text },
                { "Parameter_File", ParameterFileCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "Dataset":
                        DatasetCtl.Text = paramDef.Value;
                        break;
                    case "Tool":
                        ToolCtl.Text = paramDef.Value;
                        break;
                    case "Settings_File":
                        SettingsFileCtl.Text = paramDef.Value;
                        break;
                    case "Parameter_File":
                        ParameterFileCtl.Text = paramDef.Value;
                        break;
                }
            }
        }



        private void GetJobsCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Jobs"));
        }
    }
}
