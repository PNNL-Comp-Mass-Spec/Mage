using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PRISM.Logging;
using PRISMDatabaseUtils;

namespace Mage
{
    /// <summary>
    /// Module than can query a database and deliver
    /// results of the query via its standard tabular output events
    /// </summary>
    public sealed class SQLReader : BaseModule
    {
        // ReSharper disable CommentTypo

        // Ignore Spelling: citext, dbo, Mage, Postgres, postgresql, smalldate, username, yyyy-MM-dd hh:mm:ss tt
        // Ignore Spelling: bigint, datetime, datetimeoffset, nchar, ntext, nvarchar, smalldatetime, smallint, smallmoney, tinyint, uniqueidentifier, varbinary, varchar
        // Ignore Spelling: bool, bytea, cidr, hstore, inet, json, jsonb, lseg, macaddr, timestamptz, timetz, tsquery, tsvector, uuid, varbit

        // ReSharper restore CommentTypo

        /// <summary>
        /// SQL Command error constant
        /// </summary>
        public const string SQL_COMMAND_ERROR = "Problem forming SQL command";

        private static readonly FileLogger traceLogReader = new(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        private IDBTools mDbTools;

        private const int CommandTimeoutSeconds = 15;

        private DateTime startTime;
        private DateTime stopTime;
        private TimeSpan duration;

        /// <summary>
        /// This dictionary tracks stored procedure parameter names and values
        /// </summary>
        /// <remarks>Keys are stored procedure argument names, values are the value for each argument</remarks>
        private readonly Dictionary<string, string> mStoredProcParameters = new();

        /// <summary>
        /// Full connection string to database
        /// </summary>
        /// <remarks>
        /// Auto-defined using Server and Database, plus optionally Username and Password
        /// Alternatively, use the SQLReader constructor that takes a connection string
        /// </remarks>
        public string ConnectionString { get; private set; } = string.Empty;

        /// <summary>
        /// True when the server is a PostgreSQL server
        /// </summary>
        public bool IsPostgres { get; set; }

        /// <summary>
        /// Database Server to connect to
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
        /// <remarks>
        /// The password is optional for PostgreSQL connections, since it can be defined in a PgPass file
        /// On Linux use ~/.pgpass
        /// On Windows use C:\Users\Username\AppData\Roaming\postgresql\pgpass.conf
        /// </remarks>
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
        /// store the value to associate with the given argument
        /// </summary>
        /// <param name="name">Stored procedure argument name (must include "@"))</param>
        /// <param name="value">Value for argument</param>
        public void SetSprocParam(string name, string value)
        {
            mStoredProcParameters[name] = value;
        }

        /// <summary>
        /// Construct a new SQL Reader module
        /// </summary>
        public SQLReader()
        {
            ConnectionString = string.Empty;
        }

        /// <summary>
        /// Constructor that initialize values from xml specs and arguments
        /// </summary>
        /// <param name="xml">XML template with specifications for the query</param>
        /// <param name="args">Key/Value parameter that will be mixed into query</param>
        /// <param name="username">Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        /// <param name="isPostgres">True if a PostgreSQL server</param>
        public SQLReader(string xml, Dictionary<string, string> args, string username = "", string password = "", bool isPostgres = false)
        {
            var builder = new SQLBuilder(xml, args);
            SetPropertiesFromBuilder(builder, username, password, isPostgres);
        }

        /// <summary>
        /// Constructor that accepts a SQLBuilder, plus optionally a SQL Server username and password
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="username">Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        /// <param name="isPostgres">True if a PostgreSQL server</param>
        public SQLReader(SQLBuilder builder, string username = "", string password = "", bool isPostgres = false)
        {
            SetPropertiesFromBuilder(builder, username, password, isPostgres);
        }

        /// <summary>
        /// Constructor that accepts a server name, database name, and SQL query
        /// </summary>
        /// <remarks>The connection string will be auto-defined</remarks>
        /// <param name="server">Database server name</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        /// <param name="isPostgres">True if a PostgreSQL server</param>
        public SQLReader(string server, string database, string username, string password, bool isPostgres = false)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            IsPostgres = isPostgres;

            SQLText = string.Empty;
            ConnectionString = string.Empty;
        }

        /// <summary>
        /// Constructor that accepts a connection string
        /// </summary>
        /// <remarks>Use property SQLText to define the query to use</remarks>
        /// <param name="connectionString">Database server connection string</param>
        // ReSharper disable once UnusedMember.Global
        public SQLReader(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Set this module's properties using initialized SQLBuilder
        /// </summary>
        /// <param name="builder">SQL builder</param>
        /// <param name="username">Username; leave blank (or null) to use integrated authentication</param>
        /// <param name="password">Password if username is non-blank</param>
        /// <param name="isPostgres">True if a PostgreSQL server</param>
        private void SetPropertiesFromBuilder(SQLBuilder builder, string username, string password, bool isPostgres)
        {
            Username = username;
            Password = password;
            ConnectionString = string.Empty;
            IsPostgres = isPostgres;

            // Set this module's properties from builder's special arguments list
            // This should update Server and Database, plus optionally Username, Password, and IsPostgres
            SetParameters(builder.SpecialArgs);

            if (!string.IsNullOrEmpty(builder.SprocName))
            {
                // If query is via stored procedure call, set stored procedure arguments
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

            if (!string.IsNullOrWhiteSpace(username))
            {
                // Override any username defined in builder.SpecialArgs
                Username = username;
                Password = password;
            }
        }

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            try
            {
                var dbTools = Connect();

                try
                {
                    if (!string.IsNullOrEmpty(SprocName))
                    {
                        GetDataFromDatabaseSproc(dbTools);
                    }
                    else
                    {
                        GetDataFromDatabaseQuery(dbTools);
                    }
                }
                catch (Exception ex)
                {
                    ReportMageWarning("Error retrieving data from database: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ReportMageWarning("Error connecting to database: " + ex.Message);
            }
        }

        /// <summary>
        /// Establish connection to the database server
        /// </summary>
        private IDBTools Connect()
        {
            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                if (mDbTools != null)
                    return mDbTools;

                var dbServerType = DbToolsFactory.GetServerTypeFromConnectionString(ConnectionString);

                var connectionStringWithAppName = DbToolsFactory.AddApplicationNameToConnectionString(ConnectionString, "Mage_SQLReader", dbServerType);

                mDbTools = DbToolsFactory.GetDBTools(connectionStringWithAppName);
                IsPostgres = mDbTools.DbServerType == DbServerTypes.PostgreSQL;

                return mDbTools;
            }

            string connectionString;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (IsPostgres)
            {
                connectionString = GetPgSqlConnectionString(Server, Database, Username, Password);
            }
            else
            {
                connectionString = GetMSSqlConnectionString(Server, Database, Username, Password);
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var ex = ReportMageException("Unable to determine the connection string in the Connect() method");
                throw ex;
            }

            var serverType = IsPostgres ? DbServerTypes.PostgreSQL : DbServerTypes.MSSQLServer;

            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(connectionString, "Mage_SQLReader", serverType);

            if (ConnectionString.Equals(connectionStringToUse) && mDbTools != null)
                return mDbTools;

            ConnectionString = connectionStringToUse;

            mDbTools = DbToolsFactory.GetDBTools(connectionStringToUse);

            return mDbTools;
        }

        private string GetMSSqlConnectionString(string server, string database, string username, string password)
        {
            var cachedConnectionString = ValidateDatabaseInfo(server, database);
            if (!string.IsNullOrWhiteSpace(cachedConnectionString))
            {
                return cachedConnectionString;
            }

            if (string.IsNullOrWhiteSpace(username))
                return string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", server, database);

            if (string.IsNullOrWhiteSpace(password))
                return string.Format("Data Source={0};Initial Catalog={1};User={2};", server, database, username);

            return string.Format("Data Source={0};Initial Catalog={1};User={2};Password={3};", server, database, username, password);
        }

        private string GetPgSqlConnectionString(string server, string database, string username, string password)
        {
            var cachedConnectionString = ValidateDatabaseInfo(server, database);
            if (!string.IsNullOrWhiteSpace(cachedConnectionString))
            {
                return cachedConnectionString;
            }

            if (string.IsNullOrWhiteSpace(username))
                return string.Format("Host={0};Database={1}", server, database);

            if (string.IsNullOrWhiteSpace(password))
                return string.Format("Host={0};Database={1};Username={2}", server, database, username);

            return string.Format("Host={0};Database={1};Username={2};Password={3}", server, database, username, password);
        }

        /// <summary>
        /// Run SQL query against database and deliver data rows via standard tabular output
        /// </summary>
        private void GetDataFromDatabaseQuery(IDBTools dbTools)
        {
            var cmd = dbTools.CreateCommand(SQLText);
            cmd.CommandTimeout = CommandTimeoutSeconds;

            try
            {
                var success = dbTools.GetQueryResultsDataTable(cmd, out var queryResults);
                if (!success)
                {
                    var ex = ReportMageException("GetQueryResultsDataTable returned false running query " + SQLText);
                    throw ex;
                }

                GetData(queryResults);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException or SqlException)
                {
                    var ex = ReportMageException(SQL_COMMAND_ERROR + ": " + e.Message + ";    " + SQLText, e);
                    throw ex;
                }

                throw;
            }
        }

        /// <summary>
        /// Run stored procedure and deliver data rows via standard tabular output
        /// </summary>
        private void GetDataFromDatabaseSproc(IDBTools dbTools)
        {
            var cmd = GetSprocCmd(dbTools, SprocName, mStoredProcParameters);

            var success = dbTools.GetQueryResultsDataTable(cmd, out var queryResults);
            if (!success)
            {
                var ex = ReportMageException("GetQueryResultsDataTable returned false calling stored procedure " + SprocName);
                throw ex;
            }

            GetData(queryResults);
        }

        /// <summary>
        /// Deliver data from query via standard tabular output
        /// </summary>
        /// <param name="queryResults"></param>
        private void GetData(DataTable queryResults)
        {
            if (queryResults == null)
            {
                // Something went wrong
                UpdateStatusMessage("Error: DataTable object is null");
                return;
            }

            OutputColumnDefinitions(queryResults, out var columnDefs);

            var totalRows = 0;
            OutputDataRows(queryResults, columnDefs, ref totalRows);

            stopTime = DateTime.UtcNow;
            duration = stopTime - startTime;
            traceLogReader.Info("SQLReader.GetData --> Get data finish (" + duration + ") [" + totalRows + "]: " + SQLText);
        }

        /// <summary>
        /// Deliver data rows via standard tabular output
        /// </summary>
        /// <param name="queryResults">DataTable object from which to get data rows</param>
        /// <param name="columnDefs">Column definitions (used to find date/time columns)</param>
        /// <param name="totalRows">Total rows delivered</param>
        private void OutputDataRows(DataTable queryResults, IReadOnlyList<MageColumnDef> columnDefs, ref int totalRows)
        {
            // Now do all the rows - if anyone is registered as wanting them
            startTime = DateTime.UtcNow;
            traceLogReader.Debug("SQLReader.GetData --> Get data start: " + SQLText);

            var dateTimeColumns = new SortedSet<int>();
            var timeColumns = new SortedSet<int>();

            for (var i = 0; i < columnDefs.Count; i++)
            {
                // ReSharper disable once StringLiteralTypo
                if (columnDefs[i].DataType.StartsWith("date", StringComparison.OrdinalIgnoreCase) ||
                    columnDefs[i].DataType.StartsWith("smalldate", StringComparison.OrdinalIgnoreCase) ||
                    columnDefs[i].DataType.StartsWith("timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    dateTimeColumns.Add(i);
                    continue;
                }

                if (columnDefs[i].DataType == "time" ||
                    columnDefs[i].DataType.StartsWith("time with", StringComparison.OrdinalIgnoreCase))
                {
                    timeColumns.Add(i);
                }
            }
            foreach (DataRow row in queryResults.Rows)
            {
                var dataValues = new string[row.ItemArray.Length];

                for (var i = 0; i < dataValues.Length; i++)
                {
                    var valProcessed = false;

                    if (i < columnDefs.Count)
                    {
                        if (dateTimeColumns.Contains(i))
                        {
                            try
                            {
                                var dateValueViaCast = row.ItemArray[i].CastDBVal<DateTime>();
                                dataValues[i] = dateValueViaCast.ToString("yyyy-MM-dd hh:mm:ss tt");
                                valProcessed = true;
                            }
                            catch (Exception)
                            {
                                if (DateTime.TryParse(row.ItemArray[i].ToString(), out var dateValue))
                                {
                                    dataValues[i] = dateValue.ToString("yyyy-MM-dd hh:mm:ss tt");
                                    valProcessed = true;
                                }
                            }
                        }
                        else if (timeColumns.Contains(i))
                        {
                            try
                            {
                                var timeValueViaCast = row.ItemArray[i].CastDBVal<DateTime>();
                                dataValues[i] = timeValueViaCast.ToString("hh:mm:ss tt");
                                valProcessed = true;
                            }
                            catch (Exception)
                            {
                                if (DateTime.TryParse(row.ItemArray[i].ToString(), out var timeValue))
                                {
                                    dataValues[i] = timeValue.ToString("hh:mm:ss tt");
                                    valProcessed = true;
                                }
                            }
                        }
                    }

                    if (!valProcessed)
                    {
                        dataValues[i] = row.ItemArray[i].ToString();
                    }
                }

                OnDataRowAvailable(new MageDataEventArgs(dataValues));
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
        /// <param name="queryResults">DataTable object from which to get data rows</param>
        /// <param name="columnDefs">Column definitions</param>
        private void OutputColumnDefinitions(DataTable queryResults, out List<MageColumnDef> columnDefs)
        {
            // If anyone is registered as listening for ColumnDefAvailable events, make it happen for them
            startTime = DateTime.UtcNow;
            traceLogReader.Debug("SQLReader.GetData --> Get column info start: " + SQLText);

            // Determine the column names and column data types (

            // Get list of fields in result set and process each field
            columnDefs = new List<MageColumnDef>();

            foreach (DataColumn column in queryResults.Columns)
            {
                // Initialize column definition with canonical fields
                var columnDef = GetColumnInfo(column);
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

            // Signal that all columns have been read
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
            stopTime = DateTime.UtcNow;
            duration = stopTime - startTime;
            traceLogReader.Info("SQLReader.GetData --> Get column info finish (" + duration + "): " + SQLText);
        }

        /// <summary>
        /// Get canonical column definition fields for a DataColumn
        /// </summary>
        /// <param name="column">DataColumn instance</param>
        private static MageColumnDef GetColumnInfo(DataColumn column)
        {
            // Add the canonical column definition fields to column definition

            var columnDef = new MageColumnDef
            {
                Name = column.ColumnName,
                DataType = column.DataType.Name,
                Size = column.MaxLength.ToString(),
                Hidden = false
            };

            // Hidden columns were deprecated in February 2020 when we switched from MSSql TableSchema to DataColumn
            //var colHidden = drField["IsHidden"].ToString();
            //columnDef.Hidden = !(string.IsNullOrEmpty(colHidden) || colHidden.ToLower() == "false");

            return columnDef;
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

        /// <summary>
        /// Validate that either ConnectionString is defined, or both server and database are defined
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <returns>Cached connection string, or an empty string</returns>
        private string ValidateDatabaseInfo(string server, string database)
        {
            // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

            if (string.IsNullOrWhiteSpace(server) && string.IsNullOrWhiteSpace(database))
            {
                if (!string.IsNullOrWhiteSpace(ConnectionString))
                    return ConnectionString;

                var ex = ReportMageException(
                    "ConnectionString not defined in SQLReader. " +
                    "Either set it via the ConnectionString property or " +
                    "instantiate this class with a specific server name and database name");

                throw ex;
            }

            if (!string.IsNullOrWhiteSpace(server) && string.IsNullOrWhiteSpace(database))
            {
                var ex = ReportMageException(
                    "Server name is defined, but database name is not defined; cannot construct the connection string in SQLReader");

                throw ex;
            }

            if (string.IsNullOrWhiteSpace(server) && !string.IsNullOrWhiteSpace(database))
            {
                var ex = ReportMageException(
                    "Database name is defined, but server name is not defined; cannot construct the connection string in SQLReader");

                throw ex;
            }

            return string.Empty;
        }

        /// <summary>
        /// Inform any interested listeners about our progress
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatusMessage(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        // Methods for stored procedures

        private DbParameter AddParameter(
            IDBTools dbTools,
            DbCommand cmd,
            string parameterName,
            string dataTypeName,
            ParameterDirection paramDirection,
            ISet<string> paramNames,
            IReadOnlyDictionary<string, string> sprocParams,
            int argSize = 0)
        {
            if (paramNames.Contains(parameterName))
            {
                ReportMageWarning(string.Format(
                    "Skipping duplicate stored procedure parameter {0} for procedure {1}",
                    parameterName, cmd.CommandText));
                return null;
            }

            paramNames.Add(parameterName);

            var newParam = dbTools.AddParameter(cmd, parameterName, dataTypeName, argSize, paramDirection);
            if (newParam == null)
            {
                ReportMageWarning(string.Format(
                    "Error adding stored procedure parameter {0} for procedure {1}; dbTools.AddParameter returned null",
                    parameterName, cmd.CommandText));
                return null;
            }

            if (sprocParams.ContainsKey(parameterName))
            {
                newParam.Value = sprocParams[parameterName];
            }

            return newParam;
        }

        private object GetProcedureNameWithoutSchema(string sprocName, out string schemaName, bool isPostgres)
        {
            if (isPostgres)
            {
                schemaName = "public";
            }
            else
            {
                schemaName = "dbo";
            }

            var periodIndex = sprocName.IndexOf('.');
            if (periodIndex < 0)
            {
                return sprocName;
            }

            if (periodIndex > 0)
            {
                schemaName = sprocName.Substring(0, periodIndex);
            }

            return sprocName.Substring(1);
        }

        /// <summary>
        /// Return a SqlCommand suitable for calling the given stored procedure
        /// with the given argument values
        /// </summary>
        /// <param name="dbTools"></param>
        /// <param name="sprocName">Stored procedure name; can optionally contain a schema name, e.g. mc.get_manager_parameters</param>
        /// <param name="sprocParams">Dictionary where keys are stored procedure argument names and values are the value for each argument</param>
        private DbCommand GetSprocCmd(IDBTools dbTools, string sprocName, IReadOnlyDictionary<string, string> sprocParams)
        {
            var isPostgres = dbTools.DbServerType == DbServerTypes.PostgreSQL;

            var command = dbTools.CreateCommand(sprocName, CommandType.StoredProcedure);

            try
            {
                // Query the database to get argument definitions for the given stored procedure
                var columnNames = new List<string>
                {
                    "parameter_name",
                    "data_type",
                    "parameter_mode",
                    "character_maximum_length",
                    "numeric_precision",
                    "numeric_scale"
                };

                // Map from column name to column index
                var columnIndexMap = new Dictionary<string, int>();

                for (var i = 0; i < columnNames.Count; i++)
                {
                    columnIndexMap.Add(columnNames[i], i);
                }

                var baseQuery = "SELECT " + string.Join(", ", columnNames) + " " +
                                "FROM information_schema.parameters";
                string sqlQuery;

                // Extract the schema from the stored procedure name
                var sprocNameWithoutSchema = GetProcedureNameWithoutSchema(sprocName, out var schemaName, isPostgres);

                if (isPostgres)
                {
                    // ReSharper disable CommentTypo

                    // Procedure and function names in PostgreSQL have an integer appended to them, for example:
                    //   post_log_entry_53737
                    //   append_to_text_53730
                    //   get_manager_parameters_49424

                    // ReSharper restore CommentTypo

                    // Match the procedure using SIMILAR TO
                    sqlQuery = string.Format(
                        "{0} WHERE specific_schema = '{1}' AND specific_name::citext SIMILAR TO '{2}[_]%'",
                        baseQuery, schemaName, sprocNameWithoutSchema);
                }
                else
                {
                    sqlQuery = string.Format(
                        "{0} WHERE specific_schema = '{1}' AND specific_name = '{2}'",
                        baseQuery, schemaName, sprocNameWithoutSchema);
                }

                var cmd = dbTools.CreateCommand(sqlQuery);

                var success = dbTools.GetQueryResults(cmd, out var queryResults, 1);
                if (!success)
                {
                    var ex = ReportMageException("GetQueryResults returns false querying information_schema.parameters for procedure " + sprocName);
                    throw ex;
                }

                // Add the @Return parameter

                // Note that for Postgres databases, the DBTools object will auto-update @Return parameters to have:
                //   parameter.ParameterName = "_returnCode";
                //   parameter.DbType = DbType.String;
                //   parameter.Direction = ParameterDirection.InputOutput;
                // See UpdateSqlServerParameterNames in https://github.com/PNNL-Comp-Mass-Spec/PRISM-Class-Library/blob/master/PRISMDatabaseUtils/PostgreSQL/PostgresDBTools.cs

                dbTools.AddParameter(command, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

                var paramNames = new SortedSet<string>();

                // Loop through all the arguments and add a parameter for each one
                foreach (var resultRow in queryResults)
                {
                    var parameterName = resultRow[columnIndexMap["parameter_name"]];
                    var dataTypeName = resultRow[columnIndexMap["data_type"]].ToLower();
                    var parameterMode = resultRow[columnIndexMap["parameter_mode"]];
                    var parameterSizeText = resultRow[columnIndexMap["character_maximum_length"]];

                    var parameterSize = int.TryParse(parameterSizeText, out var parsedArgSize) ? parsedArgSize : 0;

                    var direction = ParamDirection(parameterMode);

                    var parameter = AddParameter(dbTools, command, parameterName, dataTypeName, direction, paramNames, sprocParams, parameterSize);

                    if (parameter.DbType != DbType.Decimal)
                        continue;

                    var argPrecision = resultRow[columnIndexMap["numeric_precision"]];
                    var argScale = resultRow[columnIndexMap["numeric_scale"]];

                    if (byte.TryParse(argPrecision, out var precision) &&
                        byte.TryParse(argScale, out var scale))
                    {
                        parameter.Precision = precision;
                        parameter.Scale = scale;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException or SqlException)
                {
                    var ex = ReportMageException("Problem forming SQL command: " + e.Message, e);
                    throw ex;
                }

                throw;
            }

            return command;
        }

        /// <summary>
        /// Get canonical notation for argument direction
        /// </summary>
        /// <param name="argMode"></param>
        private static ParameterDirection ParamDirection(string argMode)
        {
            return argMode is "INOUT" or "OUT" ? ParameterDirection.Output : ParameterDirection.Input;
        }
    }
}

// ReSharper disable CommentTypo

// SQL Server data types

// Numeric types:          bit, tinyint, smallint, int, bigint, decimal, real, float, numeric, smallmoney, money
// String types:           char, varchar, text, nchar, nvarchar, ntext, uniqueidentifier, xml
// Datetime types:         date, datetime, datetime2, smalldatetime, time, datetimeoffset
// Binary types:           binary, varbinary, image

// PostgreSQL data types

// Numeric types:          bit, smallint, integer, bigint, double, numeric, real, money
// String types:           text, varchar, name, citext, char(n)
// Datetime Types:         date, time, timestamp, timestamptz, interval, timetz
// Binary Types:           bytea

// Boolean type:           boolean, bool
// Bit String types:       bit, varbit

// Geometric types:        box, circle, line, lseg, path, point, polygon
// Network Address Types:  inet, cidr, macaddr, macaddr8
// Text Search Types:      tsvector, tsquery
// Additional Types:       array, hstore, json, jsonb, range, uuid, xml

// ReSharper restore CommentTypo

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
