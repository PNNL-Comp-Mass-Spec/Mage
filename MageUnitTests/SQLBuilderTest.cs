using Mage;
using NUnit.Framework;
using System.Collections.Generic;

namespace MageUnitTests
{

    /// <summary>
    /// This is a test class for SQLBuilderTest and is intended
    /// to contain all SQLBuilderTest Unit Tests
    /// </summary>
    [TestFixture]
    public class SQLBuilderTest
    {

        /// <summary>
        /// Demonstrates use of column default predicate settings
        /// </summary>
        [Test]
        public void DefaultPredicateTest()
        {
            var target = new SQLBuilder {Table = "T_X"};

            target.SetColumnDefaultPredicate("AND", "Bob", "MatchesText", "");
            target.SetColumnDefaultPredicate("OR", "Paul", "Equals", "");
            target.SetColumnDefaultPredicate("OR", "John", "NotEqual", "");
            target.SetColumnDefaultPredicate("AND", "Sue", "ContainsText", "");

            target.AddPredicateItem("Bob", "Your uncle");
            target.AddPredicateItem("Paul", "42");
            target.AddPredicateItem("John", "0");
            target.AddPredicateItem("Sue", "Your aunt");

            var expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle' AND [Sue] LIKE '%Your aunt%' AND ([Paul] = 42 OR NOT [John] = 0)";
            var actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for Table
        /// </summary>
        [Test]
        public void TableTest()
        {
            var target = new SQLBuilder {Table = "T_X"};
            var expected = "SELECT * FROM T_X";
            var actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for Columns
        /// </summary>
        [Test]
        public void ColumnsTest()
        {
            var target = new SQLBuilder();

            Assert.IsNotNull(target);
            target.Table = "T_X";
            target.Columns = "Uno, Dos, Tres";
            var expected = "SELECT Uno, Dos, Tres FROM T_X";
            var actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for AddSortingItem
        /// </summary>
        [Test]
        public void AddSortingItemTest()
        {
            var target = new SQLBuilder();

            var col = "Bob";
            var dir = "ASC";
            target.AddSortingItem(col, dir);
            target.Table = "T_X";
            var expected = "SELECT * FROM T_X ORDER BY [Bob] ASC";
            var actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for AddPredicateItem
        /// </summary>
        [Test]
        public void AddPredicateItemTest()
        {
            var target = new SQLBuilder {Table = "T_X"};


            target.AddPredicateItem("AND", "Bob", "MatchesText", "Your uncle");
            var expected = "SELECT * FROM T_X WHERE [Bob] = 'Your uncle'";
            var actual = target.BuildQuerySQL();
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

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLInitiationTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>();
            var testDB = "DMS5_T3";
            runtimeParameters[":Database"] = testDB;
            runtimeParameters["Dataset"] = "sarc";

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            var target = new SQLBuilder(queryDefXML, ref runtimeParameters);
            // SQLBuilder target = new SQLBuilder();
            // target.InitializeFromXML(queryDefXML, ref runtimeParameters);
            var specialArgs = target.SpecialArgs;

            Assert.AreEqual(Globals.DMSServer.ToLower(), specialArgs[SQLBuilder.SERVER_NAME_KEY].ToLower());
            Assert.AreEqual(testDB, specialArgs[SQLBuilder.DATABASE_NAME_KEY]);
            Assert.AreEqual("", target.SprocName);


            var expected = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE [Dataset] LIKE '%sarc%'";
            var actual = target.BuildQuerySQL();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLSprocInitiationTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Expected predefined parameter
            var defParam = "@MinimumPMTQualityScore";
            var defValue = "4";

            // Runtime parameters for query
            var testParam = "@ExperimentFilter";
            var testValue = "borked";
            var runtimeParameters = new Dictionary<string, string> {[testParam] = testValue};

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            var target = new SQLBuilder(queryDefXML, ref runtimeParameters);
            Assert.AreEqual("GetMassTagsPlusPepProphetStats", target.SprocName);

            var sprocParams = target.SprocParameters;
            Assert.AreEqual(testValue, sprocParams[testParam]);
            Assert.AreEqual(defValue, sprocParams[defParam]);
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLPredefineDescriptionsTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.AreNotEqual("", queryDefXML);

            var descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.AreEqual("", descriptions["@MinimumHighDiscriminantScore"]);
            Assert.AreEqual("Descriptive text for MassCorrectionIDFilterList", descriptions["@MassCorrectionIDFilterList"]);

            // Get XML query definition by name
            queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.AreNotEqual("", queryDefXML);

            descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.AreEqual("", descriptions["Dataset_ID"]);
            Assert.AreEqual("Descriptive text for Dataset", descriptions["Dataset"]);
            Assert.AreEqual("Get factors for selected datasets", descriptions[":Description:"]);

        }

    }
}
