﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using ShFolderBrowser.FolderBrowser;

namespace MageUIComponents
{
    public partial class LocalFolderPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        /// <summary>
        /// Can be RegEx or FileSearch
        /// </summary>
        private string mSelectionMode = "FileSearch";

        public string FileNameFilter
        {
            get => LocalFileNameFilterCtl.Text;
            set => LocalFileNameFilterCtl.Text = value;
        }

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

        public string Directory
        {
            get => LocalDirectoryCtl.Text;
            set => LocalDirectoryCtl.Text = value;
        }

        public string MostRecentDirectory { get; set; }

        public string SearchInSubdirectories
        {
            get => SearchInSubdirectoriesCtl.Checked ? "Yes" : "No";
            set => SearchInSubdirectoriesCtl.Checked = string.Equals(value, "Yes", StringComparison.OrdinalIgnoreCase);
        }

        public string SubdirectorySearchName
        {
            get => SubdirectorySearchNameCtl.Text;
            set => SubdirectorySearchNameCtl.Text = value;
        }

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
            {
                { "FileNameFilter",         FileNameFilter },
                { "FileSelectionMode",      FileSelectionMode },
                { "Directory",              Directory },
                { "SearchInSubdirectories", SearchInSubdirectories },
                { "SubdirectorySearchName", SubdirectorySearchName }
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

                    case "Directory":
                    case "Folder":
                        Directory = paramDef.Value;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalFolderPanel()
        {
            InitializeComponent();
            MostRecentDirectory = string.Empty;
        }

        private void GetFilesCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_files_from_local_directory"));
        }

        private void SelectDirectoryCtl_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowser();

            try
            {
                if (LocalDirectoryCtl.TextLength > 0 && System.IO.Directory.Exists(LocalDirectoryCtl.Text))
                {
                    folderBrowser.FolderPath = LocalDirectoryCtl.Text;
                }
                else if (!string.IsNullOrEmpty(MostRecentDirectory) && System.IO.Directory.Exists(MostRecentDirectory))
                {
                    folderBrowser.FolderPath = MostRecentDirectory;
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }

            if (folderBrowser.BrowseForFolder())
            {
                LocalDirectoryCtl.Text = folderBrowser.FolderPath;
                MostRecentDirectory = folderBrowser.FolderPath;
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
