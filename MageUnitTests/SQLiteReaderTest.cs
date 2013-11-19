using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace MageUnitTests {


    /// <summary>
    ///This is a test class for SQLiteReaderTest and is intended
    ///to contain all SQLiteReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLiteReaderTest {


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
        ///A test for Run
        ///</summary>
        [TestMethod()]
		[DeploymentItem(@"..\..\..\TestItems\Metadata.db")]
        public void QueryTest() {
            int maxRows = 7;
            string[] colList = new string[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            string colNames = string.Join(", ", colList);
            string sql = string.Format("SELECT {0} FROM factors", colNames);
            string filePath = @"Metadata.db";

            SimpleSink sink = ReadSQLiteDB(maxRows, sql, filePath);

            // did the test sink object get the expected row definitions
            Collection<MageColumnDef> cols = sink.Columns;
            for (int i = 0; i < cols.Count; i++) {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // did the test sink object get the expected number of data rows
            // on its standard tabular input?
			Collection<string[]> rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);
        }

        public static SimpleSink ReadSQLiteDB(int maxRows, string sqlText, string filePath) {
            ProcessingPipeline pipeline = new ProcessingPipeline("SQLite_Reader");

            SQLiteReader target = new SQLiteReader();
            SimpleSink result = new SimpleSink(maxRows);

            target.Database = filePath;
            target.SQLText = sqlText;

            pipeline.RootModule = pipeline.AddModule("Reader", target);
            pipeline.AddModule("Results", result);
            pipeline.ConnectModules("Reader", "Results");

            pipeline.RunRoot(null);

            return result;
        }

        [TestMethod()]
		[DeploymentItem(@"..\..\..\TestItems\Metadata.db")]
		[DeploymentItem(@"..\..\..\TestItems\SQLiteQueryDefinitions.xml")]
        public void QueryFromConfigTest() {
            int maxRows = 5;

            // runtime parameters for query 
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
            runtimeParameters["Factor"] = "Group";
            // runtimeParameters[":Database"] = "SomeOtherDatabase.db"; // if you wanted to override the database file defintion in the query defintion file

            // get data from database
            SimpleSink result = GetDataFromSQLite("Factors", runtimeParameters, maxRows);

            // did the test sink object get the expected row definitions
            string[] colList = new string[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            Collection<MageColumnDef> cols = result.Columns;
            for (int i = 0; i < cols.Count; i++) {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // did the test sink object get the expected number of data rows on its standard tabular input?
			Collection<string[]> rows = result.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);

            // go through the rows and get value in "Factor" and "Value" columns
            int nameIndex = result.ColumnIndex["Factor"];
            int valIndex = result.ColumnIndex["Value"];
			foreach (string[] row in result.Rows)
			{
                string name = row[nameIndex];
                string value = row[valIndex];
            }
        }

        /// <summary>
        /// example of packaged reader pipeline
        /// </summary>
        /// <returns></returns>
        public SimpleSink GetDataFromSQLite(string queryDefName, Dictionary<string, string> runtimeParameters, int maxRows) {

            // get XML query definition by name 
            ModuleDiscovery.QueryDefinitionFileName = "SQLiteQueryDefinitions.xml";  // omit if using default query def file
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef(queryDefName);

            // create database reader module initialized from XML definition
            SQLiteReader reader = new SQLiteReader(queryDefXML, runtimeParameters);

            // create sink module to accumulate columns and rows
            SimpleSink result = new SimpleSink(maxRows);

            // create pipeline to run the query, and run it
            ProcessingPipeline pipeline = new ProcessingPipeline("Get_Data_From_Database");
            pipeline.RootModule = pipeline.AddModule("Reader", reader);
            pipeline.AddModule("Results", result);
            pipeline.ConnectModules("Reader", "Results");
            pipeline.RunRoot(null);

            return result;
        }

        /// <summary>
        ///  example
        /// </summary>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        public SimpleSink QueryFromConfig(int maxRows) {

            // runtime parameters for query (probably pass this in as an argument)
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
            runtimeParameters["Factor"] = "Group";

            // get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = "SQLiteQueryDefinitions.xml";
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors");
            Assert.AreNotEqual("", queryDefXML);

            // create MSSQLReader module initialized from XML definition
            SQLiteReader reader = new SQLiteReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, reader);

            string expected = "SELECT * FROM factors WHERE \"Factor\" = 'Group'";
            Assert.AreEqual(expected, reader.SQLText);

            // create sink module to accumulate columns and rows
            SimpleSink result = new SimpleSink(maxRows);
            Assert.AreNotEqual(null, result);

            // create pipeline to run the query, and run it
            ProcessingPipeline pipeline = new ProcessingPipeline("SQLite_Reader");
            pipeline.RootModule = pipeline.AddModule("Reader", reader);
            pipeline.AddModule("Results", result);
            pipeline.ConnectModules("Reader", "Results");
            pipeline.RunRoot(null);

            return result;
        }

        /// <summary>
        ///A test for database
        ///</summary>
        [TestMethod()]
        public void DatabasePropertyTest() {
            SQLiteReader target = new SQLiteReader();
            string expected = "Test Value";
            string actual;
            target.Database = expected;
            actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sqlText
        ///</summary>
        [TestMethod()]
        public void SQLTextPropertyTest() {
            SQLiteReader target = new SQLiteReader();
            string expected = "Test Value";
            string actual;
            target.SQLText = expected;
            actual = target.SQLText;
            Assert.AreEqual(expected, actual);
        }
    }
}
