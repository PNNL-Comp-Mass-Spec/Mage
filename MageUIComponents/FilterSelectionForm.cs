using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{
    public partial class FilterSelectionForm : Form
    {
        private string mFilterNameToAutoSelect = string.Empty;

        private ProcessingPipeline mGetFilterListPipeline;

        /// <summary>
        /// Currently selected column mapping
        /// </summary>
        public string FilterName
        {
            get => FilterNameCtl.Text;
            set => FilterNameCtl.Text = value;
        }

        /// <summary>
        /// Filter name to auto-select
        /// </summary>
        public string FilterNameToSelect
        {
            set
            {
                FilterNameCtl.Text = value;
                mFilterNameToAutoSelect = FilterNameCtl.Text;
            }
        }

        /// <summary>
        /// Construct a new FilterSelectionForm
        /// </summary>
        public FilterSelectionForm()
        {
            InitializeComponent();
            FilterName = "All Pass";
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.List.AllowDelete = false;
        }

        private void FilterSelectionForm_Load(object sender, EventArgs e)
        {
            GetFilterList();
        }

        private void GetFilterList()
        {
            var reader = new DataGenerator
            {
                AddAdHocRow = new[] { "Name", "Description" }
            };

            var sortedNames = new List<string>();
            var filterDescriptions = new Dictionary<string, string>();
            var filters = ModuleDiscovery.Filters;
            foreach (var filter in filters)
            {
                sortedNames.Add(filter.ModLabel);
                filterDescriptions.Add(filter.ModLabel, filter.ModDescription);
            }

            sortedNames.Sort();
            foreach (var label in sortedNames)
            {
                reader.AddAdHocRow = new[] { label, filterDescriptions[label] };
            }

            var display = gridViewDisplayControl1.MakeSink("Column Mappings");
            mGetFilterListPipeline = ProcessingPipeline.Assemble("PipelineToGetFilterList", reader, display);
            mGetFilterListPipeline.OnRunCompleted += HandlePipelineCompletion;
            mGetFilterListPipeline.RunRoot(null);
        }

        private void DisplayControl_SelectionChanged(object sender, EventArgs e)
        {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0)
            {
                FilterName = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

        private void UpdateSelectedFilter()
        {
            if (string.IsNullOrEmpty(mFilterNameToAutoSelect))
                return;

            // Find the row with the given filter set ID
            foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows)
            {
                if (item.Cells[0].Value.ToString() == mFilterNameToAutoSelect)
                {
                    item.Selected = true;
                    FilterNameCtl.Text = mFilterNameToAutoSelect;
                    gridViewDisplayControl1.List.FirstDisplayedCell = item.Cells[0];
                    break;
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
            VoidFnDelegate uf = UpdateSelectedFilter;
            Invoke(uf);
        }
    }
}
