using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace MageUnitTests {


    /// <summary>
    ///This is a test class for FileListFilterTest and is intended
    ///to contain all FileListFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileListFilterTest {

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
        ///A test for FileListFilter run as source using RegEx file selector mode
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\TargetFolder")]
        public void RunFileListFilterAsSourceRegEx() {

            string testFolderPath = Path.GetFullPath(".");

            FileListFilter target = new FileListFilter();
            target.AddFolderPath(testFolderPath);
            target.FileNameSelector = "2.txt;3.txt";

            target.FileColumnName = "File";
            target.SourceFolderColumnName = "Folder";
            target.FileTypeColumnName = "Item";
            target.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text", target.FileTypeColumnName, target.FileColumnName, target.SourceFolderColumnName);
            target.IncludeFilesOrFolders = "File";

            SimpleSink sink = new SimpleSink();

            ProcessingPipeline pipeline = new ProcessingPipeline("RunAsSource");
            string sourceModName = "Source";
            string sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, sinkModName);

            pipeline.RunRoot(null);

            int hits = 0;
            int fileNameColIndx = 1;
            foreach (object[] item in sink.Rows) {
                string s = item[fileNameColIndx].ToString();
                if (s == "TargetFile2.txt") ++hits;
                if (s == "TargetFile3.txt") ++hits;
            }

            Assert.AreEqual(2, sink.Rows.Count, "Expected total of files found did not match");
            Assert.AreEqual(2, hits, "Expected specific files found did not match");

        }

        /// <summary>
        ///A test for FileListFilter run as source using file search selector mode
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\TargetFolder")]
        public void RunFileListFilterAsSourceFileSearch() {

            string testFolderPath = Path.GetFullPath(".");

            FileListFilter target = new FileListFilter();
            target.AddFolderPath(testFolderPath);
            target.FileNameSelector = "*2.txt;*3.txt";
            target.FileSelectorMode = "FileSearch";

            target.FileColumnName = "File";
            target.SourceFolderColumnName = "Folder";
            target.FileTypeColumnName = "Item";
            target.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text", target.FileTypeColumnName, target.FileColumnName, target.SourceFolderColumnName);
            target.IncludeFilesOrFolders = "File";

            SimpleSink sink = new SimpleSink();

            ProcessingPipeline pipeline = new ProcessingPipeline("RunAsSource");
            string sourceModName = "Source";
            string sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, sinkModName);

            pipeline.RunRoot(null);

            int hits = 0;
            int fileNameColIndx = 1;
            foreach (object[] item in sink.Rows) {
                string s = item[fileNameColIndx].ToString();
                if (s == "TargetFile2.txt") ++hits;
                if (s == "TargetFile3.txt") ++hits;
            }

            Assert.AreEqual(2, sink.Rows.Count, "Expected total of files found did not match");
            Assert.AreEqual(2, hits, "Expected specific files found did not match");

        }


        /// <summary>
        ///A test for GetFileNamesFromSourceFolder
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mage.dll")]
        public void GetFileNamesFromSourceFolderTest() {

            FileListFilter_Accessor target = new FileListFilter_Accessor(); // TODO: Initialize to an appropriate value
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("FolderPath", "TestFolderPath");
            target.SetParameters(parms);

            parms.Clear();
            parms.Add("FileNameSelector", "TestFileNameSelector");
            target.SetParameters(parms);

            List<object[]> outputBuffer = target.mOutputBuffer;
            Assert.AreEqual(1, outputBuffer.Count);
        }

        [TestMethod()]
        public void PropertiesSetTest() {
            string expected;
            string actual;
            Dictionary<string, string> parms = new Dictionary<string, string>();

            FileListFilter target = new FileListFilter();

            expected = "Test01";
            parms.Clear();
            parms.Add("FileColumnName", expected);
            target.SetParameters(parms);
            actual = target.FileColumnName;
            Assert.AreEqual(expected, actual);

            expected = "Test03";
            parms.Clear();
            parms.Add("SourceFolderColumnName", expected);
            target.SetParameters(parms);
            actual = target.SourceFolderColumnName;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for FileColumnName
        ///</summary>
        [TestMethod()]
        public void FileColumnNameTest() {
            FileListFilter target = new FileListFilter();
            string expected = "Test Value";
            string actual;
            target.FileColumnName = expected;
            actual = target.FileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SourceFolderColumnName
        ///</summary>
        [TestMethod()]
        public void SourceFolderColumnNameTest() {
            FileListFilter target = new FileListFilter();
            string expected = "Test Value";
            string actual;
            target.SourceFolderColumnName = expected;
            actual = target.SourceFolderColumnName;
            Assert.AreEqual(expected, actual);
        }


    }
}
