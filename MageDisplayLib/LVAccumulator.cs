using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mage;
using System.Collections.ObjectModel;

namespace MageDisplayLib
{

    /// <summary>
    /// Provides column definition and data row event handlers
    /// that can be wired into a Mage pipeline to receive data.
    /// 
    /// It converts data rows into ListView item lists suitable
    /// for populating a ListDisplayControl and accumlates them
    /// into a buffer.  When the buffer is full, it is emptied
    /// into the associated ListDisplayControl.
    /// </summary>
    public class LVAccumulator : ISinkModule
    {

        #region Events for ListDisplay listeners to register for

        /// <summary>
        /// event that is fired to pass on column definitions to our associated control
        /// </summary>
        public event EventHandler<ColumnHeaderEventArgs> OnColumnBlockRetrieved;

        /// <summary>
        /// event that is fired to pass on a block of display rows to our associated control
        /// </summary>
        public event EventHandler<ItemBlockEventArgs> OnItemBlockRetrieved;

        #endregion

        #region Member Variables

        private List<ListViewItem> itemAccumulator = new List<ListViewItem>();
        private List<ColumnHeader> columnAccumulator = new List<ColumnHeader>();
        private List<MageColumnDef> mColumnDefs = new List<MageColumnDef>();

        #endregion

        #region Properties

        /// <summary>
        /// number of data rows in an item block
        /// </summary>
        public int ItemBlockSize { get; set; }

        /// <summary>
        /// definition of columns
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs
        {
            get
            {
                return new Collection<MageColumnDef>(mColumnDefs);
            }
        }

        /// <summary>
        /// Get the column definitions
        /// </summary>
        public Collection<MageColumnDef> Columns { get { return new Collection<MageColumnDef>(mColumnDefs); } }

        #endregion

        #region Constructors

        /// <summary>
        /// number of items to accumulate before firing an upate event
        /// </summary>
        public LVAccumulator()
        {
            ItemBlockSize = 1000;
        }

        #endregion

        #region Utility functions

        /// <summary>
        /// clear any accumulate row and column information
        /// </summary>
        public void Clear()
        {
            itemAccumulator.Clear();
            columnAccumulator.Clear();
        }

        #endregion

        #region Handlers for ISinkModule

        /// <summary>
        /// Receive data row, convert to ListView item, and add to accumulator.
        /// 
        /// This event handler receives row events from upstream module, one event per row.  
        /// on the module's standard tabular input, one event per row
        /// and a null vals object signalling the end of row events.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            bool endOfData = !args.DataAvailable;
            if (args.DataAvailable)
            {
                ListViewItem lvi = null;
                for (int i = 0; i < args.Fields.Length; i++)
                {
                    object val = args.Fields[i];
                    string s = (val != null) ? val.ToString() : "-";
                    if (i == 0)
                    {
                        lvi = new ListViewItem(val.ToString());
                    }
                    else
                    {
                        lvi.SubItems.Add(s);
                    }
                }
                this.itemAccumulator.Add(lvi);
            }
            if (itemAccumulator.Count == this.ItemBlockSize || endOfData)
            {
                if (OnItemBlockRetrieved != null)
                {
                    OnItemBlockRetrieved(this, new ItemBlockEventArgs(new Collection<ListViewItem>(itemAccumulator)));
                }
                this.itemAccumulator.Clear();
            }
            if (endOfData && OnItemBlockRetrieved != null)
            {
                OnItemBlockRetrieved(this, new ItemBlockEventArgs(null));
            }
        }

        /// <summary>
        /// Build up list of ColumnHeader for list view.
        /// 
        /// This event handler receives column definition events 
        /// on the module's standard tabular input, one event per column
        /// and a null columnDef object signalling the end of column definition events.
        /// Subclasses should override this for any specialized column def handling
        /// that they need, but should be sure to also call the base class function. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mColumnDefs = new List<MageColumnDef>(args.ColumnDefs);

            foreach (MageColumnDef columnDef in mColumnDefs)
            {
                ColumnHeader ch = new ColumnHeader();
                // sort out column display size
                string colSize = columnDef.Size;
                int colSizeToUse = 6;
                if (colSize != null && colSize.Length > 0)
                {
                    int w = int.Parse(colSize);
                    w = (w < 6) ? 6 : w;
                    w = (w > 20) ? 20 : w;
                    colSizeToUse = w;
                }

                int pixels = colSizeToUse * 10;
                ch.Text = columnDef.Name;
                ch.Name = columnDef.Name;
                ch.Width = pixels;
                string colType = columnDef.DataType;
                ch.Tag = colType;
                columnAccumulator.Add(ch);
            }
            if (this.OnColumnBlockRetrieved != null)
            {
                OnColumnBlockRetrieved(this, new ColumnHeaderEventArgs(new Collection<ColumnHeader>(columnAccumulator)));
            }
        }
        #endregion
    }

    #region Event argument classes

    /// <summary>
    /// Argument class for event that passes block of items to list view control
    /// </summary>
    public class ItemBlockEventArgs : EventArgs
    {
        private ListViewItem[] ItemBlock = null;

        /// <summary>
        /// get list of items being passed in the event
        /// </summary>
        /// <returns></returns>
        public ListViewItem[] GetItemBlock()
        {
            return ItemBlock;
        }

        /// <summary>
        /// construct new ItemBlockEventArgs object
        /// with informatation in given collection of ListViewItems
        /// </summary>
        /// <param name="itemBlock"></param>
        public ItemBlockEventArgs(Collection<ListViewItem> itemBlock)
        {
            ItemBlock = (itemBlock == null) ? null : itemBlock.ToArray();
        }
    }

    /// <summary>
    /// Argument class for event that passes set of column definitions for list view control
    /// </summary>
    public class ColumnHeaderEventArgs : EventArgs
    {
        private ColumnHeader[] ColumnBlock = null;

        /// <summary>
        /// return array of column definitions
        /// </summary>
        /// <returns></returns>
        public ColumnHeader[] GetColumnBlock()
        {
            return ColumnBlock;
        }

        /// <summary>
        /// construct a new ColumnHeaderEventArgs object
        /// with information contained in given collection of ColumnHeader objects
        /// </summary>
        /// <param name="columnBlock"></param>
        public ColumnHeaderEventArgs(Collection<ColumnHeader> columnBlock)
        {
            ColumnBlock = (columnBlock == null) ? null : columnBlock.ToArray();
        }
    }

    #endregion
}
