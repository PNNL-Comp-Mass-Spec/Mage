using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mage;
using ShFolderBrowser.FolderBrowser;

namespace MageUIComponents
{

    public partial class FolderDestinationPanel : UserControl, IModuleParameters
    {

        public FolderDestinationPanel()
        {
            InitializeComponent();
        }

        #region Member Variables

        #endregion

        #region Properties

        public string OutputFolder
        {
            get { return OutputFolderCtl.Text; }
            set { OutputFolderCtl.Text = value; }
        }

        public string OutputFile
        {
            get
            {
                if (!string.IsNullOrEmpty(OutputFileCtl.Text) && !Path.HasExtension(OutputFileCtl.Text))
                {
                    OutputFileCtl.Text += ".txt";
                }
                return OutputFileCtl.Text;
            }
            set { OutputFileCtl.Text = value; }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>() { 
                { "OutputFolder",   OutputFolder},
                { "OutputFile",   OutputFile}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
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

        private void SelectFolderCtl_Click(object sender, EventArgs e)
        {

            var folderBrowser = new FolderBrowser();

            try
            {
                if (OutputFolderCtl.TextLength > 0 && Directory.Exists(OutputFolderCtl.Text))
                {
                    folderBrowser.FolderPath = OutputFolderCtl.Text;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (folderBrowser.BrowseForFolder())
            {
                OutputFolderCtl.Text = folderBrowser.FolderPath;
            }

        }

        private void DefineDestinationFileCtl_Click(object sender, EventArgs e) {
            var fileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"                
            };

            try
            {
                if (!string.IsNullOrEmpty(OutputFolderCtl.Text) && Directory.Exists(OutputFolderCtl.Text))
                {
                    fileDialog.InitialDirectory = OutputFolderCtl.Text;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (fileDialog.ShowDialog() == DialogResult.OK) {
                var filePath = fileDialog.FileName;
                OutputFileCtl.Text = Path.GetFileName(filePath);
                OutputFolderCtl.Text = Path.GetDirectoryName(filePath);
            }
        }

    }
}
