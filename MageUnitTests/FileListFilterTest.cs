using Mage;
using System.Collections.Generic;
using NUnit.Framework;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for FileListFilterTest and is intended
    /// to contain all FileListFilterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileListFilterTest
    {

        /// <summary>
        /// A test for FileListFilter run as source using RegEx file selector mode
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\TestItems\TargetDirectory")]
        public void RunFileListFilterAsSourceRegEx(string testDirectoryPath)
        {
            var testDirectory = General.GetTestDirectory(testDirectoryPath);

            var target = new FileListFilter
            {
                FileColumnName = "File",
                SourceDirectoryColumnName = "Directory",
                FileTypeColumnName = "Item"
            };
            target.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text", target.FileTypeColumnName, target.FileColumnName, target.SourceDirectoryColumnName);
            target.IncludeFilesOrDirectories = "File";

            target.AddDirectoryPath(testDirectory.FullName);
            target.FileNameSelector = @"2.txt;3\.txt";
            target.FileSelectorMode = "RegEx";

            var sink = new SimpleSink();

            var pipeline = new ProcessingPipeline("RunAsSource");
            var sourceModName = "Source";
            var sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, sinkModName);

            pipeline.RunRoot(null);

            var hits = 0;
            var fileNameColIndex = 1;
            foreach (var item in sink.Rows)
            {
                var s = item[fileNameColIndex];
                if (s == "TargetFile2.txt")
                    ++hits;
                if (s == "TargetFile3.txt")
                    ++hits;
            }

            Assert.AreEqual(2, sink.Rows.Count, "Expected total of files found did not match");
            Assert.AreEqual(2, hits, "Expected specific files found did not match");

        }

        /// <summary>
        /// A test for FileListFilter run as source using file search selector mode
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\TestItems\TargetDirectory")]
        public void RunFileListFilterAsSourceFileSearch(string testDirectoryPath)
        {

            var testDirectory = General.GetTestDirectory(testDirectoryPath);

            var target = new FileListFilter
            {
                FileColumnName = "File",
                SourceDirectoryColumnName = "Directory",
                FileTypeColumnName = "Item"
            };
            target.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text", target.FileTypeColumnName, target.FileColumnName, target.SourceDirectoryColumnName);
            target.IncludeFilesOrDirectories = "File";

            target.AddDirectoryPath(testDirectory.FullName);
            target.FileNameSelector = "*2.txt;*3.txt";
            target.FileSelectorMode = "FileSearch";

            var sink = new SimpleSink();

            var pipeline = new ProcessingPipeline("RunAsSource");
            var sourceModName = "Source";
            var sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, sinkModName);

            pipeline.RunRoot(null);

            var hits = 0;
            var fileNameColIndex = 1;
            foreach (var item in sink.Rows)
            {
                var s = item[fileNameColIndex];
                if (s == "TargetFile2.txt")
                    ++hits;
                if (s == "TargetFile3.txt")
                    ++hits;
            }

            Assert.AreEqual(2, sink.Rows.Count, "Expected total of files found did not match");
            Assert.AreEqual(2, hits, "Expected specific files found did not match");

        }

        /// <summary>
        /// A class to provide access to private member variables of FileListFilter.
        /// Original method was using private accessors, deprecated starting in 2010
        /// Another option was using PrivateObject, which requires
        ///    Microsoft.VisualStudio.TestTools.UnitTesting and performs operations using reflection.
        /// </summary>
        private class FileListFilterExtractor : FileListFilter
        {
            public List<string[]> OutputBuffer => mOutputBuffer;
        }

        /// <summary>
        /// A test for GetFileNamesFromSourceDirectory
        /// </summary>
        [Test]
        public void GetFileNamesFromSourceDirectoryTest()
        {

            var target = new FileListFilterExtractor(); // TODO: Initialize to an appropriate value
            var parms = new Dictionary<string, string> {
                { "DirectoryPath", "TestDirectoryPath"}
            };
            target.SetParameters(parms);

            parms.Clear();
            parms.Add("FileNameSelector", "TestFileNameSelector");
            target.SetParameters(parms);

            var outputBuffer = target.OutputBuffer;
            Assert.AreEqual(1, outputBuffer.Count);
        }

        [Test]
        public void PropertiesSetTest()
        {
            var parms = new Dictionary<string, string>();

            var target = new FileListFilter();

            var expected = "Test01";
            parms.Clear();
            parms.Add("FileColumnName", expected);
            target.SetParameters(parms);
            var actual = target.FileColumnName;
            Assert.AreEqual(expected, actual);

            expected = "Test03";
            parms.Clear();
            parms.Add("SourceDirectoryColumnName", expected);
            target.SetParameters(parms);
            actual = target.SourceDirectoryColumnName;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for FileColumnName
        /// </summary>
        [Test]
        public void FileColumnNameTest()
        {
            var target = new FileListFilter();
            var expected = "Test Value";
            target.FileColumnName = expected;
            var actual = target.FileColumnName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for SourceDirectoryColumnName
        /// </summary>
        [Test]
        public void SourceDirectoryColumnNameTest()
        {
            var target = new FileListFilter();
            var expected = "Test Value";
            target.SourceDirectoryColumnName = expected;
            var actual = target.SourceDirectoryColumnName;
            Assert.AreEqual(expected, actual);
        }


    }
}
