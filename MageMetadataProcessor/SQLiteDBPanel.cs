using System;
using System.Collections.Generic;
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
            foreach (var paramDef in paramList) {
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
            var saveDialog = new SaveFileDialog
            {
                Title = "Save display to file"
            };

            saveDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(saveDialog.FileName)) {
                DBFilePathCtl.Text = saveDialog.FileName;
            }
        }

        private void SaveAllBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("save_to_db", "all"));
        }

        private void SaveSelectedBtn_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("save_to_db", "selected"));
        }

        private void ClearColMapBtn_Click(object sender, EventArgs e) {
            ColumnMapping = "(automatic)";
            OutputColumnList = string.Empty;
        }

        private void SelectColMapBtn_Click(object sender, EventArgs e) {
            var selectionForm = new ColumnMapSelectionForm {ColumnMapping = ColumnMapping};
            if (selectionForm.ShowDialog() == DialogResult.OK) {
                ColumnMapping = selectionForm.ColumnMapping;
                OutputColumnList = selectionForm.OutputColumnList;
            }
        }

    }
}
