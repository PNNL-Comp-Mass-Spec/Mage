using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace MageUnitTests
{


    /// <summary>
    ///This is a test class for DelimitedFileReaderTest and is intended
    ///to contain all DelimitedFileReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DelimitedFileReaderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
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
        ///A test for Header
        ///</summary>
        [TestMethod()]
        public void HeaderTest()
        {
            DelimitedFileReader target = new DelimitedFileReader();
            string expected = "Test Value";
            string actual;
            target.Header = expected;
            actual = target.Header;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FilePath
        ///</summary>
        [TestMethod()]
        public void FilePathTest()
        {
            DelimitedFileReader target = new DelimitedFileReader();
            string expected = "Test Value";
            string actual;
            target.FilePath = expected;
            actual = target.FilePath;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Delimiter
        ///</summary>
        [TestMethod()]
        public void DelimiterTest()
        {
            DelimitedFileReader target = new DelimitedFileReader();
            string expected = "Test Value";
            string actual;
            target.Delimiter = expected;
            actual = target.Delimiter;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for Run
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\..\MageUnitTests\TestItems\Sarc_MS_Filtered_isos.csv")]
        public void ReadCommaDelimitedFileTest()
        {
            // create DelimitedFileReader object and test sink object 
            // and connect together
            DelimitedFileReader target = new DelimitedFileReader();
            int maxRows = 7;
            SimpleSink sink = new SimpleSink(maxRows);
            target.ColumnDefAvailable += sink.HandleColumnDef;
            target.DataRowAvailable += sink.HandleDataRow;

            target.FilePath = @"Sarc_MS_Filtered_isos.csv";
            target.Delimiter = ","; // "CSV"
            target.Run(null);

            // did the test sink object get the expected row definitions
            string[] colList = new string[] { "frame_num", "ims_scan_num", "charge", "abundance", "mz", "fit", "average_mw", "monoisotopic_mw", "mostabundant_mw", "fwhm", "signal_noise", "mono_abundance", "mono_plus2_abundance", "orig_intensity", "TIA_orig_intensity", "drift_time", "flag" };
            Collection<MageColumnDef> cols = sink.Columns;
            Assert.AreEqual(cols.Count, colList.Length);
            for (int i = 0; i < cols.Count; i++)
            {
                Assert.AreEqual(cols[i].Name, colList[i]);
            }

            // did the test sink object get the expected number of data rows
            // on its standard tabular input?
            Collection<string[]> rows = sink.Rows;
            Assert.AreEqual(maxRows, rows.Count);

            // are there the expected number of fields in the data row?
            Assert.AreEqual(colList.Length, rows[0].Length);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\..\MageUnitTests\TestItems\tab_delim.txt")]
        public void ReadTabDelimitedFileTest()
        {
            SimpleSink result = ReadDelimitedFile(@"tab_delim.txt");
            Assert.AreEqual(4, result.Columns.Count);
            Assert.AreEqual(17, result.Rows.Count);
        }


        public static SimpleSink ReadDelimitedFile(string filePath)
        {
            ProcessingPipeline pipeline = new ProcessingPipeline("Delimited_File_Reader");

            pipeline.RootModule = pipeline.MakeModule("Reader", "DelimitedFileReader");
            pipeline.MakeModule("Results", "SimpleSink");

            pipeline.ConnectModules("Reader", "Results");

            pipeline.SetModuleParameter("Reader", "FilePath", filePath);

            pipeline.RunRoot(null);

            SimpleSink result = (SimpleSink)pipeline.GetModule("Results");
            return result;
        }
    }
}
