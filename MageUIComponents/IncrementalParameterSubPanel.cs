using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents {

    public partial class IncrementalParameterSubPanel : UserControl, IModuleParameters {

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "ParamName", ParamNameCtl.Text },
                { "Lower", ParamLowerCtl.Text },
                { "Upper", ParamUpperCtl.Text },
                { "Increment", ParamIncrementCtl.Text },
                { "Operator", OperationCtl.Text },
                { "Active", (ActiveCtl.Checked)?"On":"Off" }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (var paramDef in paramList) {
                switch (paramDef.Key) {
                    case "ParamName":
                        ParamNameCtl.Text = paramDef.Value;
                        break;
                    case "Lower":
                        ParamLowerCtl.Text = paramDef.Value;
                        break;
                    case "Upper":
                        ParamUpperCtl.Text = paramDef.Value;
                        break;
                    case "Increment":
                        ParamIncrementCtl.Text = paramDef.Value;
                        break;
                    case "Operator":
                        OperationCtl.Text = paramDef.Value;
                        break;
                    case "Active":
                        ActiveCtl.Checked = (paramDef.Value == "On");
                        break;
                }
            }
        }

        #endregion


        public bool Active
        {
            get => ActiveCtl.Checked;
            set => ActiveCtl.Checked = value;
        }
        public string ParamName
        {
            get => ParamNameCtl.Text;
            set => ParamNameCtl.Text = value;
        }
        public string Lower
        {
            get => ParamLowerCtl.Text;
            set => ParamLowerCtl.Text = value;
        }
        public string Upper
        {
            get => ParamUpperCtl.Text;
            set => ParamUpperCtl.Text = value;
        }
        public string Increment
        {
            get => ParamIncrementCtl.Text;
            set => ParamIncrementCtl.Text = value;
        }
        public string Operator
        {
            get => OperationCtl.Text;
            set => OperationCtl.Text = value;
        }

        public void SetValues(string name, string lower, string upper, string increment, bool active) {
            ParamName = name;
            Lower = lower;
            Upper = upper;
            Increment = increment;
            Active = active;
        }

        public IncrementalParameterSubPanel() {
            InitializeComponent();
        }
    }
}
