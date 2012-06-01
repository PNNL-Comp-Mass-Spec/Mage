using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using MageUIComponents;

namespace MageMetadataProcessor {

    public partial class SQLiteDBPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        public SQLiteDBPanel() {
            InitializeComponent();
        }

        #region Properties

        public string TableName {
            get { return TableNameCtl.Text; }
            set { TableNameCtl.Text = value; }
        }

        public string DBFilePath {
            get { return DBFilePathCtl.Text; }
            set { DBFilePathCtl.Text = value; }
        }

        public string ColumnMapping {
            get { return ColumnMapSelectionCtl.Text; }
            set { ColumnMapSelectionCtl.Text = value; }
        }

        public string OutputColumnList { get; set; }

        #endregion

        // TableNameCtl;
        // DBFilePathCtl;

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "TableName",  TableName },
                { "DBFilePath",  DBFilePath }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "TableName":
                        TableName = paramDef.Value;
                        break;
                    case "DBFilePath":
                        DBFilePath = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        private void BrowseForFileBtn_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save display to file";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "") {
                DBFilePathCtl.Text = saveFileDialog1.FileName;
            }
        }

        private void SaveAllBtn_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("save_to_db", "all"));
            }
        }

        private void SaveSelectedBtn_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("save_to_db", "selected"));
            }
        }

        private void ClearColMapBtn_Click(object sender, EventArgs e) {
            ColumnMapping = "(automatic)";
            OutputColumnList = "";
        }

        private void SelectColMapBtn_Click(object sender, EventArgs e) {
            ColumnMapSelectionForm selectionForm = new ColumnMapSelectionForm();
            selectionForm.ColumnMapping = ColumnMapping;
            if (selectionForm.ShowDialog() == DialogResult.OK) {
                ColumnMapping = selectionForm.ColumnMapping;
                OutputColumnList = selectionForm.OutputColumnList;
            }
        }

    }
}
