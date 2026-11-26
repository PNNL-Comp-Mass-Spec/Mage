using Mage;
using NUnit.Framework;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for DelimitedFileWriterTest and is intended
    /// to contain all DelimitedFileWriterTest Unit Tests
    /// </summary>
    [TestFixture]
    public class DelimitedFileWriterTest
    {
        [Test]
        public void WriteTest()
        {
            const int cols = 5;
            const int rows = 21;
            const string testFile = "delim_test.txt";

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source = WriteDelimitedFileWithTestData(testFile, dGen);

            var result = DelimitedFileReaderTest.ReadDelimitedFile(testFile);

            Assert.Multiple(() =>
            {
                Assert.That(source.Rows, Has.Count.EqualTo(rows), "Source row count does not match");
                Assert.That(source.Columns, Has.Count.EqualTo(cols), "Source column count does not match");
                Assert.That(result.Rows, Has.Count.EqualTo(rows), "Result row count does not match");
                Assert.That(result.Columns, Has.Count.EqualTo(cols), "Result column count does not match");
            });
            General.CompareSinks(source, result);
        }

        private SimpleSink WriteDelimitedFileWithTestData(string filePath, IBaseModule dGen)
        {
            var pipeline = new ProcessingPipeline("Delimited_File_Writer");

            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.MakeModule("Writer", "DelimitedFileWriter");
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Gen", "Writer");
            pipeline.ConnectModules("Gen", "Results");

            pipeline.SetModuleParameter("Writer", "FilePath", filePath);

            pipeline.RunRoot(null);

            return (SimpleSink)pipeline.GetModule("Results");
        }

        /// <summary>
        /// A test for Header
        /// </summary>
        [Test]
        public void HeaderTest()
        {
            var target = new DelimitedFileWriter();
            const string expected = "Test Value";
            target.Header = expected;
            var actual = target.Header;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for FilePath
        /// </summary>
        [Test]
        public void FilePathTest()
        {
            var target = new DelimitedFileWriter();
            const string expected = "Test Value";
            target.FilePath = expected;
            var actual = target.FilePath;
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for Delimiter
        /// </summary>
        [Test]
        public void DelimiterTest()
        {
            var target = new DelimitedFileWriter();
            const string expected = ",";
            target.Delimiter = expected;
            var actual = target.Delimiter;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
