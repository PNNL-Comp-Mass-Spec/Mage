using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for ProcessingPipelineTest and is intended
    ///to contain all ProcessingPipelineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProcessingPipelineTest
    {

        /// <summary>
        ///A test for SetModuleParameter
        ///</summary>
        [TestMethod()]
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
        ///</summary>
        [TestMethod()]
        public void MakeModuleTest()
        {
            IBaseModule mod;

            mod = ProcessingPipeline.MakeModule("NullFilter");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("ContentFilter");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("CrosstabFilter");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("DelimitedFileReader");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("DelimitedFileWriter");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("FileContentProcessor");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("FileCopy");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("FileListFilter");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("FileSubPipelineBroker");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("MSSQLReader");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("PermutationGenerator");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("SimpleSink");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("SQLiteReader");
            Assert.AreNotEqual(null, mod);
            mod = ProcessingPipeline.MakeModule("SQLiteWriter");
            Assert.AreNotEqual(null, mod);
        }

        /// <summary>
        /// Tests assembling pipeline from list of module objects
        ///</summary>
        [TestMethod()]
        public void PipelineFromModuleListTest()
        {
            var pipelineName = "Test Pipeline";

            var reader = new DelimitedFileReader();
            var filter = new NullFilter();
            var writer = new SQLiteWriter();

            var moduleList = new Collection<object>() { reader, filter, writer };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            IBaseModule mod;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("NullFilter2");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("SQLiteWriter3");
            Assert.AreNotEqual(null, mod);
        }

        /// <summary>
        /// Tests assembling pipeline from list of module class names as strings
        ///</summary>
        [TestMethod()]
        public void PipelineFromNamesListTest()
        {
            var pipelineName = "Test Pipeline";

            Collection<object> moduleList;
            moduleList = new Collection<object>() { "DelimitedFileReader", "NullFilter", "SimpleSink" };
            var pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            IBaseModule mod = null;

            mod = pipeline.GetModule("DelimitedFileReader1");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("NullFilter2");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("SimpleSink3");
            Assert.AreNotEqual(null, mod);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where modules are defined by class name
        ///</summary>
        [TestMethod()]
        public void PipelineFromNamedModuleListTest1()
        {
            var pipelineName = "Test Pipeline";
            ProcessingPipeline pipeline = null;

            Collection<ModuleDef> namedModuleList = null;
            namedModuleList = new Collection<ModuleDef>() {
                new ModuleDef("Larry", "DelimitedFileReader"),
                new ModuleDef("Moe", "NullFilter"),
                new ModuleDef("Curly", "SimpleSink")
            };
            pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            IBaseModule mod = null;

            mod = pipeline.GetModule("Larry");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("Moe");
            Assert.AreNotEqual(null, mod);
            mod = pipeline.GetModule("Curly");
            Assert.AreNotEqual(null, mod);
        }

        /// <summary>
        /// Tests making pipeline from list of ModuleDef objects
        /// where some modules are directly referenced and
        /// some modules are defined by class name
        ///</summary>
        [TestMethod()]
        public void PipelineFromNamedModuleListTest2()
        {
            var pipelineName = "Test Pipeline";
            ProcessingPipeline pipeline = null;

            var reader = new DelimitedFileReader();
            var writer = new SQLiteWriter();

            Collection<ModuleDef> namedModuleList = null;
            namedModuleList = new Collection<ModuleDef>() {
                new ModuleDef("Larry", reader),
                new ModuleDef("Moe", "NullFilter"),
                new ModuleDef("Curly", writer)
            };
            pipeline = ProcessingPipeline.Assemble(pipelineName, namedModuleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            IBaseModule mod = null;

            mod = pipeline.GetModule("Larry");
            Assert.AreNotEqual(null, mod);
            Assert.AreEqual("DelimitedFileReader", mod.GetType().Name);
            mod = pipeline.GetModule("Moe");
            Assert.AreNotEqual(null, mod);
            Assert.AreEqual("NullFilter", mod.GetType().Name);
            mod = pipeline.GetModule("Curly");
            Assert.AreNotEqual(null, mod);
            Assert.AreEqual("SQLiteWriter", mod.GetType().Name);
        }

        // Default server info: gigasax and DMS5
        readonly string pipelineXML = @"
<pipeline name='Test_Pipeline' >
    <module name='Reader' type='MSSQLReader'>
        <param name='Server' >" + Globals.DMSServer + "</param>" +
        "<param name='Database' >" + Globals.DMSDatabase + "</param>" +
        "<param name='SQLText' >SELECT * FROM T_Campaign</param>" +
    "</module>" +
    "<module name='Sink' type='SimpleSink' />" +
"</pipeline>";

        /// <summary>
        ///A test for Build
        ///</summary>
        [TestMethod()]
        public void AsembleXML()
        {
            var pipeline = ProcessingPipeline.Assemble(pipelineXML);

            var mod = pipeline.GetModule("Reader");
            var target = mod as MSSQLReader;
            Assert.IsTrue(target != null);
            Assert.AreEqual(target.Server, Globals.DMSServer);
            Assert.AreEqual(pipeline.PipelineName, "Test_Pipeline");
        }

        string pipelineXML2 = @"
<pipeline name='Test_Pipeline' >
    <module type='DelimitedFileReader'>
        <param name='FilePath' >Sarc_MS_Filtered_isos.csv</param>
    </module>
    <module type='NullFilter' />
</pipeline>
";

        /// <summary>
        ///A test for Build
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\Sarc_MS_Filtered_isos.csv")]
        public void AssembleXMLAndRun()
        {
            var maxRows = 7;

            var pipeline = ProcessingPipeline.Assemble(pipelineXML2);

            var sink = new SimpleSink(maxRows);
            //            pipeline.ConnectExternalModule(sink);
            //            sink.ModuleName = "Caboose";
            pipeline.AppendModule(sink);

            var mod = pipeline.GetModule("Module1");
            var target = mod as DelimitedFileReader;
            Assert.IsTrue(target != null);
            Assert.AreEqual(target.FilePath, "Sarc_MS_Filtered_isos.csv");

            pipeline.RunRoot(null);

            // did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count, "Sink did not receive the expected number of rows");

        }

        string pipelineXML3 = @"
<?xml version=1.0 encoding=UTF-8?><!-- to get filtered list of files in local directory -->
<pipeline name='Test_Pipeline'>
	<module type='FileListFilter'>
		<param name='FolderPath'><![CDATA[C:\Data\syn2]]></param>
		<param name='FileNameSelector'>_syn.txt</param>
		<param name='FileTypeColumnName'>Item</param>
		<param name='FileColumnName'>Name</param>
		<param name='SourceFolderColumnName'>Folder</param>
		<param name='OutputColumnList'>Item|+|text, Name|+|text, Folder|+|text</param>
		<param name='IncludeFilesOrFolders'>File</param>
	</module>
	<module type='FileSubPipelineBroker'>
		<param name='FileFilterModuleName'>NullFilter</param>
		<param name='SourceFileColumnName'>Name</param>
		<param name='SourceFolderColumnName'>Folder</param>
		<param name='FileFilterParameters'>OutputColumnList:Name|+|text, *</param>
		<param name='OutputFolderPath'><![CDATA[C:\Data\syn2\]]></param>
		<param name='OutputFileName'>junk.txt</param>
		<param name='DatabaseName'></param>
		<param name='TableName'></param>
	</module>
</pipeline>
";

        [TestMethod()]
        public void AssembleComplexXML()
        {
            var pipeline = ProcessingPipeline.Assemble(pipelineXML3);

        }
    }
}
