using System;
using System.Windows.Forms;
using Mage;

namespace MageDemo
{
    public partial class MageDemo : Form
    {
        public MageDemo()
        {
            InitializeComponent();
            pnlStatus.EnableCancel = false;
        }

        private void cmdGetData_Click(object sender, EventArgs e)
        {
            var reader = new MSSQLReader {
                Server = "gigasax",
                Database = "DMS5",
                SQLText = "select * from T_Campaign"};

            pnlStatus.EnableCancel = true;
            var p = ProcessingPipeline.Assemble("x", reader, gridViewDisplayControl1);
            p.OnStatusMessageUpdated += pnlStatus.HandleStatusMessageUpdated;
            p.OnRunCompleted += pnlStatus.HandleCompletionMessageUpdate;
            p.Run();
            pnlStatus.EnableCancel = false;
        }
    }
}
