using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageUIComponents
{

    public partial class ColumnMapSelectionForm : Form
    {

        #region Member Variables

        private string mColumnMapToAutoSelect = string.Empty;

        ProcessingPipeline mGetColumnMappingPipeline;

        #endregion

        #region Properties

        public static string MappingConfigFilePath { get; set; }

        /// <summary>
        /// Currently selected column mapping
        /// </summary>
        public string ColumnMapping
        {
            get => ColumnMappingCtl.Text;
            set => ColumnMappingCtl.Text = value;
        }

        /// <summary>
        /// Column map name to auto-select
        /// </summary>
        public string ColumnMapToSelect
        {
            set
            {
                ColumnMappingCtl.Text = value;
                mColumnMapToAutoSelect = ColumnMappingCtl.Text;
            }
        }
        /// <summary>
        /// Return the output column list for the currently selected column mapping
        /// </summary>
        public string OutputColumnList
        {
            get
            {
                var outputColList = "";
                foreach (DataGridViewRow lvi in gridViewDisplayControl1.List.Rows)
                {
                    if (lvi.Cells[0].Value.ToString() == ColumnMapping)
                    {
                        outputColList = lvi.Cells[2].Value.ToString();
                    }
                }
                return outputColList;
            }
        }

        #endregion

        public ColumnMapSelectionForm()
        {
            InitializeComponent();
            ColumnMapping = "(automatic)";
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.List.AllowDelete = false;
        }

        private void ColumnMapSelectionForm_Load(object sender, EventArgs e)
        {
            LoadColumnMappingList();
        }

        /// <summary>
        /// build and run pipeline to get contents of column mapping definition file
        /// and use it to set up file processing panel
        /// </summary>
        private void LoadColumnMappingList()
        {
            var reader = new DelimitedFileReader {FilePath = MappingConfigFilePath};
            var display = gridViewDisplayControl1.MakeSink("Column Mappings", 50);

            mGetColumnMappingPipeline = ProcessingPipeline.Assemble("PipelineToGetColumnMappingConfig", reader, display);
            mGetColumnMappingPipeline.OnRunCompleted += HandlePipelineCompletion;
            mGetColumnMappingPipeline.RunRoot(null);
        }

        private void DisplayControl_SelectionChanged(object sender, EventArgs e)
        {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0)
            {
                ColumnMapping = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }


        private void UpdateSelectedColumnMapping()
        {
            if (!string.IsNullOrEmpty(mColumnMapToAutoSelect))
            {

                var toSelect = new List<DataGridViewRow>(1);

                // Find the row with the given filter set ID
                foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows)
                {
                    if (item.Cells[0].Value.ToString() == mColumnMapToAutoSelect)
                    {
                        item.Selected = true;
                        ColumnMappingCtl.Text = mColumnMapToAutoSelect;
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
        private void HandlePipelineCompletion(object sender, MageStatusEventArgs args)
        {
            // Must use a delegate and Invoke to avoid "cross-thread operation not valid" exceptions
            VoidFnDelegate uf = UpdateSelectedColumnMapping;
            Invoke(uf);
        }

        #endregion

    }
}
