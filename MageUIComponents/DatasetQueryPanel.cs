using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class DatasetQueryPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        public DatasetQueryPanel()
        {
            InitializeComponent();
        }



        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "Dataset", DatasetCtl.Text },
                { "Instrument", InstrumentCtl.Text },
                { "State", StateCtl.Text },
                { "Dataset Type", TypeCtl.Text }
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
                    case "Instrument":
                        InstrumentCtl.Text = paramDef.Value;
                        break;
                    case "State":
                        StateCtl.Text = paramDef.Value;
                        break;
                    case "Dataset Type":
                        TypeCtl.Text = paramDef.Value;
                        break;
                }
            }
        }



        private void GetDatasetsCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Datasets"));
        }
    }
}
