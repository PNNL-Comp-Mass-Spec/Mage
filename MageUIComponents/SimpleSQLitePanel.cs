using System;
using System.Windows.Forms;

namespace MageUIComponents {
    public partial class SimpleSQLitePanel : UserControl {
        public string FilePath => DBFilePathCtl.Text;

        public string TableName => TableNameCtl.Text;

        public SimpleSQLitePanel() {
            InitializeComponent();
        }

        private void BrowseForFileBtn_Click(object sender, EventArgs e) {
            var saveDialog = new SaveFileDialog 
            {
                Title = "Save to file"
            };
            saveDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(saveDialog.FileName)) {
                DBFilePathCtl.Text = saveDialog.FileName;
            }
        }
    }
}
