using System;
using System.Windows.Forms;

namespace MageUIComponents {
    public partial class SimpleFilePanel : UserControl {
        public string FilePath {
            get => outputfilePathCtl.Text;
            set => outputfilePathCtl.Text = value;
        }

        public SimpleFilePanel() {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, EventArgs e) {
            var saveDialog = new SaveFileDialog
            {
                Title = "Save to file"
            };
            saveDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(saveDialog.FileName)) {
                outputfilePathCtl.Text = saveDialog.FileName;
            }
        }
    }
}
