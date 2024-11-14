using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageExtractor
{
    [Obsolete("Deprecated in 2024")]
    public partial class ResultsFilterSelector : Form, IModuleParameters
    {
        // Ignore Spelling: Mage

        private readonly Dictionary<string, string> mParameters = new();

        private string mFilterSetIDToAutoSelect = string.Empty;

        private ProcessingPipeline mGetFilterSetsPipeline;

        public string FilterSetID
        {
            get => FilterSetIDCtl.Text;
            set => FilterSetIDCtl.Text = value;
        }

        public string FilterSetIDToSelect
        {
            set
            {
                FilterSetIDCtl.Text = value;
                mFilterSetIDToAutoSelect = FilterSetIDCtl.Text;
            }
        }

        public Dictionary<string, string> GetParameters()
        {
            mParameters["FilterSetID"] = FilterSetIDCtl.Text;
            return mParameters;
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            FilterSetIDCtl.Text = paramList["FilterSetID"];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ResultsFilterSelector()
        {
            InitializeComponent();
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.List.AllowDelete = false;
        }

        /// <summary>
        /// Populate the GridView with the filter sets defined in DMS
        /// </summary>
        public void InitializeFilterSetList()
        {
            // Create Mage module to query DMS (typically on prismdb2.emsl.pnl.gov)
            // Note that V_PDE_Filter_Sets was deprecated in 2024

            var reader = new SQLReader
            {
                Database = Globals.DMSDatabase,
                Server = Globals.DMSServer,
                SQLText = "SELECT Filter_Set_ID, Name, Description FROM V_PDE_Filter_Sets",
                IsPostgres = Globals.PostgresDMS
            };

            // Create Mage module to receive query results
            var filters = gridViewDisplayControl1.MakeSink("Filter Sets");

            // Build pipeline and run it
            mGetFilterSetsPipeline = ProcessingPipeline.Assemble("GetFilters", reader, filters);
            mGetFilterSetsPipeline.OnRunCompleted += HandlePipelineCompletion;
            mGetFilterSetsPipeline.RunRoot(null);
        }

        private void List_SelectionChanged(object sender, EventArgs e)
        {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0)
            {
                FilterSetIDCtl.Text = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

        private void FilterPanel_Load(object sender, EventArgs e)
        {
            // Previously called GetFilterSetList() here
            // Instead, call InitializeFilterSetList once the program is running
        }

        private void UpdateSelectedFilterSetID()
        {
            if (!string.IsNullOrEmpty(mFilterSetIDToAutoSelect))
            {
                // Find the row with the given filter set ID
                foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows)
                {
                    if (item.Cells[0].Value.ToString() == mFilterSetIDToAutoSelect)
                    {
                        item.Selected = true;
                        FilterSetIDCtl.Text = mFilterSetIDToAutoSelect;
                        gridViewDisplayControl1.List.FirstDisplayedCell = item.Cells[0];
                        break;
                    }
                }
            }
        }

        // Methods for handling status updates

        private delegate void VoidFnDelegate();

        /// <summary>
        /// Handle updating filter set id on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
            VoidFnDelegate uf = UpdateSelectedFilterSetID;
            Invoke(uf);
        }
    }
}
