using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MageUnitTests {


    /// <summary>
    ///This is a test class for FileCopyTest and is intended
    ///to contain all FileCopyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileCopyTest {


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
        ///A test for GetDestFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mage.dll")]
        public void GetDestFileTest() {
            FileCopy_Accessor target = new FileCopy_Accessor();
            string expected = "";
            string actual = "";
            string sourceFile = "SourceFile";

            Dictionary<string, int> fieldPos = null;
            object[] fields = null;
            string IDColName = "";

            // typical usage - ID column in column list and ApplyPrefixToFileName set to "Yes"
            IDColName = "Tres";
            target.InputColumnPos = new Dictionary<string, int>() { { "Uno", 0 }, { "Dos", 1 }, { IDColName, 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "Yes";
            target.PrefixLeader = IDColName;
            fieldPos = target.InputColumnPos;
            fields = new string[] { "FirstField", "SecondField", "ThirdField" };
            expected = IDColName + "_" + fields[target.InputColumnPos[target.ColumnToUseForPrefix]] + "_" + sourceFile;
            actual = target.GetDestFile(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "Typical usage");

            // typical usage - ID column in column list and ApplyPrefixToFileName set to "No"
            IDColName = "Tres";
            target.InputColumnPos = new Dictionary<string, int>() { { "Uno", 0 }, { "Dos", 1 }, { IDColName, 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "No";
            fieldPos = target.InputColumnPos;
            fields = new string[] { "FirstField", "SecondField", "ThirdField" };
            expected = sourceFile;
            actual = target.GetDestFile(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "No prefix");

            // ID column NOT in column list and ApplyPrefixToFileName set to "Yes"
            IDColName = "Tres";
            target.InputColumnPos = new Dictionary<string, int>() { { "Uno", 0 }, { "Dos", 1 }, { "ChoppedLiver", 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "Yes";
            fieldPos = target.InputColumnPos;
            fields = new string[] { "FirstField", "SecondField", "ThirdField" };
            expected = "Tag_0_" + sourceFile;
            actual = target.GetDestFile(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "Missing ID column");
        }

        /// <summary>
        ///A test for IDColumnName
        ///</summary>
        [TestMethod()]
        public void ColumnToUseForPrefixText() {
            FileCopy target = new FileCopy();
            string expected = "Test Value";
            string actual;
            target.ColumnToUseForPrefix = expected;
            actual = target.ColumnToUseForPrefix;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for OutputMode
        ///</summary>
        [TestMethod()]
        public void OutputModeTest() {
            FileCopy target = new FileCopy();
            string expected = "Test Value";
            string actual;
            target.ApplyPrefixToFileName = expected;
            actual = target.ApplyPrefixToFileName;
            Assert.AreEqual(expected, actual);
        }
    }
}
