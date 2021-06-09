using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;

namespace MageExtContentFilters
{
    [MageAttribute("FilterPanel", "XTFilter", "XT filter", "Uses filter criteria defined in DMS")]
    public partial class XTFilterPanel : Form, IModuleParameters
    {
        // Ignore Spelling: Mage

        #region Member Variables

        private Dictionary<string, string> mParameters = new();

        private string mFilterSetIDToAutoSelect = string.Empty;

        private ProcessingPipeline mGetFiltersPipeline;

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            mParameters["FilterSetID"] = FilterSetIDCtl.Text;
            return mParameters;
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            FilterSetIDCtl.Text = paramList["FilterSetID"];
            mFilterSetIDToAutoSelect = FilterSetIDCtl.Text;
        }

        #endregion

        public XTFilterPanel() {
            InitializeComponent();
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.SelectionChanged += new System.EventHandler<System.EventArgs>(this.listDisplayControl1_SelectionChanged);
        }

        private void GetFilterSetList() {
            // Create Mage module to query DMS (typically on gigasax)
            var reader = new SQLReader
            {
                Database = Globals.DMSDatabase,
                Server = Globals.DMSServer,
                SQLText = "SELECT Filter_Set_ID, Name, Description FROM V_PDE_Filter_Sets"
            };

            // Create Mage module to receive query results
            var filters = gridViewDisplayControl1.MakeSink("Filter Sets");

            // Build pipeline and run it
            mGetFiltersPipeline = ProcessingPipeline.Assemble("GetFilters", reader, filters);
            mGetFiltersPipeline.OnRunCompleted += HandlePipelineCompletion;
            mGetFiltersPipeline.RunRoot(null);
        }

        private void listDisplayControl1_SelectionChanged(object sender, EventArgs e) {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0) {
                FilterSetIDCtl.Text = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

        private void XTFilterPanel_Load(object sender, EventArgs e) {
            GetFilterSetList();
        }

        private void UpdateFilterSetID() {
            if (!string.IsNullOrEmpty(mFilterSetIDToAutoSelect)) {
                var toSelect = new List<DataGridViewRow>(1);

                // Find the row with the given filter set ID
                foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows) {
                    if (item.Cells[0].Value.ToString() == mFilterSetIDToAutoSelect) {
                        item.Selected = true;
                        FilterSetIDCtl.Text = mFilterSetIDToAutoSelect;
                        gridViewDisplayControl1.List.FirstDisplayedCell = item.Cells[0];
                        break;
                    }
                }
            }
        }

        #region Functions for handling status updates

        private delegate void VoidFnDelegate();

        /// <summary>
        /// Handle updating filter set id on completion of running pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args) {
            // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
            VoidFnDelegate uf = UpdateFilterSetID;
            Invoke(uf);
        }

        #endregion

    }
}
