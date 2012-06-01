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

    public partial class LocalFolderPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        #endregion

        #region Properties

        public string FileNameFilter {
            get { return LocalFileNameFilterCtl.Text; }
            set { LocalFileNameFilterCtl.Text = value; }
        }

        public string Folder {
            get { return LocalDirectoryCtl.Text; }
            set { LocalDirectoryCtl.Text = value; }
        }

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
                { "Folder",  Folder }
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (KeyValuePair<string, string> paramDef in paramList) {
                switch (paramDef.Key) {
                    case "FileNameFilter":
                        FileNameFilter = paramDef.Value;
                        break;
                    case "Folder":
                        Folder = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        public LocalFolderPanel() {
            InitializeComponent();
        }

		private void GetFilesCtl_Click(object sender, EventArgs e) {
            if (OnAction != null) {
                OnAction(this, new MageCommandEventArgs("get_files_from_local_folder"));
            }
        }

        private void SelectFolderCtl_Click(object sender, EventArgs e) {
            FolderBrowserDialog browse = new FolderBrowserDialog();
            browse.ShowNewFolderButton = true;
            browse.Description = "Please select a folder";
			browse.RootFolder = Environment.SpecialFolder.MyComputer; //Environment.SpecialFolder.DesktopDirectory;

			if (LocalDirectoryCtl.TextLength > 0 && System.IO.Directory.Exists(LocalDirectoryCtl.Text))
				browse.SelectedPath = LocalDirectoryCtl.Text;

            if (browse.ShowDialog() == DialogResult.OK) {
                LocalDirectoryCtl.Text = browse.SelectedPath;
            }
        }


    }
}
