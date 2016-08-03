using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for PermutationGeneratorTest and is intended
    ///to contain all PermutationGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PermutationGeneratorTest
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

        /// <summary>
        ///A test for AddParamColumn
        ///</summary>
        [TestMethod()]
        public void AddParamColumnTest()
        {
            int actual;
            PermutationGenerator target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            actual = target.ParamCount;
            Assert.AreEqual(3, actual, "Number of parameters did not match");
        }

        /// <summary>
        ///A test for PredictedOutputRowCount
        ///</summary>
        [TestMethod()]
        public void PredictedOutputRowCountTest()
        {
            int actual;
            PermutationGenerator target = new PermutationGenerator();

            actual = target.ParamCount;
            Assert.AreEqual(0, actual);

            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            actual = target.PredictedOutputRowCount;
            Assert.AreEqual(15, actual, "Number of rows did not match");
        }

        /// <summary>
        ///A test for IncludeHeaderInOutput
        ///</summary>
        [TestMethod()]
        public void IncludeHeaderInOutputTest()
        {
            PermutationGenerator target = new PermutationGenerator();
            bool expected = false;
            bool actual;
            target.IncludeHeaderInOutput = expected;
            actual = target.IncludeHeaderInOutput;
            Assert.AreEqual(expected, actual);

            expected = true;
            target.IncludeHeaderInOutput = expected;
            actual = target.IncludeHeaderInOutput;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Run
        ///</summary>
        [TestMethod()]
        public void BasicPermutationGenerator()
        {
            PermutationGenerator target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            // create test sink module and connect to MSSQLReader module
            SimpleSink sink = new SimpleSink();
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            int actual;
            int expected;
            expected = target.ParamCount;
            actual = sink.Columns.Count;
            Assert.AreEqual(expected, actual, "Number of columns did not match");

            actual = sink.Rows.Count;
            Assert.AreEqual(15, actual, "Number of rows did not match");
        }

        /// <summary>
        ///A test for Run
        ///</summary>
        [TestMethod()]
        public void PermutationGeneratorWithMappedColumns()
        {
            PermutationGenerator target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            target.OutputColumnList = "ref|+|text, one, two, three, results|+|float";
            target.AutoColumnName = "ref";

            // create test sink module and connect to MSSQLReader module
            SimpleSink sink = new SimpleSink();
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            int actual;
            int expected;
            expected = target.ParamCount;
            actual = sink.Columns.Count;
            Assert.AreEqual(5, actual, "Number of columns did not match");

            actual = sink.Rows.Count;
            Assert.AreEqual(15, actual, "Number of rows did not match");
        }

        [TestMethod()]
        public void PermutationGeneratorWriteSQLite()
        {

            PermutationGenerator PGen = new PermutationGenerator();
            PGen.AddParamColumn("one", "1", "5", "1");
            PGen.AddParamColumn("two", "0", "2", "1");
            PGen.AddParamColumn("three", "1", "1", "1");

            // Optional: can use built-in column mapping to add columns
            PGen.OutputColumnList = "ref|+|text, one, two, three";
            PGen.AutoColumnName = "ref";
            PGen.AutoColumnFormat = "ParamSet_{0:000000}";

            ProcessingPipeline pipeline = new ProcessingPipeline("PGen_2_SQLite");

            SQLiteWriter writer = new SQLiteWriter();
            writer.DbPath = "param_permutations.db";
            writer.TableName = "param_permutations";

            pipeline.RootModule = pipeline.AddModule("Gen", PGen);
            pipeline.AddModule("Writer", writer);
            pipeline.ConnectModules("Gen", "Writer");
            pipeline.RunRoot(null);

            Assert.AreEqual("", pipeline.CompletionCode, "Completion code did not match");
        }

        [TestMethod()]
        public void PermutationGeneratorWriteFile()
        {

            PermutationGenerator PGen = new PermutationGenerator();
            PGen.AddParamColumn("one", "1", "5", "1");
            PGen.AddParamColumn("two", "0", "2", "1");
            PGen.AddParamColumn("three", "1", "1", "1");

            // Optional: can use built-in column mapping to add columns
            PGen.OutputColumnList = "ref|+|text, one, two, three";
            PGen.AutoColumnName = "ref";
            PGen.AutoColumnFormat = "ParamSet_{0:000000}";

            ProcessingPipeline pipeline = new ProcessingPipeline("PGen_2_File");

            DelimitedFileWriter writer = new DelimitedFileWriter();
            writer.FilePath = "param_permutations.txt";

            pipeline.RootModule = pipeline.AddModule("Gen", PGen);
            pipeline.AddModule("Writer", writer);
            pipeline.ConnectModules("Gen", "Writer");
            pipeline.RunRoot(null);

            Assert.AreEqual("", pipeline.CompletionCode, "Completion code did not match");
        }

    }
}
