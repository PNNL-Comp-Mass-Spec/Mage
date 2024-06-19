using System;
using Mage;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using PRISM;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for MSSQLReaderTest and is intended
    /// to contain all MSSQLReaderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SQLReaderTest
    {
        // ReSharper disable CommentTypo

        // Ignore Spelling: dmsreader, labelling, mtuser, Postgres, sarc, sproc, username

        // ReSharper restore CommentTypo

        private const string DMS_READER = "dmsreader";
        private const string DMS_READER_PASSWORD = "dms4fun";

        private const string MTS_READER = "mtuser";
        private const string MTS_READER_PASSWORD = "mt4fun";

        /// <summary>
        /// A test for sqlText
        /// </summary>
        [Test]
        public void SQLTextPropertyTest()
        {
            var target = new SQLReader();
            RegisterEvents(target);

            const string expected = "Test Value";
            target.SQLText = expected;
            var actual = target.SQLText;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for sprocName
        /// </summary>
        [Test]
        public void SprocNamePropertyTest()
        {
            var target = new SQLReader();
            RegisterEvents(target);

            const string expected = "Test Value";
            target.SprocName = expected;
            var actual = target.SprocName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for server
        /// </summary>
        [Test]
        public void ServerPropertyTest()
        {
            var target = new SQLReader();
            RegisterEvents(target);

            const string expected = "Test Value";
            target.Server = expected;
            var actual = target.Server;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for database
        /// </summary>
        [Test]
        public void DatabasePropertyTest()
        {
            var target = new SQLReader();
            RegisterEvents(target);

            const string expected = "Test Value";
            target.Database = expected;
            var actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        [Category("DatabaseIntegrated")]
        public void XMLQueryDatasetFactorsIntegrated(string queryDefinitionsPath)
        {
            XMLQueryDatasetFactors(queryDefinitionsPath, "", "");
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        [Category("DatabaseNamedUser")]
        public void XMLQueryDatasetFactorsNamedUser(string queryDefinitionsPath)
        {
            XMLQueryDatasetFactors(queryDefinitionsPath, DMS_READER, DMS_READER_PASSWORD);
        }

        public void XMLQueryDatasetFactors(string queryDefinitionsPath, string username, string password)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>
            {
                ["Dataset"] = "sarc"
            };

            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;

            // Get XML query definition by name
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            // Create SQLReader module initialized from XML definition
            var target = new SQLReader(queryDefXML, runtimeParameters, username, password);
            Assert.AreNotEqual(null, target);
            RegisterEvents(target);

            const string expectedSql = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset LIKE '%sarc%'";
            Assert.AreEqual(expectedSql, target.SQLText);

            // Create test sink module and connect to SQLReader module
            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            CheckQueryResults(sink, maxRows, colList, target.Server, target.Database, username, expectedSql);
        }

        /// <summary>
        /// Test to get factors for dataset name
        /// </summary>
        [Test]
        [TestCase("", "", false)]
        [Category("DatabaseIntegrated")]
        public void QueryDatasetFactorsIntegrated(string serverName, string databaseName, bool isPostgres)
        {
            QueryDatasetFactors(serverName, databaseName, "", "", isPostgres);
        }

        /// <summary>
        /// Test to get factors for dataset name
        /// </summary>
        [Test]
        [TestCase("", "", DMS_READER, DMS_READER_PASSWORD, false)]
        [TestCase("prismdb1", "dms", DMS_READER, DMS_READER_PASSWORD, true)]
        [Category("DatabaseNamedUser")]
        public void QueryDatasetFactorsNamedUser(string serverName, string databaseName, string userName, string userPassword, bool isPostgres)
        {
            QueryDatasetFactors(serverName, databaseName, userName, userPassword, isPostgres);
        }

        private void QueryDatasetFactors(string serverName, string databaseName, string username, string password, bool isPostgres)
        {
            // Default server is gigasax
            if (string.IsNullOrWhiteSpace(serverName))
            {
                serverName = Globals.DMSServer;
            }

            // Default database is DMS5
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
            var colList = new[] { "dataset", "dataset_id", "factor", "value" };
            var colNames = string.Join(", ", colList);

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

            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, reader.SQLText);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        /// </summary>
        [Test]
        [Category("DatabaseIntegrated")]
        public void QueryWithSQLBuilderTestIntegrated()
        {
            QueryWithSQLBuilderTest("", "");
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        /// </summary>
        [Test]
        [Category("DatabaseNamedUser")]
        public void QueryWithSQLBuilderTestNamedUser()
        {
            QueryWithSQLBuilderTest(DMS_READER, DMS_READER_PASSWORD);
        }

        private void QueryWithSQLBuilderTest(string username, string password)
        {
            var serverName = Globals.DMSServer;
            var databaseName = Globals.DMSDatabase;
            var isPostgres = Globals.PostgresDMS;

            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database query

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] {
                "DatasetID", "volName", "path", "datasetFolder", "resultsFolder",
                "datasetName", "JobId", "ColumnID", "AcquisitionTime", "Labelling",
                "InstrumentName", "ToolID", "BlockNum", "ReplicateName", "ExperimentID",
                "RunOrder", "BatchID", "ArchPath", "DatasetFullPath", "Organism", "Campaign",
                "ParameterFileName", "SettingsFileName" };
            var colNames = string.Join(", ", colList);

            // Define query
            var builder = new SQLBuilder
            {
                Table = "V_Analysis_Job_Export_MultiAlign",
                Columns = colNames,
                IsPostgres = isPostgres
            };
            builder.AddPredicateItem("datasetName", "ContainsText", "sarc_ms");
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, target.SQLText);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        /// </summary>
        [Test]
        [Category("DatabaseIntegrated")]
        public void QueryAllMTDBsIntegrated()
        {
            QueryAllMTDBs("", "");
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        /// </summary>
        [Test]
        [Category("DatabaseNamedUser")]
        public void QueryAllMTDBsNamedUser()
        {
            QueryAllMTDBs(MTS_READER, MTS_READER_PASSWORD);
        }

        private void QueryAllMTDBs(string username, string password)
        {
            const string serverName = "Pogo";
            const string databaseName = "MTS_Master";
            const bool isPostgres = false;

            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 5;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database query

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] { "server_name", "mt_db_id", "mt_db_name", "state_id", "state" };
            var colNames = string.Join(", ", colList);

            // Define query
            var builder = new SQLBuilder
            {
                Table = "V_MT_DBs",
                Columns = colNames,
                IsPostgres = isPostgres
            };
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, target.SQLText);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// </summary>
        [Test]
        [Category("DatabaseIntegrated")]
        public void QueryTestIntegrated()
        {
            QueryTest("", "");
        }

        /// <summary>
        /// A test for straight SQL query against DMS on Gigasax
        /// </summary>
        [Test]
        [Category("DatabaseNamedUser")]
        public void QueryTestNamedUser()
        {
            QueryTest(DMS_READER, DMS_READER_PASSWORD);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// </summary>
        private void QueryTest(string username, string password)
        {
            var serverName = Globals.DMSServer;
            var databaseName = Globals.DMSDatabase;
            var isPostgres = Globals.PostgresDMS;

            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign" };
            var colNames = string.Join(", ", colList);

            // Define and run a database query
            // Defaults are gigasax and DMS5

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (isPostgres)
            {
                target.SQLText = string.Format("SELECT {0} FROM V_Mage_Analysis_Jobs WHERE Instrument LIKE '%test%' LIMIT {1}", colNames, maxRows.ToString());
            }
            else
            {
                target.SQLText = string.Format("SELECT TOP {1} {0} FROM V_Mage_Analysis_Jobs WHERE Instrument LIKE '%test%'", colNames, maxRows.ToString());
            }

            target.Run(null);

            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, target.SQLText);
        }

        [Test]
        [Category("DatabaseIntegrated")]
        public void DMSSprocTestIntegrated()
        {
            DMSSprocReadTest("", "");
        }

        [Test]
        [Category("DatabaseNamedUser")]
        public void DMSSprocTestNamedUser()
        {
            DMSSprocReadTest(DMS_READER, DMS_READER_PASSWORD);
        }

        private void DMSSprocReadTest(string username, string password)
        {
            var serverName = Globals.DMSServer;
            var databaseName = Globals.DMSDatabase;

            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database sproc query
            // Defaults are gigasax and DMS5
            target.SprocName = "find_existing_jobs_for_request";
            target.SetSprocParam("@requestID", "8142");

            target.Run(null);

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] {
                "job", "state", "priority", "request", "created", "start", "finish", "processor",
                "dataset" };

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;
            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, sprocInfo);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        [Category("DatabaseIntegrated")]
        public void XMLMTSSprocReadTestIntegrated(string queryDefinitionsPath)
        {
            XMLMTSSprocReadTest(queryDefinitionsPath, "", "");
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        [Category("DatabaseNamedUser")]
        public void XMLMTSSprocReadTestNamedUser(string queryDefinitionsPath)
        {
            XMLMTSSprocReadTest(queryDefinitionsPath, MTS_READER, MTS_READER_PASSWORD);
        }

        private void XMLMTSSprocReadTest(string queryDefinitionsPath, string username, string password)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>();

            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;

            // Get XML query definition by name
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            // Create SQLReader module initialized from XML definition
            var target = new SQLReader(queryDefXML, runtimeParameters, username, password);
            Assert.AreNotEqual(null, target);
            RegisterEvents(target);

            const string expected = "GetMassTagsPlusPepProphetStats";
            Assert.AreEqual(expected, target.SprocName);

            // Create test sink module and connect to SQLReader module
            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] {
                "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET",
                "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter",
                "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2",
                "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3",
                "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3",
                "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;
            CheckQueryResults(sink, maxRows, colList, target.Server, target.Database, username, sprocInfo);
        }

        [Test]
        [Category("DatabaseIntegrated")]
        public void MTSSprocReadTestIntegrated()
        {
            MTSSprocReadTest("", "");
        }

        [Test]
        [Category("DatabaseNamedUser")]
        public void MTSSprocReadTestNamedUser()
        {
            MTSSprocReadTest(MTS_READER, MTS_READER_PASSWORD);
        }

        private void MTSSprocReadTest(string username, string password)
        {
            const string serverName = "Pogo";
            const string databaseName = "MT_Shewanella_ProdTest_Formic_P1385";

            // Create SQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database sproc query
            target.SprocName = "GetMassTagsPlusPepProphetStats";

            // Set argument values (may be omitted if argument has acceptable default value
            target.SetSprocParam("@MassCorrectionIDFilterList", "");
            target.SetSprocParam("@ConfirmedOnly", "0");
            target.SetSprocParam("@MinimumHighNormalizedScore", "0");
            target.SetSprocParam("@MinimumPMTQualityScore", "4");
            target.SetSprocParam("@NETValueType", "0");
            target.SetSprocParam("@MinimumHighDiscriminantScore", "0");
            target.SetSprocParam("@ExperimentFilter", "");
            target.SetSprocParam("@ExperimentExclusionFilter", "");
            target.SetSprocParam("@JobToFilterOnByDataset", "0");
            target.SetSprocParam("@MinimumPeptideProphetProbability", "0");

            target.Run(null);

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] {
                "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET",
                "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter",
                "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2",
                "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3",
                "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3",
                "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;
            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, sprocInfo);
        }

        [Test]
        [Category("DatabaseIntegrated")]
        public void MTSDatabaseTestIntegrated()
        {
            MTSDatabaseListReadTest("", "");
        }

        [Test]
        [Category("DatabaseNamedUser")]
        public void MTSDatabaseTestNamedUser()
        {
            MTSDatabaseListReadTest(MTS_READER, MTS_READER_PASSWORD);
        }

        private void MTSDatabaseListReadTest(string username, string password)
        {
            const string serverName = "Pogo";
            const string databaseName = "PRISM_IFC";

            // Create SQLReader module and test sink module and connect together
            var target = new SQLReader(serverName, databaseName, username, password);
            RegisterEvents(target);

            const int maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database sproc query
            target.SprocName = "GetAllMassTagDatabases";

            // target.AddParm("@IncludeUnused", "0");
            // target.AddParm("@IncludeDeleted", "0");
            // target.AddParm("@ServerFilter", "");
            target.SetSprocParam("@VerboseColumnOutput", "0");

            target.Run(null);

            // Define columns (normally not needed for production code, but necessary for unit test)
            var colList = new[] { "Name", "Description", "Organism", "Campaign" };

            var sprocInfo = "procedure " + target.SprocName + " in " + target.Database;
            CheckQueryResults(sink, maxRows, colList, serverName, databaseName, username, sprocInfo);
        }

        private static void CheckQueryResults(
            SimpleSink sink,
            int maxRows,
            IReadOnlyList<string> colList,
            string serverName,
            string databaseName,
            string username,
            string expectedSqlOrSProc)
        {
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
            var cols = sink.Columns;

            if (cols.Count == 0)
            {
                Assert.Fail("Did not retrieve data from database {0} on server {1} using {2}", serverName, databaseName, expectedSqlOrSProc);
            }

            Assert.AreEqual(colList.Count, cols.Count, "Column count mismatch " + errorMessage);
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(colList[i], cols[i].Name, "Did not get the expected row definitions " + errorMessage);
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

            Assert.AreEqual(maxRows, rows.Count, "Did not get the expected number of data rows " + errorMessage);

            // Are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Count, rows[0].Length, "Data rows do not have the expected number of fields " + errorMessage);
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
