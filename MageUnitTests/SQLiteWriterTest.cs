using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for SQLiteWriterTest and is intended
    ///to contain all SQLiteWriterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLiteWriterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
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


        [TestMethod()]
        public void DoubleTap()
        {
            int cols = 5;
            int rows = 21;
            string dbPath = "write_test_2.db";
            string tableName = "t_test";
            string sqlText = string.Format("SELECT * FROM {0}", tableName);

            DataGenerator dGen;
            dGen = new DataGenerator();
            dGen.Rows = rows;
            dGen.Cols = cols;

            SimpleSink source1 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            SimpleSink result1 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source1.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source1.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result1.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result1.Columns.Count, "Result column count does not match");
            General.CompareSinks(source1, result1);

            dGen = new DataGenerator();
            dGen.Rows = rows;
            dGen.Cols = cols;
            SimpleSink source2 = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);
            SimpleSink result2 = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source2.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source2.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result2.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result2.Columns.Count, "Result column count does not match");
            //            General.CompareSinks(source2, result2);

        }


        [TestMethod()]
        public void WriteTest()
        {
            int cols = 5;
            int rows = 21;
            string dbPath = "write_test.db";
            string tableName = "t_test";
            string sqlText = string.Format("SELECT * FROM {0}", tableName);

            DataGenerator dGen = new DataGenerator();
            dGen.Rows = rows;
            dGen.Cols = cols;

            SimpleSink source = WriteSQLiteDBWithTestData(dbPath, tableName, dGen);

            SimpleSink result = SQLiteReaderTest.ReadSQLiteDB(rows, sqlText, dbPath);

            Assert.AreEqual(rows, source.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result.Columns.Count, "Result column count does not match");
            General.CompareSinks(source, result);
        }

        public SimpleSink WriteSQLiteDBWithTestData(string dbPath, string tableName, IBaseModule dGen)
        {
            ProcessingPipeline pipeline = new ProcessingPipeline("SQLiteWriter");

            SQLiteWriter target = new SQLiteWriter();
            target.DbPath = dbPath;
            target.TableName = tableName;

            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Writer", target);
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Gen", "Writer");
            pipeline.ConnectModules("Gen", "Results");

            pipeline.RunRoot(null);

            SimpleSink result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }

        /// <summary>
        ///A test for BlockSize
        ///</summary>
        [TestMethod()]
        public void BlockSizeTest()
        {
            SQLiteWriter target = new SQLiteWriter();
            string expected = "42";
            string actual;
            target.BlockSize = expected;
            actual = target.BlockSize;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DbPassword
        ///</summary>
        [TestMethod()]
        public void DbPasswordTest()
        {
            SQLiteWriter target = new SQLiteWriter();
            string expected = "Test Value";
            string actual;
            target.DbPassword = expected;
            actual = target.DbPassword;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DbPath
        ///</summary>
        [TestMethod()]
        public void DbPathTest()
        {
            SQLiteWriter target = new SQLiteWriter();
            string expected = "Test Value";
            string actual;
            target.DbPath = expected;
            actual = target.DbPath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TableName
        ///</summary>
        [TestMethod()]
        public void TableNameTest()
        {
            SQLiteWriter target = new SQLiteWriter();
            string expected = "Test Value";
            string actual;
            target.TableName = expected;
            actual = target.TableName;
            Assert.AreEqual(expected, actual);
        }
    }
}
