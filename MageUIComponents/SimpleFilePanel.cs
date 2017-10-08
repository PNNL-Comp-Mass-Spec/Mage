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
            var saveFileDialog1 = new SaveFileDialog {Title = "Save to file"};
            saveFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName)) {
                outputfilePathCtl.Text = saveFileDialog1.FileName;
            }
        }
    }
}
