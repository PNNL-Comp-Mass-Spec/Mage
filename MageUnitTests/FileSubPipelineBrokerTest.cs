using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace MageUnitTests {


    /// <summary>
    ///This is a test class for FileSubPipelineBrokerTest and is intended
    ///to contain all FileSubPipelineBrokerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileSubPipelineBrokerTest {

        #region Member Variables

        string mPipelineResults = "";
        string mPipelineInputFilePath = "";
        string mPipelineOutputFilePath = "";
        static string mPipelineDummyModuleResults = "";

        #endregion

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
        public void FileSubPipelineBrokerBasicTest() {

            // set up test parameters
            string IDColumnName = "Padding";
            string IDColContents = "Borked";
            string folderColName = "Folder_Col";
            string fileColName = "File_Col";
            string TestFolderPath = System.Environment.CurrentDirectory;
            string TestInputFileName = "victim.txt";
            string destFolder = System.Environment.CurrentDirectory;

            // set up data generator
            DataGenerator dGen = new DataGenerator(2, 4);
            dGen.AddAdHocRow = new string[] { folderColName, fileColName, IDColumnName };
            dGen.AddAdHocRow = new string[] { TestFolderPath, TestInputFileName, IDColContents };

            // set up test class
            FileSubPipelineBroker target = new FileSubPipelineBroker();
            target.SourceFileColumnName = fileColName;
            target.OutputFileColumnName = fileColName;
            target.SourceFolderColumnName = folderColName;
            target.OutputFolderPath = destFolder;
            target.OutputColumnList = string.Format("{0}|+|text, {1}", fileColName, folderColName);
            target.SetPipelineMaker(MakeTestSubpipeline);
            target.SetOutputFileNamer(RenameOutputFile);


            // build and run pipeline
            ProcessingPipeline pipeline = new ProcessingPipeline("FileColumnProcessorTest");
            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Target", target);
            pipeline.ConnectModules("Gen", "Target");
            pipeline.RunRoot(null);

            Assert.AreEqual("Kilroy was here", mPipelineResults, "Subpipeline creation.");
            Assert.AreEqual(Path.Combine(TestFolderPath, TestInputFileName), mPipelineInputFilePath, "Subpipeline input file path" );
            Assert.AreEqual(Path.Combine(destFolder, RenameOutputFile(TestInputFileName, null, null)), mPipelineOutputFilePath, "Subpipeline output file path");
            Assert.AreEqual("Kilroy was here too", mPipelineDummyModuleResults, "Dummy module results");
        }

        // this function builds the a file processing pipeline 
        // which is a simple file filtering process that has a file reader module, file filter module, and file writer module,
        // using a filter module specified by the FilterModuleClasssName, which must be set by the client
        private ProcessingPipeline MakeTestSubpipeline(string inputFilePath, string outputFilePath, Dictionary<string, string> context) {
            mPipelineResults = "Kilroy was here";
            mPipelineInputFilePath = inputFilePath;
            mPipelineOutputFilePath = outputFilePath;

            ProcessingPipeline pipeline = new ProcessingPipeline("TestSubpipeline");
            DummyModule dm = new DummyModule();
            pipeline.RootModule = pipeline.AddModule("Mule", dm);
            return pipeline;
        }

        // delegate that handles renaming of source file to output file 
        public string RenameOutputFile(string sourceFile, Dictionary<string, int> fieldPos, object[] fields) {
            return "out_" + sourceFile;
        }

        /// <summary>
        ///A test for SetPipelineMaker
        ///</summary>
        [TestMethod()]
        public void SetPipelineMakerTest() {
            FileSubPipelineBroker target = new FileSubPipelineBroker();
            target.SetPipelineMaker(MakeTestSubpipeline);
        }

        /// <summary>
        ///A test for DatabaseName
        ///</summary>
        [TestMethod()]
        public void DatabaseNameTest() {
            FileSubPipelineBroker target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            string expected = "Test Value";
            string actual;
            target.DatabaseName = expected;
            actual = target.DatabaseName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FileFilterModuleName
        ///</summary>
        [TestMethod()]
        public void FileFilterModuleNameTest() {
            FileSubPipelineBroker target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            string expected = "Test Value";
            string actual;
            target.FileFilterModuleName = expected;
            actual = target.FileFilterModuleName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FileFilterParameters
        ///</summary>
        [TestMethod()]
        public void FileFilterParametersTest() {
            FileSubPipelineBroker target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            Dictionary<string, string> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, string> actual;
            target.SetFileFilterParameters(expected);
            actual = target.GetFileFilterParameters();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TableName
        ///</summary>
        [TestMethod()]
        public void TableNameTest() {
            FileSubPipelineBroker target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            string expected = "Test Value";
            string actual;
            target.TableName = expected;
            actual = target.TableName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// This is a simple test version of a Mage module for the basic test pipeline.
        /// It just leaves a marker that it ran
        /// </summary>
        private class DummyModule : BaseModule {

            /// <summary>
            /// test
            /// </summary>
            /// <param name="state"></param>
            public override void Run(object state) {
                FileSubPipelineBrokerTest.mPipelineDummyModuleResults = "Kilroy was here too";
            }
        }

    }
}
