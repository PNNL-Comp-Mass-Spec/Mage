using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using PRISM.Logging;

namespace Mage
{

    /// <summary>
    /// Module than can query a database and deliver
    /// results of the query via its standard tabular output events
    /// </summary>
    public sealed class SQLReader : BaseModule, IDisposable
    {

        /// <summary>
        /// SQL Command error constant
        /// </summary>
        public const string SQL_COMMAND_ERROR = "Problem forming SQL command";

        private static readonly FileLogger traceLogReader = new FileLogger(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        #region member variables

        private SqlConnection mConnection;

        private const int CommandTimeoutSeconds = 15;

        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan duration;

        private readonly Dictionary<string, string> mStoredProcParameters = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Full connection string to database
        /// </summary>
        /// <remarks>
        /// Auto-defined using Server and Database, plus optionally Username and Password
        /// Alternatively, use the MSSQLReader constructor that takes a connection string
        /// </remarks>
        public string ConnectionString { get; private set; } = string.Empty;

        /// <summary>
        /// MS SQL Server instance to connect to
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// Database to connect to
        /// </summary>
        public string Database { get; set; } = string.Empty;

        /// <summary>
        /// Specific user to use when connecting to the database
        /// </summary>
        /// <remarks>Will use integrated authentication if Username is empty or null</remarks>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password to use when Username is defined
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// SQL statement to run to obtain data
        /// </summary>
        public string SQLText { get; set; } = string.Empty;

        /// <summary>
        /// Name of stored procedure to call to obtain data
        /// (must be blank of straight SQL query is being used instead)
        /// </summary>
        public string SprocName { get; set; } = string.Empty;

        /// <summary>
        /// If stored procedure is being used for query,
        /// set the given argument to the given value
        /// </summary>
        /// <param name="name">Stored procedure argument name (must include "@"))</param>
        /// <param name="value">Value for argument</param>
        public void SetSprocParam(string name, string value)
        {
            mStoredProcParameters[name] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Mage SQL Server reader module
        /// </summary>
        public MSSQLReader()
        {
            SprocName = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ConnectionString = string.Empty;
        }

        /// <summary>
        /// Construct a new Mage SQL Server reader module, using the given SQL Server user
        /// </summary>
        /// <param name="username">SQL Server Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        public MSSQLReader(string username, string password)
        {
            SprocName = string.Empty;
            Username = username;
            Password = password;
            ConnectionString = string.Empty;
        }

        /// <summary>
        /// Constructor that initialize values from xml specs and args
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="args"></param>
        /// <param name="username">SQL Server Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        public MSSQLReader(string xml, Dictionary<string, string> args, string username = "", string password = "")
        {
            SprocName = string.Empty;
            var builder = new SQLBuilder(xml, ref args);
            SetPropertiesFromBuilder(builder, username, password);
        }

        /// <summary>
        /// Constructor that accepts a SQLBuilder, plus optionally a SQL Server username and password
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="username">SQL Server Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        public MSSQLReader(SQLBuilder builder, string username = "", string password = "")
        {
            SetPropertiesFromBuilder(builder, username, password);
        }

        /// <summary>
        /// Constructor that accepts a server name, database name, and SQL query
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="sql"></param>
        public MSSQLReader(string server, string database, string sql)
        {
            Server = server;
            Database = database;
            SQLText = sql;
            ConnectionString = string.Empty;
        }

        /// <summary>
        /// Constructor that accepts a connection string
        /// </summary>
        /// <param name="connectionString">SQL Server connection string</param>
        /// <remarks>Use property SQLText to define the query to use</remarks>
        public MSSQLReader(string connectionString)
        {
            SprocName = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ConnectionString = connectionString;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of held resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of held resources
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

            // isDisposed = true;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Set this module's properties using initialized SQLBuilder
        /// </summary>
        /// <param name="builder">SQL builder</param>
        /// <param name="username">SQL Server Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        private void SetPropertiesFromBuilder(SQLBuilder builder, string username, string password)
        {

            Username = username;
            Password = password;
            ConnectionString = string.Empty;

            // Set this module's properties from builder's special arguments list
            // This should update Server and Database
            SetParameters(builder.SpecialArgs);

            if (!string.IsNullOrEmpty(builder.SprocName))
            {
                // If query is via sproc call, set sproc arguments
                SprocName = builder.SprocName;
                foreach (var param in builder.SprocParameters)
                {
                    SetSprocParam(param.Key, param.Value);
                }
            }
            else
            {
                // Otherwise, we are doing straight SQL query, build the SQL
                SprocName = string.Empty;
                SQLText = builder.BuildQuerySQL();
            }
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(Object state)
        {
            try
            {
                Connect();

                try
                {
                    if (!string.IsNullOrEmpty(SprocName))
                    {
                        GetDataFromDatabaseSproc();
                    }
                    else
                    {
                        GetDataFromDatabaseQuery();
                    }
                }
                catch (Exception ex)
                {
                    OnWarningMessage(new MageStatusEventArgs("Error retrieving data from database: " + ex.Message));
                }

            }
            catch (Exception ex)
            {
                OnWarningMessage(new MageStatusEventArgs("Error connecting to database: " + ex.Message));
            }
            finally
            {
                Close();
            }
        }

        #endregion

        #region protected Functions

        /// <summary>
        /// Establish connection to the database server
        /// </summary>
        private void Connect()
        {
            var cnStr = GetConnectionString(Server, Database, Username, Password);
            mConnection = new SqlConnection
            {
                ConnectionString = cnStr
            };
            mConnection.Open();
        }

        /// <summary>
        /// Close the connection to the database
        /// </summary>
        private void Close()
        {
            mConnection.Close();
        }

        private string GetConnectionString(string server, string database, string username, string password)
        {

            if (string.IsNullOrWhiteSpace(server) && string.IsNullOrWhiteSpace(database))
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                    return ConnectionString;

                throw new Exception(
                    "ConnectionString not defined in MSSQLReader. " +
                    "Either set it via the ConnectionString property or " +
                    "instantiate this class with a specific server name and database name");
            }

            if (!string.IsNullOrWhiteSpace(server) && string.IsNullOrWhiteSpace(database))
            {
                throw new Exception(
                    "Server name is defined, but database name is not defined; cannot construct the connection string in MSSQLReader");
            }

            if (string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(database))
            {
                throw new Exception(
                    "Database name is defined, but server name is not defined; cannot construct the connection string in MSSQLReader");
            }

            if (string.IsNullOrWhiteSpace(username))
                ConnectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", server, database);
            else if (string.IsNullOrWhiteSpace(password))
                ConnectionString = string.Format("Data Source={0};Initial Catalog={1};User={2};", server, database, username);
            else
                ConnectionString = string.Format("Data Source={0};Initial Catalog={1};User={2};Password={3};", server, database, username, password);

            return ConnectionString;
        }

        /// <summary>
        /// Run SQL query against database and deliver data rows via standard tabular output
        /// </summary>
        private void GetDataFromDatabaseQuery()
        {
            var cmd = new SqlCommand
            {
                Connection = mConnection,
                CommandText = SQLText,
                CommandTimeout = CommandTimeoutSeconds
            };

            try
            {
                var myReader = cmd.ExecuteReader();
                GetData(myReader);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is SqlException)
                {
                    throw new MageException(SQL_COMMAND_ERROR + ": " + e.Message + ";    " + SQLText);
                }

                throw;
            }
        }

        /// <summary>
        /// Run stored procedure and deliver data rows via standard tabular output
        /// </summary>
        private void GetDataFromDatabaseSproc()
        {
            var myCmd = GetSprocCmd(SprocName, mStoredProcParameters);
            var myReader = myCmd.ExecuteReader();
            GetData(myReader);
        }

        /// <summary>
        /// Deliver data from query via standard tabular output
        /// </summary>
        /// <param name="myReader"></param>
        private void GetData(IDataReader myReader)
        {
            if (myReader == null)
            {
                // Something went wrong
                UpdateStatusMessage("Error: SqlDataReader object is null");
                return;
            }

            OutputColumnDefinitions(myReader, out var columnDefs);

            var totalRows = 0;
            OutputDataRows(myReader, columnDefs, ref totalRows);

            stopTime = DateTime.UtcNow;
            duration = stopTime - startTime;
            traceLogReader.Info("MSSQLReader.GetData --> Get data finish (" + duration + ") [" + totalRows + "]:" + SQLText);

            // Always close the DataReader
            myReader.Close();
        }

        /// <summary>
        /// Deliver data rows via standard tabular output
        /// </summary>
        /// <param name="myReader">DataReader object from which to get data rows</param>
        /// <param name="columnDefs">Column definitions (used to find date/time columns)</param>
        /// <param name="totalRows">Total rows delivered</param>
        private void OutputDataRows(IDataReader myReader, IReadOnlyList<MageColumnDef> columnDefs, ref int totalRows)
        {
            // Now do all the rows - if anyone is registered as wanting them
            startTime = DateTime.UtcNow;
            traceLogReader.Debug("MSSQLReader.GetData --> Get data start:" + SQLText);
            while (myReader.Read())
            {
                var a = new object[myReader.FieldCount];
                myReader.GetValues(a);

                var dataVals = new string[a.Length];

                for (var i = 0; i < a.Length; i++)
                {
                    var valProcessed = false;

                    if (i < columnDefs.Count)
                    {
                        if ((columnDefs[i].DataType.StartsWith("date") ||
                             columnDefs[i].DataType.StartsWith("smalldate")
                            ) && DateTime.TryParse(a[i].ToString(), out var dateValue))
                        {
                            dataVals[i] = dateValue.ToString("yyyy-MM-dd hh:mm:ss tt");
                            valProcessed = true;
                        }
                        else
                        {
                            if (columnDefs[i].DataType == "time" && DateTime.TryParse(a[i].ToString(), out var timeValue))
                            {
                                dataVals[i] = timeValue.ToString("hh:mm:ss tt");
                                valProcessed = true;
                            }
                        }

                    }

                    if (!valProcessed)
                    {
                        dataVals[i] = a[i].ToString();
                    }
                }

                OnDataRowAvailable(new MageDataEventArgs(dataVals));
                totalRows++;
                if (Abort)
                {
                    ReportProcessingAborted();
                    break;
                }
                if (totalRows % 1000 == 0)
                    OnStatusMessageUpdated(new MageStatusEventArgs("Running ... " + totalRows + " records retrieved"));
            }

            // Signal listeners that all data rows have been read
            if (!Abort)
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        /// <summary>
        /// Deliver column definitions via standard tabular output
        /// </summary>
        /// <param name="myReader">DataReader object from which to get data rows</param>
        /// <param name="columnDefs">Column definitions</param>
        private void OutputColumnDefinitions(IDataReader myReader, out List<MageColumnDef> columnDefs)
        {
            // If anyone is registered as listening for ColumnDefAvailable events, make it happen for them
            startTime = DateTime.UtcNow;
            traceLogReader.Debug("MSSQLReader.GetData --> Get column info start:" + SQLText);

            // Determine the column names and column data types (

            // Get list of fields in result set and process each field
            columnDefs = new List<MageColumnDef>();

            var schemaTable = myReader.GetSchemaTable();
            if (schemaTable != null)
            {
                foreach (DataRow drField in schemaTable.Rows)
                {
                    // Initialize column definition with canonical fields
                    var columnDef = GetColumnInfo(drField);
                    if (!columnDef.Hidden)
                    {
                        // Pass information about this column to the listeners
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
            traceLogReader.Info("MSSQLReader.GetData --> Get column info finish (" + duration + "):" + SQLText);
        }

        /// <summary>
        /// Get canonical column definition fields from MSSQL TableSchema row
        /// </summary>
        /// <param name="drField">MSSQL TableSchema row containing definition for a column</param>
        /// <returns></returns>
        private static MageColumnDef GetColumnInfo(DataRow drField)
        {
            // Add the canonical column definition fields to column definition

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
        /// Inform any interested listeners about our progress
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatusMessage(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion

        #region Code for stored procedures

        /// <summary>
        /// Return a SqlCommand suitable for calling the given stored procedure
        /// with the given argument values
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        private SqlCommand GetSprocCmd(string sprocName, IReadOnlyDictionary<string, string> parms)
        {

            // Start the SqlCommand that we are building up for the sproc
            var builtCmd = new SqlCommand
            {
                Connection = mConnection
            };

            try
            {
                // Query the database to get argument definitions for the given stored procedure
                var cmd = new SqlCommand
                {
                    Connection = mConnection
                };

                var sqlText = string.Format("SELECT * FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = '{0}'", sprocName);
                cmd.CommandText = sqlText;

                var rdr = cmd.ExecuteReader();

                // Column positions for the argument data we need
                var namIdx = rdr.GetOrdinal("PARAMETER_NAME");
                var typIdx = rdr.GetOrdinal("DATA_TYPE");
                var modIdx = rdr.GetOrdinal("PARAMETER_MODE");
                var sizIdx = rdr.GetOrdinal("CHARACTER_MAXIMUM_LENGTH");

                // More stuff for the SqlCommand being built
                builtCmd.CommandType = CommandType.StoredProcedure;
                builtCmd.CommandText = sprocName;
                builtCmd.Parameters.Add(new SqlParameter("@Return", SqlDbType.Int));
                builtCmd.Parameters["@Return"].Direction = ParameterDirection.ReturnValue;

                // Loop through all the arguments and add a parameter for each one
                // the the SqlCommand being built
                while (rdr.Read())
                {
                    var a = new object[rdr.FieldCount];
                    rdr.GetValues(a);
                    var argName = a[namIdx].ToString();
                    var argType = a[typIdx].ToString();
                    var argMode = a[modIdx].ToString();
                    switch (argType)
                    {
                        case "tinyint":
                        case "float":
                        case "real":
                        case "int":
                            builtCmd.Parameters.Add(new SqlParameter(argName, SqlDbType.Int));
                            builtCmd.Parameters[argName].Direction = ParamDirection(argMode);
                            if (parms.ContainsKey(argName))
                            {
                                builtCmd.Parameters[argName].Value = parms[argName];
                            }
                            break;
                        case "varchar":
                            var size = (Int32)a[sizIdx];
                            builtCmd.Parameters.Add(new SqlParameter(argName, SqlDbType.VarChar, size));
                            builtCmd.Parameters[argName].Direction = ParamDirection(argMode);
                            if (parms.ContainsKey(argName))
                            {
                                builtCmd.Parameters[argName].Value = parms[argName];
                            }
                            break;
                        case "decimal":
                            var preIdx = rdr.GetOrdinal("NUMERIC_PRECISION");
                            var scaIdx = rdr.GetOrdinal("NUMERIC_SCALE");
                            builtCmd.Parameters.Add(new SqlParameter(argName, SqlDbType.Decimal));
                            builtCmd.Parameters[argName].Direction = ParamDirection(argMode);
                            builtCmd.Parameters[argName].Precision = (byte)a[preIdx];
                            var obj = a[scaIdx];
                            builtCmd.Parameters[argName].Scale = Convert.ToByte(obj);
                            if (parms.ContainsKey(argName))
                            {
                                builtCmd.Parameters[argName].Value = parms[argName];
                            }
                            break;
                        // FUTURE: Add code for more data types
                        default:
                            Console.WriteLine("Couldn't figure out " + argName);
                            break;
                    }
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is SqlException)
                {
                    throw new MageException("Problem forming SQL command:" + e.Message);
                }

                throw;
            }
            return builtCmd;
        }

        /// <summary>
        /// Get canonical notation for argument direction
        /// </summary>
        /// <param name="argMode"></param>
        /// <returns></returns>
        private static ParameterDirection ParamDirection(string argMode)
        {
            return (argMode == "INOUT") ? ParameterDirection.Output : ParameterDirection.Input;
        }

        #endregion

    }
}

// Numeric types:   bit, tinyint, smallint, int, bigint, decimal, real, float, numeric, smallmoney, money
// String types:    char, varchar, text, nchar, nvarchar, ntext, uniqueidentifier, xml
// Datetime types:  date, datetime, datetime2, smalldatetime, time, datetimeoffset
// Binary types:    binary, varbinary, image

/*
---column def object ---
AllowDBNull = False
BaseCatalogName =
BaseColumnName = Job
BaseSchemaName =
BaseServerName =
BaseTableName =
ColumnName = Job
ColumnOrdinal = 0
ColumnSize = 4
DataType = System.Int32
DataTypeName = int
IsAliased =
IsAutoIncrement = False
IsColumnSet = False
IsExpression =
IsHidden =
IsIdentity = False
IsKey =
IsLong = False
IsReadOnly = False
IsRowVersion = False
IsUnique = False
NonVersionedProviderType = 8
NumericPrecision = 10
NumericScale = 255
ProviderSpecificDataType = System.Data.SqlTypes.SqlInt32
ProviderType = 8
--
UdtAssemblyQualifiedName =
XmlSchemaCollectionDatabase =
XmlSchemaCollectionName =
XmlSchemaCollectionOwningSchema =
 */
