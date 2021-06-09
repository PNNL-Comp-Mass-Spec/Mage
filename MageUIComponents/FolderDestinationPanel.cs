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
        // Ignore Spelling: txt

        public FolderDestinationPanel()
        {
            InitializeComponent();
        }

        #region Member Variables

        #endregion

        #region Properties

        public string OutputDirectory
        {
            get => OutputDirectoryCtl.Text;
            set => OutputDirectoryCtl.Text = value;
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
            set => OutputFileCtl.Text = value;
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "OutputDirectory", OutputDirectory},
                { "OutputFile", OutputFile}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "OutputDirectory":
                    case "OutputFolder":
                        OutputDirectory = paramDef.Value;
                        break;
                    case "OutputFile":
                        OutputFile = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        private void SelectDirectoryCtl_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowser();

            try
            {
                if (OutputDirectoryCtl.TextLength > 0 && Directory.Exists(OutputDirectoryCtl.Text))
                {
                    folderBrowser.FolderPath = OutputDirectoryCtl.Text;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (folderBrowser.BrowseForFolder())
            {
                OutputDirectoryCtl.Text = folderBrowser.FolderPath;
            }
        }

        private void DefineDestinationFileCtl_Click(object sender, EventArgs e)
        {
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
                if (!string.IsNullOrEmpty(OutputDirectoryCtl.Text) && Directory.Exists(OutputDirectoryCtl.Text))
                {
                    fileDialog.InitialDirectory = OutputDirectoryCtl.Text;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = fileDialog.FileName;
                OutputFileCtl.Text = Path.GetFileName(filePath);
                OutputDirectoryCtl.Text = Path.GetDirectoryName(filePath);
            }
        }
    }
}
