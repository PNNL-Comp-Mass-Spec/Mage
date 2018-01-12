using Mage;
using NUnit.Framework;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for CrosstabFilterTest and is intended
    /// to contain all CrosstabFilterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class CrosstabFilterTest
    {

        /// <summary>
        /// A test for FactorValueCol
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\TestItems\factors_test.txt")]
        public void CrosstabFilterMainTest(string filePath)
        {
            //  "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report"

            var dataFile = General.GetTestFile(filePath);

            // Create Delimited file reader module
            // and connect together
            var reader = new DelimitedFileReader {FilePath = dataFile.FullName};

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
        /// A test for FactorValueCol
        /// </summary>
        [Test]
        public void FactorValueColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.FactorValueCol = expected;
            var actual = target.FactorValueCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for FactorNameCol
        /// </summary>
        [Test]
        public void FactorNameColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.FactorNameCol = expected;
            var actual = target.FactorNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for EntityNameCol
        /// </summary>
        [Test]
        public void EntityNameColTest()
        {
            var target = new CrosstabFilter();
            var expected = "Test Value";
            target.EntityNameCol = expected;
            var actual = target.EntityNameCol;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for EntityIDCol
        /// </summary>
        [Test]
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
