using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for SQLiteReaderTest and is intended
    ///to contain all SQLiteReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLiteReaderTest
    {

        /// <summary>
        ///A test for Run
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\..\TestItems\Metadata.db")]
        public void QueryTest()
        {
            var maxRows = 7;
            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            var colNames = string.Join(", ", colList);
            var sql = string.Format("SELECT {0} FROM factors", colNames);
            var filePath = @"Metadata.db";

            var sink = ReadSQLiteDB(maxRows, sql, filePath);

            // did the test sink object get the expected row definitions
            var cols = sink.Columns;
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);
        }

        public static SimpleSink ReadSQLiteDB(int maxRows, string sqlText, string filePath)
        {
            var pipeline = new ProcessingPipeline("SQLite_Reader");

            var target = new SQLiteReader();
            var result = new SimpleSink(maxRows);

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
        public void QueryFromConfigTest()
        {
            var maxRows = 5;

            // runtime parameters for query 
            var runtimeParameters = new Dictionary<string, string> {["Factor"] = "Group"};
            // runtimeParameters[":Database"] = "SomeOtherDatabase.db"; // if you wanted to override the database file definition in the query definition file

            // get data from database
            var result = GetDataFromSQLite("Factors", runtimeParameters, maxRows);

            // did the test sink object get the expected row definitions
            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            var cols = result.Columns;
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // did the test sink object get the expected number of data rows on its standard tabular input?
            var rows = result.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);

            // go through the rows and get value in "Factor" and "Value" columns
            var nameIndex = result.ColumnIndex["Factor"];
            var valIndex = result.ColumnIndex["Value"];
            foreach (var row in result.Rows)
            {
                var name = row[nameIndex];
                var value = row[valIndex];
            }
        }

        /// <summary>
        /// example of packaged reader pipeline
        /// </summary>
        /// <returns></returns>
        public SimpleSink GetDataFromSQLite(string queryDefName, Dictionary<string, string> runtimeParameters, int maxRows)
        {

            // get XML query definition by name 
            ModuleDiscovery.QueryDefinitionFileName = "SQLiteQueryDefinitions.xml";  // omit if using default query def file
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef(queryDefName);

            // create database reader module initialized from XML definition
            var reader = new SQLiteReader(queryDefXML, runtimeParameters);

            // create sink module to accumulate columns and rows
            var result = new SimpleSink(maxRows);

            // create pipeline to run the query, and run it
            var pipeline = new ProcessingPipeline("Get_Data_From_Database");
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
        public SimpleSink QueryFromConfig(int maxRows)
        {

            // runtime parameters for query (probably pass this in as an argument)
            var runtimeParameters = new Dictionary<string, string> {["Factor"] = "Group"};

            // get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = "SQLiteQueryDefinitions.xml";
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors");
            Assert.AreNotEqual("", queryDefXML);

            // create MSSQLReader module initialized from XML definition
            var reader = new SQLiteReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, reader);

            var expected = "SELECT * FROM factors WHERE \"Factor\" = 'Group'";
            Assert.AreEqual(expected, reader.SQLText);

            // create sink module to accumulate columns and rows
            var result = new SimpleSink(maxRows);
            Assert.AreNotEqual(null, result);

            // create pipeline to run the query, and run it
            var pipeline = new ProcessingPipeline("SQLite_Reader");
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
        public void DatabasePropertyTest()
        {
            var target = new SQLiteReader();
            var expected = "Test Value";
            target.Database = expected;
            var actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sqlText
        ///</summary>
        [TestMethod()]
        public void SQLTextPropertyTest()
        {
            var target = new SQLiteReader();
            var expected = "Test Value";
            target.SQLText = expected;
            var actual = target.SQLText;
            Assert.AreEqual(expected, actual);
        }
    }
}
