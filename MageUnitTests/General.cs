using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mage;
using System.Collections.ObjectModel;

namespace MageUnitTests {
    /// <summary>
    /// Summary description for General
    /// </summary>
    [TestClass]
    public class General {
        public General() {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod()]
        public void DataGeneratorTest() {
            DataGenerator dGen = new DataGenerator();
            int cols = 4;
            int rows = 17;
            dGen.Rows = rows;
            dGen.Cols = cols;
            SimpleSink sink = new SimpleSink();
            dGen.ColumnDefAvailable += sink.HandleColumnDef;
            dGen.DataRowAvailable += sink.HandleDataRow;
            dGen.Run(null);
            Assert.AreEqual(rows, sink.Rows.Count);
            Assert.AreEqual(cols, sink.Columns.Count);
        }

        public static void CompareSinks(SimpleSink source, SimpleSink result) {
            Collection<MageColumnDef> sourceCols = source.Columns;
            Collection<MageColumnDef> resultCols = result.Columns;
            Collection<object[]> sourceRows = source.Rows;
            Collection<object[]> resultRows = result.Rows;

            Assert.AreEqual(sourceCols.Count, resultCols.Count);
            Assert.AreEqual(sourceRows.Count, resultRows.Count);

            for (int i = 0; i < sourceCols.Count; i++) {
                Assert.AreEqual(sourceCols[i].Name, resultCols[i].Name, "Column names do not match");
            }

            for (int i = 0; i < sourceRows.Count; i++) {
                for (int j = 0; j < sourceCols.Count; j++) {
                    Assert.AreEqual(sourceRows[i][j], resultRows[i][j], string.Format("Row {0} cell {1} content does not match", i, j));
                }
            }
        }

    }
}
