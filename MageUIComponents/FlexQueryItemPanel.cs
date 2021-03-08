using System;
using System.Windows.Forms;

namespace MageUIComponents
{
    public partial class FlexQueryItemPanel : UserControl
    {
        public FlexQueryItemPanel()
        {
            InitializeComponent();
            RelationCtl.Items.Add("AND");
            RelationCtl.Items.Add("OR");
            RelationCtl.Items.Add("(off)");
        }

        public string Value
        {
            get => ValueCtl.Text;
            set => ValueCtl.Text = value;
        }

        public string Comparision
        {
            get => ComparisonCtl.Text;
            set => ComparisonCtl.Text = value;
        }

        public string Column
        {
            get => ColumnCtl.Text;
            set => ColumnCtl.Text = value;
        }

        public string Relation
        {
            get => (RelationCtl.Text != "(off)") ? RelationCtl.Text : "";
            set => RelationCtl.Text = value;
        }

        public void SetColumnPickList(string[] items)
        {
            // ColumnCtl.Items.Add("");
            ColumnCtl.Items.AddRange(items);
        }

        public void SetComparisionPickList(string[] items)
        {
            // ComparisonCtl.Items.Add("");
            ComparisonCtl.Items.AddRange(items);
        }

        private void ColumnCtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ComparisonCtl.Text))
            {
                ComparisonCtl.Text = "ContainsText";
            }
        }

        private void RelationCtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing for now
        }
    }
}
