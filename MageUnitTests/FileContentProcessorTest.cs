using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for FileContentProcessorTest and is intended
    ///to contain all FileContentProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileContentProcessorTest
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
        public void FileColumnProcessorTest()
        {

            // set up test parameters
            string folderColName = "Folder_Col";
            string fileColName = "File_Col";
            string folder = System.Environment.CurrentDirectory;
            string file = "tab_delim.txt";
            string destFolder = @"C:\data\";

            // set up data generator
            DataGenerator dGen = new DataGenerator(2, 4);
            dGen.AddAdHocRow = new string[] { folderColName, fileColName, "Padding" };
            dGen.AddAdHocRow = new string[] { folder, file, "Padding" };

            // set up test mule (subclass of file processor module)
            TestFileContentProcessorModule target = new TestFileContentProcessorModule();
            target.SourceFileColumnName = fileColName;
            target.OutputFileColumnName = fileColName;
            target.SourceFolderColumnName = folderColName;
            target.OutputFolderPath = destFolder;
            target.OutputColumnList = string.Format("{0}|+|text, {1}", fileColName, folderColName);

            // tell the test mule what to expect
            target.ExpectedSourceFile = file;
            target.ExpectedSourcePath = Path.GetFullPath(Path.Combine(folder, file));
            target.ExpectedDestPath = Path.GetFullPath(Path.Combine(destFolder, file));

            // build and run pipeline
            ProcessingPipeline pipeline = new ProcessingPipeline("FileColumnProcessorTest");
            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Target", target);
            pipeline.ConnectModules("Gen", "Target");
            pipeline.RunRoot(null);
        }

        /// <summary>
        ///A test for FileColumnName
        ///</summary>
        [TestMethod()]
        public void OutputFileColumnNameTest()
        {
            FileContentProcessor target = new FileContentProcessor();
            string expected = "Test Value";
            string actual;
            target.OutputFileColumnName = expected;
            actual = target.OutputFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for OutputFolderPath
        ///</summary>
        [TestMethod()]
        public void OutputFolderPathTest()
        {
            FileContentProcessor target = new FileContentProcessor();
            string expected = "Test Value";
            string actual;
            target.OutputFolderPath = expected;
            actual = target.OutputFolderPath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SourceFileColumnName
        ///</summary>
        [TestMethod()]
        public void SourceFileColumnNameTest()
        {
            FileContentProcessor target = new FileContentProcessor();
            string expected = "Test Value";
            string actual;
            target.SourceFileColumnName = expected;
            actual = target.SourceFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SourceFolderColumnName
        ///</summary>
        [TestMethod()]
        public void SourceFolderColumnNameTest()
        {
            FileContentProcessor target = new FileContentProcessor();
            string expected = "Test Value";
            string actual;
            target.SourceFolderColumnName = expected;
            actual = target.SourceFolderColumnName;
            Assert.AreEqual(expected, actual);
        }
    }
}
