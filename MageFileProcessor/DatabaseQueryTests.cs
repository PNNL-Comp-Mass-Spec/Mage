using Mage;
using PRISM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MageFileProcessor
{
    // ReSharper disable once UnusedMember.Global
    internal class DatabaseQueryTests
    {
        // ReSharper disable UnusedMember.Global

        internal const string DMS_READER = "dmsreader";
        internal const string DMS_READER_PASSWORD = "dms4fun";

        // ReSharper restore UnusedMember.Global

        private static void CheckQueryResults(
            SimpleSink sink,
            int expectedRows,
            IReadOnlyList<string> expectedColumnNames,
            string serverName,
            string databaseName,
            string username,
            string expectedSqlOrSProc)
        {
            Console.WriteLine();

            string errorMessage;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "using integrated authentication, for " + expectedSqlOrSProc;
            }
            else
            {
                errorMessage = "as user " + username + ", for " + expectedSqlOrSProc;
            }

            // Did the test sink module get the expected row definitions
            var actualColumns = sink.Columns;

            if (actualColumns.Count == 0)
            {
                ConsoleMsgUtils.ShowError("Did not retrieve data from database {0} on server {1} using {2}", databaseName, serverName, expectedSqlOrSProc);
                return;
            }

            if (expectedColumnNames.Count != actualColumns.Count)
            {
                ConsoleMsgUtils.ShowWarning("Column count mismatch " + errorMessage);
            }

            for (var i = 0; i < actualColumns.Count; i++)
            {
                if (expectedColumnNames[i] != actualColumns[i].Name)
                {
                    ConsoleMsgUtils.ShowWarning("Did not get the expected row definitions {0} (column is named {1}, but expecting {2})", errorMessage, actualColumns[i].Name, expectedColumnNames[i]);
                }
            }

            // Did the test sink module get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;

            const int COL_COUNT_TO_SHOW = 6;

            // Keys in this dictionary are column index; values are the maximum length of the data in that column
            var columnWidths = new Dictionary<int, int>();

            var headerNames = (from item in actualColumns.Take(COL_COUNT_TO_SHOW) select item.Name).ToList();

            for (var i = 0; i < headerNames.Count; i++)
            {
                columnWidths.Add(i, headerNames[i].Length);
            }

            // Display the 10 rows of results (limiting to the first 6 columns)
            // Determine the optimal column width for each column
            foreach (var currentRow in rows.Take(10))
            {
                for (var i = 0; i < currentRow.Length; i++)
                {
                    if (i >= COL_COUNT_TO_SHOW)
                        break;

                    var dataLength = currentRow[i].Length;
                    if (dataLength > columnWidths[i])
                        columnWidths[i] = dataLength;
                }
            }

            // Show the column headers
            var paddedHeaderNames = PadData(headerNames, columnWidths);
            Console.WriteLine(string.Join("  ", paddedHeaderNames));

            // Show the column values
            foreach (var currentRow in rows.Take(10))
            {
                var dataValues = PadData(currentRow.Take(COL_COUNT_TO_SHOW).ToList(), columnWidths);
                Console.WriteLine(string.Join("  ", dataValues));
            }

            if (expectedRows != rows.Count)
            {
                ConsoleMsgUtils.ShowWarning("Did not get the expected number of data rows {0} ({1} obtained instead of {2})", rows.Count, expectedRows, errorMessage);
            }

            // Are there the expected number of fields in the data row?
            if (expectedColumnNames.Count != rows[0].Length)
            {
                ConsoleMsgUtils.ShowWarning("Data rows do not have the expected number of fields {0} ({1} obtained instead of {2})", expectedRows, rows[0].Length, errorMessage);
            }
        }

        // ReSharper disable once UnusedMember.Global
#pragma warning disable RCS1213, IDE0051
        internal void DMSSprocReadTest(string serverName, string databaseName, bool isPostgres, string username, string password)
#pragma warning restore RCS1213,IDE0051
        {
            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password, isPostgres);
            RegisterEvents(target);

            const int maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database stored procedure query
            // Defaults are prismdb2.emsl.pnl.gov and dms
            target.SprocName = "predefined_analysis_rules_proc";

            // ReSharper disable once StringLiteralTypo
            target.SetSprocParam("@datasetName", "QC_Mam_19_01_d_09Aug22_Pippin_WBEH-22-02-04-50u");

            target.Run(null);

            var columnList = new List<string>();

            if (isPostgres)
            {
                columnList.Add("_results");
                columnList.Add("_message");
                // ReSharper disable once StringLiteralTypo
                columnList.Add("_returncode");
            }
            else
            {
                columnList.AddRange(
                    new List<string>
                    {
                        "step",
                        "level",
                        "seq",
                        "predefine_id",
                        "next_lvl",
                        "trigger_mode",
                        "export_mode",
                        "action",
                        "reason",
                        "notes",
                        "analysis_tool",
                        "instrument_class_criteria",
                        "instrument_criteria",
                        "instrument_exclusion",
                        "campaign_criteria",
                        "campaign_exclusion",
                        "experiment_criteria",
                        "experiment_exclusion",
                        "organism_criteria",
                        "dataset_criteria",
                        "dataset_exclusion",
                        "dataset_type",
                        "scan_type_criteria",
                        "scan_type_exclusion",
                        "exp_comment_criteria",
                        "labelling_inclusion",
                        "labelling_exclusion",
                        "separation_type_criteria",
                        "scan_count_min",
                        "scan_count_max",
                        "param_file",
                        "settings_file",
                        "organism",
                        "protein_collections",
                        "protein_options",
                        "organism_db",
                        "special_processing",
                        "priority"
                    });
            }

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;

            var expectedRows = isPostgres ? 1 : 4;

            CheckQueryResults(sink, expectedRows, columnList, serverName, databaseName, username, sprocInfo);
        }

        private static List<string> PadData(IReadOnlyList<string> dataValues, IReadOnlyDictionary<int, int> columnWidths)
        {
            var paddedData = new List<string>();

            for (var i = 0; i < dataValues.Count; i++)
            {
                if (i >= columnWidths.Count)
                    break;

                var columnWidth = columnWidths[i];
                paddedData.Add(dataValues[i].PadRight(columnWidth));
            }

            return paddedData;
        }

        // ReSharper disable once UnusedMember.Global

        /// <summary>
        /// Get factors for dataset name
        /// </summary>
#pragma warning disable RCS1213, IDE0051
        internal void QueryDatasetFactorsNamedUser(string serverName, string databaseName, bool isPostgres, string username, string password)
#pragma warning restore RCS1213,IDE0051
        {
            QueryDatasetFactors(serverName, databaseName, username, password, isPostgres);
        }

        private void QueryDatasetFactors(string serverName, string databaseName, string username, string password, bool isPostgres)
        {
            // Default server is prismdb2.emsl.pnl.gov
            if (string.IsNullOrWhiteSpace(serverName))
            {
                serverName = Globals.DMSServer;
            }

            // Default database is dms
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = Globals.DMSDatabase;
            }

            // Create SQLReader module and test sink module
            // and connect together
            var reader = new SQLReader(serverName, databaseName, username, password, isPostgres);
            RegisterEvents(reader);

            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            reader.ColumnDefAvailable += sink.HandleColumnDef;
            reader.DataRowAvailable += sink.HandleDataRow;

            // Define query
            var columnList = new[] { "dataset", "dataset_id", "factor", "value" };
            var colNames = string.Join(", ", columnList);

            var builder = new SQLBuilder
            {
                Table = "V_Custom_Factors_List_Report",
                Columns = colNames,
                IsPostgres = isPostgres
            };

            // The query builder will convert the following predicate to "Dataset LIKE '%sarc%'"
            builder.AddPredicateItem("dataset", "sarc");

            reader.SQLText = builder.BuildQuerySQL();

            reader.Run(null);

            CheckQueryResults(sink, maxRows, columnList, serverName, databaseName, username, reader.SQLText);
        }

        private static void RegisterEvents(IBaseModule mageModule)
        {
            mageModule.StatusMessageUpdated += MageModule_StatusMessageUpdated;
            mageModule.MageExceptionReported += MageModule_ErrorMessageUpdated;
            mageModule.WarningMessageUpdated += MageModule_WarningMessageUpdated;
        }

        private static void MageModule_StatusMessageUpdated(object sender, MageStatusEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private static void MageModule_ErrorMessageUpdated(object sender, MageExceptionEventArgs e)
        {
            ConsoleMsgUtils.ShowError(e.Message, e.Exception);
        }

        private static void MageModule_WarningMessageUpdated(object sender, MageStatusEventArgs e)
        {
            ConsoleMsgUtils.ShowWarning(e.Message);
        }
    }
}
