using System;
using Mage;
using NUnit.Framework;
using System.Collections.ObjectModel;
namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for ProcessingPipelineTest and is intended
    /// to contain all ProcessingPipelineTest Unit Tests
    /// </summary>
    [TestFixture]
    public class ProcessingPipelineTest
    {
        // Ignore Spelling: dms, Mage

        /// <summary>
        /// A test for SetModuleParameter
        /// </summary>
        [Test]
        public void SetModuleParameterTest()
        {
            var target = new ProcessingPipeline("TestPipeline");
            var tm = new TestModule();

            target.AddModule("TM", tm);
            const string tVal1 = "Test Value1";
            target.SetModuleParameter("TM", "OutputColumnList", tVal1);
            const string tVal2 = "Dummy Value";
            target.SetModuleParameter("TM", "Dummy", tVal2);

            Assert.Multiple(() =>
            {
                Assert.That(tm.OutputColumnList, Is.EqualTo(tVal1));
                Assert.That(tm.Dummy, Is.EqualTo(tVal2));
            });
        }

        /// <summary>
        /// Tests ability to make Mage basic modules by class name
        /// </summary>
        [Test]
        [TestCase("NullFilter", true)]
        [TestCase("NullFilter", true)]
        [TestCase("ContentFilter", true)]
        [TestCase("CrosstabFilter", true)]
        [TestCase("DelimitedFileReader", true)]
        [TestCase("DelimitedFileWriter", true)]
        [TestCase("FileContentProcessor", true)]
        [TestCase("FileCopy", true)]
        [TestCase("FileListFilter", true)]
        [TestCase("FileSubPipelineBroker", true)]
        [TestCase("SQLReader", true)]
        [TestCase("PermutationGenerator", true)]
        [TestCase("SimpleSink", true)]
        [TestCase("SQLiteReader", true)]
        [TestCase("SQLiteWriter", true)]
        [TestCase("NonExistentModuleName", false)]
        public void MakeModuleTest(string moduleName, bool validModule)
        {
            var mod = ProcessingPipeline.MakeModule(moduleName);

            if (validModule)
            {
                if (mod == null)
                {
                    Assert.Fail("Unrecognized module name: " + moduleName);
                }
                else
                {
                    Console.WriteLine("Created: " + mod);
                }
            }
            else
            {
                if (mod != null)
                {
                    Assert.Fail("A module was created, even though the module name should be unrecognized: " + moduleName);
                }
            }
        }

        /// <summary>
        /// Tests assembling pipeline from list of module objects
        /// </summary>
        [Test]
        public void PipelineFromModuleListTest()
        {
            const string pipelineName = "Test Pipeline";

            var reader = new DelimitedFileReader();
            var filter = new NullFilter();
            var writer = new SQLiteWriter();

            var moduleList = new Collection<object> { reader, filter, writer };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.That(pipeline, Is.Not.Null, "Pipeline not created");
            Assert.That(pipeline.PipelineName, Is.EqualTo(pipelineName), "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("NullFilter2");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("SQLiteWriter3");
            Assert.That(mod, Is.Not.Null);
        }

        /// <summary>
        /// Tests assembling pipeline from list of module class names as strings
        /// </summary>
        [Test]
        public void PipelineFromNamesListTest()
        {
            const string pipelineName = "Test Pipeline";

            var moduleList = new Collection<object> { "DelimitedFileReader", "NullFilter", "SimpleSink" };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.That(pipeline, Is.Not.Null, "Pipeline not created");
            Assert.That(pipeline.PipelineName, Is.EqualTo(pipelineName), "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("NullFilter2");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("SimpleSink3");
            Assert.That(mod, Is.Not.Null);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where modules are defined by class name
        /// </summary>
        [Test]
        public void PipelineFromNamedModuleListTest1()
        {
            const string pipelineName = "Test Pipeline";

            var namedModuleList = new Collection<ModuleDef>
            {
                new("Larry", "DelimitedFileReader"),
                new("Moe", "NullFilter"),
                new("Curly", "SimpleSink")
            };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.That(pipeline, Is.Not.Null, "Pipeline not created");
            Assert.That(pipeline.PipelineName, Is.EqualTo(pipelineName), "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("Larry");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("Moe");
            Assert.That(mod, Is.Not.Null);
            mod = pipeline.GetModule("Curly");
            Assert.That(mod, Is.Not.Null);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where some modules are directly referenced and
        /// some modules are defined by class name
        /// </summary>
        [Test]
        public void PipelineFromNamedModuleListTest2()
        {
            const string pipelineName = "Test Pipeline";

            var reader = new DelimitedFileReader();
            var writer = new SQLiteWriter();

            var namedModuleList = new Collection<ModuleDef>
            {
                new("Larry", reader),
                new("Moe", "NullFilter"),
                new("Curly", writer)
            };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.That(pipeline, Is.Not.Null, "Pipeline not created");
            Assert.That(pipeline.PipelineName, Is.EqualTo(pipelineName), "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("Larry");
            Assert.That(mod, Is.Not.Null);
            Assert.That(mod.GetType().Name, Is.EqualTo("DelimitedFileReader"));
            mod = pipeline.GetModule("Moe");
            Assert.That(mod, Is.Not.Null);
            Assert.That(mod.GetType().Name, Is.EqualTo("NullFilter"));
            mod = pipeline.GetModule("Curly");
            Assert.That(mod, Is.Not.Null);
            Assert.That(mod.GetType().Name, Is.EqualTo("SQLiteWriter"));
        }

        // Default server info: prismdb2.emsl.pnl.gov and dms
        private readonly string pipelineXML = @"
<pipeline name='Test_Pipeline' >
    <module name='Reader' type='SQLReader'>
        <param name='Server' >" + Globals.DMSServer + "</param>" +
        "<param name='Database' >" + Globals.DMSDatabase + "</param>" +
        "<param name='SQLText' >SELECT * FROM T_Campaign</param>" +
    "</module>" +
    "<module name='Sink' type='SimpleSink' />" +
"</pipeline>";

        /// <summary>
        /// A test for Build
        /// </summary>
        [Test]
        public void AssembleXML()
        {
            var pipeline = ProcessingPipeline.Assemble(pipelineXML);

            var mod = pipeline.GetModule("Reader");
            var target = mod as SQLReader;

            Assert.That(target, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(Globals.DMSServer, Is.EqualTo(target.Server));
                Assert.That(pipeline.PipelineName, Is.EqualTo("Test_Pipeline"));
            });
        }

        private readonly string pipelineXML2 = @"
<pipeline name='Test_Pipeline' >
    <module type='DelimitedFileReader'>
        <param name='FilePath'>Sarc_MS_Filtered_isos.csv</param>
    </module>
    <module type='NullFilter' />
</pipeline>
";

        /// <summary>
        /// A test for Build
        /// </summary>
        [Test]
        public void AssembleXMLAndRun()
        {
            const string dataFileName = "Sarc_MS_Filtered_isos.csv";

            const int maxRows = 7;

            var dataFile = General.GetTestFile(dataFileName);

            var pipeline = ProcessingPipeline.Assemble(pipelineXML2.Replace(dataFileName, dataFile.FullName));

            var sink = new SimpleSink(maxRows);
            // pipeline.ConnectExternalModule(sink);
            // sink.ModuleName = "Caboose";
            pipeline.AppendModule(sink);

            var mod = pipeline.GetModule("Module1");
            var target = mod as DelimitedFileReader;

            Assert.Multiple(() =>
            {
                Assert.That(target, Is.Not.Null);
                Assert.That(dataFile.FullName, Is.EqualTo(target.FilePath));
            });

            pipeline.RunRoot(null);

            // Did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;

            Console.WriteLine("Pipeline returned {0} rows of data", rows.Count);
            Assert.That(rows, Has.Count.EqualTo(maxRows), "Sink did not receive the expected number of rows");
        }

        private readonly string pipelineXML3 = @"<?xml version='1.0' encoding='UTF-8'?><!-- to get filtered list of files in local directory -->
<pipeline name='Test_Pipeline'>
    <module type='FileListFilter'>
        <param name='DirectoryPath'><![CDATA[\\proto-2\unitTest_Files\Mage\SynopsisFiles]]></param>
        <param name='FileNameSelector'>_syn.txt</param>
        <param name='FileTypeColumnName'>Item</param>
        <param name='FileColumnName'>Name</param>
        <param name='SourceDirectoryColumnName'>Directory</param>
        <param name='OutputColumnList'>Item|+|text, Name|+|text, Directory|+|text</param>
        <param name='IncludeFilesOrDirectories'>File</param>
    </module>
    <module type='FileSubPipelineBroker'>
        <param name='FileFilterModuleName'>NullFilter</param>
        <param name='SourceFileColumnName'>Name</param>
        <param name='SourceDirectoryColumnName'>Directory</param>
        <param name='FileFilterParameters'>OutputColumnList:Name|+|text, *</param>
        <param name='OutputDirectoryPath'><![CDATA[\\proto-2\unitTest_Files\Mage\TargetDirectory]]></param>
        <param name='OutputFileName'>junk.txt</param>
        <param name='DatabaseName'></param>
        <param name='TableName'></param>
    </module>
</pipeline>
";

        [Test]
        public void AssembleComplexXML()
        {
            var pipeline = ProcessingPipeline.Assemble(pipelineXML3);
            Assert.That(pipeline.ModuleList, Has.Count.EqualTo(2), "Module list should have 2 modules");

            Assert.Multiple(() =>
            {
                Assert.That(pipeline.ModuleList[0].ToString(), Is.EqualTo("Mage.FileListFilter"), "Unexpected module type for Module " + pipeline.ModuleList[0]);
                Assert.That(pipeline.ModuleList[1].ToString(), Is.EqualTo("Mage.FileSubPipelineBroker"), "Unexpected module type for Module " + pipeline.ModuleList[1]);
            });

            foreach (var module in pipeline.ModuleList)
            {
                Console.WriteLine(module.ToString());
            }
        }
    }
}
