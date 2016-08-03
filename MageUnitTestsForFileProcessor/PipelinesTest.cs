using MageFileProcessor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mage;
using System.Collections.Generic;

namespace MageUnitTestsForFileProcessor
{

    /// <summary>
    ///This is a test class for PipelinesTest and is intended
    ///to contain all PipelinesTest Unit Tests
    ///</summary>
    [TestClass()]
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
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
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
            Assert.IsInstanceOfType(source, typeof(MSSQLReader));

            var target = (MSSQLReader)source;
            Assert.AreEqual(Globals.DMSDatabase.ToLower(), target.Database.ToLower());
            Assert.AreEqual(Globals.DMSServer.ToLower(), target.Server.ToLower());
            Assert.AreEqual("SELECT * FROM V_Mage_Analysis_Jobs WHERE [Dataset] LIKE '%sarc_ms%' ORDER BY [Job] ASC", target.SQLText);
        }

        /// <summary>
        ///A test for MakePipelineToGetLocalFileList
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
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
            Assert.IsInstanceOfType(source, typeof(FileListFilter));

            var target = (FileListFilter)source;
            Assert.AreEqual("TestVal", target.FileNameSelector);
        }

        /// <summary>
        ///A test for MakePipelineToGetFilesFromManifest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
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
            Assert.IsInstanceOfType(source, typeof(DelimitedFileReader));

            var target = (DelimitedFileReader)source;
            Assert.AreEqual("TestVal", target.FilePath);

            source = pipeline.GetModule("NullFilter2");
            Assert.AreNotEqual(null, source);
            Assert.IsInstanceOfType(source, typeof(NullFilter));
        }

        /// <summary>
        ///A test for MakePipelineToFilterSelectedfiles
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
        public void MakePipelineToFilterSelectedfilesTest()
        {

            IBaseModule sourceObject = new DataGenerator(rows, cols);

            // for mode "File_Output"

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
            Assert.IsInstanceOfType(module, typeof(DataGenerator));
            var target = (DataGenerator)module;

            module = pipeline.GetModule("FileSubPipelineBroker2");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOfType(module, typeof(FileSubPipelineBroker));
            var broker = (FileSubPipelineBroker)module;
            Assert.AreEqual("Bogus", broker.FileFilterModuleName);
            Assert.AreEqual("", broker.DatabaseName);
            Assert.AreEqual("", broker.TableName);

            module = pipeline.GetModule("DelimitedFileWriter3");
            Assert.AreNotEqual(null, module);
            Assert.IsInstanceOfType(module, typeof(DelimitedFileWriter));
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
            Assert.IsInstanceOfType(module, typeof(FileSubPipelineBroker));
            broker = (FileSubPipelineBroker)module;
            Assert.AreEqual("--", broker.DatabaseName);
            Assert.AreEqual("--", broker.TableName);
        }

        /// <summary>
        ///A test for MakeFileListPipeline
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
        public void MakeFileListPipelineTest()
        {

            IBaseModule sourceObject = new DataGenerator(rows, cols);

            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>() {
                { "FileColumnName", "File" },
                { "IncludeFilesOrFolders", "Files" },
                { "FileSelectors", "log.txt" },
                { "FileSelectionMode", "RegEx" },
                { "SearchInSubfolders", "No"},
                { "SubfolderSearchName", "*"},
                { "SourceFolderColumnName", "Folder" },
                { "OutputColumnList", "File|+|text, Folder, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument" }
            };

            pipeline = Pipelines.MakeFileListPipeline(sourceObject, sinkObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);
        }

        /// <summary>
        ///A test for MakeFileCopyPipeline
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MageFileProcessor.exe")]
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
                { "ManifestFileName", string.Format("Manifest_{0:yyMMddhhmmss}.txt", System.DateTime.Now) }
            };

            pipeline = Pipelines.MakeFileCopyPipeline(sourceObject, runtimeParms);
            Assert.AreNotEqual(null, pipeline);
        }

    }
}
