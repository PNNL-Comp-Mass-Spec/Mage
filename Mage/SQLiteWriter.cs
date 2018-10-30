using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using PRISM.Logging;

namespace Mage
{

    /// <summary>
    /// Insert data received on standard tabular input
    /// into the specified SQLite database and table.
    /// The table and database will be created if they do not already exist.
    /// </summary>
    public sealed class SQLiteWriter : BaseModule, IDisposable
    {

        private static readonly FileLogger traceLogWriter = new FileLogger(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        #region Member Variables

        // Buffer for accumulating rows into output block
        private readonly List<string[]> mRows = new List<string[]>();

        // Description of table we will be inserting rows into
        private TableSchema mSchema;

        // Connection to SQLite database
        private SQLiteConnection mConnection;

        private int mRowsAccumulated;

        private int mBlockSize = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the table in the database into which the tabular data will be inserted
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Optional list of column defs that will be used when creating the target table in the SqLite database
        /// </summary>
        /// <remarks>This list does not need to contain all of the columns; only those for which the data type is not text (e.g. integer or real)</remarks>
        public List<MageColumnDef> ColDefOverride { get; set; }

        /// <summary>
        /// Path to the SQLite database file
        /// </summary>
        public string DbPath { get; set; }

        /// <summary>
        /// Password for SQLite database (if there is one)
        /// </summary>
        public string DbPassword { get; set; }

        /// <summary>
        /// Number of input rows that are grouped into SQLite transaction blocks
        /// </summary>
        public string BlockSize
        {
            get => mBlockSize.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (int.TryParse(value, out var val))
                {
                    mBlockSize = val;
                }
            }
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

        #region IBaseModule Members

        /// <summary>
        /// Called after pipeline run is complete - module can do any special cleanup
        /// this module closes the database connection
        /// (override of base class)
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            CloseDBConnection();
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        ///
        /// In addition to base module column definition
        /// forms column description suitable for creating the SQL
        /// table definition and data insertion commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);

            // Make table schema
            var cd = OutputColumnDefs ?? InputColumnDefs;
            mSchema = MakeTableSchema(cd);

            // Create db and table in database
            CreateTableInDatabase();
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        ///
        /// Receive data row, add to accumulator, write to SQLite when buffer is full, or reader finishes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                if (OutputColumnDefs != null)
                {
                    mRows.Add(MapDataRow(args.Fields));
                }
                else
                {
                    mRows.Add(args.Fields);
                }
                if (++mRowsAccumulated < mBlockSize)
                {
                    // Accumulate row (so that data can be added in chunks, using a single transaction for each chunk)
                }
                else
                {
                    mRowsAccumulated = 0;
                    // Add the cached data to the SQLite database
                    CopyTabularDataRowsToSQLiteDB();
                    mRows.Clear();
                }
            }
            else
            {
                if (mRowsAccumulated > 0)
                {
                    // Add the cached data to the SQLite database
                    CopyTabularDataRowsToSQLiteDB();
                }
            }
        }

        #endregion

        #region Helper Functions

        private TableSchema MakeTableSchema(IEnumerable<MageColumnDef> colDefs)
        {
            var ts = new TableSchema
            {
                TableName = TableName,
                Columns = new List<ColumnSchema>()
            };
            foreach (var colDef in colDefs)
            {
                var cs = new ColumnSchema
                {
                    ColumnName = colDef.Name,
                    ColumnType = colDef.DataType
                };

                if (ColDefOverride != null)
                {
                    foreach (var overrideDef in ColDefOverride)
                    {
                        if (string.Compare(overrideDef.Name, cs.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            cs.ColumnType = overrideDef.DataType;
                            break;
                        }
                    }
                }

                // Check for a column type of "varchar" or "char"
                if (cs.ColumnType.Contains("char"))
                {
                    // Change to text
                    cs.ColumnType = "text";
                }

                ts.Columns.Add(cs);
            }
            return ts;
        }

        private void UpdateStatus(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion

        #region "Top level SQLite Stuff"

        // Create the target file if it doesn't exist.
        private void AssureDBExists()
        {
            if (!File.Exists(DbPath))
            {
                CreateSQLiteDatabaseOnly(DbPath);
                // File.Delete(DbPath)
            }
        }

        private void AssureDBConnection()
        {
            if (mConnection == null)
            {
                var sqliteConnString = CreateSQLiteConnectionString(DbPath, DbPassword);

                // Note: providing true for parseViaFramework as a workaround for reading SqLite files located on UNC or in readonly directories
                mConnection = new SQLiteConnection(sqliteConnString, true);
                mConnection.Open();
            }
        }
        private void CloseDBConnection()
        {
            mConnection?.Close();
        }

        private void CreateTableInDatabase()
        {
            // Create the target file if it doesn't exist.
            AssureDBExists();

            // Prepare a CREATE TABLE DDL statement
            var stmt = BuildCreateTableQuery(mSchema);
            traceLogWriter.Info(Environment.NewLine + Environment.NewLine + stmt + Environment.NewLine + Environment.NewLine);

            try
            {
                // Execute the query in order to actually create the table.
                AssureDBConnection();
                var cmd = new SQLiteCommand(stmt, mConnection);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                traceLogWriter.Error("CreateTableInDatabase failed: " + ex.Message);
            }
            traceLogWriter.Debug("added schema for SQLite table [" + mSchema.TableName + "]");
        }


        private void CopyTabularDataRowsToSQLiteDB()
        {
            // traceLogWriter.Debug("preparing to insert tabular data ...");

            AssureDBConnection();
            var tx = mConnection.BeginTransaction();
            // traceLogWriter.Debug("Starting to insert block of rows for table [" + mSchema.TableName + "]");
            try
            {
                var insert = BuildSQLiteInsert(mSchema, out var columnDataTypes);

                foreach (var row in mRows)
                {
                    insert.Connection = mConnection;
                    insert.Transaction = tx;
                    var paramNames = new List<string>();
                    for (var j = 0; j <= mSchema.Columns.Count - 1; j++)
                    {
                        // Check for the row having fewer columns of data than the header
                        if (j >= row.Length)
                            break;

                        var paramName = "@" + GetNormalizedName(mSchema.Columns[j].ColumnName, paramNames);

                        if (columnDataTypes[j] == DbType.DateTime)
                        {
                            if (DateTime.TryParse(row[j], out var dtDate))
                            {
                                insert.Parameters[paramName].Value = dtDate.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                insert.Parameters[paramName].Value = null;
                            }
                        }
                        else
                        {
                            // Old: insert.Parameters[paramName].Value = CastValueForColumn(row[j], mSchema.Columns[j]);
                            insert.Parameters[paramName].Value = row[j];
                        }
                        paramNames.Add(paramName);
                    }
                    insert.ExecuteNonQuery();
                }

                tx.Commit();

                // traceLogWriter.Debug("finished inserting block of rows for table [" + mSchema.TableName + "]");
            }
            catch (SQLiteException ex)
            {
                tx.Rollback();
                traceLogWriter.Error("unexpected exception: " + ex.Message);
                UpdateStatus("unexpected exception: " + ex.Message);
            }
        }

        private void ExecuteSQLInDatabase(string stmt)
        {
            AssureDBExists();

            traceLogWriter.Info(Environment.NewLine + Environment.NewLine + stmt + Environment.NewLine + Environment.NewLine);

            try
            {
                AssureDBConnection();
                var cmd = new SQLiteCommand(stmt, mConnection);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                traceLogWriter.Error("ExecuteSQLInDatabase failed: " + ex.Message);
            }
            traceLogWriter.Debug("Executed raw SQL");
        }

        #endregion

        #region "General SQLite Stuff"

        /// <summary>
        /// Returns the CREATE TABLE DDL for creating the SQLite table
        /// from the specified table schema object.
        /// </summary>
        private string BuildCreateTableQuery(TableSchema ts)
        {
            var sb = new StringBuilder();

            sb.Append("CREATE TABLE [" + ts.TableName + "] (" + Environment.NewLine);

            for (var i = 0; i <= ts.Columns.Count - 1; i++)
            {
                var col = ts.Columns[i];
                var cline = BuildColumnStatement(col);
                sb.Append(cline);
                if (i < ts.Columns.Count - 1)
                {
                    sb.Append("," + Environment.NewLine);
                }
            }
            sb.Append(Environment.NewLine);
            sb.Append(");" + Environment.NewLine);

            var query = sb.ToString();
            return query;
        }

        /// <summary>
        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        /// </summary>
        private string BuildColumnStatement(ColumnSchema col)
        {
            var sb = new StringBuilder();
            sb.Append("\t" + "\"" + col.ColumnName + "\"" + "\t" + "\t");

            if (col.ColumnType == "int")
            {
                sb.Append("integer");
            }
            else
            {
                sb.Append(col.ColumnType);
            }

            if (!col.IsNullable)
            {
                sb.Append(" NOT NULL");
            }

            var defaultValue = StripParens(col.DefaultValue);
            defaultValue = DiscardNational(defaultValue);
            // traceLogWriter.Debug(("DEFAULT VALUE BEFORE [" & col.DefaultValue & "] AFTER [") + defval & "]")
            if (!string.IsNullOrEmpty(defaultValue) && defaultValue.ToUpper().Contains("GETDATE"))
            {
                traceLogWriter.Debug("converted SQL Server GETDATE() to CURRENT_TIMESTAMP for column [" + col.ColumnName + "]");
                sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            }
            else if (string.IsNullOrEmpty(defaultValue) && IsValidDefaultValue(defaultValue))
            {
                sb.Append(" DEFAULT " + defaultValue);
            }

            return sb.ToString();
        }

        // Creates SQLite connection string from the specified DB file path.
        private static string CreateSQLiteConnectionString(string sqlitePath, string password)
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = sqlitePath
            };

            if (password != null)
            {
                builder.Password = password;
            }
            // builder.PageSize = 4096
            // builder.UseUTF16Encoding = True
            var connstring = builder.ConnectionString;

            return connstring;
        }

        // Creates the SQLite database from the schema read from the SQL Server.
        private static void CreateSQLiteDatabaseOnly(string sqlitePath)
        {
            traceLogWriter.Debug("Creating SQLite database...");

            // Create the SQLite database file
            var dirPath = Path.GetDirectoryName(sqlitePath);
            if (!string.IsNullOrEmpty(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            SQLiteConnection.CreateFile(sqlitePath);

            traceLogWriter.Debug("SQLite file was created successfully at [" + sqlitePath + "]");
        }

        // Creates a command object needed to insert values into a specific SQLite table.
        private SQLiteCommand BuildSQLiteInsert(TableSchema ts, out List<DbType> columnDataTypes)
        {
            var res = new SQLiteCommand();

            columnDataTypes = new List<DbType>(ts.Columns.Count - 1);

            var sb = new StringBuilder();
            sb.Append("INSERT INTO [" + ts.TableName + "] (");
            for (var i = 0; i <= ts.Columns.Count - 1; i++)
            {
                sb.Append("[" + ts.Columns[i].ColumnName + "]");
                if (i < ts.Columns.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(") VALUES (");

            var paramNames = new List<string>();
            for (var i = 0; i <= ts.Columns.Count - 1; i++)
            {
                var paramName = "@" + GetNormalizedName(ts.Columns[i].ColumnName, paramNames);
                sb.Append(paramName);
                if (i < ts.Columns.Count - 1)
                {
                    sb.Append(", ");
                }

                var dbType = GetDbTypeOfColumn(ts.Columns[i]);
                var prm = new SQLiteParameter(paramName, dbType, ts.Columns[i].ColumnName);
                res.Parameters.Add(prm);

                columnDataTypes.Add(dbType);

                // Remember the parameter name in order to avoid duplicates
                paramNames.Add(paramName);
            }

            sb.Append(")");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }

        // Used in order to avoid breaking naming rules (e.g., when a table has
        // a name in SQL Server that cannot be used as a basis for a matching index
        // name in SQLite).
        private string GetNormalizedName(string str, ICollection<string> names)
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= str.Length - 1; i++)
            {
                if (char.IsLetterOrDigit(str[i]) || str[i] == '_')
                {
                    sb.Append(str[i]);
                }
                else
                {
                    sb.Append("_");
                }
            }

            // Avoid returning duplicate name
            if (names.Contains(sb.ToString()))
            {
                return GetNormalizedName(sb + "_", names);
            }

            return sb.ToString();
        }

        // Used in order to adjust the value received from SQL Server for the SQLite database.
        private static object CastValueForColumn(object val, ColumnSchema columnSchema)
        {
            if (val is DBNull)
            {
                return null;
            }

            var dt = GetDbTypeOfColumn(columnSchema);

            switch (dt)
            {
                case DbType.Int32:
                    if (val is string s && string.IsNullOrEmpty(s))
                    {
                        return null;
                    }
                    if (val is short)
                    {
                        return Convert.ToInt32(Convert.ToInt16(val));
                    }
                    if (val is byte)
                    {
                        return Convert.ToInt32(Convert.ToByte(val));
                    }
                    if (val is long)
                    {
                        return Convert.ToInt32(Convert.ToInt64(val));
                    }
                    if (val is decimal)
                    {
                        return Convert.ToInt32(Convert.ToDecimal(val));
                    }
                    break;

                case DbType.Int16:
                    if (val is int)
                    {
                        return Convert.ToInt16(Convert.ToInt32(val));
                    }
                    if (val is byte)
                    {
                        return Convert.ToInt16(Convert.ToByte(val));
                    }
                    if (val is long)
                    {
                        return Convert.ToInt16(Convert.ToInt64(val));
                    }
                    if (val is decimal)
                    {
                        return Convert.ToInt16(Convert.ToDecimal(val));
                    }
                    break;

                case DbType.Int64:
                    if (val is int)
                    {
                        return Convert.ToInt64(Convert.ToInt32(val));
                    }
                    if (val is short)
                    {
                        return Convert.ToInt64(Convert.ToInt16(val));
                    }
                    if (val is byte)
                    {
                        return Convert.ToInt64(Convert.ToByte(val));
                    }
                    if (val is decimal)
                    {
                        return Convert.ToInt64(Convert.ToDecimal(val));
                    }
                    break;

                case DbType.Single:
                    if (val is double)
                    {
                        return Convert.ToSingle(Convert.ToDouble(val));
                    }
                    if (val is decimal)
                    {
                        return Convert.ToSingle(Convert.ToDecimal(val));
                    }
                    break;

                case DbType.Double:
                    if (val is float)
                    {
                        return Convert.ToDouble(Convert.ToSingle(val));
                    }
                    if (val is double)
                    {
                        return Convert.ToDouble(val);
                    }
                    if (val is decimal)
                    {
                        return Convert.ToDouble(Convert.ToDecimal(val));
                    }
                    break;

                case DbType.String:
                    if (val is Guid guid)
                    {
                        return guid.ToString();
                    }
                    break;

                case DbType.Binary:
                case DbType.Boolean:
                case DbType.DateTime:
                    break;

                default:
                    traceLogWriter.Error("argument exception - illegal database type");
                    throw new ArgumentException("Illegal database type [" + Enum.GetName(typeof(DbType), dt) + "]");
            }

            return val;
        }

        /// Matches SQL Server types to general DB types
        private static DbType GetDbTypeOfColumn(ColumnSchema cs)
        {
            if (cs.ColumnType == "tinyint")
            {
                return DbType.Byte;
            }
            if (cs.ColumnType == "int")
            {
                return DbType.Int32;
            }
            if (cs.ColumnType == "smallint")
            {
                return DbType.Int16;
            }
            if (cs.ColumnType == "bigint")
            {
                return DbType.Int64;
            }
            if (cs.ColumnType == "bit")
            {
                return DbType.Boolean;
            }
            if (cs.ColumnType == "nvarchar" || cs.ColumnType == "varchar" || cs.ColumnType == "text" || cs.ColumnType == "ntext")
            {
                return DbType.String;
            }
            if (cs.ColumnType == "float")
            {
                return DbType.Double;
            }
            if (cs.ColumnType == "real")
            {
                return DbType.Single;
            }
            if (cs.ColumnType == "blob")
            {
                return DbType.Binary;
            }
            if (cs.ColumnType == "numeric")
            {
                return DbType.Double;
            }
            if (cs.ColumnType == "timestamp" || cs.ColumnType == "datetime")
            {
                return DbType.DateTime;
            }
            if (cs.ColumnType == "nchar" || cs.ColumnType == "char")
            {
                return DbType.String;
            }
            if (cs.ColumnType == "uniqueidentifier")
            {
                return DbType.String;
            }
            if (cs.ColumnType == "xml")
            {
                return DbType.String;
            }
            if (cs.ColumnType == "sql_variant")
            {
                return DbType.Object;
            }
            if (cs.ColumnType == "integer")
            {
                return DbType.Int64;
            }
            if (cs.ColumnType == "double")
            {
                return DbType.Double;
            }

            traceLogWriter.Error("GetDbTypeOfColumn: illegal db type found");
            throw new MageException("GetDbTypeOfColumn: Illegal DB type found (" + cs.ColumnType + ")");
        }

        // Strip any parentheses from the string.
        private string StripParens(string value)
        {
            var rx = new Regex("\\(([^\\)]*)\\)");
            var m = rx.Match(value);
            if (!m.Success)
            {
                return value;
            }

            return StripParens(m.Groups[1].Value);
        }

        // Check if the DEFAULT clause is valid by SQLite standards
        private static bool IsValidDefaultValue(string value)
        {
            if (IsSingleQuoted(value))
            {
                return true;
            }

            if (!double.TryParse(value, out _))
            {
                return false;
            }
            return true;
        }

        private static bool IsSingleQuoted(string value)
        {
            value = value.Trim();
            if (value.StartsWith("'") && value.EndsWith("'"))
            {
                return true;
            }
            return false;
        }

        // Discards the national prefix if exists (e.g., N'sometext') which is not supported in SQLite.
        private static string DiscardNational(string value)
        {
            var rx = new Regex("N\\'([^\\']*)\\'");
            var m = rx.Match(value);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }

            return value;
        }

        #endregion

        #region "Internal classes for SQLite

        private class ColumnSchema
        {
            public string ColumnName = "";
            public string ColumnType = "";
            public readonly bool IsNullable = true;
            public readonly string DefaultValue = "";
            // public bool IsIdentity = false;
            // public bool IsCaseSensitivite = false; // null??
        }

        private class TableSchema
        {
            public string TableName = "";
            public List<ColumnSchema> Columns;
        }
        #endregion

    }


}
