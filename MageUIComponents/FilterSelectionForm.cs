using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mage;
using System.Collections.ObjectModel;

namespace MageUIComponents {

    public partial class FilterSelectionForm : Form {

		#region Member Variables

		string mFilterNameToAutoSelect = string.Empty;

		ProcessingPipeline mGetFilterListPipeline;

		#endregion

        #region Properties

        /// <summary>
        /// Currently selected column mapping
        /// </summary>
        public string FilterName {
            get {
                return FilterNameCtl.Text;
            }
            set {
                FilterNameCtl.Text = value;
            }
        }

		/// <summary>
		/// Filter name to auto-select
		/// </summary>
		public string FilterNameToSelect {
			set {
				FilterNameCtl.Text = value;
				mFilterNameToAutoSelect = FilterNameCtl.Text;
			}
		}

        #endregion

		/// <summary>
		/// Construct a new FilterSelectionForm
		/// </summary>
        public FilterSelectionForm() {
            InitializeComponent();
            FilterName = "All Pass";
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.List.AllowDelete = false;
        }

        private void FilterSelectionForm_Load(object sender, EventArgs e) {
            GetFilterList();
        }

        private void GetFilterList() {
            DataGenerator reader = new DataGenerator();
            reader.AddAdHocRow = new string[] { "Name", "Description" };

            List<string> sortedNames = new List<string>();
            Dictionary<string, string> filterDescriptions = new Dictionary<string, string>();
            Collection<MageAttribute> filters = ModuleDiscovery.Filters;
            foreach (MageAttribute filter in filters) {
                sortedNames.Add(filter.ModLabel);
                filterDescriptions.Add(filter.ModLabel, filter.ModDescription);
            }

            sortedNames.Sort();
            foreach (string label in sortedNames) {
                reader.AddAdHocRow = new string[] { label, filterDescriptions[label] };
            }

            ISinkModule display = gridViewDisplayControl1.MakeSink("Column Mappings", 50);
			mGetFilterListPipeline = ProcessingPipeline.Assemble("PipelineToGetFilterList", reader, display);
			mGetFilterListPipeline.OnRunCompleted += HandlePipelineCompletion;
			mGetFilterListPipeline.RunRoot(null);
        }

        private void DisplayControl_SelectionChanged(object sender, EventArgs e) {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0) {
                FilterName = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

		private void UpdateSelectedFilter() {
			if (!string.IsNullOrEmpty(mFilterNameToAutoSelect)) {

				List<DataGridViewRow> toSelect = new List<DataGridViewRow>(1);

				// Find the row with the given filter set ID
				foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows) {
					if (item.Cells[0].Value.ToString() == mFilterNameToAutoSelect) {
						item.Selected = true;
						FilterNameCtl.Text = mFilterNameToAutoSelect;
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
			VoidFnDelegate uf = UpdateSelectedFilter;
			Invoke(uf);
		}
		
		#endregion
    }
}
