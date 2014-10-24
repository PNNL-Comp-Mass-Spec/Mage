using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents {

    public partial class LocalFolderPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        /// <summary>
        /// Can be RegEx or FileSearch
        /// </summary>
        private string mSelectionMode = "FileSearch";

        #endregion

        #region Properties

        public string FileNameFilter {
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
                if (mSelectionMode != "RegEx")
                {
                    FileSearchRadioBtn.Checked = true;
                }
                else
                {
                    RegExRadioBtn.Checked = true;
                }
            }
        }

        public string Folder {
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
				SearchInSubfoldersCtl.Checked = (value == "Yes") ? true : false;
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

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "FileNameFilter",  FileNameFilter },
                { "FileSelectionMode", FileSelectionMode },
                { "Folder",  Folder },
                { "SearchInSubfolders", SearchInSubfolders},
                { "SubfolderSearchName", SubfolderSearchName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
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

        public LocalFolderPanel() {
            InitializeComponent();
            MostRecentFolder = string.Empty;
        }

		private void GetFilesCtl_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("get_files_from_local_folder"));
            }
        }

        private void SelectFolderCtl_Click(object sender, EventArgs e) {
            var browse = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Please select a folder",
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            if (LocalDirectoryCtl.TextLength > 0 && System.IO.Directory.Exists(LocalDirectoryCtl.Text))
                browse.SelectedPath = LocalDirectoryCtl.Text;
            else if (!string.IsNullOrEmpty(MostRecentFolder) && System.IO.Directory.Exists(MostRecentFolder))
                browse.SelectedPath = MostRecentFolder;

            if (browse.ShowDialog() == DialogResult.OK) {
                LocalDirectoryCtl.Text = browse.SelectedPath;
                MostRecentFolder = browse.SelectedPath;
            }
        }
 
        private void RegExRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            mSelectionMode = "RegEx";
        }

        private void FileSearchRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            mSelectionMode = "FileSearch";
        }

    }
}
