using Mage;
using NUnit.Framework;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for DelimitedFileReaderTest and is intended
    /// to contain all DelimitedFileReaderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class DelimitedFileReaderTest
    {
        // ReSharper disable once CommentTypo
        // Ignore Spelling: fwhm

        /// <summary>
        /// A test for Header
        /// </summary>
        [Test]
        public void HeaderTest()
        {
            var target = new DelimitedFileReader();
            const string expected = "Test Value";
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
            var target = new DelimitedFileReader();
            const string expected = "Test Value";
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
            var target = new DelimitedFileReader();
            const string expected = "Test Value";
            target.Delimiter = expected;
            var actual = target.Delimiter;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Run
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\MageUnitTests\TestItems\Sarc_MS_Filtered_isos.csv")]
        public void ReadCommaDelimitedFileTest(string isosFilePath)
        {
            var dataFile = General.GetTestFile(isosFilePath);

            // Create DelimitedFileReader object and test sink object
            // and connect together
            var target = new DelimitedFileReader();
            const int maxRows = 7;
            var sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.FilePath = dataFile.FullName;
            target.Delimiter = ","; // "CSV"
            target.Run(null);

            // Did the test sink object get the expected row definitions
            var colList = new[] {
                "frame_num", "ims_scan_num", "charge", "abundance", "mz", "fit", "average_mw",
                "monoisotopic_mw", "mostabundant_mw", "fwhm", "signal_noise", "mono_abundance",
                "mono_plus2_abundance", "orig_intensity", "TIA_orig_intensity", "drift_time", "flag" };
            var cols = sink.Columns;
            Assert.AreEqual(cols.Count, colList.Length);
            for (var i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // Did the test sink object get the expected number of data rows
            // on its standard tabular input?
            var rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // Are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);
        }

        [Test]
        [TestCase(@"..\..\..\MageUnitTests\TestItems\tab_delim.txt")]
        public void ReadTabDelimitedFileTest(string delimitedFilePath)
        {
            var dataFile = General.GetTestFile(delimitedFilePath);

            var result = ReadDelimitedFile(dataFile.FullName);
            Assert.AreEqual(4, result.Columns.Count);
            Assert.AreEqual(17, result.Rows.Count);
        }

        public static SimpleSink ReadDelimitedFile(string filePath)
        {
            var pipeline = new ProcessingPipeline("Delimited_File_Reader");

            pipeline.RootModule = pipeline.MakeModule("Reader", "DelimitedFileReader");
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Reader", "Results");

            pipeline.SetModuleParameter("Reader", "FilePath", filePath);

            pipeline.RunRoot(null);

            var result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }
    }
}
