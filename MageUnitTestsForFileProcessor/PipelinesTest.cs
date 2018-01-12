using MageFileProcessor;
using Mage;
using System.Collections.Generic;
using NUnit.Framework;

namespace MageUnitTestsForFileProcessor
{

    /// <summary>
    ///This is a test class for PipelinesTest and is intended
    ///to contain all PipelinesTest Unit Tests
    ///</summary>
    [TestFixture]
    public class PipelinesTest
    {

        private readonly int maxRows = 5;
        private readonly int rows = 3;
        private readonly int cols = 5;
        ProcessingPipeline pipeline;
        Dictionary<string, string> runtimeParms;

        /// <summary>
        ///A test for MakeJobQueryPipeline
        ///</summary>
        [Test]
        public void MakeJobQueryPipelineTest()
        {

            ISinkModule sinkObject = new SimpleSink(maxRows);

            // Default server info: gigasax and DMS5
            var queryDefXML = @"
  <query name='Mage_Analysis_Jobs'>
    <description>Get selected list of analysis jobs</description>
    <connection server='" + Globals.DMSServer + "' database='" + Globals.DMSDatabase + "'/>" +
    "<table name='V_Mage_Analysis_Jobs' cols='*'/>" +
    "<predicate rel='AND' col='Job' cmp='Equals' val=''>Descriptive text for Job</predicate>" +
    "<predicate rel='AND' col='Dataset' cmp='ContainsText' val=''>Descriptive text for Dataset</predicate>" +
    "<sort col='Job' dir='ASC'/>" +
  "</query>";
            runtimeParms = new Dictionary<string, string>() {
                {"Dataset", "sarc_ms"}
            };

            pipeline = Pipelines.MakeJobQueryPipeline(sinkObject, queryDefXML, runtimeParms);
            Assert.AreNotEqual(null, pipeline);

            var source = pipeline.GetModule("MSSQLReader1");
            Assert.AreNotEqual(null, source);
            Assert.IsInstanceOf(typeof(MSSQLReader), source);

            var target = (MSSQLReader)source;
            Assert.AreEqual(Globals.DMSDatabase.ToLower(), target.Database.ToLower());
            Assert.AreEqual(Globals.DMSServer.ToLower(), target.Server.ToLower());
            Assert.AreEqual("SELECT * FROM V_Mage_Analysis_Jobs WHERE [Dataset] LIKE '%sarc_ms%' ORDER BY [Job] ASC", target.SQLText);
        }

        /// <summary>
        ///A test for MakePipelineToGetLocalFileList
        ///</summary>
        [Test]
        public void MakePipelineToGetLocalFileListTest()
        {

            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>() {
                {"Folder", "TestVal" },
                {"FileNameFilter", "TestVal" }
            };

            pipeline = Pipelines.MakePipelineToGetLocalFileList(sinkObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);

            var source = pipeline.GetModule("FileListFilter1");
            Assert.AreNotEqual(null, source);
            Assert.IsInstanceOf(typeof(FileListFilter), source);

            var target = (FileListFilter)source;
            Assert.AreEqual("TestVal", target.FileNameSelector);
        }

        /// <summary>
        ///A test for MakePipelineToGetFilesFromManifest
        ///</summary>
        [Test]
        public void MakePipelineToGetFilesFromManifestTest()
        {

            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>() {
                {"ManifestFilePath", "TestVal" }
            };

            pipeline = Pipelines.MakePipelineToGetFilesFromManifest(sinkObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);

            var source = pipeline.GetModule("DelimitedFileReader1");
            Assert.AreNotEqual(null, source);
            Assert.IsInstanceOf(typeof(DelimitedFileReader), source);

            var target = (DelimitedFileReader)source;
            Assert.AreEqual("TestVal", target.FilePath);

            source = pipeline.GetModule("NullFilter2");
            Assert.AreNotEqual(null, source);
            Assert.IsInstanceOf(typeof(NullFilter), source);
        }

        /// <summary>
        ///A test for MakePipelineToFilterSelectedfiles
        ///</summary>
        [Test]
        public void MakePipelineToFilterSelectedfilesTest()
        {

            IBaseModule sourceObject = new DataGenerator(rows, cols);

            // For mode "File_Output"

            runtimeParms = new Dictionary<string, string>() {
                { "OutputFolder", "--" },
                { "OutputFile", "" },
                { "OutputMode", ""},
                { "ApplyPrefixToFileName", "File_Output" }
            };

            var filterParms = new Dictionary<string, string>() {
                {"SelectedFilterClassName", "Bogus"}
            };

            pipeline = Pipelines.MakePipelineToFilterSelectedfiles(sourceObject, runtimeParms, filterParms);
            Assert.AreNotEqual(null, pipeline);

            var module = pipeline.GetModule("DataGenerator1");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOf(typeof(DataGenerator), module);
            var target = (DataGenerator)module;

            module = pipeline.GetModule("FileSubPipelineBroker2");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOf(typeof(FileSubPipelineBroker), module);
            var broker = (FileSubPipelineBroker)module;
            Assert.AreEqual("Bogus", broker.FileFilterModuleName);
            Assert.AreEqual("", broker.DatabaseName);
            Assert.AreEqual("", broker.TableName);

            module = pipeline.GetModule("DelimitedFileWriter3");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOf(typeof(DelimitedFileWriter), module);
            var writer = (DelimitedFileWriter)module;
            Assert.IsTrue(writer.FilePath.Contains("Runlog_Bogus_"));

            // For mode "SQLite_Output":

            filterParms = new Dictionary<string, string>() {
                {"SelectedFilterClassName", "Bogus"}
            };

            runtimeParms = new Dictionary<string, string>() {
                { "OutputFolder", "--" },
                { "OutputFile", "" },
                { "OutputMode", "SQLite_Output"},
                { "ApplyPrefixToFileName", "SQLite_Output" },
                { "DatabaseName", "--" },
                { "TableName", "--" }
            };

            pipeline = Pipelines.MakePipelineToFilterSelectedfiles(sourceObject, runtimeParms, filterParms);
            Assert.AreNotEqual(null, pipeline, "Failed to make pipeline for SQLite mode");

            module = pipeline.GetModule("FileSubPipelineBroker2");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOf(typeof(FileSubPipelineBroker), module);
            broker = (FileSubPipelineBroker)module;
            Assert.AreEqual("--", broker.DatabaseName);
            Assert.AreEqual("--", broker.TableName);
        }

        /// <summary>
        ///A test for MakeFileListPipeline
        ///</summary>
        [Test]
        public void MakeFileListPipelineTest()
        {

            IBaseModule sourceObject = new DataGenerator(rows, cols);

            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>() {
                { "FileColumnName", "File" },
                { "IncludeFilesOrFolders", "Files" },
                { "FileSelectors", "log.txt" },
                { "FileSelectionMode", FileListFilter.FILE_SELECTOR_NORMAL },
                { "SearchInSubfolders", "No"},
                { "SubfolderSearchName", "*"},
                { "SourceFolderColumnName", "Folder" },
                { "ResolveCacheInfoFiles", "True"},
                { "OutputColumnList", "File|+|text, Folder, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument" }
            };

            pipeline = Pipelines.MakeFileListPipeline(sourceObject, sinkObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);
        }

        /// <summary>
        ///A test for MakeFileCopyPipeline
        ///</summary>
        [Test]
        public void MakeFileCopyPipelineTest()
        {

            IBaseModule sourceObject = new DataGenerator(rows, cols);

            runtimeParms = new Dictionary<string, string>() {
                { "SourceFolderColumnName", "Folder" },
                { "SourceFileColumnName", "File" },
                { "OutputFileColumnName", "File" },
                { "OutputFolder", "--" },
                { "OutputColumnList", "File, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument" },
                { "ApplyPrefixToFileName", "Yes" },
                { "PrefixLeader", "Blue" },
                { "ColumnToUseForPrefix", "Job" },
                { "OverwriteExistingFiles", "No"},
                { "ResolveCacheInfoFiles", "True"},
                { "ManifestFileName", string.Format("Manifest_{0:yyMMddhhmmss}.txt", System.DateTime.Now) }
            };

            pipeline = Pipelines.MakeFileCopyPipeline(sourceObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);
        }

    }
}
