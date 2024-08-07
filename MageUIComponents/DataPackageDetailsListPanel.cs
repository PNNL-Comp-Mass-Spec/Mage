﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    public partial class DataPackageDetailsListPanel : UserControl, IModuleParameters
    {
        // Ignore Spelling: Ctrl

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

        private void LegendCtlClick(object sender, EventArgs e)
        {
        }
    }
}
