using System;
using System.Collections.Generic;
using System.Data.SQLite;
using log4net;
using System.Data;

namespace Mage
{

    /// <summary>
    /// Module that can query a SQLite database and deliver
    /// results of query via its standard tabular output events
    /// </summary>
    public sealed class SQLiteReader : BaseModule, IDisposable
    {
        private static readonly ILog traceLog = LogManager.GetLogger("TraceLog");

        #region Member Variables

        private const int CommandTimeoutSeconds = 15;

        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan duration;

        private SQLiteConnection mConnection;

        #endregion

        #region Properties

        /// <summary>
        /// Full path to SQLite database file
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// SQL query to execute agains database
        /// </summary>
        public string SQLText { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage SQLite reader module
        /// </summary>
        public SQLiteReader()
        {
        }

        /// <summary>
        /// Construct a new Mage SQLite reader module
        /// using an xml query template and runtime parameters to
        /// define the SQLText property
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="args"></param>
        public SQLiteReader(string xml, Dictionary<string, string> args)
        {
            var builder = new SQLBuilder(xml, ref args);
            SetPropertiesFromBuilder(builder);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// dispose of held resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// dispose of held resources
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            mConnection?.Dispose();

            //            isDisposed = true;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Set this module's properties using initialized SQLBuiler
        /// </summary>
        /// <param name="builder">SQL builder</param>
        private void SetPropertiesFromBuilder(SQLBuilder builder)
        {

            // set this module's properties from builder's special arguments list
            SetParameters(builder.SpecialArgs);

            // we are doing straight SQL query, build the SQL
            var sql = builder.BuildQuerySQL();

            // change MSSQL quote characters to SQLite quote characters
            sql = sql.Replace('[', '"').Replace(']', '"');
            SQLText = sql;
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// Execute query against database and stream results to Mage standard tabular output
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(Object state)
        {
            try
            {
                Connect();
                Access();
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region Private Functions

        private void Access()
        {
            var cmd = new SQLiteCommand(SQLText, mConnection)
            {
                CommandTimeout = CommandTimeoutSeconds
            };

            var myReader = cmd.ExecuteReader();
            GetData(myReader);
        }

        private void Connect()
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = Database
            };
            /*
                        if (password != null) {
                            builder.Password = password;
                        }
                        //builder.PageSize = 4096
                        //builder.UseUTF16Encoding = True
            */
            var connstring = builder.ConnectionString;
            mConnection = new SQLiteConnection(connstring);
            mConnection.Open();
        }

        private void Close()
        {
            mConnection.Close();
        }

        /// <summary>
        /// output results of query to Mage standard tabular output
        /// </summary>
        /// <param name="myReader"></param>
        private void GetData(IDataReader myReader)
        {
            if (myReader == null)
            { // Something went wrong
                UpdateStatusMessage("Error: SqlDataReader object is null");
                return;
            }

            OutputColumnDefinitions(myReader); // if ColumnDefAvailable

            var totalRows = 0;
            OutputDataRows(myReader, ref totalRows);

            stopTime = DateTime.UtcNow;
            duration = stopTime - startTime;
            traceLog.Info("SQLiteReader.GetData --> Get data finish (" + duration + ") [" + totalRows + "]:" + SQLText);

            //Always close the DataReader
            myReader.Close();

        }

        private void OutputDataRows(IDataReader myReader, ref int totalRows)
        {
            startTime = DateTime.UtcNow;
            traceLog.Debug("SQLiteReader.GetData --> Get data start:" + SQLText);
            while (myReader.Read())
            {
                var a = new object[myReader.FieldCount];
                myReader.GetValues(a);

                var dataVals = new string[a.Length];
                for (var i = 0; i < a.Length; i++)
                    dataVals[i] = a[i].ToString();

                OnDataRowAvailable(new MageDataEventArgs(dataVals));
                totalRows++;
                if (Abort)
                {
                    ReportProcessingAborted();
                    break;
                }
            }

            if (!Abort)
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        private void OutputColumnDefinitions(IDataReader myReader)
        {
            // if anyone is registered as listening for ColumnDefAvailable events, make it happen for them
            startTime = DateTime.UtcNow;
            traceLog.Debug("SQLiteReader.GetData --> Get column info start:" + SQLText);
            // Determine the column names and column data types (

            // Get list of fields in result set and process each field
            var columnDefs = new List<MageColumnDef>();
            var schemaTable = myReader.GetSchemaTable();
            if (schemaTable != null)
            {
                foreach (DataRow drField in schemaTable.Rows)
                {
                    var columnDef = GetColumnInfo(drField);
                    if (!columnDef.Hidden)
                    {
                        // pass information about this column to the listeners
                        columnDefs.Add(columnDef);
                    }
                    else
                    {
                        // Column is marked as hidden; do not process it
                        UpdateStatusMessage("Skipping hidden column [" + columnDef.Name + "]");
                    }
                }
            }

            // Signal that all columns have been read
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
            stopTime = DateTime.UtcNow;
            duration = stopTime - startTime;
            traceLog.Info("SQLiteReader.GetData --> Get column info finish (" + duration + "):" + SQLText);
        }

        /// <summary>
        /// return a MageColumnDef object constructed from information
        /// from given DataRow object
        /// </summary>
        /// <param name="drField"></param>
        /// <returns></returns>
        private static MageColumnDef GetColumnInfo(DataRow drField)
        {
            // add the canonical column definition fields to column definition

            var columnDef = new MageColumnDef
            {
                Name = drField["ColumnName"].ToString(),
                DataType = drField["DataTypeName"].ToString(),
                Size = drField["ColumnSize"].ToString()
            };

            var colHidden = drField["IsHidden"].ToString();
            columnDef.Hidden = !(string.IsNullOrEmpty(colHidden) || colHidden.ToLower() == "false");
            return columnDef;
        }

        /// <summary>
        /// inform any listeners about our progress
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatusMessage(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion

    }
}
// Numeric types:   bit, tinyint, smallint, int, bigint, decimal, real, float, numeric, smallmoney, money
// String types:    char, varchar, text, nchar, nvarchar, ntext, uniqueidentifier, xml
// Datetime types:  date, datetime, datetime2, smalldatetime, time, datetimeoffset
// Binary types:    binary, varbinary, image

/*
 ---column---
AllowDBNull = False
BaseCatalogName = main
BaseColumnName = ID
BaseSchemaName =
BaseServerName =
BaseTableName = T_Data_Package
ColumnName = ID
ColumnOrdinal = 0
ColumnSize = 8
DataType = System.Int64
DataTypeName = integer
DefaultValue =
IsAliased = False
IsAutoIncrement = False
IsExpression = False
IsHidden = False
IsKey = False
IsLong = False
IsReadOnly = False
IsRowVersion = False
IsUnique = False
NumericPrecision = 19
NumericScale = 0
ProviderSpecificDataType =
ProviderType = 12
--
CollationType = BINARY
         */
