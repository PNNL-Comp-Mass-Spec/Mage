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

    public partial class FolderDestinationPanel : UserControl, IModuleParameters {

        public FolderDestinationPanel() {
            InitializeComponent();
        }

        #region Member Variables

        #endregion

        #region Properties

        public string OutputFolder {
            get { return OutputFolderCtl.Text; }
            set { OutputFolderCtl.Text = value; }
        }

        public string OutputFile {
            get {
                if (!string.IsNullOrEmpty(OutputFileCtl.Text) && !Path.HasExtension(OutputFileCtl.Text)) {
                    OutputFileCtl.Text += ".txt";
                }
                return OutputFileCtl.Text; 
            }
            set { OutputFileCtl.Text = value; }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "OutputFolder",   OutputFolder},
                { "OutputFile",   OutputFile}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "OutputFolder":
                        OutputFolder = paramDef.Value;
                        break;
                    case "OutputFile":
                        OutputFile = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        private void SelectFolderCtl_Click(object sender, EventArgs e) {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowNewFolderButton = true;
            browse.Description = "Please select a folder";
            browse.RootFolder = Environment.SpecialFolder.MyComputer; //Environment.SpecialFolder.MyComputer; //Environment.SpecialFolder.DesktopDirectory;

			if (OutputFolderCtl.TextLength > 0 && System.IO.Directory.Exists(OutputFolderCtl.Text))
				browse.SelectedPath = OutputFolderCtl.Text;
			
            if (browse.ShowDialog() == DialogResult.OK) {
                OutputFolderCtl.Text = browse.SelectedPath;
            }
        }

        private void DefineDestinationFileCtl_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.AddExtension = true;
            openFileDialog1.CheckFileExists = false;
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                string filePath = openFileDialog1.FileName;
                OutputFileCtl.Text = Path.GetFileName(filePath);
                OutputFolderCtl.Text = Path.GetDirectoryName(filePath);
            }
        }

    }
}
