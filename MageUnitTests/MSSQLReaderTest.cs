using Mage;
using NUnit.Framework;
using System.Collections.Generic;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for MSSQLReaderTest and is intended
    /// to contain all MSSQLReaderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class MSSQLReaderTest
    {

        /// <summary>
        /// A test for sqlText
        /// </summary>
        [Test]
        public void SQLTextPropertyTest()
        {
            var target = new MSSQLReader();
            var expected = "Test Value";
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
            var target = new MSSQLReader();
            var expected = "Test Value";
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
            var target = new MSSQLReader();
            var expected = "Test Value";
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
            var target = new MSSQLReader();
            var expected = "Test Value";
            target.Database = expected;
            var actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLQueryDatasetFactors(string queryDefinitionsPath)
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

            // Create MSSQLReader module initialized from XML definition
            var target = new MSSQLReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, target);

            var expected = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE [Dataset] LIKE '%sarc%'";
            Assert.AreEqual(expected, target.SQLText);

            // Create test sink module and connect to MSSQLReader module
            var maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// Test to get factors for dataset name
        /// </summary>
        [Test]
        public void QueryDatasetFactors()
        {
            // Create MSSQLReader module and test sink module
            // and connect together
            var target = new MSSQLReader();
            var maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database query
            // Defaults are gigasax and DMS5
            target.Server = Globals.DMSServer;
            target.Database = Globals.DMSDatabase;

            // Define query
            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            var colNames = string.Join(", ", colList);

            var builder = new SQLBuilder
            {
                Table = "V_Custom_Factors_List_Report",
                Columns = colNames
            };
            builder.AddPredicateItem("Dataset", "sarc");
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        ///</summary>
        [Test]
        public void QueryWithSQLBuilderTest()
        {
            // Create MSSQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new MSSQLReader();
            var maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database query
            // Defaults are gigasax and DMS5
            target.Server = Globals.DMSServer;
            target.Database = Globals.DMSDatabase;

            // Define columns (noramlly not needed for production code, but necessary for unit test)
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
                Columns = colNames
            };
            builder.AddPredicateItem("datasetName", "ContainsText", "sarc_ms");
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        ///</summary>
        [Test]
        public void QueryAllMTDBs()
        {
            // Create MSSQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new MSSQLReader();
            var maxRows = 5;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database query
            target.Server = "pogo";
            target.Database = "MTS_Master";

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] { "Server_Name", "MT_DB_ID", "MT_DB_Name", "State_ID", "State" };
            var colNames = string.Join(", ", colList);

            // Define query
            var builder = new SQLBuilder
            {
                Table = "V_MT_DBs",
                Columns = colNames
            };
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        ///A test for straight SQL query against DMS
        ///</summary>
        [Test]
        public void QueryTest()
        {
            // Create MSSQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new MSSQLReader();
            var maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign" };
            var colNames = string.Join(", ", colList);

            // Define and run a database query
            // Defaults are gigasax and DMS5
            target.Server = Globals.DMSServer;
            target.Database = Globals.DMSDatabase;
            target.SQLText = string.Format("SELECT TOP ({1}) {0} FROM V_Mage_Analysis_Jobs WHERE Instrument LIKE '%test%'", colNames, maxRows.ToString());

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        [Test]
        public void DMSSprocReadTest()
        {
            // Create MSSQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new MSSQLReader();
            var maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;


            // Define and run a database sproc query
            // Defaults are gigasax and DMS5
            target.Server = Globals.DMSServer;
            target.Database = Globals.DMSDatabase;
            target.SprocName = "FindExistingJobsForRequest";
            target.SetSprocParam("@requestID", "8142");


            target.Run(null);

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] {
                "Job", "State", "Priority", "Request", "Created", "Start", "Finish", "Processor",
                "Dataset" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLMTSSprocReadTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>();

            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;

            // Get XML query definition by name
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            // Create MSSQLReader module initialized from XML definition
            var target = new MSSQLReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, target);

            var expected = "GetMassTagsPlusPepProphetStats";
            Assert.AreEqual(expected, target.SprocName);

            // Create test sink module and connect to MSSQLReader module
            var maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] {
                "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET",
                "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter",
                "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2",
                "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3",
                "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3",
                "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [Test]
        public void MTSSprocReadTest()
        {
            // Create MSSQLReader module and test sink module
            // and connect together (no pipeline object used)
            var target = new MSSQLReader();
            var maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database sproc query
            target.Server = "roadrunner";
            target.Database = "MT_Shewanella_ProdTest_Formic_P966";
            target.SprocName = "GetMassTagsPlusPepProphetStats";

            // Set argument values (may be omitted of argument has acceptable default value
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

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] {
                "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET",
                "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter",
                "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2",
                "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3",
                "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3",
                "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [Test]
        public void MTSDatabaseListReadTest()
        {
            // Create MSSQLReader module and test sink module and connect together
            var target = new MSSQLReader();
            var maxRows = 4;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // Define and run a database sproc query
            target.Server = "pogo";
            target.Database = "PRISM_IFC";
            target.SprocName = "GetAllMassTagDatabases";

            // target.AddParm("@IncludeUnused", "0");
            // target.AddParm("@IncludeDeleted", "0");
            // target.AddParm("@ServerFilter", "");
            target.SetSprocParam("@VerboseColumnOutput", "0");

            target.Run(null);

            // Define columns (noramlly not needed for production code, but necessary for unit test)
            var colList = new[] { "Name", "Description", "Organism", "Campaign" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        private static void CheckRowsetResults(SimpleSink sink, int maxRows, IReadOnlyList<string> colList)
        {

            // Did the test sink module get the expected row definitions
            var cols = sink.Columns;
            Assert.AreEqual(cols.Count, colList.Count);
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i], "Did not get get the expected row definitions");
            }

            // Did the test sink module get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;

            Assert.AreEqual(maxRows, rows.Count, "Did not get get the expected number of data rows");

            // Are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Count, rows[0].Length, "Data rows do not have the expected number of fields");
        }

    }
}
