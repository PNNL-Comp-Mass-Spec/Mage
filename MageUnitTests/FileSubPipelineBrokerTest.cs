using Mage;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for FileSubPipelineBrokerTest and is intended
    /// to contain all FileSubPipelineBrokerTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileSubPipelineBrokerTest
    {

        #region Member Variables

        string mPipelineResults = "";
        string mPipelineInputFilePath = "";
        string mPipelineOutputFilePath = "";
        static string mPipelineDummyModuleResults = "";

        #endregion

        [Test]
        public void FileSubPipelineBrokerBasicTest()
        {

            // Set up test parameters
            var idColumnName = "Padding";
            var idColContents = "Borked";
            var folderColName = "Folder_Col";
            var fileColName = "File_Col";
            var testFolderPath = System.Environment.CurrentDirectory;
            var testInputFileName = "victim.txt";
            var destFolder = System.Environment.CurrentDirectory;

            // Set up data generator
            var dGen = new DataGenerator(2, 4)
            {
                AddAdHocRow = new[] { folderColName, fileColName, idColumnName }
            };

            dGen.AddAdHocRow = new[] { testFolderPath, testInputFileName, idColContents };

            // Create the test input file
            var testFile = new FileInfo(Path.Combine(testFolderPath, testInputFileName));
            using (var writer = new StreamWriter(new FileStream(testFile.FullName, FileMode.Create, FileAccess.Write)))
            {
                writer.WriteLine(idColumnName + '\t' + folderColName + '\t' + fileColName);
                writer.WriteLine(idColContents + '\t' + testFolderPath + '\t' + testInputFileName);
            }

            // Set up test class
            var target = new FileSubPipelineBroker
            {
                SourceFileColumnName = fileColName,
                OutputFileColumnName = fileColName,
                SourceFolderColumnName = folderColName,
                OutputFolderPath = destFolder,
                OutputColumnList = string.Format("{0}|+|text, {1}", fileColName, folderColName)
            };
            target.SetPipelineMaker(MakeTestSubpipeline);
            target.SetOutputFileNamer(RenameOutputFile);

            // Build and run pipeline
            var pipeline = new ProcessingPipeline("FileColumnProcessorTest");
            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.AddModule("Target", target);
            pipeline.ConnectModules("Gen", "Target");
            pipeline.RunRoot(null);

            Assert.AreEqual("Kilroy was here", mPipelineResults, "Subpipeline creation.");
            Assert.AreEqual(Path.Combine(testFolderPath, testInputFileName), mPipelineInputFilePath, "Subpipeline input file path");
            Assert.AreEqual(Path.Combine(destFolder, RenameOutputFile(testInputFileName, null, null)), mPipelineOutputFilePath, "Subpipeline output file path");
            Assert.AreEqual("Kilroy was here too", mPipelineDummyModuleResults, "Dummy module results");
        }

        // This function builds the a file processing pipeline
        // which is a simple file filtering process that has a file reader module, file filter module, and file writer module,
        // using a filter module specified by the FilterModuleClasssName, which must be set by the client
        private ProcessingPipeline MakeTestSubpipeline(string inputFilePath, string outputFilePath, Dictionary<string, string> context)
        {
            mPipelineResults = "Kilroy was here";
            mPipelineInputFilePath = inputFilePath;
            mPipelineOutputFilePath = outputFilePath;

            var pipeline = new ProcessingPipeline("TestSubpipeline");
            var dm = new DummyModule();
            pipeline.RootModule = pipeline.AddModule("Mule", dm);
            return pipeline;
        }

        // Delegate that handles renaming of source file to output file
        public string RenameOutputFile(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
        {
            return "out_" + sourceFile;
        }

        /// <summary>
        /// A test for SetPipelineMaker
        /// </summary>
        [Test]
        public void SetPipelineMakerTest()
        {
            var target = new FileSubPipelineBroker();
            target.SetPipelineMaker(MakeTestSubpipeline);
        }

        /// <summary>
        /// A test for DatabaseName
        /// </summary>
        [Test]
        public void DatabaseNameTest()
        {
            var target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            var expected = "Test Value";
            target.DatabaseName = expected;
            var actual = target.DatabaseName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for FileFilterModuleName
        /// </summary>
        [Test]
        public void FileFilterModuleNameTest()
        {
            var target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            var expected = "Test Value";
            target.FileFilterModuleName = expected;
            var actual = target.FileFilterModuleName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for FileFilterParameters
        /// </summary>
        [Test]
        public void FileFilterParametersTest()
        {
            var target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            Dictionary<string, string> expected = null; // TODO: Initialize to an appropriate value
            target.SetFileFilterParameters(expected);
            var actual = target.GetFileFilterParameters();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for TableName
        /// </summary>
        [Test]
        public void TableNameTest()
        {
            var target = new FileSubPipelineBroker(); // TODO: Initialize to an appropriate value
            var expected = "Test Value";
            target.TableName = expected;
            var actual = target.TableName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// This is a simple test version of a Mage module for the basic test pipeline.
        /// It just leaves a marker that it ran
        /// </summary>
        private class DummyModule : BaseModule
        {

            /// <summary>
            /// Test
            /// </summary>
            /// <param name="state"></param>
            public override void Run(object state)
            {
                mPipelineDummyModuleResults = "Kilroy was here too";
            }
        }

    }
}
