using System.Collections.Generic;
using Mage;
using MageFileProcessor;
using NUnit.Framework;

namespace MageUnitTestsForFileProcessor
{
    /// <summary>
    /// This is a test class for PipelinesTest and is intended
    /// to contain all PipelinesTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PipelinesTest
    {
        // Ignore Spelling: dms, Mage, postgres

        private readonly int maxRows = 5;
        private readonly int rows = 3;
        private readonly int cols = 5;
        private ProcessingPipeline pipeline;
        private Dictionary<string, string> runtimeParms;

        /// <summary>
        /// A test for MakeJobQueryPipeline
        /// </summary>
        [Test]
        public void MakeJobQueryPipelineTest()
        {
            ISinkModule sinkObject = new SimpleSink(maxRows);

            // Default server info: prismdb2.emsl.pnl.gov and dms
            var queryDefXML = @"
  <query name='Mage_Analysis_Jobs'>
    <description>Get selected list of analysis jobs</description>
    <connection server='" + Globals.DMSServer + "' postgres='true' database='" + Globals.DMSDatabase + "' user='" + Globals.DMSUser + "' password='" + Globals.DMSUserPassword + "'/>" +
    "<table name='V_Mage_Analysis_Jobs' cols='*'/>" +
    "<predicate rel='AND' col='Job' cmp='Equals' val=''>Descriptive text for Job</predicate>" +
    "<predicate rel='AND' col='Dataset' cmp='ContainsText' val=''>Descriptive text for Dataset</predicate>" +
    "<sort col='Job' dir='ASC'/>" +
  "</query>";

            runtimeParms = new Dictionary<string, string>
            {
                {"Dataset", "sarc_ms"}
            };

            pipeline = Pipelines.MakeJobQueryPipeline(sinkObject, queryDefXML, runtimeParms);
            Assert.That(pipeline, Is.Not.Null);

            var source = pipeline.GetModule("SQLReader1");

            Assert.That(source, Is.Not.Null);
            Assert.That(source, Is.InstanceOf(typeof(SQLReader)));

            var target = (SQLReader)source;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(target.Database.ToLower(), Is.EqualTo(Globals.DMSDatabase.ToLower()));
                Assert.That(target.Server.ToLower(), Is.EqualTo(Globals.DMSServer.ToLower()));
                Assert.That(target.Username.ToLower(), Is.EqualTo(Globals.DMSUser.ToLower()));
                Assert.That(target.Password.ToLower(), Is.EqualTo(Globals.DMSUserPassword.ToLower()));
                Assert.That(target.SQLText, Is.EqualTo("SELECT * FROM V_Mage_Analysis_Jobs WHERE Dataset LIKE '%sarc_ms%' ORDER BY Job ASC"));
            }
        }

        /// <summary>
        /// A test for MakePipelineToGetLocalFileList
        /// </summary>
        [Test]
        public void MakePipelineToGetLocalFileListTest()
        {
            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>
            {
                {"Folder", "TestVal" },
                {"FileNameFilter", "TestVal" }
            };

            pipeline = Pipelines.MakePipelineToGetLocalFileList(sinkObject, runtimeParms);
            Assert.That(pipeline, Is.Not.Null);

            var source = pipeline.GetModule("FileListFilter1");

            Assert.That(source, Is.Not.Null);
            Assert.That(source, Is.InstanceOf(typeof(FileListFilter)));

            var target = (FileListFilter)source;

            Assert.That(target.FileNameSelector, Is.EqualTo("TestVal"));
        }

        /// <summary>
        /// A test for MakePipelineToGetFilesFromManifest
        /// </summary>
        [Test]
        public void MakePipelineToGetFilesFromManifestTest()
        {
            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>
            {
                {"ManifestFilePath", "TestVal" }
            };

            pipeline = Pipelines.MakePipelineToGetFilesFromManifest(sinkObject, runtimeParms);
            Assert.That(pipeline, Is.Not.Null);

            var source = pipeline.GetModule("DelimitedFileReader1");

            Assert.That(source, Is.Not.Null);
            Assert.That(source, Is.InstanceOf(typeof(DelimitedFileReader)));

            var target = (DelimitedFileReader)source;

            Assert.That(target.FilePath, Is.EqualTo("TestVal"));

            source = pipeline.GetModule("NullFilter2");
            Assert.That(source, Is.Not.Null);
            Assert.That(source, Is.InstanceOf(typeof(NullFilter)));
        }

        /// <summary>
        /// A test for MakePipelineToFilterSelectedFiles
        /// </summary>
        [Test]
        public void MakePipelineToFilterSelectedFilesTest()
        {
            IBaseModule sourceObject = new DataGenerator(rows, cols);

            // For mode "File_Output"

            runtimeParms = new Dictionary<string, string> {
                { "OutputDirectory", "--" },
                { "OutputFile", "" },
                { "OutputMode", ""},
                { "ApplyPrefixToFileName", "File_Output" }
            };

            var filterParms = new Dictionary<string, string> {
                {"SelectedFilterClassName", "Bogus"}
            };

            pipeline = Pipelines.MakePipelineToFilterSelectedFiles(sourceObject, runtimeParms, filterParms);
            Assert.That(pipeline, Is.Not.Null);

            var module = pipeline.GetModule("DataGenerator1");

            Assert.That(module, Is.Not.Null);
            Assert.That(module, Is.InstanceOf(typeof(DataGenerator)));

            // ReSharper disable once UnusedVariable
            var target = (DataGenerator)module;

            module = pipeline.GetModule("FileSubPipelineBroker2");
            Assert.That(module, Is.Not.Null);
            Assert.That(module, Is.InstanceOf(typeof(FileSubPipelineBroker)));
            var broker = (FileSubPipelineBroker)module;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(broker.FileFilterModuleName, Is.EqualTo("Bogus"));
                Assert.That(broker.DatabaseName, Is.Empty);
                Assert.That(broker.TableName, Is.Empty);
            }

            module = pipeline.GetModule("DelimitedFileWriter3");
            Assert.That(module, Is.Not.Null);
            Assert.That(module, Is.InstanceOf(typeof(DelimitedFileWriter)));
            var writer = (DelimitedFileWriter)module;

            Assert.That(writer.FilePath, Does.Contain("RunLog_Bogus_"), "Unexpected FilePath for writer: " + writer.FilePath);

            // For mode "SQLite_Output":

            filterParms = new Dictionary<string, string> {
                {"SelectedFilterClassName", "Bogus"}
            };

            runtimeParms = new Dictionary<string, string> {
                { "OutputDirectory", "--" },
                { "OutputFile", "" },
                { "OutputMode", "SQLite_Output"},
                { "ApplyPrefixToFileName", "SQLite_Output" },
                { "DatabaseName", "--" },
                { "TableName", "--" }
            };

            pipeline = Pipelines.MakePipelineToFilterSelectedFiles(sourceObject, runtimeParms, filterParms);
            Assert.That(pipeline, Is.Not.Null, "Failed to make pipeline for SQLite mode");

            module = pipeline.GetModule("FileSubPipelineBroker2");
            Assert.That(module, Is.Not.Null);
            Assert.That(module, Is.InstanceOf(typeof(FileSubPipelineBroker)));
            broker = (FileSubPipelineBroker)module;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(broker.DatabaseName, Is.EqualTo("--"));
                Assert.That(broker.TableName, Is.EqualTo("--"));
            }
        }

        /// <summary>
        /// A test for MakeFileListPipeline
        /// </summary>
        [Test]
        public void MakeFileListPipelineTest()
        {
            IBaseModule sourceObject = new DataGenerator(rows, cols);

            ISinkModule sinkObject = new SimpleSink(maxRows);

            runtimeParms = new Dictionary<string, string>
            {
                { "FileColumnName", "File" },
                { "IncludeFilesOrDirectories", "Files" },
                { "FileSelectors", "log.txt" },
                { "FileSelectionMode", FileListFilter.FILE_SELECTOR_NORMAL },
                { "SearchInSubdirectories", "No"},
                { "SubdirectorySearchName", "*"},
                { "SourceDirectoryColumnName", "Folder" },
                { "ResolveCacheInfoFiles", "True"},
                { "OutputColumnList", "File|+|text, Directory, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument" }
            };

            pipeline = Pipelines.MakeFileListPipeline(sourceObject, sinkObject, runtimeParms);
            Assert.That(pipeline, Is.Not.Null);
        }

        /// <summary>
        /// A test for MakeFileCopyPipeline
        /// </summary>
        [Test]
        public void MakeFileCopyPipelineTest()
        {
            IBaseModule sourceObject = new DataGenerator(rows, cols);

            runtimeParms = new Dictionary<string, string> {
                { "SourceDirectoryColumnName", "Directory" },
                { "SourceFileColumnName", "File" },
                { "OutputFileColumnName", "File" },
                { "OutputDirectory", "--" },
                { "OutputColumnList", "File, Job, Dataset, Dataset_ID, Tool, Settings_File, Parameter_File, Instrument" },
                { "ApplyPrefixToFileName", "Yes" },
                { "PrefixLeader", "Blue" },
                { "ColumnToUseForPrefix", "Job" },
                { "OverwriteExistingFiles", "No"},
                { "ResolveCacheInfoFiles", "True"},
                { "ManifestFileName", string.Format("Manifest_{0:yyMMddhhmmss}.txt", System.DateTime.Now) }
            };

            pipeline = Pipelines.MakeFileCopyPipeline(sourceObject, runtimeParms);
            Assert.That(pipeline, Is.Not.Null);
        }
    }
}
