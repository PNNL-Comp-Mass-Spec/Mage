using Mage;
using PRISM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MageFileProcessor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Ignore Spelling: Mage

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Uncomment to debug calling procedure predefined_analysis_rules_proc
                // (to see the console messages, change the Output Type of this project to Console Application)

                // DMSSprocReadTest(Globals.DMSServer, Globals.DMSDatabase, Globals.PostgresDMS, "dmsreader", "dms4fun");

                Application.Run(new FileProcessorForm());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Critical error: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                var fiExe = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (fiExe.FullName.StartsWith(@"\\") && fiExe.Directory != null)
                {
                    MessageBox.Show(string.Format(
                                        "You are running this program from a network share. " +
                                        "Try copying directory {0} to your local computer and then re-running {1}",
                                        fiExe.Directory.Name, fiExe.Name),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
#pragma warning disable RCS1213, IDE0051
        private static void DMSSprocReadTest(string serverName, string databaseName, bool isPostgres, string username, string password)
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
            target.SetSprocParam("@datasetName", "QC_Mam_19_01_d_09Aug22_Pippin_WBEH-22-02-04-50u");

            target.Run(null);

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;
            CheckQueryResults(sink, serverName, databaseName, sprocInfo);
        }

        private static void CheckQueryResults(
            SimpleSink sink,
            string serverName,
            string databaseName,
            string expectedSqlOrSProc)
        {
            // Did the test sink module get the expected row definitions
            var cols = sink.Columns;

            if (cols.Count == 0)
            {
                Console.WriteLine("Did not retrieve data from database {0} on server {1} using {2}", databaseName, serverName, expectedSqlOrSProc);
            }

            // Did the test sink module get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;

            const int COL_COUNT_TO_SHOW = 6;

            // Keys in this dictionary are column index; values are the maximum length of the data in that column
            var columnWidths = new Dictionary<int, int>();

            var headerNames = (from item in cols.Take(COL_COUNT_TO_SHOW) select item.Name).ToList();
            for (var i = 0; i < headerNames.Count; i++)
            {
                columnWidths.Add(i, headerNames[i].Length);
            }

            // Display the first 10 rows of results (limiting to the first 6 columns)
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
