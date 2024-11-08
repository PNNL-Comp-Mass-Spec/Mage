﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageUIComponents
{
    /// <summary>
    /// Dialog that allows column mappings to be created, edited, and saved
    /// </summary>
    public partial class ColumnMappingForm : Form
    {
        // Ignore Spelling: datetime, Mage, smallint, varchar

        /// <summary>
        /// Definition of columns that was read from input file
        /// </summary>
        private Collection<MageColumnDef> mInputColumnDefs = new();

        private bool mAddingMapping;

        /// <summary>
        /// Path to the SQLite configuration database
        /// that provides persistent storage for column mapping definitions
        /// </summary>
        public static string MappingConfigFilePath { get; set; }

        /// <summary>
        /// Information from selected row in file list.
        /// Contains file and folder fields that point
        /// to file that supplies input column information.
        /// </summary>
        /// <remarks>
        /// Set of key/value parameters
        /// </remarks>
        public Dictionary<string, string> InputFileInfo { get; set; } = new();

        /// <summary>
        /// Information about output file or database
        /// </summary>
        /// <remarks>
        /// Set of key/value parameters
        /// </remarks>
        public Dictionary<string, string> OutputInfo { get; set; } = new();

        /// <summary>
        /// Editing field for the column mapping name
        /// in the col spec editing panel
        /// </summary>
        public string MappingName
        {
            get => MappingNameCtl.Text;
            set => MappingNameCtl.Text = value;
        }

        /// <summary>
        /// Editing field for the column mapping description
        /// in the col spec editing panel
        /// </summary>
        public string MappingDescription
        {
            get => MappingDescriptionCtl.Text;
            set => MappingDescriptionCtl.Text = value;
        }

        /// <summary>
        /// Name of selected column mapping
        /// </summary>
        public string ColumnMapping
        {
            get
            {
                var selectedColMapping = ColumnMappingDisplayList.SelectedItemFields;
                if (selectedColMapping?.TryGetValue("name", out var columnMap) is true)
                {
                    return columnMap;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Output column specification (in string format) for selected column mapping
        /// </summary>
        public string OutputColumnList
        {
            get
            {
                var selectedColMapping = ColumnMappingDisplayList.SelectedItemFields;
                if (selectedColMapping?.TryGetValue("column_list", out var columnList) is true)
                {
                    return columnList;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Number of rows to show in row preview
        /// </summary>
        public int PreviewRowLimit { get; }

        public ColumnMappingForm()
        {
            InitializeComponent();
            PreviewRowLimit = 200;
        }

        /// <summary>
        /// Initialization actions after form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnMappingForm_Load(object sender, EventArgs e)
        {
            SetupEventHandlers();
            SetupGridDisplayListBehavior();
            LoadColumnMappingList();
            LoadEditingPanel();
        }

        /// <summary>
        /// How we want grid view display list to auto size themselves
        /// </summary>
        private void SetupGridDisplayListBehavior()
        {
            ColumnMappingDisplayList.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            ColumnMappingDisplayList.MultiSelect = false;

            ColumnSpecEditingDisplayList.List.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ColumnSpecEditingDisplayList.List.ReadOnly = false;
            ColumnSpecEditingDisplayList.MultiSelect = true;
            ColumnSpecEditingDisplayList.AllowDisableShiftClickMode = false;

            ColumnSpecEditingDisplayList.List.DataError += HandleColumnSpecEditingListDataError;

            RowPreviewDisplayList.MultiSelect = true;
            RowPreviewDisplayList.AllowDisableShiftClickMode = false;
            RowPreviewDisplayList.List.AllowDelete = false;
        }

        /// <summary>
        /// Connect events on the associated display list to our handlers
        /// </summary>
        private void SetupEventHandlers()
        {
            ColumnMappingDisplayList.SelectionChanged += DisplaySelectedColumnMappingInEditingPanel;
        }

        /// <summary>
        /// Setup initial values in editing panel
        /// </summary>
        private void LoadEditingPanel()
        {
            MappingName = "New Column Mapping";
            MappingDescription = "Describe purpose";
            DisplayColumnListInEditingPanel("", false);
        }

        // Column Mapping List Methods

        /// <summary>
        /// Use Mage pipeline to get contents of column mapping definition file
        /// and use it to populate column mapping display panel
        /// </summary>
        private void LoadColumnMappingList()
        {
            if (!File.Exists(MappingConfigFilePath))
            {
                // Need to create config file
                CreateDefaultColumnMappingConfigFile();
            }
            var reader = new DelimitedFileReader { FilePath = MappingConfigFilePath };
            var display = ColumnMappingDisplayList.MakeSink("ColumnMap");
            var pipeline = ProcessingPipeline.Assemble("PipelineToGetColumnMappingConfig", reader, display);
            pipeline.RunRoot(null);
        }

        /// <summary>
        /// Make a default column mapping config file if one does not exist
        /// </summary>
        private static void CreateDefaultColumnMappingConfigFile()
        {
            var dGen = new DataGenerator
            {
                AddAdHocRow = ["name", "description", "column_list"]
            };

            dGen.AddAdHocRow = ["Add Job Column", "Add new column that will contain Job number", "Job|+|text, *"];

            var writer = new DelimitedFileWriter { FilePath = MappingConfigFilePath };
            var pipeline2 = ProcessingPipeline.Assemble("CreateColumnMapping", dGen, writer);
            pipeline2.RunRoot(null);
        }

        /// <summary>
        /// Save the contents of the column mapping display list
        /// to the config file
        /// </summary>
        private void SaveColumnMappingList()
        {
            IBaseModule source = new GVPipelineSource(ColumnMappingDisplayList, DisplaySourceMode.All);
            var writer = new DelimitedFileWriter { FilePath = MappingConfigFilePath };
            var pipeline = ProcessingPipeline.Assemble("SaveColumnMapping", source, writer);
            pipeline.RunRoot(null);
            UnsavedChanges(false);
        }

        /// <summary>
        /// Button click to save column mapping list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveColumnMappingsBtn_Click(object sender, EventArgs e)
        {
            SaveColumnMappingList();
        }

        /// <summary>
        /// Delete the selected column mappings from the column mapping display list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColumnMappingBtn_Click(object sender, EventArgs e)
        {
            ColumnMappingDisplayList.DeleteSelectedItems();
            UnsavedChanges(true);
        }

        /// <summary>
        /// Copy information from the currently selected column mapping
        /// in the column mapping display list to the editing panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplaySelectedColumnMappingInEditingPanel(object sender, EventArgs e)
        {
            var fields = ColumnMappingDisplayList.SelectedItemFields;
            if (fields.Count == 0)
                return;

            MappingName = fields["name"];
            MappingDescription = fields["description"];
            var colSpecs = fields["column_list"];

            DisplayColumnListInEditingPanel(colSpecs, false);
        }

        /// <summary>
        /// Check given name against existing column mappings in list,
        /// and return indication of whether it is unique
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if name is not in list</returns>
        private bool IsColumnMappingNameUnique(string name)
        {
            var isUnique = true;
            foreach (DataGridViewRow row in ColumnMappingDisplayList.List.Rows)
            {
                if (row.Cells[0].Value.ToString() == name)
                {
                    isUnique = false;
                    break;
                }
            }
            return isUnique;
        }

        // Conversion Of Column List Between String And Table Formats

        /// <summary>
        /// Positions of fields in column spec
        /// </summary>
        private const int OutputColIdx = 0;
        private const int InputColIdx = 1;
        private const int DataTypeColIdx = 2;
        private const int SizeColIdx = 3;

        /// <summary>
        /// Roll the individual column specs in collection of field arrays
        /// int a column list in column mapping format
        /// </summary>
        /// <param name="colItems"></param>
        private static string GetColumnListFromColumnSpecItems(IEnumerable<string[]> colItems)
        {
            var specs = new List<string>();
            foreach (var colFields in colItems)
            {
                var fields = new List<string> { colFields[OutputColIdx] };

                // Always have an output column name

                // Input column name may be blank if same as output column name
                var inputCol = (colFields[InputColIdx] != colFields[OutputColIdx]) ? colFields[InputColIdx] : "";

                // Add input column name if present, or if placeholder needed
                if (!string.IsNullOrEmpty(inputCol) || !string.IsNullOrEmpty(colFields[DataTypeColIdx]))
                {
                    fields.Add(inputCol);
                }

                // Add data type to spec if present
                if (!string.IsNullOrEmpty(colFields[DataTypeColIdx]))
                {
                    fields.Add(colFields[DataTypeColIdx]);
                }

                // Add data size to spec if present
                if (!string.IsNullOrEmpty(colFields[SizeColIdx]))
                {
                    fields.Add(colFields[SizeColIdx]);
                }
                // Roll this column spec's fields up to delimited string
                specs.Add(string.Join("|", fields));
            }
            // Roll column specs up to delimited string
            return string.Join(",", specs);
        }

        /// <summary>
        /// Parse the column list in string format into list of separate column specs
        /// </summary>
        /// <param name="colList">column list in string format</param>
        /// <returns>List of column specs</returns>
        private static Collection<string[]> GetColumnSpecItemsFromColumnList(string colList)
        {
            var rows = new Collection<string[]>();
            if (!string.IsNullOrEmpty(colList))
            {
                foreach (var colSpec in colList.Split(','))
                {
                    var specFields = colSpec.Trim().Split('|');
                    var row = new[] { "", "", "", "" };
                    for (var i = 0; i < specFields.Length; i++)
                    {
                        row[i] = specFields[i];
                    }
                    // if (string.IsNullOrEmpty(row[1]) && row[0] != "*") {
                    //    row[1] = row[0];
                    //}
                    rows.Add(row);
                }
            }
            return rows;
        }

        // Editing Panel Methods

        private void DisplayColumnListInEditingPanel(IEnumerable<MageColumnDef> colDefs)
        {
            var colItems = new Collection<string[]>();
            foreach (var colDef in colDefs)
            {
                colItems.Add([colDef.Name, "", colDef.DataType, colDef.Size]);
            }
            var colSpecs = GetColumnListFromColumnSpecItems(colItems);
            DisplayColumnListInEditingPanel(colSpecs, true);
        }

        /// <summary>
        /// Parse the column list into separate column specs
        /// and use Mage pipeline to
        /// load into the column spec display list in the editing panel
        /// </summary>
        /// <param name="colList"></param>
        /// <param name="useInputColPicker"></param>
        private void DisplayColumnListInEditingPanel(string colList, bool useInputColPicker)
        {
            ColumnSpecEditingDisplayList.List.Rows.Clear();
            ColumnSpecEditingDisplayList.List.Columns.Clear();

            mAddingMapping = true;

            AddEditingColumnForOutputColumnName();
            AddEditingColumnForInputColumnName(useInputColPicker);
            AddEditingColumnForDataType();
            AddEditingColumnForDataSize();

            // Parse the column list string into fields
            // and add them to pipeline source module
            var rows = GetColumnSpecItemsFromColumnList(colList);
            AddMissingInputColumns(rows, mInputColumnDefs);
            foreach (var row in rows)
            {
                // Auto-fix data type int to integer
                if (row.Length > 3 && string.Equals(row[2], "int", StringComparison.OrdinalIgnoreCase))
                    row[2] = "integer";

                ColumnSpecEditingDisplayList.List.Rows.Add((object)row);
            }

            mAddingMapping = false;
        }

        private void AddEditingColumnForOutputColumnName()
        {
            var col1 = new DataGridViewTextBoxColumn { Name = "Output Column" };
            ColumnSpecEditingDisplayList.List.Columns.Add(col1);
        }

        private void AddEditingColumnForInputColumnName(bool useInputColPicker)
        {
            if (mInputColumnDefs.Count > 0 && useInputColPicker)
            {
                var col2 = new DataGridViewComboBoxColumn
                {
                    Name = "Input Column",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                    FlatStyle = FlatStyle.Popup
                };
                col2.Items.AddRange("", "+");
                foreach (var colDef in mInputColumnDefs)
                {
                    col2.Items.Add(colDef.Name);
                }
                ColumnSpecEditingDisplayList.List.Columns.Add(col2);
            }
            else
            {
                var col2 = new DataGridViewTextBoxColumn { Name = "Input Column" };
                ColumnSpecEditingDisplayList.List.Columns.Add(col2);
            }
        }

        private void AddEditingColumnForDataType()
        {
            var col3 = new DataGridViewComboBoxColumn
            {
                Name = "Data Type",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox,
                FlatStyle = FlatStyle.Popup
            };
            col3.Items.AddRange("", "text", "integer", "smallint", "double", "real", "float", "char", "datetime");
            ColumnSpecEditingDisplayList.List.Columns.Add(col3);
        }

        private void AddEditingColumnForDataSize()
        {
            var col4 = new DataGridViewTextBoxColumn { Name = "Data Size" };
            ColumnSpecEditingDisplayList.List.Columns.Add(col4);
        }

        /// <summary>
        /// For each input column field that is blank (and not in a wildcard row),
        /// try to find a match in input column list and set it if found
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="colDefs"></param>
        private static void AddMissingInputColumns(IEnumerable<string[]> rows, Collection<MageColumnDef> colDefs)
        {
            if (colDefs == null)
            {
                throw new ArgumentNullException(nameof(colDefs));
            }

            // Make list of input column names
            var inputColList = new List<string>();
            foreach (var colDef in colDefs)
            {
                inputColList.Add(colDef.Name);
            }

            // Try to match up missing input columns
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row[1]) && row[0] != "*")
                {
                    if (inputColList.Contains(row[0]))
                    {
                        row[1] = row[0];
                    }
                }
            }
        }

        /// <summary>
        /// Roll the individual column specs in the column spec editing display list
        /// int a column list in column mapping format
        /// </summary>
        private string GetColListFromEditingPanel()
        {
            var colItems = GetColumnSpecItemsFromEditingPanel();
            return GetColumnListFromColumnSpecItems(colItems);
        }

        /// <summary>
        /// Get the individual column specs in the column spec editing display list
        /// as a collection of field arrays
        /// </summary>
        private Collection<string[]> GetColumnSpecItemsFromEditingPanel()
        {
            var colItems = new Collection<string[]>();
            foreach (DataGridViewRow lvi in ColumnSpecEditingDisplayList.List.Rows)
            {
                var colItem = new[] { "", "", "", "" };
                for (var i = 0; i < 4; i++)
                {
                    if (lvi.Cells[i].Value != null)
                    {
                        colItem[i] = lvi.Cells[i].Value.ToString();
                    }
                }
                colItems.Add(colItem);
            }
            return colItems;
        }

        /// <summary>
        /// Roll up the column specs in the column spec editing panel
        /// into a column mapping and add it to the column mapping display list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewColumnMappingBtn_Click(object sender, EventArgs e)
        {
            if (!IsColumnMappingNameUnique(MappingName))
            {
                MessageBox.Show("There is already a column mapping with the same name");
                return;
            }
            var data = new[] { MappingName, MappingDescription, GetColListFromEditingPanel(), "", "" };
            var args = new MageDataEventArgs(data);
            ColumnMappingDisplayList.HandleDataRow(this, args);
            ColumnMappingDisplayList.HandleDataRow(this, new MageDataEventArgs(null));
            UnsavedChanges(true);
        }

        /// <summary>
        /// Roll up the column specs in the column spec editing panel
        /// into a column mapping and replace the currently selected
        /// column mapping display list item with it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceExistingColumnMappingBtn_Click(object sender, EventArgs e)
        {
            if (ColumnMappingDisplayList.SelectedItemCount > 0)
            {
                var lvi = ColumnMappingDisplayList.List.SelectedRows[0];
                lvi.Cells[0].Value = MappingName;
                lvi.Cells[1].Value = MappingDescription;
                lvi.Cells[2].Value = GetColListFromEditingPanel();
                UnsavedChanges(true);
            }
        }

        /// <summary>
        /// Delete the selected column spec from the column spec display list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteColumnSpecBtn_Click(object sender, EventArgs e)
        {
            ColumnSpecEditingDisplayList.DeleteSelectedItems();
        }

        /// <summary>
        /// Clear all the fields in the column spec editing panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearEditingPanelBtn_Click(object sender, EventArgs e)
        {
            var r = MessageBox.Show("Are you sure you want to clear the current column mapping?", "Confirm clear", MessageBoxButtons.OKCancel);
            if (r == DialogResult.OK)
            {
                MappingName = "New Column Map";
                MappingDescription = string.Empty;
                ColumnSpecEditingDisplayList.List.Rows.Clear();
            }
        }

        /// <summary>
        /// Add new column row to column list in editing panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewColumnBtn_Click(object sender, EventArgs e)
        {
            var lvi = GetDefaultNewColumn();
            ColumnSpecEditingDisplayList.List.Rows.Add((object)lvi);
        }

        /// <summary>
        /// Insert new column row to column list ahead of current selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertColumnBtn_Click(object sender, EventArgs e)
        {
            var lvi = GetDefaultNewColumn();
            var idx = 0;
            if (ColumnSpecEditingDisplayList.SelectedItemCount > 0)
            {
                idx = ColumnSpecEditingDisplayList.List.SelectedRows[0].Index;
            }
            ColumnSpecEditingDisplayList.List.Rows.Insert(idx, (object)lvi);
        }

        /// <summary>
        /// Get a new column row to insert
        /// </summary>
        private static string[] GetDefaultNewColumn()
        {
            var newColData = new[] { "New_Column", "", "text", "" };
            return newColData;
        }

        private void MoveColSpecItemUpBtn_Click(object sender, EventArgs e)
        {
            ColumnSpecEditingDisplayList.MoveListItem(true);
        }

        private void MoveColSpecItemDownBtn_Click(object sender, EventArgs e)
        {
            ColumnSpecEditingDisplayList.MoveListItem(false);
        }

        /// <summary>
        /// Enable/disable controls depending on unsaved status
        /// </summary>
        /// <param name="dirty"></param>
        private void UnsavedChanges(bool dirty)
        {
            SaveColumnMappingsBtn.Enabled = dirty;
        }

        /// <summary>
        /// Are there unsaved changes to the column mapping
        /// </summary>
        private bool UnsavedChanges()
        {
            return SaveColumnMappingsBtn.Enabled;
        }

        /// <summary>
        /// Intercept form closing process and give user change to cancel
        /// if there are unsaved changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnMappingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UnsavedChanges())
            {
                var r = MessageBox.Show("There are unsaved changes - are you sure you want to close", "Confirm close", MessageBoxButtons.OKCancel);
                if (r != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        private void HandleColumnSpecEditingListDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string sAction;

            if (mAddingMapping)
                sAction = "Error adding mapping";
            else
                sAction = "Data grid error";

            MessageBox.Show(sAction + ", row " + (e.RowIndex + 1) + ", column " + (e.ColumnIndex + 1) + ": " + e.Exception.Message);
        }

        // Column Cleanup Methods

        /// <summary>
        /// Clean off superfluous data size values for data types that don't need them
        /// </summary>
        /// <param name="colDefs"></param>
        private static void NormalizeColumnSize(IEnumerable<MageColumnDef> colDefs)
        {
            foreach (var colDef in colDefs)
            {
                switch (colDef.DataType)
                {
                    case "text":
                    case "int":
                    case "float":
                        colDef.Size = string.Empty;
                        break;
                }
            }
        }

        private BaseModule GetReaderForInputPreview()
        {
            BaseModule rdr = null;
            if (InputFileInfo.ContainsKey("Name") && InputFileInfo.TryGetValue("Folder", out var value))
            {
                rdr = new DelimitedFileReader
                {
                    FilePath = Path.Combine(value, InputFileInfo["Name"])
                };
                mPreviewSourceLabel = Path.GetFileName(InputFileInfo["Name"]);
            }
            if (rdr == null)
            {
                throw new MageException("No input file was selected");
            }
            return rdr;
        }

        private string mPreviewSourceLabel = string.Empty;

        /// <summary>
        /// Return a Mage reader module that is set up
        /// to read preview of selected file processing output file/database
        /// </summary>
        private BaseModule GetReaderForOutputPreview()
        {
            BaseModule rdr;
            if (OutputInfo.ContainsKey("OutputDirectory") && OutputInfo.ContainsKey("OutputFile"))
            {
                var reader = new DelimitedFileReader
                {
                    FilePath = Path.Combine(OutputInfo["OutputDirectory"], OutputInfo["OutputFile"])
                };
                rdr = reader;
                mPreviewSourceLabel = Path.GetFileName(OutputInfo["OutputFile"]);
            }
            else if (OutputInfo.ContainsKey("OutputFolder") && OutputInfo.ContainsKey("OutputFile"))
            {
                var reader = new DelimitedFileReader
                {
                    FilePath = Path.Combine(OutputInfo["OutputFolder"], OutputInfo["OutputFile"])
                };
                rdr = reader;
                mPreviewSourceLabel = Path.GetFileName(OutputInfo["OutputFile"]);
            }
            else if (OutputInfo.ContainsKey("DatabaseName"))
            {
                var reader = new SQLiteReader
                {
                    Database = OutputInfo["DatabaseName"],
                    SQLText = string.Format("SELECT * FROM \"{0}\";", OutputInfo["TableName"])
                };
                rdr = reader;
                mPreviewSourceLabel = Path.GetFileName(OutputInfo["DatabaseName"]) + "/" + OutputInfo["TableName"];
            }
            else
            {
                throw new MageException("No output file or database was specified");
            }
            return rdr;
        }

        /// <summary>
        /// Return a Mage SimpleSink object that is populated
        /// with preview results by a Mage pipeline using the previously set up Mage reader module
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="numRows"></param>
        private static SimpleSink GetPreviewFromSource(BaseModule reader, int numRows)
        {
            var sink = new SimpleSink { RowsToSave = numRows };

            var pipeline = ProcessingPipeline.Assemble("GetFileColumns", reader, sink);
            pipeline.RunRoot(null);

            return sink;
        }

        /// <summary>
        /// Return a Mage SimpleSink module populated with
        /// preview from file processing input target
        /// </summary>
        private SimpleSink GetInputPreview(int numRows)
        {
            var reader = GetReaderForInputPreview();
            var sink = GetPreviewFromSource(reader, numRows);
            ImputeColumnTypes(sink);
            InitializeInputColumnDefs(sink.Columns);
            return sink;
        }

        private static void ImputeColumnTypes(SimpleSink sink)
        {
            // 1) If it has letters, it is text/varchar
            // 2) If it has a decimal point, it's a float
            // 3) Otherwise, it's an int

            for (var i = 0; i < sink.Columns.Count; i++)
            {
                var intType = true;
                var floatType = true;
                foreach (var row in sink.Rows)
                {
                    if (!int.TryParse(row[i], out _))
                    {
                        intType = false;
                    }
                    if (!float.TryParse(row[i], out _))
                    {
                        floatType = false;
                    }
                }

                sink.Columns[i].Size = string.Empty;
                if (intType)
                {
                    sink.Columns[i].DataType = "integer";
                }
                else if (floatType)
                {
                    sink.Columns[i].DataType = "float";
                }
                else
                {
                    sink.Columns[i].DataType = "text";
                }
            }
        }

        /// <summary>
        /// Return a Mage SimpleSink module populated with
        /// preview from file processing output target
        /// </summary>
        private SimpleSink GetOutputPreview(int numRows)
        {
            var reader = GetReaderForOutputPreview();
            var sink = GetPreviewFromSource(reader, numRows);
            return sink;
        }

        /// <summary>
        /// Setup input column stuff from given set of column definitions
        /// </summary>
        /// <param name="colDefs"></param>
        private void InitializeInputColumnDefs(Collection<MageColumnDef> colDefs)
        {
            NormalizeColumnSize(colDefs);
            mInputColumnDefs = colDefs;
        }

        /// <summary>
        /// Display the data contained in the given Mage SimpleSink module
        /// in the preview display list
        /// </summary>
        /// <param name="sink"></param>
        private void DisplayPreviewRows(BaseModule sink)
        {
            var display = RowPreviewDisplayList.MakeSink("DisplayPreview");
            var pipeline = ProcessingPipeline.Assemble("DisplayPreview", sink, display);
            pipeline.RunRoot(null);
            RowPreviewDisplayList.PageTitle = "Preview of " + mPreviewSourceLabel;
        }

        private void LoadColumnListFromInput()
        {
            try
            {
                var sink = GetInputPreview(PreviewRowLimit);
                DisplayColumnListInEditingPanel(mInputColumnDefs);
                DisplayPreviewRows(sink);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LoadColumnListFromOutput()
        {
            try
            {
                GetInputPreview(2);
                var sink = GetOutputPreview(PreviewRowLimit);
                var colDefs = sink.Columns;
                NormalizeColumnSize(colDefs);
                DisplayColumnListInEditingPanel(colDefs);
                DisplayPreviewRows(sink);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void LoadColumnListFromInputBtn_Click(object sender, EventArgs e)
        {
            LoadColumnListFromInput();
        }

        private void LoadColumnListFromOutputBtn_Click(object sender, EventArgs e)
        {
            LoadColumnListFromOutput();
        }
    }
}
