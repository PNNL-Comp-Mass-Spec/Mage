using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class SQLiteDestinationPanel : UserControl, IModuleParameters
    {
        public SQLiteDestinationPanel()
        {
            InitializeComponent();
        }

        public string DatabaseName
        {
            get
            {
                if (!string.IsNullOrEmpty(DatabaseNameCtl.Text) && !Path.HasExtension(DatabaseNameCtl.Text))
                {
                    if (DatabaseNameCtl.Text.EndsWith("\\"))
                        DatabaseNameCtl.Text += "Output";

                    DatabaseNameCtl.Text += ".db3";
                }
                return DatabaseNameCtl.Text;
            }
            set => DatabaseNameCtl.Text = value;
        }

        public string TableName
        {
            get => TableNameCtl.Text;
            set => TableNameCtl.Text = value;
        }

        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "DatabaseName",   DatabaseName},
                { "TableName",   TableName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "DatabaseName":
                        DatabaseName = paramDef.Value;
                        break;
                    case "TableName":
                        TableName = paramDef.Value;
                        break;
                }
            }
        }

        private void SelectSqLiteDbCtl_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = "db3",
                Filter = "SQLite3|*.db;*.db3|All files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                DatabaseName = ValidateFileExtension(fileDialog.FileName);
            }
        }

        private void DefineSqLiteTableCtl_Click(object sender, EventArgs e)
        {
            var selectionForm = new SQLiteTableSelectionPanel { DatabasePath = DatabaseName };
            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                TableName = selectionForm.TableName;
            }
        }

        private string ValidateFileExtension(string filePath)
        {
            var fiFile = new FileInfo(filePath);
            var extension = fiFile.Extension.ToLower();

            if (extension != ".db3" && extension != ".db" && extension != ".sqlite3" && extension != ".sqlite")
            {
                extension = ".db3";
                filePath = Path.ChangeExtension(filePath, extension);
            }

            var baseFileName = Path.GetFileNameWithoutExtension(filePath);
            if (baseFileName.Length == 0)
            {
                if (filePath.Length == extension.Length)
                    filePath = Path.Combine("MageResults") + extension;
                else
                    filePath = filePath.Substring(0, filePath.Length - extension.Length) + Path.Combine("MageExtractorResults") + extension;
            }

            return filePath;
        }

        private void ValidateSQLiteDBPath(object sender, CancelEventArgs e)
        {
            DatabaseNameCtl.Text = ValidateFileExtension(DatabaseNameCtl.Text);
        }
    }
}
