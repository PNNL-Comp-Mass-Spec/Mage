using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents {

    public partial class SQLiteTableSelectionPanel : Form {

        #region Properties

        public string DatabasePath {
            get { return DatabasePathCtl.Text; }
            set { DatabasePathCtl.Text = value; }
        }

        public string TableName {
            get { return TableNameCtl.Text;  }
            set { TableNameCtl.Text = value; }
        }


        #endregion

        public SQLiteTableSelectionPanel() {
            InitializeComponent();
            TableName = "";
            TableListCtl.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SQLiteTableSelectionPanel_Load(object sender, EventArgs e) {
            SQLiteReader reader = new SQLiteReader();
            reader.Database = DatabasePath;
            reader.SQLText = "SELECT tbl_name as Table_Name FROM sqlite_master WHERE type = 'table'";
            ISinkModule display = TableListCtl.MakeSink();
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble("GetDBTableList", reader, display);
            pipeline.RunRoot(null);
        }

        private void TableListCtl_SelectionChanged(object sender, EventArgs e) {
            if (TableListCtl.List.SelectedRows.Count > 0) {
                TableName = TableListCtl.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }
    }
}
