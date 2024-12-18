﻿using System;
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
        // Ignore Spelling: bool, Mage, Nullable, sqlite, varchar, yyyy-MM-dd, HH:mm:ss

        private static readonly FileLogger traceLogWriter = new(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        /// <summary>
        /// Buffer for accumulating rows into output block
        /// </summary>
        private readonly List<string[]> mRows = new();

        /// <summary>
        /// Description of table we will be inserting rows into
        /// </summary>
        private TableSchema mSchema;

        /// <summary>
        /// Connection to SQLite database
        /// </summary>
        private SQLiteConnection mConnection;

        private int mRowsAccumulated;

        private int mBlockSize = 1000;

        /// <summary>
        /// Name of the table in the database into which the tabular data will be inserted
        /// </summary>
        public string TableName { get; set; }

        // ReSharper disable once GrammarMistakeInComment

        /// <summary>
        /// Optional list of column definitions that will be used when creating the target table in the SqLite database
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

            // Code to dispose the unmanaged resources of the class
            mConnection?.Dispose();

            // isDisposed = true;
        }

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
        /// <para>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </para>
        /// <para>
        /// In addition to base module column definition
        /// forms column description suitable for creating the SQL
        /// table definition and data insertion commands
        /// </para>
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
        /// <para>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </para>
        /// <para>
        /// Receive data row, add to accumulator, write to SQLite when buffer is full, or reader finishes
        /// </para>
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

        // Helper methods

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
                        if (string.Equals(overrideDef.Name, cs.ColumnName, StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Create the target file if it doesn't exist.
        /// </summary>
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

                // Note: providing true for parseViaFramework as a workaround for reading SqLite files located on UNC or in read only directories
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

            try
            {
                // Check whether the table already exists
                // Note that this will call AssureDBConnection();

                if (TableExists(mSchema.TableName))
                {
                    traceLogWriter.Info("SQLite table already exists: [" + mSchema.TableName + "]");
                    return;
                }

                // Prepare a CREATE TABLE DDL statement
                var query = BuildCreateTableQuery(mSchema);
                traceLogWriter.Info(Environment.NewLine + Environment.NewLine + query + Environment.NewLine + Environment.NewLine);

                // Execute the query in order to actually create the table.
                var cmd = new SQLiteCommand(query, mConnection);
                cmd.ExecuteNonQuery();

                traceLogWriter.Debug("added schema for SQLite table [" + mSchema.TableName + "]");
            }
            catch (SQLiteException ex)
            {
                traceLogWriter.Error("CreateTableInDatabase failed: " + ex.Message);
            }
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

        /// <summary>
        /// Returns the CREATE TABLE DDL for creating the SQLite table
        /// from the specified table schema object.
        /// </summary>
        private string BuildCreateTableQuery(TableSchema ts)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("CREATE TABLE [{0}] ({1}", ts.TableName, Environment.NewLine);

            for (var i = 0; i <= ts.Columns.Count - 1; i++)
            {
                var col = ts.Columns[i];
                var columnLine = BuildColumnStatement(col);
                sb.Append(columnLine);
                if (i < ts.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }
            sb.AppendLine();
            sb.AppendLine(");");

            return sb.ToString();
        }

        /// <summary>
        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        /// </summary>
        private string BuildColumnStatement(ColumnSchema col)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("\t\"{0}\"\t\t", col.ColumnName);

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

            var defaultValue = DiscardUnicodePrefix(StripParens(col.DefaultValue));

            // traceLogWriter.Debug(("DEFAULT VALUE BEFORE [" & col.DefaultValue & "] AFTER [") + defaultValue & "]")
            if (!string.IsNullOrEmpty(defaultValue) && defaultValue.IndexOf("GETDATE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                traceLogWriter.Debug("converted SQL Server GETDATE() to CURRENT_TIMESTAMP for column [" + col.ColumnName + "]");
                sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            }
            else if (string.IsNullOrEmpty(defaultValue) && IsValidDefaultValue(defaultValue))
            {
                sb.AppendFormat(" DEFAULT {0}", defaultValue);
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
            return builder.ConnectionString;
        }

        /// <summary>
        /// Creates a SQLite database
        /// </summary>
        /// <param name="sqlitePath"></param>
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
            sb.AppendFormat("INSERT INTO [{0}] (", ts.TableName);

            for (var i = 0; i <= ts.Columns.Count - 1; i++)
            {
                sb.AppendFormat("[{0}]", ts.Columns[i].ColumnName);
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

        /// <summary>
        /// Matches database data types to .NET types
        /// </summary>
        /// <param name="cs"></param>
        private static DbType GetDbTypeOfColumn(ColumnSchema cs)
        {
            var success = PRISMDatabaseUtils.DBToolsBase.GetDbTypeByDataTypeName(cs.ColumnType, out var dataType, out _);
            if (success)
                return dataType;

            var errorMessage = string.Format("GetDbTypeOfColumn: invalid DB type found for column {0}: {1}", cs.ColumnName, cs.ColumnType);

            traceLogWriter.Error(errorMessage);
            throw new MageException(errorMessage);
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

        private bool TableExists(string tableName)
        {
            try
            {
                AssureDBConnection();

                var query = string.Format("SELECT count(*) as Items FROM sqlite_master WHERE type='table' AND name='{0}' COLLATE NOCASE;", tableName);

                var cmd = new SQLiteCommand(query, mConnection);
                var result = cmd.ExecuteScalar();

                return Convert.ToInt32(result) > 0;
            }
            catch (SQLiteException ex)
            {
                traceLogWriter.Error("TableExists check failed: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Check if the DEFAULT clause is valid by SQLite standards
        /// </summary>
        /// <param name="value"></param>
        private static bool IsValidDefaultValue(string value)
        {
            if (IsSingleQuoted(value))
            {
                return true;
            }

            return double.TryParse(value, out _);
        }

        private static bool IsSingleQuoted(string value)
        {
            value = value.Trim();
            return value.StartsWith("'") && value.EndsWith("'");
        }

        /// <summary>
        /// Discards the Unicode prefix if it exists (e.g., N'SomeText') which is not supported in SQLite.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>String without leading N'</returns>
        private static string DiscardUnicodePrefix(string value)
        {
            var rx = new Regex(@"N\'([^\']*)\'");
            var m = rx.Match(value);
            return m.Success ? m.Groups[1].Value : value;
        }

        private class ColumnSchema
        {
            public string ColumnName = string.Empty;
            public string ColumnType = string.Empty;
            public readonly bool IsNullable = true;
            public readonly string DefaultValue = string.Empty;
            // public bool IsIdentity = false;
            // public bool IsCaseSensitive = false; // null??

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(ColumnName))
                    return "Unnamed column";

                if (string.IsNullOrWhiteSpace(ColumnType))
                    return ColumnName;

                return string.Format("{0}: {1}", ColumnName, ColumnType);
            }
        }

        private class TableSchema
        {
            public string TableName = string.Empty;
            public List<ColumnSchema> Columns;
        }
    }
}
