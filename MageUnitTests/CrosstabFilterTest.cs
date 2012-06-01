using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MageUnitTests
{
    
    
    /// <summary>
    ///This is a test class for CrosstabFilterTest and is intended
    ///to contain all CrosstabFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CrosstabFilterTest {

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
        ///A test for FactorValueCol
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\factors_test.txt")]
        public void CrosstabFilterMainTest() {
            //  "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report"

            // create Delimited file reader module
            // and connect together
            DelimitedFileReader reader = new DelimitedFileReader();
            reader.FilePath = "factors_test.txt";

            CrosstabFilter target = new CrosstabFilter();
            target.EntityNameCol = "Dataset";
            target.EntityIDCol = "Dataset_ID";
            target.FactorNameCol = "Factor";
            target.FactorValueCol = "Value";

            SimpleSink sink = new SimpleSink();

            ProcessingPipeline pipeline = new ProcessingPipeline("Test");
            pipeline.RootModule = pipeline.AddModule("Reader", reader);
            pipeline.AddModule("Target", target);
            pipeline.AddModule("Sink", sink);
            pipeline.ConnectModules("Reader", "Target");
            pipeline.ConnectModules("Target", "Sink");

            pipeline.RunRoot(null);

            Assert.AreEqual(151, sink.Rows.Count, "Expected row count did not match");
            Assert.AreEqual(10, sink.Rows[0].Length, "Expected column count did not match");
        }


        /// <summary>
        ///A test for FactorValueCol
        ///</summary>
        [TestMethod()]
        public void FactorValueColTest() {
            CrosstabFilter target = new CrosstabFilter(); 
            string expected = "Test Value";
            string actual;
            target.FactorValueCol = expected;
            actual = target.FactorValueCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FactorNameCol
        ///</summary>
        [TestMethod()]
        public void FactorNameColTest() {
            CrosstabFilter target = new CrosstabFilter(); 
            string expected = "Test Value";
            string actual;
            target.FactorNameCol = expected;
            actual = target.FactorNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EntityNameCol
        ///</summary>
        [TestMethod()]
        public void EntityNameColTest() {
            CrosstabFilter target = new CrosstabFilter(); 
            string expected = "Test Value";
            string actual;
            target.EntityNameCol = expected;
            actual = target.EntityNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EntityIDCol
        ///</summary>
        [TestMethod()]
        public void EntityIDColTest() {
            CrosstabFilter target = new CrosstabFilter(); 
            string expected = "Test Value";
            string actual;
            target.EntityIDCol = expected;
            actual = target.EntityIDCol;
            Assert.AreEqual(expected, actual);
        }
    }
}
