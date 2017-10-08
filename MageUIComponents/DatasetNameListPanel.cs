using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class DatasetNameListPanel : UserControl, IModuleParameters
    {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "Dataset", DatasetListCtl.Text }
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

        #endregion

        public DatasetNameListPanel()
        {
            InitializeComponent();
        }

        private void GetDatasetsCtl_Click(object sender, EventArgs e)
        {
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
    }
}
