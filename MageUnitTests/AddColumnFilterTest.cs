using Mage;
using NUnit.Framework;
using System.Collections.Generic;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for AddColumnFilterTest and is intended
    /// to contain all AddColumnFilterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class AddColumnFilterTest
    {
        /// <summary>
        /// A test for AddColumnFilter
        /// </summary>
        [Test]
        public void AddColumnFilterConstructorTest()
        {
            var initialColumns = new[] { "Alpha", "Beta", "Gamma" };
            string[] expectedColumns = { "Alpha", "Added1|+|text", "Beta", "Added2|+|text", "Gamma" };

            // Set up data generator
            var gen = new DataGenerator
            {
                AddAdHocRow = initialColumns    // Header
            };
            gen.AddAdHocRow = new[] { "A1", "B1", "C1" };
            gen.AddAdHocRow = new[] { "A2", "B2", "C2" };

            var target = new NullFilter
            {
                OutputColumnList = string.Join(", ", expectedColumns)
            };
            target.SetContext(new Dictionary<string, string> { { "Added1", "Overwrite1" }, { "Added2", "Overwrite2" } });

            var sink = new SimpleSink();

            var pipeline = new ProcessingPipeline("AddColumnFilterConstructorTest");
            var sourceModName = "Generator";
            var filterModName = "Filter";
            var sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, gen);
            pipeline.AddModule(filterModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, filterModName);
            pipeline.ConnectModules(filterModName, sinkModName);

            pipeline.RunRoot(null);

            Assert.AreEqual(expectedColumns.Length, sink.Columns.Count, "Expected number of columns did not match");

            Assert.AreNotEqual(gen.AdHocRows.Count, sink.Rows.Count, "Expected number of rows did not match");

            var rows = sink.Rows;

            Assert.AreEqual("Overwrite1", rows[0][1], "Expected overwritten values did not match");
            Assert.AreEqual("Overwrite2", rows[0][3], "Expected overwritten values did not match");
        }
    }
}
