using System.Windows.Forms;

namespace MageFilePackager
{
    public partial class SubmissionForm : Form
    {
        public SubmissionForm()
        {
            InitializeComponent();
        }

        public string NotificationEmail => notificationEmailCtl.Text;

        public string PackageName => packageNameCtl.Text;

        public string PackageDescription => packageDescriptonCtl.Text;

        public string URL => urlCtl.Text;

        public bool SaveToFile => saveToFileCtl.Checked;

        public bool SendToServer => sendToServerCtl.Checked;

        public string ManifestFilePath => manifestFilePathCtl.Text;

        private void SetFilePathBtnClick(object sender, System.EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = "xml",
                Filter = "XML|*.xml;|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                manifestFilePathCtl.Text = openFileDialog.FileName;
            }
        }
    }
}
