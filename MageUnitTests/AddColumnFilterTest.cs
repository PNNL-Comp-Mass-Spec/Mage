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
            string[] expectedColumns = ["Alpha", "Added1|+|text", "Beta", "Added2|+|text", "Gamma"];

            // Set up data generator
            var gen = new DataGenerator
            {
                AddAdHocRow = initialColumns    // Header
            };
            gen.AddAdHocRow = ["A1", "B1", "C1"];
            gen.AddAdHocRow = ["A2", "B2", "C2"];

            var target = new NullFilter
            {
                OutputColumnList = string.Join(", ", expectedColumns)
            };
            target.SetContext(new Dictionary<string, string> { { "Added1", "Overwrite1" }, { "Added2", "Overwrite2" } });

            var sink = new SimpleSink();

            var pipeline = new ProcessingPipeline("AddColumnFilterConstructorTest");
            const string sourceModName = "Generator";
            const string filterModName = "Filter";
            const string sinkModName = "Sink";
            pipeline.RootModule = pipeline.AddModule(sourceModName, gen);
            pipeline.AddModule(filterModName, target);
            pipeline.AddModule(sinkModName, sink);
            pipeline.ConnectModules(sourceModName, filterModName);
            pipeline.ConnectModules(filterModName, sinkModName);

            pipeline.RunRoot(null);

            Assert.Multiple(() =>
            {
                Assert.That(sink.Columns, Has.Count.EqualTo(expectedColumns.Length), "Expected number of columns did not match");

                Assert.That(sink.Rows, Has.Count.Not.EqualTo(gen.AdHocRows.Count), "Expected number of rows did not match");
            });

            var rows = sink.Rows;

            Assert.Multiple(() =>
            {
                Assert.That(rows[0][1], Is.EqualTo("Overwrite1"), "Expected overwritten values did not match");
                Assert.That(rows[0][3], Is.EqualTo("Overwrite2"), "Expected overwritten values did not match");
            });
        }
    }
}
