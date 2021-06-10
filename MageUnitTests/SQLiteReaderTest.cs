using System;
using Mage;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for SQLiteReaderTest and is intended
    /// to contain all SQLiteReaderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SQLiteReaderTest
    {
        public const string SQLITE_QUERY_DEF_FILE_PATH_KEY = "SQLiteQueryDefinitionsFilePath";

        /// <summary>
        /// A test for Run
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\TestItems\Metadata.db")]
        public void QueryTest(string filePath)
        {
            var dataFile = General.GetTestFile(filePath);

            const int maxRows = 7;
            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            var colNames = string.Join(", ", colList);
            var sql = string.Format("SELECT {0} FROM factors", colNames);

            var sink = ReadSQLiteDB(maxRows, sql, dataFile.FullName);

            // Did the test sink object get the expected row definitions
            var cols = sink.Columns;
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // Did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // Are there the expected number of fields in the data row?
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

        [Test]
        [TestCase(@"..\..\..\TestItems\Metadata.db", @"..\..\..\TestItems\SQLiteQueryDefinitions.xml")]
        public void QueryFromConfigTest(string filePath, string queryDefinitionsPath)
        {
            var dbFile = General.GetTestFile(filePath);
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            const int maxRows = 5;

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>
            {
                ["Factor"] = "Group",
                [General.DATABASE_PATH_KEY] = dbFile.FullName,
                [SQLITE_QUERY_DEF_FILE_PATH_KEY] = queryDefsFile.FullName
            };

            // Get data from database
            var result = GetDataFromSQLite("Factors", runtimeParameters, maxRows);

            // Did the test sink object get the expected row definitions
            var colList = new[] { "Dataset", "Dataset_ID", "Factor", "Value" };
            var cols = result.Columns;
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // Did the test sink object get the expected number of data rows on its standard tabular input?
            var rows = result.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // Are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);

            Console.WriteLine("Factors");
            Console.WriteLine();

            // Go through the rows and get value in "Factor" and "Value" columns
            var nameIndex = result.ColumnIndex["Factor"];
            var valIndex = result.ColumnIndex["Value"];
            foreach (var row in result.Rows)
            {
                var name = row[nameIndex];
                var value = row[valIndex];

                Console.WriteLine("{0}: {1}", name, value);
            }
        }

        /// <summary>
        /// Example of packaged reader pipeline
        /// </summary>
        /// <returns></returns>
        public SimpleSink GetDataFromSQLite(string queryDefName, Dictionary<string, string> runtimeParameters, int maxRows)
        {
            if (runtimeParameters.TryGetValue(SQLITE_QUERY_DEF_FILE_PATH_KEY, out var queryDefFilePath))
            {
                runtimeParameters.Remove(SQLITE_QUERY_DEF_FILE_PATH_KEY);
            }
            else
            {
                queryDefFilePath = "SQLiteQueryDefinitions.xml";
            }

            if (runtimeParameters.TryGetValue(General.DATABASE_PATH_KEY, out var databaseFilePath))
            {
                runtimeParameters.Remove(General.DATABASE_PATH_KEY);
            }
            else
            {
                databaseFilePath = "Metadata.db";
            }

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefFilePath;  // Omit if using default query def file
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef(queryDefName);

            // Create database reader module initialized from XML definition
            var reader = new SQLiteReader(queryDefXML, runtimeParameters, databaseFilePath);

            // Create sink module to accumulate columns and rows
            var result = new SimpleSink(maxRows);

            // Create pipeline to run the query, and run it
            var pipeline = new ProcessingPipeline("Get_Data_From_Database");
            pipeline.RootModule = pipeline.AddModule("Reader", reader);
            pipeline.AddModule("Results", result);
            pipeline.ConnectModules("Reader", "Results");
            pipeline.RunRoot(null);

            return result;
        }

        [Test]
        [TestCase(25)]
        public void QueryFromConfig(int maxRows)
        {
            var queryDefsFile = General.GetTestFile(@"..\..\..\TestItems\SQLiteQueryDefinitions.xml");

            // Runtime parameters for query (probably pass this in as an argument)
            var runtimeParameters = new Dictionary<string, string> { ["Factor"] = "Group" };

            // Get XML query definition by name
            // This will query the factors table in SQLite database Metadata.db
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;

            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors");
            Assert.AreNotEqual("", queryDefXML);

            // Create SQLReader module initialized from XML definition
            var reader = new SQLiteReader(queryDefXML, runtimeParameters);
            Assert.AreNotEqual(null, reader);

            const string expected = "SELECT * FROM factors WHERE Factor = 'Group'";
            Assert.AreEqual(expected, reader.SQLText);

            // Create sink module to accumulate columns and rows
            var result = new SimpleSink(maxRows);
            Assert.AreNotEqual(null, result);

            // Create pipeline to run the query, and run it
            var pipeline = new ProcessingPipeline("SQLite_Reader");
            pipeline.RootModule = pipeline.AddModule("Reader", reader);
            pipeline.AddModule("Results", result);
            pipeline.ConnectModules("Reader", "Results");
            pipeline.RunRoot(null);

            Console.WriteLine(string.Join("\t", from item in result.Columns select item.Name));

            foreach (var item in result.Rows)
            {
                Console.WriteLine(string.Join("\t", item));
            }
        }

        /// <summary>
        /// A test for database
        /// </summary>
        [Test]
        public void DatabasePropertyTest()
        {
            var target = new SQLiteReader();
            const string expected = "Test Value";
            target.Database = expected;
            var actual = target.Database;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for sqlText
        /// </summary>
        [Test]
        public void SQLTextPropertyTest()
        {
            var target = new SQLiteReader();
            const string expected = "Test Value";
            target.SQLText = expected;
            var actual = target.SQLText;
            Assert.AreEqual(expected, actual);
        }
    }
}
