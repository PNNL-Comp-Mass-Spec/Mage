using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class DataPackageDetailsListPanel : UserControl, IModuleParameters
    {
        // Ignore Spelling: Ctrl, Mage

        public event EventHandler<MageCommandEventArgs> OnAction;

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "ID", ItemListCtl.Text }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "ID":
                        ItemListCtl.Text = paramDef.Value;
                        break;
                }
            }
        }

        public DataPackageDetailsListPanel()
        {
            InitializeComponent();
        }

        private void ItemListCtlClick(object sender, EventArgs e)
        {
            if (!ValidateDataPackageIDs())
                return;

            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_query", "Data Packages"));
        }

        private void ItemListCtlLeave(object sender, EventArgs e)
        {
            ItemListCtl.Text = PanelSupport.CleanUpDelimitedList(ItemListCtl.Text);
        }

        private void ItemListCtlKeyDown(object sender, EventArgs e)
        {
            var args = (KeyEventArgs)e;

            if (args.Control && args.KeyCode == Keys.A)
            {
                // Ctrl+A pressed
                ItemListCtl.SelectAll();
            }
        }

        private bool ValidateDataPackageIDs()
        {
            if (string.IsNullOrWhiteSpace(ItemListCtl.Text))
            {
                MessageBox.Show("Please enter one or more data package IDs", "Missing Data Package ID(s)",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            foreach (var dataPackageId in ItemListCtl.Text.Split(','))
            {
                if (string.IsNullOrWhiteSpace(dataPackageId))
                    continue;

                if (int.TryParse(dataPackageId, out _))
                    continue;

                MessageBox.Show(string.Format("Invalid data package ID '{0}'; must be an integer", dataPackageId.Trim()), "Invalid Data Package ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return false;
            }

            return true;
        }
    }
}
