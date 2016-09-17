using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{

    public partial class LocalFolderPanel : UserControl, IModuleParameters
    {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        /// <summary>
        /// Can be RegEx or FileSearch
        /// </summary>
        private string mSelectionMode = "FileSearch";

        #endregion

        #region Properties

        public string FileNameFilter
        {
            get { return LocalFileNameFilterCtl.Text; }
            set { LocalFileNameFilterCtl.Text = value; }
        }

        public string FileSelectionMode
        {
            get
            {
                return mSelectionMode;
            }
            set
            {
                mSelectionMode = value;
                if (mSelectionMode != FileListFilter.FILE_SELECTOR_REGEX)
                {
                    FileSearchRadioBtn.Checked = true;
                }
                else
                {
                    RegExRadioBtn.Checked = true;
                }
            }
        }

        public string Folder
        {
            get { return LocalDirectoryCtl.Text; }
            set { LocalDirectoryCtl.Text = value; }
        }

        public string MostRecentFolder { get; set; }

        public string SearchInSubfolders
        {
            get
            {
                return (SearchInSubfoldersCtl.Checked) ? "Yes" : "No";
            }
            set
            {
                SearchInSubfoldersCtl.Checked = string.Equals(value, "Yes", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public string SubfolderSearchName
        {
            get
            {
                return SubfolderSearchNameCtl.Text;
            }
            set
            {
                SubfolderSearchNameCtl.Text = value;
            }
        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>() { 
                { "FileNameFilter",  FileNameFilter },
                { "FileSelectionMode", FileSelectionMode },
                { "Folder",  Folder },
                { "SearchInSubfolders", SearchInSubfolders},
                { "SubfolderSearchName", SubfolderSearchName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "FileNameFilter":
                        FileNameFilter = paramDef.Value;
                        break;
                    case "FileSelectionMode":
                        FileSelectionMode = paramDef.Value;
                        break;
                    case "Folder":
                        Folder = paramDef.Value;
                        break;
                    case "SearchInSubfolders":
                        SearchInSubfolders = paramDef.Value;
                        break;
                    case "SubfolderSearchName":
                        SubfolderSearchName = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        public LocalFolderPanel()
        {
            InitializeComponent();
            MostRecentFolder = string.Empty;
        }

        private void GetFilesCtl_Click(object sender, EventArgs e)
        {
            if (OnAction != null)
            {
                OnAction(this, new MageCommandEventArgs("get_files_from_local_folder"));
            }
        }

        private void SelectFolderCtl_Click(object sender, EventArgs e)
        {

            var folderBrowser = new PRISM.Files.FolderBrowser();

            try
            {
                if (LocalDirectoryCtl.TextLength > 0 && Directory.Exists(LocalDirectoryCtl.Text))
                {
                    folderBrowser.FolderPath = LocalDirectoryCtl.Text;
                }
                else if (!string.IsNullOrEmpty(MostRecentFolder) && Directory.Exists(MostRecentFolder))
                {
                    folderBrowser.FolderPath = MostRecentFolder;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (folderBrowser.BrowseForFolder())
            {
                LocalDirectoryCtl.Text = folderBrowser.FolderPath;
                MostRecentFolder = folderBrowser.FolderPath;
            }

        }

        private void RegExRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            mSelectionMode = FileListFilter.FILE_SELECTOR_REGEX;
        }

        private void FileSearchRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            mSelectionMode = FileListFilter.FILE_SELECTOR_NORMAL;
        }

    }
}
