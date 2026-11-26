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
            const int cols = 5;
            const int rows = 21;
            const string dbPath = "write_test_2.db";
            const string tableName = "t_test";
            var sqlText = string.Format("SELECT * FROM {0}", tableName);

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source1 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            var result1 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.Multiple(() =>
            {
                Assert.That(source1.Rows, Has.Count.EqualTo(rows), "Source row count does not match");
                Assert.That(source1.Columns, Has.Count.EqualTo(cols), "Source column count does not match");
                Assert.That(result1.Rows, Has.Count.EqualTo(rows), "Result row count does not match");
                Assert.That(result1.Columns, Has.Count.EqualTo(cols), "Result column count does not match");
            });
            General.CompareSinks(source1, result1);

            dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };
            var source2 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            var result2 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.Multiple(() =>
            {
                Assert.That(source2.Rows, Has.Count.EqualTo(rows), "Source row count does not match");
                Assert.That(source2.Columns, Has.Count.EqualTo(cols), "Source column count does not match");
                Assert.That(result2.Rows, Has.Count.EqualTo(rows), "Result row count does not match");
                Assert.That(result2.Columns, Has.Count.EqualTo(cols), "Result column count does not match");
            });
            //            General.CompareSinks(source2, result2);

        }

        [Test]
        public void WriteTest()
        {
            const int cols = 5;
            const int rows = 21;
            const string dbPath = "write_test.db";
            const string tableName = "t_test";
            var sqlText = string.Format("SELECT * FROM {0}", tableName);

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);

            var result = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.Multiple(() =>
            {
                Assert.That(source.Rows, Has.Count.EqualTo(rows), "Source row count does not match");
                Assert.That(source.Columns, Has.Count.EqualTo(cols), "Source column count does not match");
                Assert.That(result.Rows, Has.Count.EqualTo(rows), "Result row count does not match");
                Assert.That(result.Columns, Has.Count.EqualTo(cols), "Result column count does not match");
            });
            General.CompareSinks(source, result);
        }

        private SimpleSink WriteSQLiteDBWithTestData(string dbPath, string tableName, IBaseModule dGen)
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
            const string expected = "42";
            target.BlockSize = expected;
            var actual = target.BlockSize;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for DbPassword
        /// </summary>
        [Test]
        public void DbPasswordTest()
        {
            var target = new SQLiteWriter();
            const string expected = "Test Value";
            target.DbPassword = expected;
            var actual = target.DbPassword;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for DbPath
        /// </summary>
        [Test]
        public void DbPathTest()
        {
            var target = new SQLiteWriter();
            const string expected = "Test Value";
            target.DbPath = expected;
            var actual = target.DbPath;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for TableName
        /// </summary>
        [Test]
        public void TableNameTest()
        {
            var target = new SQLiteWriter();
            const string expected = "Test Value";
            target.TableName = expected;
            var actual = target.TableName;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
