using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{
    /// <summary>
    /// <para>
    /// Provides column definition and data row event handlers
    /// that can be wired into a Mage pipeline to receive data.
    /// </para>
    /// <para>
    /// It converts data rows into ListView item lists suitable
    /// for populating a ListDisplayControl and accumulates them
    /// into a buffer.  When the buffer is full, it is emptied
    /// into the associated ListDisplayControl.
    /// </para>
    /// </summary>
    public class LVAccumulator : ISinkModule
    {
        // Ignore Spelling: Mage

        #region Events for ListDisplay listeners to register for

        /// <summary>
        /// Event that is fired to pass on column definitions to our associated control
        /// </summary>
        public event EventHandler<ColumnHeaderEventArgs> OnColumnBlockRetrieved;

        /// <summary>
        /// Event that is fired to pass on a block of display rows to our associated control
        /// </summary>
        public event EventHandler<ItemBlockEventArgs> OnItemBlockRetrieved;

        #endregion

        #region Member Variables

        private readonly List<ListViewItem> itemAccumulator = new();
        private readonly List<ColumnHeader> columnAccumulator = new();
        private List<MageColumnDef> mColumnDefs = new();

        #endregion

        #region Properties

        /// <summary>
        /// Number of data rows in an item block
        /// </summary>
        public int ItemBlockSize { get; set; }

        /// <summary>
        /// Definition of columns
        /// </summary>
        public Collection<MageColumnDef> ColumnDefs => new(mColumnDefs);

        /// <summary>
        /// Get the column definitions
        /// </summary>
        public Collection<MageColumnDef> Columns => new(mColumnDefs);

        #endregion

        #region Constructors

        /// <summary>
        /// Number of items to accumulate before firing an update event
        /// </summary>
        public LVAccumulator()
        {
            ItemBlockSize = 1000;
        }

        #endregion

        #region Utility functions

        /// <summary>
        /// Clear any accumulate row and column information
        /// </summary>
        public void Clear()
        {
            itemAccumulator.Clear();
            columnAccumulator.Clear();
        }

        #endregion

        #region Handlers for ISinkModule

        /// <summary>
        /// <para>
        /// Receive data row, convert to ListView item, and add to accumulator.
        /// </para>
        /// <para>
        /// This event handler receives row events from upstream module, one event per row.
        /// on the module's standard tabular input, one event per row
        /// and a null value object signaling the end of row events.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleDataRow(object sender, MageDataEventArgs args)
        {
            var endOfData = !args.DataAvailable;
            if (args.DataAvailable)
            {
                ListViewItem lvi = null;
                for (var i = 0; i < args.Fields.Length; i++)
                {
                    object val = args.Fields[i];
                    var s = (val != null) ? val.ToString() : "-";
                    if (i == 0)
                    {
                        if (val != null)
                            lvi = new ListViewItem(val.ToString());
                    }
                    else
                    {
                        lvi?.SubItems.Add(s);
                    }
                }
                itemAccumulator.Add(lvi);
            }
            if (itemAccumulator.Count == ItemBlockSize || endOfData)
            {
                OnItemBlockRetrieved?.Invoke(this, new ItemBlockEventArgs(new Collection<ListViewItem>(itemAccumulator)));
                itemAccumulator.Clear();
            }
            if (endOfData)
            {
                OnItemBlockRetrieved?.Invoke(this, new ItemBlockEventArgs(null));
            }
        }

        /// <summary>
        /// <para>
        /// Build up list of ColumnHeader for list view.
        /// </para>
        /// <para>
        /// This event handler receives column definition events
        /// on the module's standard tabular input, one event per column
        /// and a null columnDef object signaling the end of column definition events.
        /// Subclasses should override this for any specialized column def handling
        /// that they need, but should be sure to also call the base class function.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mColumnDefs = new List<MageColumnDef>(args.ColumnDefs);

            foreach (var columnDef in mColumnDefs)
            {
                var ch = new ColumnHeader();

                // Sort out column display size
                var colSize = columnDef.Size;
                var colSizeToUse = 6;
                if (!string.IsNullOrEmpty(colSize))
                {
                    var w = int.Parse(colSize);
                    w = (w < 6) ? 6 : w;
                    w = (w > 20) ? 20 : w;
                    colSizeToUse = w;
                }

                var pixels = colSizeToUse * 10;
                ch.Text = columnDef.Name;
                ch.Name = columnDef.Name;
                ch.Width = pixels;
                var colType = columnDef.DataType;
                ch.Tag = colType;
                columnAccumulator.Add(ch);
            }
            OnColumnBlockRetrieved?.Invoke(this, new ColumnHeaderEventArgs(new Collection<ColumnHeader>(columnAccumulator)));
        }
        #endregion
    }

    #region Event argument classes

    /// <summary>
    /// Argument class for event that passes block of items to list view control
    /// </summary>
    public class ItemBlockEventArgs : EventArgs
    {
        private readonly ListViewItem[] ItemBlock;

        /// <summary>
        /// Get list of items being passed in the event
        /// </summary>
        /// <returns></returns>
        public ListViewItem[] GetItemBlock()
        {
            return ItemBlock;
        }

        /// <summary>
        /// Construct new ItemBlockEventArgs object
        /// with information in given collection of ListViewItems
        /// </summary>
        /// <param name="itemBlock"></param>
        public ItemBlockEventArgs(Collection<ListViewItem> itemBlock)
        {
            ItemBlock = itemBlock?.ToArray();
        }
    }

    /// <summary>
    /// Argument class for event that passes set of column definitions for list view control
    /// </summary>
    public class ColumnHeaderEventArgs : EventArgs
    {
        private readonly ColumnHeader[] ColumnBlock;

        /// <summary>
        /// Return array of column definitions
        /// </summary>
        /// <returns></returns>
        public ColumnHeader[] GetColumnBlock()
        {
            return ColumnBlock;
        }

        /// <summary>
        /// Construct a new ColumnHeaderEventArgs object
        /// with information contained in given collection of ColumnHeader objects
        /// </summary>
        /// <param name="columnBlock"></param>
        public ColumnHeaderEventArgs(IEnumerable<ColumnHeader> columnBlock)
        {
            ColumnBlock = columnBlock?.ToArray();
        }
    }

    #endregion
}
