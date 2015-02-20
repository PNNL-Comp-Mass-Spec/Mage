using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace MageUnitTests {

    /// <summary>
    ///This is a test class for BaseModuleTest and is intended
    ///to contain all BaseModuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseModuleTest {


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
        ///A test for OutputColumnList
        ///</summary>
        [TestMethod()]
        public void OutputColumnListTest() {
            BaseModule target = new BaseModule(); // TODO: Initialize to an appropriate value
            string expected = "Test Value";
            string actual;
            target.OutputColumnList = expected;
            actual = target.OutputColumnList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ModuleName
        ///</summary>
        [TestMethod()]
        public void ModuleNameTest() {
            BaseModule target = new BaseModule(); // TODO: Initialize to an appropriate value
            string expected = "Test Value";
            string actual;
            target.ModuleName = expected;
            actual = target.ModuleName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SetParameters
        ///</summary>
        [TestMethod()]
        public void SetParametersTest() {
            BaseModule target = new BaseModule(); // TODO: Initialize to an appropriate value
            string key = "OutputColumnList";
            string val = "Test Value";
            Dictionary<string, string> parameters = new Dictionary<string, string>() { { key, val } };
            target.SetParameters(parameters);
            Assert.AreEqual(val, target.OutputColumnList);
        }

        /// <summary>
        /// Test the column mapping feature
        /// </summary>
        [TestMethod()]
        public void MappedOutputColumnsOverrideColTypeTest() {

            DataGenerator dGen = new DataGenerator();
            dGen.AddAdHocRow = new string[] { "Larry", "Moe", "Curly" };
            dGen.AddAdHocRow = new string[] { "11", "22", "33" };

            TestModule target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            string outColList = string.Join(", ", new string[] { "Larry||varchar|256", "Morris|Moe|int", "Curly" });
            target.OutputColumnList = outColList;

            dGen.Run(null);

            Assert.AreEqual("Larry", target.OutColDefs[0].Name, "First column name mapping did not match");
            Assert.AreEqual("varchar", target.OutColDefs[0].DataType, "First column type mapping did not match");
            Assert.AreEqual("256", target.OutColDefs[0].Size, "First column size mapping did not match");

            Assert.AreEqual("Morris", target.OutColDefs[1].Name, "Second column name mapping did not match");
            Assert.AreEqual("int", target.OutColDefs[1].DataType, "Second column type mapping did not match");
            Assert.AreEqual("", target.OutColDefs[1].Size, "Second column size mapping did not match");

            Assert.AreEqual("Curly", target.OutColDefs[2].Name, "Third column name mapping did not match");
            Assert.AreEqual("text", target.OutColDefs[2].DataType, "Third column type mapping did not match");
            Assert.AreEqual("10", target.OutColDefs[2].Size, "Third column size mapping did not match");
        }

        /// <summary>
        /// Test the column mapping feature
        /// </summary>
        [TestMethod()]
        public void MappedOutputColumnsWildcardTest() {
            int rows = 11;
            int cols = 7;
            string insertedColumnName = "NewColName";
            DataGenerator dGen = new DataGenerator(rows, cols);

            TestModule target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            // generate list of input column names that dGen will output
            List<string> inColNames = new List<string>(DataGenerator.MakeSimulatedHeaderRow(cols));

            // generate shuffled list of input column names as output column names
            List<string> outColNames = new List<string>();

            // insert new column and use wildcard for remainder of input columns
            outColNames.Add(inColNames[0]);
            outColNames.Add(string.Format("{0}|+|text", insertedColumnName));
            outColNames.Add("*");

            string outColList = string.Join(", ", outColNames);
            target.OutputColumnList = outColList;

            dGen.Run(null);

            // compare output column definitions from target module 
            // against expected output column list
            outColNames.Clear(); // make list of expected column names
            for (int i = 0; i < inColNames.Count; i++) {
                outColNames.Add(inColNames[i]);
                if (i == 0) {
                    outColNames.Add(insertedColumnName);
                }
            }
            List<MageColumnDef> actual = target.OutColDefs;
            Assert.AreEqual(outColNames.Count, actual.Count, "Number of output columns does not match.");
            for (int i = 0; i < actual.Count; i++) {
                Assert.AreEqual(outColNames[i], actual[i].Name, "Output column name does not match");
            }

            // compare output column position index
            // against expected position index
            Dictionary<string, int> outColPos = new Dictionary<string, int>();
            for (int i = 0; i < outColNames.Count; i++) {
                outColPos[outColNames[i]] = i;
            }
            foreach (string item in target.OutColPos.Keys) {
                Assert.AreEqual(outColPos[item], target.OutColPos[item], "Column postion map index does not match");
            }
        }

        /// <summary>
        /// Test the basic column mapping feature
        /// </summary>
        [TestMethod()]
        public void MappedOutputColumnsTest() {
            int rows = 11;
            int cols = 7;
            DataGenerator dGen = new DataGenerator(rows, cols);

            TestModule target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            // generate list of input column names that dGen will output
            List<string> inColNames = new List<string>(DataGenerator.MakeSimulatedHeaderRow(cols));

            // generate shuffled list of input column names as output column names
            List<string> outColNames = new List<string>(inColNames);
            Shuffle(outColNames);
            string outColList = string.Join(", ", outColNames);
            Dictionary<string, int> outColPos = new Dictionary<string, int>();
            for (int i = 0; i < outColNames.Count; i++) {
                outColPos[outColNames[i]] = i;
            }
            target.OutputColumnList = outColList;

            dGen.Run(null);

            // compare output column definitions from target module 
            // against expected output column list
            List<MageColumnDef> actual = target.OutColDefs;
            Assert.AreEqual(inColNames.Count, actual.Count, "Number of output columns does not match.");
            for (int i = 0; i < actual.Count; i++) {
                Assert.AreEqual(outColNames[i], actual[i].Name, "Output column name does not match");
            }

            // compare output column position index
            // against expected position index
            foreach (string item in target.OutColPos.Keys) {
                Assert.AreEqual(outColPos[item], target.OutColPos[item], "Column postion map index does not match");
            }

            // check remapped data rows
			List<string[]> inputRows = target.Rows;
			List<string[]> mappedRows = target.MappedRows;
            Assert.AreEqual(inputRows.Count, mappedRows.Count, "Input rows and mapped rows count does not match.");

            for (int i = 0; i < inputRows.Count; i++) {
                foreach (KeyValuePair<int, int> colMap in target.OutToInPosMap) {
                    Assert.AreEqual(mappedRows[i][colMap.Key], inputRows[i][colMap.Value], "Mapped data does not match");
                }
            }

        }

        // rearrange the order of items in the List
        private static void Shuffle(List<string> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
