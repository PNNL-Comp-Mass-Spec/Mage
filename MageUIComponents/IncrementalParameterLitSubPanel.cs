using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents {

    public partial class IncrementalParameterLitSubPanel : UserControl, IModuleParameters {

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "ParamName", ParamNameCtl.Text },
                { "Operator", OperationCtl.Text },
                { "Active", (ActiveCtl.Checked)?"On":"Off" },
                { "IncrementList", IncrementListCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "ParamName":
                        ParamNameCtl.Text = paramDef.Value;
                        break;
                    case "Operator":
                        OperationCtl.Text = paramDef.Value;
                        break;
                    case "Active":
                        ActiveCtl.Checked = (paramDef.Value == "On");
                        break;
                    case "IncrementList":
                        IncrementListCtl.Text = paramDef.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        public IncrementalParameterLitSubPanel() {
            InitializeComponent();
        }

        //FUTURE: Add automatic conversion to comma delimited for increment list
    }
}
