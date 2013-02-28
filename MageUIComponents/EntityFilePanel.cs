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

    public partial class EntityFilePanel : UserControl, IModuleParameters {
        public event EventHandler<MageCommandEventArgs> OnAction;

        public EntityFilePanel() {
            InitializeComponent();
        }

        private string mSelectionMode = "RegEx";

        public string FileSelectionMode {
            get {
                return mSelectionMode;
            }
            set {
                mSelectionMode = value;
                if (mSelectionMode != "RegEx") {
                    FileSearchRadioBtn.Checked = true;
                } else {
                    RegExRadioBtn.Checked = true;
                }
            }
        }

        public string IncludeFilesOrFolders {
            set {
                IncludefilesCtl.Checked = false;
                IncludeFoldersCtl.Checked = false;
                if (value.Contains("File")) {
                    IncludefilesCtl.Checked = true;
                }
                if (value.Contains("Folder")) {
                    IncludeFoldersCtl.Checked = true;
                }
            }
            get {
                string state = "";
                if (IncludefilesCtl.Checked) {
                    state += "File";
                }
                if (IncludeFoldersCtl.Checked) {
                    state += "Folder";
                }
                return state;
            }
        }

        public string SearchInSubfolders {
            get {
                return (SearchInSubfoldersCtl.Checked) ? "Yes" : "No";
            }
            set {
                SearchInSubfoldersCtl.Checked = (value == "Yes") ? true : false;
            }
        }

        public string SubfolderSearchName {
            get {
                return SubfolderSearchNameCtl.Text;
            }
            set {
                SubfolderSearchNameCtl.Text = value;
            }
        }

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "FileSelectors", FileSelectors },
                { "FileSelectionMode", FileSelectionMode },
                { "IncludeFilesOrFolders", IncludeFilesOrFolders},
                { "SearchInSubfolders", SearchInSubfolders},
                { "SubfolderSearchName", SubfolderSearchName}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
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

        public string FileSelectors {
            get { return FileSelectorsCtl.Text; }
            set { FileSelectorsCtl.Text = value; }
        }

        #endregion

		private void GetFilesForSelectedEntriesCtl_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("get_files_from_entities", "selected"));
            }
        }

		private void GetFilesForAllEntriesCtl_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("get_files_from_entities", "all"));
            }
        }

        #endregion

        private void RegExRadioBtn_CheckedChanged(object sender, EventArgs e) {
            mSelectionMode = "RegEx";
        }

        private void FileSearchRadioBtn_CheckedChanged(object sender, EventArgs e) {
            mSelectionMode = "FileSearch";
        }
    }
}
