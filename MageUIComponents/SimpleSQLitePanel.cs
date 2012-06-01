using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MageUIComponents {

    public partial class SimpleSQLitePanel : UserControl {

        public string FilePath {
            get { return DBFilePathCtl.Text; }
        }

        public string TableName {
            get { return TableNameCtl.Text; }
        }

        public SimpleSQLitePanel() {
            InitializeComponent();
        }

        private void BrowseForFileBtn_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save to file";
            saveFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName)) {
                DBFilePathCtl.Text = saveFileDialog1.FileName;
            }
        }
    }
}
