using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageMetadataProcessor
{

    public partial class RawQueryPanel : UserControl, IModuleParameters
    {

        public event EventHandler<MageCommandEventArgs> OnAction;

        public RawQueryPanel()
        {
            InitializeComponent();
        }

        #region Properties

        public string RawSQL
        {
            get { return RawSQLCtl.Text; }
            set { RawSQLCtl.Text = value; }
        }

        #endregion


        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>() {
                { "RawSQL",  RawSQL }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "RawSQL":
                        RawSQL = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        private void GetResultsBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("basic_read", ""));
        }

        private void GetResultsCrosstabBtn_Click(object sender, EventArgs e)
        {
        }


    }
}
