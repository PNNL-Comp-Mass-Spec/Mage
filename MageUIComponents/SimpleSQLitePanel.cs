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
            var saveFileDialog1 = new SaveFileDialog {
                Title = "Save to file"
            };
            saveFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName)) {
                DBFilePathCtl.Text = saveFileDialog1.FileName;
            }
        }
    }
}
