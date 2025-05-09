﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;

namespace MageExtContentFilters
{
    [Mage(MageAttribute.FILTER_PANEL_MODULE, "SEQUEST", "SEQUEST Filter", "Parameters for SEQUEST Filter")]
    public partial class SequestFilterPanel : Form, IModuleParameters
    {
        // Ignore Spelling: Mage, Sequest

        private readonly Dictionary<string, string> mParameters = new();

        private string mFilterSetIDToAutoSelect = string.Empty;

        private ProcessingPipeline mGetFiltersPipeline;

        public Dictionary<string, string> GetParameters()
        {
            mParameters["FilterSetID"] = FilterSetIDCtl.Text;
            return mParameters;
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            FilterSetIDCtl.Text = paramList["FilterSetID"];
            mFilterSetIDToAutoSelect = FilterSetIDCtl.Text;
        }

        public SequestFilterPanel()
        {
            InitializeComponent();
            gridViewDisplayControl1.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViewDisplayControl1.MultiSelect = false;
            gridViewDisplayControl1.SelectionChanged += ListDisplayControl1_SelectionChanged;
        }

        private void GetFilterSetList()
        {
            // Create Mage module to query DMS (typically on prismdb2.emsl.pnl.gov)
            // Note that V_PDE_Filter_Sets was deprecated in 2024

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

        private void ListDisplayControl1_SelectionChanged(object sender, EventArgs e)
        {
            if (gridViewDisplayControl1.List.SelectedRows.Count > 0)
            {
                FilterSetIDCtl.Text = gridViewDisplayControl1.List.SelectedRows[0].Cells[0].Value.ToString();
            }
        }

        private void SequestFilterPanel_Load(object sender, EventArgs e)
        {
            GetFilterSetList();
        }

        private void UpdateFilterSetID()
        {
            if (string.IsNullOrEmpty(mFilterSetIDToAutoSelect))
            {
                return;
            }

            // Find the row with the given filter set ID
            foreach (DataGridViewRow item in gridViewDisplayControl1.List.Rows)
            {
                if (item.Cells[0].Value.ToString() != mFilterSetIDToAutoSelect)
                {
                    continue;
                }

                item.Selected = true;
                FilterSetIDCtl.Text = mFilterSetIDToAutoSelect;
                gridViewDisplayControl1.List.FirstDisplayedCell = item.Cells[0];
                break;
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
            VoidFnDelegate uf = UpdateFilterSetID;
            Invoke(uf);
        }
    }
}
