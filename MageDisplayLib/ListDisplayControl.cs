using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    // Callbacks

    /// <summary>
    /// Callback
    /// </summary>
    /// <param name="control"></param>
    /// <param name="active"></param>
    public delegate void ActivationCallback(UserControl control, bool active);

    /// <summary>
    /// Callback
    /// </summary>
    /// <param name="text"></param>
    public delegate void NoticeCallback(string text);

    /// <summary>
    /// <para>
    /// Encapsulates a Windows.Forms.ListView and provides supplementary
    /// functions to handle column definition and sorting.
    /// </para>
    /// <para>
    /// When combined with an LVAccumulator object, it can be populated
    /// from a Mage pipeline.
    /// </para>
    /// <para>
    /// When combined with a LVPipelineSource object, it can supply
    /// data rows to a Mage pipeline.
    /// </para>
    /// <para>
    /// An external module (usually an LVAccumulator objet) provides
    /// a block of column definitions and blocks of row items for the
    /// List view at the heart of this user control.
    /// These blocks are delivered by the external module via events
    /// that are connected to the HandleLVItemBlock and HandleColumnBlock
    /// delegates.  Since the external object is usually running in its
    /// own thread, there are appropriate helper functions to handle the
    /// transfer of received objects to the actual list view control
    /// running in the UI thread.
    /// </para>
    /// </summary>
    public partial class ListDisplayControl : UserControl
    {
        /// <summary>
        /// This event fires to send command to external handler(s)
        /// </summary>
        public event EventHandler<MageCommandEventArgs> OnAction;

        /// <summary>
        /// Signals anyone interested that row selection has changed
        /// </summary>
        public event EventHandler<EventArgs> SelectionChanged;

        #region "Delegate Functions"

        // Callback for accessing fields from worker thread
        private delegate void ColumnBlockCallback(ColumnHeader[] columnBlock);
        private delegate void ItemBlockCallback(ListViewItem[] itemBlock);

        #endregion

        #region "Member Variables"

        /// <summary>
        /// SQL data types
        /// </summary>
        protected enum eSqlDataColType
        {
            /// <summary>
            /// Text
            /// </summary>
            text,
            /// <summary>
            /// Integer
            /// </summary>
            numInt,
            /// <summary>
            /// Float
            /// </summary>
            numFloat,
            /// <summary>
            /// Date
            /// </summary>
            date,
            /// <summary>
            /// Binary
            /// </summary>
            binary
        }

        /// <summary>
        /// Generic data type of each column
        /// </summary>
        private readonly List<eSqlDataColType> mListViewColTypes = new();

        /// <summary>
        /// Convenience array listing column names
        /// </summary>
        private readonly List<string> mListViewColNames = new();

        /// <summary>
        /// Sort info
        /// </summary>
        private int mListViewSortColIndex = -1;
        private bool mListViewSortAscending = true;

        /// <summary>
        /// Cell editor for ListView for this object
        /// </summary>
        private readonly ListViewCellEditor mCellEditor;

        #endregion

        #region Properties

        /// <summary>
        /// Supplemental information about rows in list display (typically number of rows)
        /// </summary>
        public string Notice
        {
            get => lblNotice.Text;
            set => lblNotice.Text = value;
        }

        /// <summary>
        /// Title field in header above list display rows
        /// </summary>
        public string PageTitle
        {
            get => lblPageTitle.Text;
            set => lblPageTitle.Text = value;
        }

        /// <summary>
        /// The accumulator object that supplies blocks of list view items to this object
        /// </summary>
        public LVAccumulator Accumulator { get; set; }

        /// <summary>
        /// Get collection of definitions of columns
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs => new(Accumulator.ColumnDefs);

        /// <summary>
        /// Get collection of columnn names
        /// </summary>
        public Collection<string> ColumnNames
        {
            get
            {
                var names = new Collection<string>();
                foreach (var colDef in Accumulator.ColumnDefs)
                {
                    names.Add(colDef.Name);
                }
                return names;
            }
        }

        /// <summary>
        /// Get the current cell editor for the ListView of this object
        /// </summary>
        public ListViewCellEditor CellEditor => mCellEditor;

        /// <summary>
        /// The list view control used by this object
        /// </summary>
        public ListView List => lvQueryResults;

        /// <summary>
        /// Number of items currently in display
        /// </summary>
        public int ItemCount => lvQueryResults.Items.Count;

        /// <summary>
        /// Number of selected items currently in display
        /// </summary>
        public int SelectedItemCount => lvQueryResults.SelectedItems.Count;

        /// <summary>
        /// Get contents of first selected row as key/value pairs
        /// where key is column name and value is contents of column
        /// </summary>
        public Dictionary<string, string> SelectedItemFields
        {
            get
            {
                var fields = new Dictionary<string, string>();
                if (lvQueryResults.SelectedItems.Count > 0)
                {
                    var item = lvQueryResults.SelectedItems[0];
                    for (var i = 0; i < ColumnDefs.Count; i++)
                    {
                        var colName = ColumnDefs[i].Name;
                        var fieldVal = item.SubItems[i].Text;
                        fields.Add(colName, fieldVal);
                    }
                }
                return fields;
            }
        }

        /// <summary>
        /// Show or hide the header panel
        /// </summary>
        public bool HeaderVisible
        {
            get => panel1.Visible;
            set => panel1.Visible = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct new Mage ListDisplayControl object
        /// </summary>
        public ListDisplayControl()
        {
            InitializeComponent();
            SetupContextMenus(lvQueryResults);
            mCellEditor = new ListViewCellEditor(lvQueryResults);
        }

        #endregion

        #region List Maintenence

        /// <summary>
        /// Remove the currently selected items from the display list
        /// </summary>
        public void DeleteSelectedItems()
        {
            foreach (ListViewItem item in lvQueryResults.Items)
            {
                if (item.Selected)
                {
                    item.Remove();
                }
            }
            lvQueryResults.Update();
            UpdateNoticeFieldWithRowInfo();
        }

        /// <summary>
        /// Remove the items from the display list that are currently not selected
        /// </summary>
        public void DeleteNotSelectedItems()
        {
            foreach (ListViewItem item in lvQueryResults.Items)
            {
                if (!item.Selected)
                {
                    item.Remove();
                }
            }
            lvQueryResults.Update();
            UpdateNoticeFieldWithRowInfo();
        }

        /// <summary>
        /// Empty the current display contents
        /// </summary>
        public void Clear()
        {
            lvQueryResults.Items.Clear();
            lvQueryResults.Columns.Clear();
            lvQueryResults.Update();
            lblNotice.Text = string.Empty;
        }

        /// <summary>
        /// Select all rows in display
        /// </summary>
        public void SelectAllRows()
        {
            lvQueryResults.SuspendLayout();
            foreach (ListViewItem objItem in lvQueryResults.Items)
            {
                objItem.Selected = true;
            }
            lvQueryResults.ResumeLayout();
        }

        #endregion

        #region "LVAccumulator Functions"

        /// <summary>
        /// Create a new LVAccumulator for this user control
        /// and wire up its events to this user control's event handlers
        /// </summary>
        /// <returns></returns>
        public LVAccumulator MakeAccumulator()
        {
            const int BLOCK_SIZE = 100;
            return MakeAccumulator(BLOCK_SIZE);
        }

        /// <summary>
        /// Return an LVAccumulator object that can supply input rows to this object
        /// </summary>
        /// <param name="blksz"></param>
        /// <returns></returns>
        public LVAccumulator MakeAccumulator(int blksz)
        {
            Accumulator = new LVAccumulator
            {
                ItemBlockSize = blksz
            };
            Accumulator.OnItemBlockRetrieved += HandleLVItemBlock;
            Accumulator.OnColumnBlockRetrieved += HandleColumnBlock;
            return Accumulator;
        }

        /// <summary>
        /// This is a delegate that is called with a block of ListView items
        /// (typically from the associated LVAccumulator object)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleLVItemBlock(object sender, ItemBlockEventArgs args)
        {
            NoticeCallback ncb = UpdateNoticeFld;
            var itemBlock = args.GetItemBlock();
            if (itemBlock != null)
            {
                ItemBlockCallback lcb = UpdateListViewItems;
                Invoke(lcb, new object[] { itemBlock });
                Invoke(ncb, ".");
            }
            else
            {
                Invoke(ncb, string.Empty);
            }
        }

        /// <summary>
        /// This is a delegate that is called with definitions for ListView columns
        /// (typically from the associated LVAccumulator object)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnBlock(object sender, ColumnHeaderEventArgs args)
        {
            // lvQueryResults.Columns.AddRange(columnBlock);
            ColumnBlockCallback cb = UpdateListViewColumns;
            Invoke(cb, new object[] { args.GetColumnBlock() });
        }

        /// <summary>
        /// Called as target of invoke operation to
        /// actually update the ListView control
        /// </summary>
        /// <param name="columnBlock"></param>
        private void UpdateListViewColumns(ColumnHeader[] columnBlock)
        {
            lvQueryResults.Columns.AddRange(columnBlock);

            // Determine the data types for each column
            mListViewColTypes.Clear();
            mListViewColNames.Clear();

            // Parse out the data type from the .Tag member of each column
            for (var i = 0; i < lvQueryResults.Columns.Count; i++)
            {
                mListViewColNames.Add(lvQueryResults.Columns[i].Name);

                var colType = lvQueryResults.Columns[i].Tag.ToString();

                switch (colType)
                {
                    case "bit":
                    case "tinyint":
                    case "smallint":
                    case "int":
                    case "bigint":
                        // Integer number
                        mListViewColTypes.Add(eSqlDataColType.numInt);
                        break;

                    case "decimal":
                    case "real":
                    case "float":
                    case "numeric":
                    case "smallmoney":
                    case "money":
                        // Non-integer number
                        mListViewColTypes.Add(eSqlDataColType.numFloat);
                        break;

                    case "char":
                    case "varchar":
                    case "text":
                    case "nchar":
                    case "nvarchar":
                    case "ntext":
                    case "uniqueidentifier":
                    case "xml":
                        // Text-based data type
                        mListViewColTypes.Add(eSqlDataColType.text);
                        break;

                    case "date":
                    case "datetime":
                    case "datetime2":
                    case "smalldatetime":
                    case "time":
                    case "datetimeoffset":
                        // Date data type
                        mListViewColTypes.Add(eSqlDataColType.date);
                        break;

                    case "binary":
                    case "varbinary":
                    case "image":
                        mListViewColTypes.Add(eSqlDataColType.binary);
                        break;

                    default:
                        // Unknown data type
                        // If the data type contains "date" or "time", then treat as datetime
                        if (colType.Contains("date") || colType.Contains("time"))
                        {
                            mListViewColTypes.Add(eSqlDataColType.date);
                        }
                        else
                        {
                            // Assume text
                            mListViewColTypes.Add(eSqlDataColType.text);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Called as target of invoke operation to
        /// actually update the ListView control
        /// </summary>
        /// <param name="itemBlock"></param>
        private void UpdateListViewItems(ListViewItem[] itemBlock)
        {
            var i = lvQueryResults.Items.Count;
            lvQueryResults.Items.AddRange(itemBlock);
            if (i > 0)
                lvQueryResults.Update();
        }

        /// <summary>
        /// Called as target of invoke operation to
        /// actually update the notice filed
        /// </summary>
        /// <param name="text"></param>
        private void UpdateNoticeFld(string text)
        {
            if (text != null && text == ".")
            {
                lblNotice.Text += ".";
                lblNotice.Update();
            }
            else
            {
                var strStatus = lvQueryResults.Items.Count.ToString() + " row";
                if (lvQueryResults.Items.Count != 1)
                    strStatus += "s";

                if (!string.IsNullOrEmpty(text))
                    strStatus = text + "; " + strStatus;

                lblNotice.Text = strStatus;
            }
        }

        #endregion

        #region "Control Handlers"

        /// <summary>
        /// Handler that receives user click on a column header
        /// and then launches a sort operation on that colmun
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvQueryResults_ColumnClicked(object sender, ColumnClickEventArgs e)
        {
            var intColIndex = e.Column;

            var sortNumeric = false;
            var sortDate = false;

            if (mListViewSortColIndex == intColIndex)
            {
                // User clicked the same column; reverse the sort order
                mListViewSortAscending = !mListViewSortAscending;
            }
            else
            {
                // User clicked a new column, change the column sort index
                mListViewSortColIndex = intColIndex;
            }

            switch (mListViewColTypes[intColIndex])
            {
                case eSqlDataColType.numInt:
                case eSqlDataColType.numFloat:
                    sortNumeric = true;
                    break;
                case eSqlDataColType.date:
                    sortDate = true;
                    break;
            }

            // ReSharper disable once NotAccessedVariable
            var sortInfo = "Sort " + lvQueryResults.Columns[intColIndex].Text;

            if (!mListViewSortAscending)
                sortInfo += " desc";

            if (sortNumeric)
                sortInfo += " (numeric)";

            if (sortDate)
                sortInfo += " (date)";

            //--           AddToMessageQueue(strSortInfo, (float)0.1);

            lvQueryResults.ListViewItemSorter = new ListViewItemComparer(e.Column, mListViewSortAscending, sortNumeric, sortDate);

            lvQueryResults.Update();
        }

        /// <summary>
        /// Update notice when number of selected rows changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvQueryResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateNoticeFieldWithRowInfo();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Update notice field with number of rows and selection
        /// </summary>
        private void UpdateNoticeFieldWithRowInfo()
        {
            var s = (SelectedItemCount > 0) ? SelectedItemCount.ToString() + "/" : "";
            var i = ItemCount.ToString();
            var r = (ItemCount == 1) ? " row" : " rows";
            lblNotice.Text = s + i + r;
        }

        #endregion

        #region Pipeline Helper Functions

        /// <summary>
        /// Connect given object to the standard tabular output
        /// of the given module in the given Mage pipeline
        /// </summary>
        /// <param name="title"></param>
        /// <param name="pipeline">Mage pipeline object containing target module</param>
        /// <param name="mod">Module name of module whose standard tabular output we will connect to</param>
        /// <param name="lc">ListDisplayControl object to connect to module</param>
        /// <param name="blkSz">Number of rows to be accumulated into each update block</param>
        public static void ConnectToPipeline(string title, ProcessingPipeline pipeline, string mod, ListDisplayControl lc, int blkSz)
        {
            lc.PageTitle = title;
            ConnectToPipeline(pipeline, mod, lc, blkSz);
        }

        /// <summary>
        /// Connect given object to the standard tabular output
        /// of the given module in the given Mage pipeline
        /// </summary>
        /// <param name="pipeline">Mage pipeline object containing target module</param>
        /// <param name="mod">Module name of module whose standard tabular output we will connect to</param>
        /// <param name="lc">ListDisplayControl object to connect to module</param>
        /// <param name="blkSz">Number of rows to be accumulated into each update block</param>
        public static void ConnectToPipeline(ProcessingPipeline pipeline, string mod, ListDisplayControl lc, int blkSz)
        {
            // Connect list display control to pipeline via an accumulator object
            lc.Clear();
            var lva = lc.MakeAccumulator();
            pipeline.ConnectExternalModule(mod, lva.HandleColumnDef, lva.HandleDataRow);
            lva.ItemBlockSize = blkSz;
            // lc.PageTitle = mod;
        }

        /// <summary>
        /// Return an ISinkModule reference that
        /// Mage pipeline can use to populate this control
        /// </summary>
        /// <returns></returns>
        public ISinkModule MakeSink()
        {
            const int BLOCK_SIZE = 10;
            return MakeSink(BLOCK_SIZE);
        }

        /// <summary>
        /// Return an ISinkModule reference that
        /// Mage pipeline can use to populate this control
        /// </summary>
        /// <param name="title"></param>
        /// <param name="blkSz"></param>
        /// <returns></returns>
        public ISinkModule MakeSink(string title, int blkSz)
        {
            PageTitle = title;
            return MakeSink(blkSz);
        }

        /// <summary>
        /// Return an ISinkModule reference that
        /// Mage pipeline can use to populate this control
        /// </summary>
        /// <param name="blkSz"></param>
        /// <returns></returns>
        public ISinkModule MakeSink(int blkSz)
        {
            Clear();
            var lva = MakeAccumulator();
            lva.ItemBlockSize = blkSz;
            return lva;
        }

        #endregion

        #region Move Items

        /// <summary>
        /// Move the first currently selected item up or down in the list
        /// </summary>
        /// <param name="moveUp"></param>
        public void MoveListViewItem(bool moveUp)
        {
            string cache;
            ListView lv = lvQueryResults;

            var selIdx = lv.SelectedItems[0].Index;
            if (moveUp)
            {
                // Ignore moveup of row(0)
                if (selIdx == 0)
                    return;

                // Move the subitems for the previous row
                // to cache to make room for the selected row
                for (var i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx - 1].SubItems[i].Text;
                    lv.Items[selIdx - 1].SubItems[i].Text =
                      lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx - 1].Selected = true;
                lv.Items[selIdx].Selected = false;

                lv.Refresh();
                lv.Focus();
            }
            else
            {
                // Ignore movedown of last item
                if (selIdx == lv.Items.Count - 1)
                    return;

                // Move the subitems for the next row
                // to cache so we can move the selected row down
                for (var i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx + 1].SubItems[i].Text;
                    lv.Items[selIdx + 1].SubItems[i].Text =
                      lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx + 1].Selected = true;
                lv.Items[selIdx].Selected = false;
                lv.Refresh();
                lv.Focus();
            }
        }

        #endregion

    }
}
