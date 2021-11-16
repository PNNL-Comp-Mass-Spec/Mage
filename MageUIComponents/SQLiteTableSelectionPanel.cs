using System;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class SQLiteTableSelectionPanel : Form
    {
        public string DatabasePath
        {
            get => DatabasePathCtl.Text;
            set => DatabasePathCtl.Text = value;
        }

        public string TableName
        {
            get => TableNameCtl.Text;
            set => TableNameCtl.Text = value;
        }

        public SQLiteTableSelectionPanel()
        {
            InitializeComponent();
            TableName = string.Empty;
            TableListCtl.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SQLiteTableSelectionPanel_Load(object sender, EventArgs e)
        {
            var reader = new SQLiteReader
            {
                Database = DatabasePath,
                SQLText = "SELECT tbl_name as Table_Name FROM sqlite_master WHERE type = 'table'"
            };
            var display = TableListCtl.MakeSink();
            var pipeline = ProcessingPipeline.Assemble("GetDBTableList", reader, display);
            pipeline.RunRoot(null);
        }

        private void TableListCtl_SelectionChanged(object sender, EventArgs e)
        {
            if (TableListCtl.List.SelectedRows.Count > 0)
            {
                TableName = TableListCtl.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }
    }
}
