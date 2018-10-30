using Mage;
using NUnit.Framework;
using System.IO;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for FileContentProcessorTest and is intended
    /// to contain all FileContentProcessorTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileContentProcessorTest
    {

        [Test]
        public void FileColumnProcessorTest()
        {

            // Set up test parameters
            var directoryColName = "Directory_Col";
            var fileColName = "File_Col";
            var currentDir = System.Environment.CurrentDirectory;
            var file = "tab_delimited.txt";
            var destinationDir = @"C:\data\";

            // Set up data generator
            var dGen = new DataGenerator(2, 4)
            {
                AddAdHocRow = new[] { directoryColName, fileColName, "Padding" }
            };

            dGen.AddAdHocRow = new[] { currentDir, file, "Padding" };

            // Set up test harness (subclass of file processor module)
            var target = new TestFileContentProcessorModule
            {
                SourceFileColumnName = fileColName,
                OutputFileColumnName = fileColName,
                SourceDirectoryColumnName = directoryColName,
                OutputDirectoryPath = destinationDir,
                OutputColumnList = string.Format("{0}|+|text, {1}", fileColName, directoryColName),
                ExpectedSourceFile = file,
                ExpectedSourcePath = Path.GetFullPath(Path.Combine(currentDir, file)),
                ExpectedDestPath = Path.GetFullPath(Path.Combine(destinationDir, file))
            };

            // Tell the test harness what to expect

            // Build and run pipeline
            var pipeline = new ProcessingPipeline("FileColumnProcessorTest");
            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Target", target);
            pipeline.ConnectModules("Gen", "Target");
            pipeline.RunRoot(null);
        }

        /// <summary>
        /// A test for FileColumnName
        /// </summary>
        [Test]
        public void OutputFileColumnNameTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.OutputFileColumnName = expected;
            var actual = target.OutputFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for OutputDirectoryPath
        /// </summary>
        [Test]
        public void OutputDirectoryPathTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.OutputDirectoryPath = expected;
            var actual = target.OutputDirectoryPath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for SourceFileColumnName
        /// </summary>
        [Test]
        public void SourceFileColumnNameTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.SourceFileColumnName = expected;
            var actual = target.SourceFileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for SourceDirectoryColumnName
        /// </summary>
        [Test]
        public void SourceDirectoryColumnNameTest()
        {
            var target = new FileContentProcessor();
            var expected = "Test Value";
            target.SourceDirectoryColumnName = expected;
            var actual = target.SourceDirectoryColumnName;
            Assert.AreEqual(expected, actual);
        }
    }
}
