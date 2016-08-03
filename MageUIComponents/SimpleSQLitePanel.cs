using System;
using System.Windows.Forms;

namespace MageUIComponents {

    public partial class SimpleSQLitePanel : UserControl {

        public string FilePath {
            get { return DBFilePathCtl.Text; }
        }

        public string TableName {
            get { return TableNameCtl.Text; }
        }

        public SimpleSQLitePanel() {
            InitializeComponent();
        }

        private void BrowseForFileBtn_Click(object sender, EventArgs e) {
            var saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save to file";
            saveFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName)) {
                DBFilePathCtl.Text = saveFileDialog1.FileName;
            }
        }
    }
}
