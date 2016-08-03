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

        [TestMethod()]
        public void FileColumnProcessorTest()
        {

            // set up test parameters
            var folderColName = "Folder_Col";
            var fileColName = "File_Col";
            var folder = System.Environment.CurrentDirectory;
            var file = "tab_delim.txt";
            var destFolder = @"C:\data\";

            // set up data generator
            var dGen = new DataGenerator(2, 4)
            {
                AddAdHocRow = new[] { folderColName, fileColName, "Padding" }
            };

            dGen.AddAdHocRow = new[] { folder, file, "Padding" };

            // set up test mule (subclass of file processor module)
            var target = new TestFileContentProcessorModule
            {
                SourceFileColumnName = fileColName,
                OutputFileColumnName = fileColName,
                SourceFolderColumnName = folderColName,
                OutputFolderPath = destFolder,
                OutputColumnList = string.Format("{0}|+|text, {1}", fileColName, folderColName),
                ExpectedSourceFile = file,
                ExpectedSourcePath = Path.GetFullPath(Path.Combine(folder, file)),
                ExpectedDestPath = Path.GetFullPath(Path.Combine(destFolder, file))
            };

            // tell the test mule what to expect

            // build and run pipeline
            var pipeline = new ProcessingPipeline("FileColumnProcessorTest");
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
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.OutputFileColumnName = expected;
            var actual = target.OutputFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for OutputFolderPath
        ///</summary>
        [TestMethod()]
        public void OutputFolderPathTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.OutputFolderPath = expected;
            var actual = target.OutputFolderPath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SourceFileColumnName
        ///</summary>
        [TestMethod()]
        public void SourceFileColumnNameTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.SourceFileColumnName = expected;
            var actual = target.SourceFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SourceFolderColumnName
        ///</summary>
        [TestMethod()]
        public void SourceFolderColumnNameTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.SourceFolderColumnName = expected;
            var actual = target.SourceFolderColumnName;
            Assert.AreEqual(expected, actual);
        }
    }
}
