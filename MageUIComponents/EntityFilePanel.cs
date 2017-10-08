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

        public string IncludeFilesOrFolders
        {
            set
            {
                IncludefilesCtl.Checked = false;
                IncludeFoldersCtl.Checked = false;
                if (value.Contains("File"))
                {
                    IncludefilesCtl.Checked = true;
                }
                if (value.Contains("Folder"))
                {
                    IncludeFoldersCtl.Checked = true;
                }
            }
            get
            {
                var state = "";
                if (IncludefilesCtl.Checked)
                {
                    state += "File";
                }
                if (IncludeFoldersCtl.Checked)
                {
                    state += "Folder";
                }
                return state;
            }
        }

        public string SearchInSubfolders
        {
            get => (SearchInSubfoldersCtl.Checked) ? "Yes" : "No";
            set => SearchInSubfoldersCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        public string SubfolderSearchName
        {
            get => SubfolderSearchNameCtl.Text;
            set => SubfolderSearchNameCtl.Text = value;
        }

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>() {
                { "FileSelectors", FileSelectors },
                { "FileSelectionMode", FileSelectionMode },
                { "IncludeFilesOrFolders", IncludeFilesOrFolders},
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
                    case "FileSelectors":
                        FileSelectors = paramDef.Value;
                        break;
                    case "FileSelectionMode":
                        FileSelectionMode = paramDef.Value;
                        break;
                    case "IncludeFilesOrFolders":
                        IncludeFilesOrFolders = paramDef.Value;
                        break;
                    case "SearchInSubfolders":
                        SearchInSubfolders = paramDef.Value;
                        break;
                    case "SubfolderSearchName":
                        SubfolderSearchName = paramDef.Value;
                        break;
                    default:
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
