using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace Mage {

    /// <summary>
    /// Insert data received on standard tabular input
    /// into the specified SQLite database and table.
    /// The table and database will be created if they do not already exist.
    /// </summary>
    public class SQLiteWriter : BaseModule, IDisposable {

        private static readonly ILog traceLog = LogManager.GetLogger("TraceLog"); // traceLog.Debug

        #region Member Variables

        // buffer for accumulating rows into output block
		private List<string[]> mRows = new List<string[]>();

        // description of table we will be inserting rows into
        private TableSchema mSchema = null;

        // connection to SQLite database 
        private SQLiteConnection mConnection = null;

        private int mRowsAccumulated = 0;

        private int mBlockSize = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Name of the table in the database into which the tabular data will be inserted
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Path to the SQLite database file
        /// </summary>
        public string DbPath { get; set; }

        /// <summary>
        /// Password for SQLite database (if there is one)
        /// </summary>
        public string DbPassword { get; set; }

        /// <summary>
        /// number of input rows that are grouped into SQLite transaction blocks 
        /// </summary>
        public string BlockSize {
            get { return mBlockSize.ToString(); }
            set {
                int val = 0;
                if (int.TryParse(value, out val)) {
                    mBlockSize = val;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage SQLite writer module
        /// </summary>
        public SQLiteWriter() {
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// dispose of held resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// dispose of held resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            if (mConnection != null) {
                mConnection.Dispose();
            }

            //           isDisposed = true;
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// called after pipeline run is complete - module can do any special cleanup
        /// this module closes the database connection
        /// (override of base class)
        /// </summary>
        public override void Cleanup() {
            base.Cleanup();
            CloseDBConnection();
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        ///
        /// In addition to base module column definition
        /// forms column description suitable for creating the SQL 
        /// table definition and data insertion commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            // make table schema
            List<MageColumnDef> cd = (OutputColumnDefs == null) ? InputColumnDefs : OutputColumnDefs;
            mSchema = MakeTableSchema(cd);
            // create db and table in database
            CreateTableInDatabase();
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// 
        /// receive data row, add to accumulator, write to SQLite when buffer is full, or reader finishes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                if (OutputColumnDefs != null) {
                    mRows.Add(MapDataRow(args.Fields));
                } else {
                    mRows.Add(args.Fields);
                }
                if (++mRowsAccumulated < mBlockSize) {
                    // accumulate row
                } else {
                    mRowsAccumulated = 0;
                    // do trasaction block against SQLite database
                    CopyTabularDataRowsToSQLiteDB();
                    mRows.Clear();
                }
            } else {
                if (mRowsAccumulated > 0) {
                    // do trasaction block against SQLite database
                    CopyTabularDataRowsToSQLiteDB();
                }
            }
        }

        #endregion

        #region Helper Functions

        private TableSchema MakeTableSchema(List<MageColumnDef> colDefs) {
            TableSchema ts = new TableSchema();
            ts.TableName = TableName;
            ts.Columns = new List<ColumnSchema>();
            foreach (MageColumnDef colDef in colDefs) {
                ColumnSchema cs = new ColumnSchema();
                cs.ColumnName = colDef.Name;
                string type = colDef.DataType;
                if (type.Contains("char")) {
                    cs.ColumnType = "text";
                } else {
                    cs.ColumnType = type;
                }
                ts.Columns.Add(cs);
            }
            return ts;
        }

        private void UpdateStatus(string message) {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion

        #region "Top level SQLite Stuff"

        // create the target file if it doesn't exist.
        private void AssureDBExists() {
            if (!File.Exists(DbPath)) {
                CreateSQLiteDatabaseOnly(DbPath);
                //File.Delete(DbPath)
            }
        }

        private void AssureDBConnection() {
            if (mConnection == null) {
                string sqliteConnString = CreateSQLiteConnectionString(DbPath, DbPassword);
                mConnection = new SQLiteConnection(sqliteConnString);
                mConnection.Open();
            }
        }
        private void CloseDBConnection() {
            if (mConnection != null) {
                mConnection.Close();
            }
        }

        private void CreateTableInDatabase() {
            // create the target file if it doesn't exist.
            AssureDBExists();

            // Prepare a CREATE TABLE DDL statement
            string stmt = BuildCreateTableQuery(mSchema);
            traceLog.Info(System.Environment.NewLine + System.Environment.NewLine + stmt + System.Environment.NewLine + System.Environment.NewLine);

            try {
                // Execute the query in order to actually create the table.
                AssureDBConnection();
                SQLiteCommand cmd = new SQLiteCommand(stmt, mConnection);
                cmd.ExecuteNonQuery();
            } catch (SQLiteException ex) {
                traceLog.Debug("CreateTableInDatabase failed: " + ex.Message);
            }
            traceLog.Debug("added schema for SQLite table [" + mSchema.TableName + "]");
        }


        private void CopyTabularDataRowsToSQLiteDB() {
            // traceLog.Debug("preparing to insert tablular data ...");

            AssureDBConnection();
            SQLiteTransaction tx = mConnection.BeginTransaction();
            // traceLog.Debug("Starting to insert block of rows for table [" + mSchema.TableName + "]");
            try {

                SQLiteCommand insert = BuildSQLiteInsert(mSchema);

				foreach (string[] row in mRows)
				{
                    insert.Connection = mConnection;
                    insert.Transaction = tx;
                    List<string> pnames = new List<string>();
                    for (int j = 0; j <= mSchema.Columns.Count - 1; j++) {
                        string pname = "@" + GetNormalizedName(mSchema.Columns[j].ColumnName, pnames);
                        // Old: insert.Parameters[pname].Value = CastValueForColumn(row[j], mSchema.Columns[j]);
						insert.Parameters[pname].Value = row[j];
                        pnames.Add(pname);
                    }
                    insert.ExecuteNonQuery();
                }// foreach

                tx.Commit();

                // traceLog.Debug("finished inserting block of rows for table [" + mSchema.TableName + "]");
            } catch (SQLiteException ex) {
                tx.Rollback();
                traceLog.Debug("unexpected exception: " + ex.Message);
                UpdateStatus("unexpected exception: " + ex.Message);
            }
        }

        private void ExecuteSQLInDatabase(string stmt) {
            AssureDBExists();

            traceLog.Info(System.Environment.NewLine + System.Environment.NewLine + stmt + System.Environment.NewLine + System.Environment.NewLine);

            try {
                AssureDBConnection();
                SQLiteCommand cmd = new SQLiteCommand(stmt, mConnection);
                cmd.ExecuteNonQuery();
            } catch (SQLiteException ex) {
                traceLog.Debug("ExecuteSQLInDatabase failed: " + ex.Message);
            }
            traceLog.Debug("Executed raw SQL");
        }


        #endregion

        #region "General SQLite Stuff"


        // returns the CREATE TABLE DDL for creating the SQLite table 
        // from the specified table schema object.
        private string BuildCreateTableQuery(TableSchema ts) {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE [" + ts.TableName + "] (" + System.Environment.NewLine);

            for (int i = 0; i <= ts.Columns.Count - 1; i++) {
                ColumnSchema col = ts.Columns[i];
                string cline = BuildColumnStatement(col);
                sb.Append(cline);
                if (i < ts.Columns.Count - 1) {
                    sb.Append("," + System.Environment.NewLine);
                }
            }
            sb.Append(System.Environment.NewLine);
            sb.Append(");" + System.Environment.NewLine);

            string query = sb.ToString();
            return query;
        }

        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        private string BuildColumnStatement(ColumnSchema col) {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t" + "\"" + col.ColumnName + "\"" + "\t" + "\t");

            if (col.ColumnType == "int") {
                sb.Append("integer");
            } else {
                sb.Append(col.ColumnType);
            }
            //End If
            if (!col.IsNullable) {
                sb.Append(" NOT NULL");
            }

            string defval = StripParens(col.DefaultValue);
            defval = DiscardNational(defval);
            //traceLog.Debug(("DEFAULT VALUE BEFORE [" & col.DefaultValue & "] AFTER [") + defval & "]")
            if (!string.IsNullOrEmpty(defval) && defval.ToUpper().Contains("GETDATE")) {
                traceLog.Debug("converted SQL Server GETDATE() to CURRENT_TIMESTAMP for column [" + col.ColumnName + "]");
                sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            } else if (string.IsNullOrEmpty(defval) && IsValidDefaultValue(defval)) {
                sb.Append(" DEFAULT " + defval);
            }

            return sb.ToString();
        }

        // Creates SQLite connection string from the specified DB file path.
        private static string CreateSQLiteConnectionString(string sqlitePath, string password) {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = sqlitePath;
            if (password != null) {
                builder.Password = password;
            }
            //builder.PageSize = 4096
            //builder.UseUTF16Encoding = True
            string connstring = builder.ConnectionString;

            return connstring;
        }

        // Creates the SQLite database from the schema read from the SQL Server.
        private static void CreateSQLiteDatabaseOnly(string sqlitePath) {
            traceLog.Debug("Creating SQLite database...");

            // Create the SQLite database file
            string dirPath = Path.GetDirectoryName(sqlitePath);
            if (!string.IsNullOrEmpty(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            SQLiteConnection.CreateFile(sqlitePath);

            traceLog.Debug("SQLite file was created successfully at [" + sqlitePath + "]");
        }

        // Creates a command object needed to insert values into a specific SQLite table.
        private SQLiteCommand BuildSQLiteInsert(TableSchema ts) {
            SQLiteCommand res = new SQLiteCommand();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO [" + ts.TableName + "] (");
            for (int i = 0; i <= ts.Columns.Count - 1; i++) {
                sb.Append("[" + ts.Columns[i].ColumnName + "]");
                if (i < ts.Columns.Count - 1) {
                    sb.Append(", ");
                }
            }
            // for
            sb.Append(") VALUES (");

            List<string> pnames = new List<string>();
            for (int i = 0; i <= ts.Columns.Count - 1; i++) {
                string pname = "@" + GetNormalizedName(ts.Columns[i].ColumnName, pnames);
                sb.Append(pname);
                if (i < ts.Columns.Count - 1) {
                    sb.Append(", ");
                }

                DbType dbType = GetDbTypeOfColumn(ts.Columns[i]);
                SQLiteParameter prm = new SQLiteParameter(pname, dbType, ts.Columns[i].ColumnName);
                res.Parameters.Add(prm);

                // Remember the parameter name in order to avoid duplicates
                pnames.Add(pname);
            }
            // for
            sb.Append(")");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }

        // Used in order to avoid breaking naming rules (e.g., when a table has
        // a name in SQL Server that cannot be used as a basis for a matching index
        // name in SQLite).
        private string GetNormalizedName(string str, List<string> names) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= str.Length - 1; i++) {
                if (Char.IsLetterOrDigit(str[i]) || str[i] == '_') {
                    sb.Append(str[i]);
                } else {
                    sb.Append("_");
                }
            }
            // for
            // Avoid returning duplicate name
            if (names.Contains(sb.ToString())) {
                return GetNormalizedName(sb.ToString() + "_", names);
            } else {
                return sb.ToString();
            }
        }

        // Used in order to adjust the value received from SQL Server for the SQLite database.
        private static object CastValueForColumn(object val, ColumnSchema columnSchema) {
            if (val is DBNull) {
                return null;
            }

            DbType dt = GetDbTypeOfColumn(columnSchema);

            switch (dt) {
                case DbType.Int32:
                    if (val is string && string.IsNullOrEmpty((string)val)) {
                        return null;
                    }
                    if (val is short) {
                        return Convert.ToInt32(Convert.ToInt16(val));
                    }
                    if (val is byte) {
                        return Convert.ToInt32(Convert.ToByte(val));
                    }
                    if (val is long) {
                        return Convert.ToInt32(Convert.ToInt64(val));
                    }
                    if (val is decimal) {
                        return Convert.ToInt32(Convert.ToDecimal(val));
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.Int16:
                    if (val is int) {
                        return Convert.ToInt16(Convert.ToInt32(val));
                    }
                    if (val is byte) {
                        return Convert.ToInt16(Convert.ToByte(val));
                    }
                    if (val is long) {
                        return Convert.ToInt16(Convert.ToInt64(val));
                    }
                    if (val is decimal) {
                        return Convert.ToInt16(Convert.ToDecimal(val));
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.Int64:
                    if (val is int) {
                        return Convert.ToInt64(Convert.ToInt32(val));
                    }
                    if (val is short) {
                        return Convert.ToInt64(Convert.ToInt16(val));
                    }
                    if (val is byte) {
                        return Convert.ToInt64(Convert.ToByte(val));
                    }
                    if (val is decimal) {
                        return Convert.ToInt64(Convert.ToDecimal(val));
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.Single:
                    if (val is double) {
                        return Convert.ToSingle(Convert.ToDouble(val));
                    }
                    if (val is decimal) {
                        return Convert.ToSingle(Convert.ToDecimal(val));
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.Double:
                    if (val is float) {
                        return Convert.ToDouble(Convert.ToSingle(val));
                    }
                    if (val is double) {
                        return Convert.ToDouble(val);
                    }
                    if (val is decimal) {
                        return Convert.ToDouble(Convert.ToDecimal(val));
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.String:
                    if (val is Guid) {
                        return ((Guid)val).ToString();
                    }
                    break; // TODO: might not be correct. Was : Exit Select

                case DbType.Binary:
                case DbType.Boolean:
                case DbType.DateTime:
                    break; // TODO: might not be correct. Was : Exit Select
                default:

                    traceLog.Error("argument exception - illegal database type");
                    throw new ArgumentException("Illegal database type [" + Enum.GetName(typeof(DbType), dt) + "]");
            }
            // switch
            return val;
        }

        /// Matches SQL Server types to general DB types
        private static DbType GetDbTypeOfColumn(ColumnSchema cs) {
            if (cs.ColumnType == "tinyint") {
                return DbType.Byte;
            }
            if (cs.ColumnType == "int") {
                return DbType.Int32;
            }
            if (cs.ColumnType == "smallint") {
                return DbType.Int16;
            }
            if (cs.ColumnType == "bigint") {
                return DbType.Int64;
            }
            if (cs.ColumnType == "bit") {
                return DbType.Boolean;
            }
            if (cs.ColumnType == "nvarchar" || cs.ColumnType == "varchar" || cs.ColumnType == "text" || cs.ColumnType == "ntext") {
                return DbType.String;
            }
            if (cs.ColumnType == "float") {
                return DbType.Double;
            }
            if (cs.ColumnType == "real") {
                return DbType.Single;
            }
            if (cs.ColumnType == "blob") {
                return DbType.Binary;
            }
            if (cs.ColumnType == "numeric") {
                return DbType.Double;
            }
            if (cs.ColumnType == "timestamp" || cs.ColumnType == "datetime") {
                return DbType.DateTime;
            }
            if (cs.ColumnType == "nchar" || cs.ColumnType == "char") {
                return DbType.String;
            }
            if (cs.ColumnType == "uniqueidentifier") {
                return DbType.String;
            }
            if (cs.ColumnType == "xml") {
                return DbType.String;
            }
            if (cs.ColumnType == "sql_variant") {
                return DbType.Object;
            }
            if (cs.ColumnType == "integer") {
                return DbType.Int64;
            }
            if (cs.ColumnType == "double") {
                return DbType.Double;
            }

            traceLog.Error("GetDbTypeOfColumn: illegal db type found");
            throw new MageException("GetDbTypeOfColumn: Illegal DB type found (" + cs.ColumnType + ")");
        }

        // Strip any parentheses from the string.
        private string StripParens(string value) {
            Regex rx = new Regex("\\(([^\\)]*)\\)");
            Match m = rx.Match(value);
            if (!m.Success) {
                return value;
            } else {
                return StripParens(m.Groups[1].Value);
            }
        }

        // Check if the DEFAULT clause is valid by SQLite standards
        private static bool IsValidDefaultValue(string value) {
            if (IsSingleQuoted(value)) {
                return true;
            }

            double testnum = 0;
            if (!double.TryParse(value, out testnum)) {
                return false;
            }
            return true;
        }

        private static bool IsSingleQuoted(string value) {
            value = value.Trim();
            if (value.StartsWith("'") && value.EndsWith("'")) {
                return true;
            }
            return false;
        }

        // Discards the national prefix if exists (e.g., N'sometext') which is not supported in SQLite.
        private static string DiscardNational(string value) {
            Regex rx = new Regex("N\\'([^\\']*)\\'");
            Match m = rx.Match(value);
            if (m.Success) {
                return m.Groups[1].Value;
            } else {
                return value;
            }
        }

        #endregion

        #region "Internal classes for SQLite

        private class ColumnSchema {
            public string ColumnName = "";
            public string ColumnType = "";
            public bool IsNullable = true;
            public string DefaultValue = "";
            //           public bool IsIdentity = false;
            //           public bool IsCaseSensitivite = false; // null??
        }

        private class TableSchema {
            public string TableName = "";
            public List<ColumnSchema> Columns = null;
        }
        #endregion

    }


}
