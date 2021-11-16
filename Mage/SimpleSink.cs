using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// ReSharper disable UnusedMember.Global

namespace Mage
{
    /// <summary>
    /// <para>
    /// This Mage module can receive data over its standard tabular input
    /// and perform several actions useful for testing other Mage modules
    /// or as a simple end terminus for a pipeline
    /// </para>
    /// <para>
    /// By default, it outputs descriptions of column definition events and data rows that it
    /// receives to the console (this behavior can be disabled via the WriteToConsole property)
    /// </para>
    /// <para>
    /// By default, it retains column definitions (via BaseModule behavior)
    /// </para>
    /// <para>
    /// It also can accumulate data rows in an internal buffer.  The maximum number of rows
    /// that it will accumulate is set by the RowsToSave property (defaults to a few rows)
    /// </para>
    /// <para>
    /// Both the column definitions and any accumulated data rows can be retrieved by client
    /// </para>
    /// </summary>
    public class SimpleSink : BaseModule
    {
        // Ignore Spelling: Mage

        /// <summary>
        /// An internal buffer for accumulating rows passed in via the standard tabular input handler
        /// </summary>
        private readonly List<string[]> SavedRows = new();

        /// <summary>
        /// Echo each column event and each row event to console if set true (default)
        /// </summary>
        public bool WriteToConsole { get; set; }

        /// <summary>
        /// Number of rows to accumulate in internal row buffer
        /// </summary>
        public int RowsToSave { get; set; }

        /// <summary>
        /// Get the column definitions
        /// </summary>
        public Collection<MageColumnDef> Columns => new(InputColumnDefs);

        /// <summary>
        /// Association of name of input column with its column position index
        /// (case insensitive)
        /// </summary>
        public Dictionary<string, int> ColumnIndex => InputColumnPos;

        /// <summary>
        /// Get rows that were accumulated in the internal row buffer
        /// </summary>
        public Collection<string[]> Rows => new(SavedRows);

        /// <summary>
        /// Construct a new Mage sink object
        /// </summary>
        public SimpleSink()
        {
            RowsToSave = int.MaxValue;
            WriteToConsole = false;
        }

        /// <summary>
        /// Construct a new Mage sink object with input row limit
        /// </summary>
        /// <param name="rows">input row limit</param>
        public SimpleSink(int rows)
        {
            RowsToSave = rows;
            WriteToConsole = false;
        }

        /// <summary>
        /// Construct a new Mage sink object that (optionally) spills it guts to console out
        /// </summary>
        /// <param name="rows">input row limit</param>
        /// <param name="verbose">spill my guts?</param>
        public SimpleSink(int rows, bool verbose)
        {
            RowsToSave = rows;
            WriteToConsole = verbose;
        }

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                if (WriteToConsole)
                {
                    foreach (var item in args.Fields)
                    {
                        Console.Write(item + "|");
                    }
                    Console.WriteLine();
                }
                if (SavedRows.Count < RowsToSave)
                {
                    SavedRows.Add(args.Fields);
                }
                else
                {
                    CancelPipeline();
                }

                OnDataRowAvailable(args);
            }
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            foreach (var columnDef in args.ColumnDefs)
            {
                if (WriteToConsole)
                {
                    Console.WriteLine("Column {0}, {1}, {2} ", columnDef.Name, columnDef.DataType, columnDef.Size);
                }
            }

            OnColumnDefAvailable(args);
        }

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events.
        /// It will output via standard tabular output any rows it has accumulated
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            OnColumnDefAvailable(new MageColumnEventArgs(InputColumnDefs.ToArray()));
            foreach (var row in SavedRows)
            {
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        // Utility methods

        /// <summary>
        /// Populates value with the value for column colIndex in row rowIndex
        /// </summary>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (double, Output) </param>
        /// <returns>True if the column exists and contains a numeric value; otherwise false</returns>
        public bool TryGetValueViaColumnIndex(int colIndex, int rowIndex, out double value)
        {
            value = 0;

            return colIndex > -1 && double.TryParse(Rows[rowIndex][colIndex], out value);
        }

        /// <summary>
        /// Populates value with the value for column colIndex in row rowIndex
        /// </summary>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (integer, Output) </param>
        /// <returns>True if the column exists and contains a numeric value; otherwise false</returns>
        public bool TryGetValueViaColumnIndex(int colIndex, int rowIndex, out int value)
        {
            value = 0;

            return colIndex > -1 && int.TryParse(Rows[rowIndex][colIndex], out value);
        }

        /// <summary>
        /// Populates value with the value for column colIndex in row rowIndex
        /// </summary>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (string, Output) </param>
        /// <returns>True if the column exists; otherwise false</returns>
        public bool TryGetValueViaColumnIndex(int colIndex, int rowIndex, out string value)
        {
            value = null;

            if (colIndex > -1)
            {
                value = Rows[rowIndex][colIndex];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates value with the value for column columnName in row rowIndex
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (double, Output) </param>
        /// <returns>True if the column exists and contains a numeric value; otherwise false</returns>
        public bool TryGetValueViaColumnName(string columnName, int rowIndex, out double value)
        {
            value = 0;

            if (ColumnIndex.TryGetValue(columnName, out var colIndex))
            {
                if (double.TryParse(Rows[rowIndex][colIndex], out value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Populates value with the value for column columnName in row rowIndex
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (integer, Output) </param>
        /// <returns>True if the column exists and contains a numeric value; otherwise false</returns>
        public bool TryGetValueViaColumnName(string columnName, int rowIndex, out int value)
        {
            value = 0;

            if (ColumnIndex.TryGetValue(columnName, out var colIndex))
            {
                if (int.TryParse(Rows[rowIndex][colIndex], out value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Populates value with the value for column columnName in row rowIndex
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <param name="rowIndex">Row to examine</param>
        /// <param name="value">Value (string, Output) </param>
        /// <returns>True if the column exists; otherwise false</returns>
        public bool TryGetValueViaColumnName(string columnName, int rowIndex, out string value)
        {
            value = null;

            if (ColumnIndex.TryGetValue(columnName, out var colIndex))
            {
                value = Rows[rowIndex][colIndex];
                return true;
            }
            return false;
        }
    }
}
