using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace MageUnitTests {

    /// <summary>
    ///This is a test class for MSSQLReaderTest and is intended
    ///to contain all MSSQLReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MSSQLReaderTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for sqlText
        ///</summary>
        [TestMethod()]
        public void SQLTextPropertyTest() {
            MSSQLReader target = new MSSQLReader();
            string expected = "Test Value";
            string actual;
            target.SQLText = expected;
            actual = target.SQLText;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sprocName
        ///</summary>
        [TestMethod()]
        public void SprocNamePropertyTest() {
            MSSQLReader target = new MSSQLReader();
            string expected = "Test Value";
            string actual;
            target.SprocName = expected;
            actual = target.SprocName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for server
        ///</summary>
        [TestMethod()]
        public void ServerPropertyTest() {
            MSSQLReader target = new MSSQLReader();
            string expected = "Test Value";
            string actual;
            target.Server = expected;
            actual = target.Server;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for database
        ///</summary>
        [TestMethod()]
        public void DatabasePropertyTest() {
            MSSQLReader target = new MSSQLReader();
            string expected = "Test Value";
            string actual;
            target.Database = expected;
            actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\QueryDefinitions.xml")]
        public void XMLQueryDatasetFactors() {

            // runtime parameters for query
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
            runtimeParameters["Dataset"] = "sarc";

            // get XML query definition by name
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            // create MSSQLReader module initialized from XML definition
            MSSQLReader target = new MSSQLReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, target);

            string expected = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE [Dataset] LIKE '%sarc%'";
            Assert.AreEqual(expected, target.SQLText);

            // create test sink module and connect to MSSQLReader module
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            string[] colList = new string[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// Test to get factors for dataset name
        /// </summary>
        [TestMethod()]
        public void QueryDatasetFactors() {
            // create MSSQLReader module and test sink module 
            // and connect together
            MSSQLReader target = new MSSQLReader();
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define and run a database query
            target.Server = "gigasax";
            target.Database = "DMS5";

            // define query
            string[] colList = new string[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            string colNames = string.Join(", ", colList);
            //
            SQLBuilder builder = new SQLBuilder();
            builder.Table = "V_Custom_Factors_List_Report";
            builder.Columns = colNames;
            builder.AddPredicateItem("Dataset", "sarc");
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        ///</summary>
        [TestMethod()]
        public void QueryWithSQLBuilderTest() {
            // create MSSQLReader module and test sink module 
            // and connect together (no pipeline object used)
            MSSQLReader target = new MSSQLReader();
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define and run a database query
            target.Server = "gigasax";
            target.Database = "DMS5";

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "DatasetID", "volName", "path", "datasetFolder", "resultsFolder", "datasetName", "JobId", "ColumnID", "AcquisitionTime", "Labelling", "InstrumentName", "ToolID", "BlockNum", "ReplicateName", "ExperimentID", "RunOrder", "BatchID", "ArchPath", "DatasetFullPath", "Organism", "Campaign", "ParameterFileName", "SettingsFileName" };
            string colNames = string.Join(", ", colList);

            // define query
            SQLBuilder builder = new SQLBuilder();
            builder.Table = "V_Analysis_Job_Export_MultiAlign";
            builder.Columns = colNames;
            builder.AddPredicateItem("datasetName", "ContainsText", "sarc_ms");
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        /// A test for straight SQL query against DMS
        /// using SQLBuilder to make the SQL
        ///</summary>
        [TestMethod()]
        public void QueryAllMTDBs() {
            // create MSSQLReader module and test sink module 
            // and connect together (no pipeline object used)
            MSSQLReader target = new MSSQLReader();
            int maxRows = 5;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define and run a database query
            target.Server = "pogo";
            target.Database = "MTS_Master";

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Server_Name", "MT_DB_ID", "MT_DB_Name", "State_ID", "State" };
            string colNames = string.Join(", ", colList);

            // define query
            SQLBuilder builder = new SQLBuilder();
            builder.Table = "V_MT_DBs";
            builder.Columns = colNames;
            target.SQLText = builder.BuildQuerySQL();

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        /// <summary>
        ///A test for straight SQL query against DMS
        ///</summary>
        [TestMethod()]
        public void QueryTest() {
            // create MSSQLReader module and test sink module 
            // and connect together (no pipeline object used)
            MSSQLReader target = new MSSQLReader();
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Job", "State", "Dataset", "Dataset_ID", "Tool", "Parameter_File", "Settings_File", "Instrument", "Experiment", "Campaign" };
            string colNames = string.Join(", ", colList);
 
            // define and run a database query
            target.Server = "gigasax";
            target.Database = "DMS5";
            target.SQLText = string.Format("SELECT TOP ({1}) {0} FROM V_Mage_Analysis_Jobs WHERE Instrument LIKE '%test%'", colNames, maxRows.ToString());

            target.Run(null);

            CheckRowsetResults(sink, maxRows, colList);
        }

        [TestMethod()]
        public void DMSSprocReadTest() {
            // create MSSQLReader module and test sink module 
            // and connect together (no pipeline object used)
            MSSQLReader target = new MSSQLReader();
            int maxRows = 4;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;


            // define and run a database sproc query
            target.Server = "gigasax";
            target.Database = "DMS5";
            target.SprocName = "FindExistingJobsForRequest";
            target.SetSprocParam("@requestID", "8142");


            target.Run(null);

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Job", "State", "Priority", "Request", "Created", "Start", "Finish", "Processor", "Dataset", "Processor Group", "Processor Group Assignee" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\QueryDefinitions.xml")]
        public void XMLMTSSprocReadTest() {

            // runtime parameters for query
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
//            runtimeParameters["@ExperimentFilter"] = "sarc";

            // get XML query definition by name
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            // create MSSQLReader module initialized from XML definition
            MSSQLReader target = new MSSQLReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, target);

            string expected = "GetMassTagsPlusPepProphetStats";
            Assert.AreEqual(expected, target.SprocName);

            // create test sink module and connect to MSSQLReader module
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;
            
            target.Run(null);

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET", "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter", "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2", "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3", "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3", "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [TestMethod()]
        public void MTSSprocReadTest() {
            // create MSSQLReader module and test sink module 
            // and connect together (no pipeline object used)
            MSSQLReader target = new MSSQLReader();
            int maxRows = 4;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define and run a database sproc query
            target.Server = "elmer";
            target.Database = "MT_Human_Sarcopenia_QC_P584";
            target.SprocName = "GetMassTagsPlusPepProphetStats";
            // set argument values (may be omitted of argument has acceptable default value
            target.SetSprocParam("@MassCorrectionIDFilterList", "");
            target.SetSprocParam("@ConfirmedOnly", "0");
            target.SetSprocParam("@MinimumHighNormalizedScore", "0");
            target.SetSprocParam("@MinimumPMTQualityScore", "9");
            target.SetSprocParam("@NETValueType", "0");
            target.SetSprocParam("@MinimumHighDiscriminantScore", "0");
            target.SetSprocParam("@ExperimentFilter", "");
            target.SetSprocParam("@ExperimentExclusionFilter", "");
            target.SetSprocParam("@JobToFilterOnByDataset", "0");
            target.SetSprocParam("@MinimumPeptideProphetProbability", "0");

            target.Run(null);

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Mass_Tag_ID", "Peptide", "Monoisotopic_Mass", "NET_Value_to_Use", "NET_Obs_Count", "PNET", "High_Normalized_Score", "StD_GANET", "High_Discriminant_Score", "Peptide_Obs_Count_Passing_Filter", "Mod_Count", "Mod_Description", "High_Peptide_Prophet_Probability", "ObsCount_CS1", "ObsCount_CS2", "ObsCount_CS3", "PepProphet_FScore_Max_CS1", "PepProphet_FScore_Max_CS2", "PepProphet_FScore_Max_CS3", "PepProphet_Probability_Max_CS1", "PepProphet_Probability_Max_CS2", "PepProphet_Probability_Max_CS3", "PepProphet_FScore_Avg_CS1", "PepProphet_FScore_Avg_CS2", "PepProphet_FScore_Avg_CS3", "Cleavage_State" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        [TestMethod()]
        public void MTSDatabaseListReadTest() {
            // create MSSQLReader module and test sink module and connect together
            MSSQLReader target = new MSSQLReader();
            int maxRows = 4;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            // define and run a database sproc query
            target.Server = "pogo";
            target.Database = "PRISM_IFC";
            target.SprocName = "GetAllMassTagDatabases";

            //target.AddParm("@IncludeUnused", "0");
            //target.AddParm("@IncludeDeleted", "0");
            //target.AddParm("@ServerFilter", "");
            target.SetSprocParam("@VerboseColumnOutput", "0");

            target.Run(null);

            // define columns (noramlly not needed for production code, but necessary for unit test)
            string[] colList = new string[] { "Name", "Description", "Organism", "Campaign" };

            CheckRowsetResults(sink, maxRows, colList);
        }

        private static void CheckRowsetResults(SimpleSink sink, int maxRows, string[] colList) {

            // did the test sink module get the expected row definitions
            Collection<MageColumnDef> cols = sink.Columns;
            Assert.AreEqual(cols.Count, colList.Length);
            for (int i = 0; i < cols.Count; i++) {
                Assert.AreEqual(cols[i].Name, colList[i], "Did not get get the expected row definitions");
            }

            // did the test sink module get the expected number of data rows
            // on its standard tabular input?
             Collection<object[]> rows = sink.Rows;

            Assert.AreEqual(maxRows, rows.Count, "Did not get get the expected number of data rows");

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length, "Data rows do not have the expected number of fields");
        }

    }
}
