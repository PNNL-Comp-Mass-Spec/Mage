using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MageUnitTests {


    /// <summary>
    ///This is a test class for SQLBuilderTest and is intended
    ///to contain all SQLBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLBuilderTest {

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
        /// Demonstrates use of column default predicate settings
        /// </summary>
        [TestMethod()]
        public void DefaultPredicateTest() {
            SQLBuilder target = new SQLBuilder();
            string expected;
            string actual;

            target.Table = "T_X";

            target.SetColumnDefaultPredicate("AND", "Bob", "MatchesText", "");
            target.SetColumnDefaultPredicate("OR", "Paul", "Equals", "");
            target.SetColumnDefaultPredicate("OR", "John", "NotEqual", "");
            target.SetColumnDefaultPredicate("AND", "Sue", "ContainsText", "");

            target.AddPredicateItem("Bob", "Your uncle");
            target.AddPredicateItem("Paul", "42");
            target.AddPredicateItem("John", "0");
            target.AddPredicateItem("Sue", "Your aunt");

            expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle' AND [Sue] LIKE '%Your aunt%' AND ([Paul] = 42 OR NOT [John] = 0)";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for Table
        ///</summary>
        [TestMethod()]
        public void TableTest() {
            SQLBuilder target = new SQLBuilder();
            string expected;
            string actual;
            target.Table = "T_X";
            expected = "SELECT * FROM T_X";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Columns
        ///</summary>
        [TestMethod()]
        public void ColumnsTest() {
            SQLBuilder target = new SQLBuilder();
            string expected;
            string actual;

            Assert.IsNotNull(target);
            target.Table = "T_X";
            target.Columns = "Uno, Dos, Tres";
            expected = "SELECT Uno, Dos, Tres FROM T_X";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddSortingItem
        ///</summary>
        [TestMethod()]
        public void AddSortingItemTest() {
            SQLBuilder target = new SQLBuilder();
            string expected;
            string actual;

            string col = "Bob";
            string dir = "ASC";
            target.AddSortingItem(col, dir);
            target.Table = "T_X";
            expected = "SELECT * FROM T_X ORDER BY [Bob] ASC";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AddPredicateItem
        ///</summary>
        [TestMethod()]
        public void AddPredicateItemTest() {
            SQLBuilder target = new SQLBuilder();
            string expected;
            string actual;

            target.Table = "T_X";

            target.AddPredicateItem("AND", "Bob", "MatchesText", "Your uncle");
            expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle'";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);

            target.AddPredicateItem("AND", "Sue", "ContainsText", "Your aunt");
            expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle' AND [Sue] LIKE '%Your aunt%'";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);

            target.AddPredicateItem("OR", "Paul", "Equals", "42");
            target.AddPredicateItem("OR", "John", "NotEqual", "0");

            expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle' AND [Sue] LIKE '%Your aunt%' AND ([Paul] = 42 OR NOT [John] = 0)";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\QueryDefinitions.xml")]
        public void XMLInitiationTest() {
            string expected;
            string actual;

            // runtime parameters for query
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
            string testDB = "DMS5_T3";
            runtimeParameters[":Database"] = testDB;
            runtimeParameters["Dataset"] = "sarc";

            // get XML query definition by name
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            SQLBuilder target = new SQLBuilder(queryDefXML, ref runtimeParameters);
            //           SQLBuilder target = new SQLBuilder();
            //           target.InitializeFromXML(queryDefXML, ref runtimeParameters);
            Dictionary<string, string> specialArgs = target.SpecialArgs;

            Assert.AreEqual("gigasax", specialArgs["Server"]);
            Assert.AreEqual(testDB, specialArgs["Database"]);
            Assert.AreEqual("", target.SprocName);


            expected = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE [Dataset] LIKE '%sarc%'";
            actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\QueryDefinitions.xml")]
        public void XMLSprocInitiationTest() {

            // expected predefined parameter
            string defParam = "@MinimumPMTQualityScore";
            string defValue = "9";

            // runtime parameters for query
            string testParam = "@ExperimentFilter";
            string testValue = "borked";
            Dictionary<string, string> runtimeParameters = new Dictionary<string, string>();
            runtimeParameters[testParam] = testValue;

            // get XML query definition by name
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            SQLBuilder target = new SQLBuilder(queryDefXML, ref runtimeParameters);
            Assert.AreEqual("GetMassTagsPlusPepProphetStats", target.SprocName);

            Dictionary<string, string> sprocParams = target.SprocParameters;
            Assert.AreEqual(testValue, sprocParams[testParam]);
            Assert.AreEqual(defValue, sprocParams[defParam]);
        }

        [TestMethod()]
        [DeploymentItem(@"..\..\TestItems\QueryDefinitions.xml")]
        public void XMLPredefineDescriptionsTest() {

            // get XML query definition by name
            string queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            Dictionary<string, string> descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.AreEqual("", descriptions["@MinimumHighDiscriminantScore"]);
            Assert.AreEqual("Descriptive text for MassCorrectionIDFilterList", descriptions["@MassCorrectionIDFilterList"]);

            // get XML query definition by name
            queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.AreEqual("", descriptions["Dataset_ID"]);
            Assert.AreEqual("Descriptive text for Dataset", descriptions["Dataset"]);
            Assert.AreEqual("Get factors for selected datasets", descriptions[":Description:"]);

        }

    }
}
