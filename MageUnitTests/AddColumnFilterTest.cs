using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace MageUnitTests {


    /// <summary>
    ///This is a test class for AddColumnFilterTest and is intended
    ///to contain all AddColumnFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddColumnFilterTest {


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
        ///A test for AddColumnFilter 
        ///</summary>
        [TestMethod()]
        public void AddColumnFilterConstructorTest() {

            var initialColumns = new string[] { "Alpha", "Beta", "Gamma" };
            string[] expectedColumns = { "Alpha", "Added1|+|text", "Beta", "Added2|+|text", "Gamma" };

            // set up data generator
            var gen = new DataGenerator
            {
                AddAdHocRow = initialColumns    // header
            };
            gen.AddAdHocRow = new string[] { "A1", "B1", "C1" };
            gen.AddAdHocRow = new string[] { "A2", "B2", "C2" };

            var target = new NullFilter
            {
                OutputColumnList = string.Join(", ", expectedColumns)
            };
            target.SetContext(new Dictionary<string, string>() { { "Added1", "Overwrite1" }, { "Added2", "Overwrite2" } });

            var sink = new SimpleSink();

            ProcessingPipeline pipeline = new ProcessingPipeline("AddColumnFilterConstructorTest");
            string sourceModName = "Generator";
            string filterModName = "Filter";
            string sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, gen);
            pipeline.AddModule(filterModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, filterModName);
            pipeline.ConnectModules(filterModName, sinkModName);

            pipeline.RunRoot(null);

            Assert.AreEqual(expectedColumns.Length, sink.Columns.Count, "Expected number of columns did not match");

            Assert.AreNotEqual(gen.AdHocRows.Count, sink.Rows.Count, "Expected number of rows did not match");

			Collection<string[]> rows = sink.Rows;

            Assert.AreEqual("Overwrite1", rows[0][1].ToString(), "Expected overwritten values did not match");
            Assert.AreEqual("Overwrite2", rows[0][3].ToString(), "Expected overwritten values did not match");
        }
    }
}
