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

        /// <summary>
        /// A test for SetModuleParameter
        /// </summary>
        [Test]
        public void SetModuleParameterTest()
        {
            var target = new ProcessingPipeline("TestPipeline");
            var tm = new TestModule();

            target.AddModule("TM", tm);
            var tVal1 = "Test Value1";
            target.SetModuleParameter("TM", "OutputColumnList", tVal1);
            var tVal2 = "Dummy Value";
            target.SetModuleParameter("TM", "Dummy", tVal2);

            Assert.AreEqual(tVal1, tm.OutputColumnList);
            Assert.AreEqual(tVal2, tm.Dummy);
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
            var pipelineName = "Test Pipeline";

            var reader = new DelimitedFileReader();
            var filter = new NullFilter();
            var writer = new SQLiteWriter();

            var moduleList = new Collection<object> { reader, filter, writer };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("NullFilter2");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("SQLiteWriter3");
            Assert.NotNull(mod);
        }

        /// <summary>
        /// Tests assembling pipeline from list of module class names as strings
        /// </summary>
        [Test]
        public void PipelineFromNamesListTest()
        {
            var pipelineName = "Test Pipeline";

            var moduleList = new Collection<object> { "DelimitedFileReader", "NullFilter", "SimpleSink" };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("NullFilter2");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("SimpleSink3");
            Assert.NotNull(mod);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where modules are defined by class name
        /// </summary>
        [Test]
        public void PipelineFromNamedModuleListTest1()
        {
            var pipelineName = "Test Pipeline";

            var namedModuleList = new Collection<ModuleDef>
            {
                new ModuleDef("Larry", "DelimitedFileReader"),
                new ModuleDef("Moe", "NullFilter"),
                new ModuleDef("Curly", "SimpleSink")
            };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("Larry");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("Moe");
            Assert.NotNull(mod);
            mod = pipeline.GetModule("Curly");
            Assert.NotNull(mod);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where some modules are directly referenced and
        /// some modules are defined by class name
        /// </summary>
        [Test]
        public void PipelineFromNamedModuleListTest2()
        {
            var pipelineName = "Test Pipeline";

            var reader = new DelimitedFileReader();
            var writer = new SQLiteWriter();

            var namedModuleList = new Collection<ModuleDef>
            {
                new ModuleDef("Larry", reader),
                new ModuleDef("Moe", "NullFilter"),
                new ModuleDef("Curly", writer)
            };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            // ReSharper disable once JoinDeclarationAndInitializer
            IBaseModule mod;

            mod = pipeline.GetModule("Larry");
            Assert.NotNull(mod);
            Assert.AreEqual("DelimitedFileReader", mod.GetType().Name);
            mod = pipeline.GetModule("Moe");
            Assert.NotNull(mod);
            Assert.AreEqual("NullFilter", mod.GetType().Name);
            mod = pipeline.GetModule("Curly");
            Assert.NotNull(mod);
            Assert.AreEqual("SQLiteWriter", mod.GetType().Name);
        }

        // Default server info: gigasax and DMS5
        readonly string pipelineXML = @"
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
            Assert.IsTrue(target != null);
            Assert.AreEqual(target.Server, Globals.DMSServer);
            Assert.AreEqual(pipeline.PipelineName, "Test_Pipeline");
        }

        readonly string pipelineXML2 = @"
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

            var maxRows = 7;

            var dataFile = General.GetTestFile(dataFileName);

            var pipeline = ProcessingPipeline.Assemble(pipelineXML2.Replace(dataFileName, dataFile.FullName));

            var sink = new SimpleSink(maxRows);
            // pipeline.ConnectExternalModule(sink);
            // sink.ModuleName = "Caboose";
            pipeline.AppendModule(sink);

            var mod = pipeline.GetModule("Module1");
            var target = mod as DelimitedFileReader;
            Assert.IsTrue(target != null);
            Assert.AreEqual(target.FilePath, dataFile.FullName);

            pipeline.RunRoot(null);

            // Did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;

            Console.WriteLine("Pipeline returned {0} rows of data", rows.Count);
            Assert.AreEqual(maxRows, rows.Count, "Sink did not receive the expected number of rows");

        }

        readonly string pipelineXML3 = @"<?xml version='1.0' encoding='UTF-8'?><!-- to get filtered list of files in local directory -->
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
            Assert.AreEqual(2, pipeline.ModuleList.Count, "Module list should have 2 modules");

            Assert.AreEqual("Mage.FileListFilter", pipeline.ModuleList[0].ToString(), "Unexpected module type for Module " + pipeline.ModuleList[0]);
            Assert.AreEqual("Mage.FileSubPipelineBroker", pipeline.ModuleList[1].ToString(), "Unexpected module type for Module " + pipeline.ModuleList[1]);

            foreach (var module in pipeline.ModuleList)
            {
                Console.WriteLine(module.ToString());
            }
        }
    }
}
