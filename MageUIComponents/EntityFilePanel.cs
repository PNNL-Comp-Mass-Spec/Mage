using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class EntityFilePanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        public EntityFilePanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Can be RegEx or FileSearch
        /// </summary>
        private string mSelectionMode = FileListFilter.FILE_SELECTOR_NORMAL;

        public string FileSelectionMode
        {
            get => mSelectionMode;
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

        /// <summary>
        /// This should be File or Directory or Folder
        /// </summary>
        public string IncludeFilesOrDirectories
        {
            set
            {
                IncludefilesCtl.Checked = false;
                IncludeDirectoriesCtl.Checked = false;
                if (value.Contains("File"))
                {
                    IncludefilesCtl.Checked = true;
                }

                if (value.Contains("Directory") || value.Contains("Directories"))
                {
                    IncludeDirectoriesCtl.Checked = true;
                }
                else if (value.Contains("Folder"))
                {
                    IncludeDirectoriesCtl.Checked = true;
                }

                if (!IncludefilesCtl.Checked && !IncludeDirectoriesCtl.Checked)
                {
                    // Search for files by default
                    IncludefilesCtl.Checked = true;
                }
            }
            get
            {
                var state = string.Empty;
                if (IncludefilesCtl.Checked)
                {
                    state += "File";
                }

                if (IncludeDirectoriesCtl.Checked)
                {
                    state += "Directory";
                }

                if (string.IsNullOrWhiteSpace(state))
                {
                    // Search for files by default
                    state = "File";
                }

                return state;
            }
        }

        [Obsolete("Use IncludeFilesOrDirectories")]
        public string IncludeFilesOrFolders
        {
            get => IncludeFilesOrDirectories;
            set => IncludeFilesOrDirectories = value;
        }

        public string SearchInSubdirectories
        {
            get => (SearchInSubdirectoriesCtl.Checked) ? "Yes" : "No";
            set => SearchInSubdirectoriesCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        [Obsolete("Use SearchInSubdirectories")]
        public string SearchInSubfolders
        {
            get => SearchInSubdirectories;
            set => SearchInSubdirectories = value;
        }

        public string SubdirectorySearchName
        {
            get => SubdirectorySearchNameCtl.Text;
            set => SubdirectorySearchNameCtl.Text = value;
        }

        [Obsolete("Use SubdirectorySearchName")]
        public string SubfolderSearchName
        {
            get => SubdirectorySearchName;
            set => SubdirectorySearchName = value;
        }

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "FileSelectors", FileSelectors },
                { "FileSelectionMode", FileSelectionMode },
                { "IncludeFilesOrDirectories", IncludeFilesOrDirectories},
                { "SearchInSubdirectories", SearchInSubdirectories},
                { "SubdirectorySearchName", SubdirectorySearchName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "FileSelectors":
                        FileSelectors = paramDef.Value;
                        break;
                    case "FileSelectionMode":
                        FileSelectionMode = paramDef.Value;
                        break;
                    case "IncludeFilesOrDirectories":
                        IncludeFilesOrDirectories = paramDef.Value;
                        break;
                    case "SearchInSubdirectories":
                        SearchInSubdirectories = paramDef.Value;
                        break;
                    case "SubdirectorySearchName":
                        SubdirectorySearchName = paramDef.Value;
                        break;
                }
            }
        }

        #region Properties

        public string FileSelectors
        {
            get => FileSelectorsCtl.Text;
            set => FileSelectorsCtl.Text = value;
        }

        #endregion

        private void GetFilesForSelectedEntriesCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_files_from_entities", "selected"));
        }

        private void GetFilesForAllEntriesCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_files_from_entities", "all"));
        }

        #endregion

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
