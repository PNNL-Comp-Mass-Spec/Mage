using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MageUIComponents {

    public partial class SimpleFilePanel : UserControl {

        public string FilePath { 
            get { return outputfilePathCtl.Text; }
            set { outputfilePathCtl.Text = value;  }
        }

        public SimpleFilePanel() {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save to file";
            saveFileDialog1.ShowDialog();

            if (!string.IsNullOrEmpty(saveFileDialog1.FileName)) {
                outputfilePathCtl.Text = saveFileDialog1.FileName;
            }
        }
    }
}
