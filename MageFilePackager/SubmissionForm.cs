using System.Windows.Forms;

namespace MageFilePackager {

    public partial class SubmissionForm : Form {

        public SubmissionForm() {
            InitializeComponent();
        }

        public string NotificationEmail {
            get { return notificationEmailCtl.Text; }
        }

        public string PackageName {
            get { return packageNameCtl.Text; }
        }

        public string PackageDescription {
            get { return packageDescriptonCtl.Text; }
        }

        public string URL {
            get { return urlCtl.Text; }
        }

        public bool SaveToFile {
            get { return saveToFileCtl.Checked; }
        }

        public bool SendToServer {
            get { return sendToServerCtl.Checked; }
        }

        public string ManifestFilePath {
            get { return manifestFilePathCtl.Text; }
        }

        private void SetFilePathBtnClick(object sender, System.EventArgs e) {
            var openFileDialog = new OpenFileDialog {
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = "xml",
                Filter = "XML|*.xml;|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                manifestFilePathCtl.Text = openFileDialog.FileName;
            }
        }

    }
}
