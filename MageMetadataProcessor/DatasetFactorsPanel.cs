using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageMetadataProcessor
{
    public partial class DatasetFactorsPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Properties

        public string DatasetName
        {
            get => DatasetNameCtl.Text;
            set => DatasetNameCtl.Text = value;
        }
        public string DataPackageNumber
        {
            get => DataPackageNumberCtl.Text;
            set => DataPackageNumberCtl.Text = value;
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new() {
                { "DatasetName",  DatasetName },
                { "DataPackageNumber", DataPackageNumber }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "DatasetName":
                        DatasetName = paramDef.Value;
                        break;
                    case "DataPackageNumber":
                        DataPackageNumber = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        public DatasetFactorsPanel()
        {
            InitializeComponent();
        }

        private void FactorCountBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("factor_count", ""));
        }

        private void GetFactorsBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("factor_list", ""));
        }

        private void GetResultsCrosstabBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("factor_crosstab", ""));
        }

        private void GetMetadataBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("dataset_metadata", ""));
        }
    }
}
