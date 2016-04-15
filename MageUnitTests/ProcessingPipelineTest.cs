using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace MageUnitTests {


    /// <summary>
    ///This is a test class for ProcessingPipelineTest and is intended
    ///to contain all ProcessingPipelineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProcessingPipelineTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SetModuleParameter
        ///</summary>
        [TestMethod()]
        public void SetModuleParameterTest() {
            ProcessingPipeline target = new ProcessingPipeline("TestPipeline");
            TestModule tm = new TestModule();

            target.AddModule("TM", tm);
            string tVal1 = "Test Value1";
            target.SetModuleParameter("TM", "OutputColumnList", tVal1);
            string tVal2 = "Dummy Value";
            target.SetModuleParameter("TM", "Dummy", tVal2);

            Assert.AreEqual(tVal1, tm.OutputColumnList);
            Assert.AreEqual(tVal2, tm.Dummy);
        }

        /// <summary>
        /// Tests ability to make Mage basic modules by class name
        ///</summary>
        [TestMethod()]
        public void MakeModuleTest() {
            IBaseModule mod = null;

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
        public void PipelineFromModuleListTest() {
            string pipelineName = "Test Pipeline";
            ProcessingPipeline pipeline = null;

            DelimitedFileReader reader = new DelimitedFileReader();
            NullFilter filter = new NullFilter();
            SQLiteWriter writer = new SQLiteWriter();

            Collection<object> moduleList = null;
            moduleList = new Collection<object>() { reader, filter, writer };
            pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

            Assert.AreNotEqual(null, pipeline, "Pipeline not created");
            Assert.AreEqual(pipelineName, pipeline.PipelineName, "Pipeline name does not match");

            IBaseModule mod = null;

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
        public void PipelineFromNamesListTest() {
            string pipelineName = "Test Pipeline";
            ProcessingPipeline pipeline = null;

            Collection<object> moduleList = null;
            moduleList = new Collection<object>() { "DelimitedFileReader", "NullFilter", "SimpleSink" };
            pipeline = ProcessingPipeline.Assemble(pipelineName, moduleList);

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
        public void PipelineFromNamedModuleListTest1() {
            string pipelineName = "Test Pipeline";
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
        public void PipelineFromNamedModuleListTest2() {
            string pipelineName = "Test Pipeline";
            ProcessingPipeline pipeline = null;

            DelimitedFileReader reader = new DelimitedFileReader();
            SQLiteWriter writer = new SQLiteWriter();

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
        public void AsembleXML() {
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble(pipelineXML);

            IBaseModule mod = pipeline.GetModule("Reader");
            MSSQLReader target = mod as MSSQLReader;
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
        public void AssembleXMLAndRun() {
            int maxRows = 7;

            ProcessingPipeline pipeline = ProcessingPipeline.Assemble(pipelineXML2);

            SimpleSink sink = new SimpleSink(maxRows);
            //            pipeline.ConnectExternalModule(sink);
            //            sink.ModuleName = "Caboose";
            pipeline.AppendModule(sink);

            IBaseModule mod = pipeline.GetModule("Module1");
            DelimitedFileReader target = mod as DelimitedFileReader;
            Assert.AreEqual(target.FilePath, "Sarc_MS_Filtered_isos.csv");

            pipeline.RunRoot(null);

            // did the test sink object get the expected number of data rows
            // on its standard tabular input?
			Collection<string[]> rows = sink.Rows;
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
        public void AssembleComplexXML() {
            ProcessingPipeline pipeline = ProcessingPipeline.Assemble(pipelineXML3);

        }
    }
}
