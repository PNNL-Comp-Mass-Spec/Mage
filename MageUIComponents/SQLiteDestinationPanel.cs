using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using System.IO;

namespace MageUIComponents {

    public partial class SQLiteDestinationPanel : UserControl, IModuleParameters {

        public SQLiteDestinationPanel() {
            InitializeComponent();
        }

        #region Properties

        public string DatabaseName {
            get {
                if (!string.IsNullOrEmpty(DatabaseNameCtl.Text) && !Path.HasExtension(DatabaseNameCtl.Text)) {
					if (DatabaseNameCtl.Text.EndsWith("\\"))
						DatabaseNameCtl.Text += "Output";

                    DatabaseNameCtl.Text += ".db3";
                }
                return DatabaseNameCtl.Text; 
            }
            set { DatabaseNameCtl.Text = value; }
        }

        public string TableName {
            get { return TableNameCtl.Text; }
            set { TableNameCtl.Text = value; }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "DatabaseName",   DatabaseName},
                { "TableName",   TableName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "DatabaseName":
                        DatabaseName = paramDef.Value;
                        break;
                    case "TableName":
                        TableName = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

		private void SelectSqLiteDbCtl_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = false;
            openFileDialog1.DefaultExt = "db3";
            openFileDialog1.Filter = "SQLite3|*.db;*.db3|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                DatabaseName = ValidateFileExtension(openFileDialog1.FileName);
            }
        }

		private void DefineSqLiteTableCtl_Click(object sender, EventArgs e) {
            SQLiteTableSelectionPanel selectionForm = new SQLiteTableSelectionPanel();
            selectionForm.DatabasePath = DatabaseName;
            if (selectionForm.ShowDialog() == DialogResult.OK) {
                TableName = selectionForm.TableName;
            }
        }

		private string ValidateFileExtension(string filePath)
		{
			var fiFile = new FileInfo(filePath);
			string extension = fiFile.Extension.ToLower();			

			if (extension != ".db3" && extension != ".db" && extension != ".sqlite3" && extension != ".sqlite")
			{
				extension = ".db3";
				filePath = Path.ChangeExtension(filePath, extension);
			}

			if (Path.GetFileNameWithoutExtension(filePath).Length == 0)
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
