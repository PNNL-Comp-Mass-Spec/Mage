using Mage;
using NUnit.Framework;
namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for PermutationGeneratorTest and is intended
    /// to contain all PermutationGeneratorTest Unit Tests
    /// </summary>
    [TestFixture]
    public class PermutationGeneratorTest
    {
        /// <summary>
        /// A test for AddParamColumn
        /// </summary>
        [Test]
        public void AddParamColumnTest()
        {
            var target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            var actual = target.ParamCount;

            Assert.That(actual, Is.EqualTo(3), "Number of parameters did not match");
        }

        /// <summary>
        /// A test for PredictedOutputRowCount
        /// </summary>
        [Test]
        public void PredictedOutputRowCountTest()
        {
            var target = new PermutationGenerator();
            var actual = target.ParamCount;

            Assert.That(actual, Is.Zero);

            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            actual = target.PredictedOutputRowCount;
            Assert.That(actual, Is.EqualTo(15), "Number of rows did not match");
        }

        /// <summary>
        /// A test for IncludeHeaderInOutput
        /// </summary>
        [Test]
        public void IncludeHeaderInOutputTest()
        {
            var target = new PermutationGenerator();
            var expected = false;
            target.IncludeHeaderInOutput = expected;
            var actual = target.IncludeHeaderInOutput;

            Assert.That(actual, Is.EqualTo(expected));

            expected = true;
            target.IncludeHeaderInOutput = expected;
            actual = target.IncludeHeaderInOutput;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for Run
        /// </summary>
        [Test]
        public void BasicPermutationGenerator()
        {
            var target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            // Create test sink module and connect to SQLReader module
            var sink = new SimpleSink();
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            var expected = target.ParamCount;
            var actual = sink.Columns.Count;

            Assert.That(actual, Is.EqualTo(expected), "Number of columns did not match");

            actual = sink.Rows.Count;
            Assert.That(actual, Is.EqualTo(15), "Number of rows did not match");
        }

        /// <summary>
        /// A test for Run
        /// </summary>
        [Test]
        public void PermutationGeneratorWithMappedColumns()
        {
            var target = new PermutationGenerator();
            target.AddParamColumn("one", "1", "5", "1");
            target.AddParamColumn("two", "0", "2", "1");
            target.AddParamColumn("three", "1", "1", "1");

            target.OutputColumnList = "ref|+|text, one, two, three, results|+|float";
            target.AutoColumnName = "ref";

            // Create test sink module and connect to SQLReader module
            var sink = new SimpleSink();
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.Run(null);

            var expected = target.ParamCount;
            var actual = sink.Columns.Count;

            Assert.That(actual, Is.EqualTo(5), "Number of columns did not match");

            actual = sink.Rows.Count;
            Assert.That(actual, Is.EqualTo(15), "Number of rows did not match");
        }

        [Test]
        public void PermutationGeneratorWriteSQLite()
        {
            var PGen = new PermutationGenerator();
            PGen.AddParamColumn("one", "1", "5", "1");
            PGen.AddParamColumn("two", "0", "2", "1");
            PGen.AddParamColumn("three", "1", "1", "1");

            // Optional: can use built-in column mapping to add columns
            PGen.OutputColumnList = "ref|+|text, one, two, three";
            PGen.AutoColumnName = "ref";
            PGen.AutoColumnFormat = "ParamSet_{0:000000}";

            var pipeline = new ProcessingPipeline("PGen_2_SQLite");

            var writer = new SQLiteWriter
            {
                DbPath = "param_permutations.db",
                TableName = "param_permutations"
            };

            pipeline.RootModule = pipeline.AddModule("Gen", PGen);
            pipeline.AddModule("Writer", writer);
            pipeline.ConnectModules("Gen", "Writer");
            pipeline.RunRoot(null);

            Assert.That(pipeline.CompletionCode, Is.Empty, "Completion code did not match");
        }

        [Test]
        public void PermutationGeneratorWriteFile()
        {
            var PGen = new PermutationGenerator();
            PGen.AddParamColumn("one", "1", "5", "1");
            PGen.AddParamColumn("two", "0", "2", "1");
            PGen.AddParamColumn("three", "1", "1", "1");

            // Optional: can use built-in column mapping to add columns
            PGen.OutputColumnList = "ref|+|text, one, two, three";
            PGen.AutoColumnName = "ref";
            PGen.AutoColumnFormat = "ParamSet_{0:000000}";

            var pipeline = new ProcessingPipeline("PGen_2_File");

            var writer = new DelimitedFileWriter { FilePath = "param_permutations.txt" };

            pipeline.RootModule = pipeline.AddModule("Gen", PGen);
            pipeline.AddModule("Writer", writer);
            pipeline.ConnectModules("Gen", "Writer");
            pipeline.RunRoot(null);

            Assert.That(pipeline.CompletionCode, Is.Empty, "Completion code did not match");
        }
    }
}
