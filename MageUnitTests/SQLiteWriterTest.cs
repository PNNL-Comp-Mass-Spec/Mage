using Mage;
using NUnit.Framework;

namespace MageUnitTests
{


    /// <summary>
    /// This is a test class for SQLiteWriterTest and is intended
    /// to contain all SQLiteWriterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SQLiteWriterTest
    {

        [Test]
        public void DoubleTap()
        {
            var cols = 5;
            var rows = 21;
            var dbPath = "write_test_2.db";
            var tableName = "t_test";
            var sqlText = string.Format("SELECT * FROM {0}", tableName);

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source1 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            var result1 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source1.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source1.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result1.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result1.Columns.Count, "Result column count does not match");
            General.CompareSinks(source1, result1);

            dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };
            var source2 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            var result2 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source2.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source2.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result2.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result2.Columns.Count, "Result column count does not match");
            //            General.CompareSinks(source2, result2);

        }


        [Test]
        public void WriteTest()
        {
            var cols = 5;
            var rows = 21;
            var dbPath = "write_test.db";
            var tableName = "t_test";
            var sqlText = string.Format("SELECT * FROM {0}", tableName);

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);

            var result = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result.Columns.Count, "Result column count does not match");
            General.CompareSinks(source, result);
        }

        public SimpleSink WriteSQLiteDBWithTestData(string dbPath, string tableName, IBaseModule dGen)
        {
            var pipeline = new ProcessingPipeline("SQLiteWriter");

            var target = new SQLiteWriter
            {
                DbPath = dbPath,
                TableName = tableName
            };

            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Writer", target);
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Gen", "Writer");
            pipeline.ConnectModules("Gen", "Results");

            pipeline.RunRoot(null);

            var result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }

        /// <summary>
        /// A test for BlockSize
        /// </summary>
        [Test]
        public void BlockSizeTest()
        {
            var target = new SQLiteWriter();
            var expected = "42";
            target.BlockSize = expected;
            var actual = target.BlockSize;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for DbPassword
        /// </summary>
        [Test]
        public void DbPasswordTest()
        {
            var target = new SQLiteWriter();
            var expected = "Test Value";
            target.DbPassword = expected;
            var actual = target.DbPassword;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for DbPath
        /// </summary>
        [Test]
        public void DbPathTest()
        {
            var target = new SQLiteWriter();
            var expected = "Test Value";
            target.DbPath = expected;
            var actual = target.DbPath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for TableName
        /// </summary>
        [Test]
        public void TableNameTest()
        {
            var target = new SQLiteWriter();
            var expected = "Test Value";
            target.TableName = expected;
            var actual = target.TableName;
            Assert.AreEqual(expected, actual);
        }
    }
}
