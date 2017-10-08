using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mage;
using ShFolderBrowser.FolderBrowser;

namespace MageUIComponents
{

    public partial class FileCopyPanel : UserControl, IModuleParameters
    {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        #endregion

        #region Properties

        public string OverwriteExistingFiles
        {
            get => (OverwriteExistingCtl.Checked) ? "Yes" : "No";
            set => OverwriteExistingCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        public string OutputFolder
        {
            get => OutputFolderCtl.Text;
            set => OutputFolderCtl.Text = value;
        }

        public string ApplyPrefixToFileName
        {
            get => (usePrefixCtl.Checked) ? "Yes" : "No";
            set
            {
                usePrefixCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
                AdjustPrefixNameFields();
            }
        }

        public string PrefixLeader
        {
            get => prefixLeaderCtl.Text;
            set => prefixLeaderCtl.Text = value;
        }

        public string PrefixColumnName
        {
            get => prefixColNameCtl.Text;
            set => prefixColNameCtl.Text = value;
        }

        public string ResolveCacheInfoFiles
        {
            get => (ResolveCacheInfoFilesCtl.Checked) ? "Yes" : "No";
            set => ResolveCacheInfoFilesCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "OutputFolder",           OutputFolder},
                { "OverwriteExistingFiles", OverwriteExistingFiles},
                { "ApplyPrefixToFileName",  ApplyPrefixToFileName},
                { "PrefixLeader",           PrefixLeader},
                { "PrefixColumnName",       PrefixColumnName},
                { "ResolveCacheInfoFiles",  ResolveCacheInfoFiles}
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
                    case "OverwriteExistingFiles":
                        OverwriteExistingFiles = paramDef.Value;
                        break;
                    case "ApplyPrefixToFileName":
                        ApplyPrefixToFileName = paramDef.Value;
                        break;
                    case "PrefixLeader":
                        PrefixLeader = paramDef.Value;
                        break;
                    case "PrefixColumnName":
                        PrefixColumnName = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        public FileCopyPanel()
        {
            InitializeComponent();
        }

        private void CopyAllCtl_Click(object sender, EventArgs e)
        {
            if (OnAction != null)
            {
                var command = new MageCommandEventArgs
                {
                    Mode = "all",
                    Action = "copy_files"
                };
                OnAction(this, command);
            }
        }

        private void CopySelectedCtl_Click(object sender, EventArgs e)
        {
            if (OnAction != null)
            {
                var command = new MageCommandEventArgs
                {
                    Mode = "selected",
                    Action = "copy_files"
                };
                OnAction(this, command);
            }
        }

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

        private void usePrefixCtl_CheckedChanged(object sender, EventArgs e)
        {
            AdjustPrefixNameFields();
        }

        private void AdjustPrefixNameFields()
        {
            if (usePrefixCtl.Checked)
            {
                prefixColNameCtl.Visible = true;
                prefixColNameLabelCtl.Visible = true;
                prefixLeaderCtl.Visible = true;
                prefixLeaderLabelCtl.Visible = true;
            }
            else
            {
                prefixColNameCtl.Visible = false;
                prefixColNameLabelCtl.Visible = false;
                prefixLeaderCtl.Visible = false;
                prefixLeaderLabelCtl.Visible = false;
            }
        }


    }
}
