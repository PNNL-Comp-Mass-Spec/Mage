using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents {

    public partial class LocalManifestPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        public LocalManifestPanel() {
            InitializeComponent();
        }

        #region Member Variables

        #endregion

        #region Properties

        public string ManifestFilePath {
            get { return LocalManifestFileCtl.Text; }
            set { LocalManifestFileCtl.Text = value; }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "ManifestFilePath",  ManifestFilePath }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "ManifestFilePath":
                        ManifestFilePath = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        private void DefineManifestFileCtl_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                LocalManifestFileCtl.Text = openFileDialog1.FileName;
                //string dirName = Path.GetDirectoryName(openFileDialog1.FileName);
            }
        }

		private void GetFilesCtl_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("get_files_from_local_manifest"));
            }
        }

    }
}
