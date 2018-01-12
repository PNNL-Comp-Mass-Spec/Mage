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
            var cols = 5;
            var rows = 21;
            var testFile = "delim_test.txt";

            var dGen = new DataGenerator
            {
                Rows = rows,
                Cols = cols
            };

            var source = WriteDelimitedFileWithTestData(testFile, dGen);

            var result = DelimitedFileReaderTest.ReadDelimitedFile(testFile);

            Assert.AreEqual(rows, source.Rows.Count, "Source row count does not match");
            Assert.AreEqual(cols, source.Columns.Count, "Source column count does ot match");
            Assert.AreEqual(rows, result.Rows.Count, "Result row count does not match");
            Assert.AreEqual(cols, result.Columns.Count, "Result column count does not match");
            General.CompareSinks(source, result);
        }

        public SimpleSink WriteDelimitedFileWithTestData(string filePath, IBaseModule dGen)
        {
            var pipeline = new ProcessingPipeline("Delimited_File_Writer");

            pipeline.RootModule = pipeline.AddModule("Gen", dGen);
            pipeline.MakeModule("Writer", "DelimitedFileWriter");
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Gen", "Writer");
            pipeline.ConnectModules("Gen", "Results");

            pipeline.SetModuleParameter("Writer", "FilePath", filePath);

            pipeline.RunRoot(null);

            var result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }



        /// <summary>
        /// A test for Header
        /// </summary>
        [Test]
        public void HeaderTest()
        {
            var target = new DelimitedFileWriter();
            var expected = "Test Value";
            target.Header = expected;
            var actual = target.Header;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for FilePath
        /// </summary>
        [Test]
        public void FilePathTest()
        {
            var target = new DelimitedFileWriter();
            var expected = "Test Value";
            target.FilePath = expected;
            var actual = target.FilePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Delimiter
        /// </summary>
        [Test]
        public void DelimiterTest()
        {
            var target = new DelimitedFileWriter();
            var expected = ",";
            target.Delimiter = expected;
            var actual = target.Delimiter;
            Assert.AreEqual(expected, actual);
        }
    }
}
