using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for CrosstabFilterTest and is intended
    ///to contain all CrosstabFilterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CrosstabFilterTest
    {

        /// <summary>
        ///A test for FactorValueCol
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\..\TestItems\factors_test.txt")]
        public void CrosstabFilterMainTest()
        {
            //  "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report"

            // create Delimited file reader module
            // and connect together
            var reader = new DelimitedFileReader {FilePath = "factors_test.txt"};

            var target = new CrosstabFilter
            {
                EntityNameCol = "Dataset",
                EntityIDCol = "Dataset_ID",
                FactorNameCol = "Factor",
                FactorValueCol = "Value"
            };

            var sink = new SimpleSink();

            var pipeline = new ProcessingPipeline("Test");
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
        public void FactorValueColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.FactorValueCol = expected;
            var actual = target.FactorValueCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FactorNameCol
        ///</summary>
        [TestMethod()]
        public void FactorNameColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.FactorNameCol = expected;
            var actual = target.FactorNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EntityNameCol
        ///</summary>
        [TestMethod()]
        public void EntityNameColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.EntityNameCol = expected;
            var actual = target.EntityNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for EntityIDCol
        ///</summary>
        [TestMethod()]
        public void EntityIDColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.EntityIDCol = expected;
            var actual = target.EntityIDCol;
            Assert.AreEqual(expected, actual);
        }
    }
}
