using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace MageUnitTests {


    /// <summary>
    ///This is a test class for DelimitedFileWriterTest and is intended
    ///to contain all DelimitedFileWriterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DelimitedFileWriterTest {


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

        [TestMethod()]
        public void WriteTest() {
            int cols = 5;
            int rows = 21;
            string testFile = "delim_test.txt";

            DataGenerator dGen = new DataGenerator();
            dGen.Rows = rows;
            dGen.Cols = cols;

            SimpleSink source = WriteDelimitedFileWithTestData(testFile, dGen);

            SimpleSink result = DelimitedFileReaderTest.ReadDelimitedFile(testFile);

            Assert.AreEqual(rows, source.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result.Columns.Count, "Result column count does not match");
            General.CompareSinks(source, result);
        }

        public SimpleSink WriteDelimitedFileWithTestData(string filePath, IBaseModule dGen) {
            ProcessingPipeline pipeline = new ProcessingPipeline("Delimited_File_Writer");

            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.MakeModule("Writer", "DelimitedFileWriter");
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Gen", "Writer");
            pipeline.ConnectModules("Gen", "Results");

            pipeline.SetModuleParameter("Writer", "FilePath", filePath);

            pipeline.RunRoot(null);

            SimpleSink result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }



        /// <summary>
        ///A test for Header
        ///</summary>
        [TestMethod()]
        public void HeaderTest() {
            DelimitedFileWriter target = new DelimitedFileWriter(); 
            string expected = "Test Value";
            string actual;
            target.Header = expected;
            actual = target.Header;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FilePath
        ///</summary>
        [TestMethod()]
        public void FilePathTest() {
            DelimitedFileWriter target = new DelimitedFileWriter(); 
            string expected = "Test Value";
            string actual;
            target.FilePath = expected;
            actual = target.FilePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Delimiter
        ///</summary>
        [TestMethod()]
        public void DelimiterTest() {
            DelimitedFileWriter target = new DelimitedFileWriter(); 
            string expected = ",";
            string actual;
            target.Delimiter = expected;
            actual = target.Delimiter;
            Assert.AreEqual(expected, actual);
        }
    }
}
