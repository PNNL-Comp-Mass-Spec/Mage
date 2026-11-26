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
        // ReSharper disable CommentTypo

        // Ignore Spelling: borked, dos, Mage, sarc, Sproc, tres, uno

        // ReSharper restore CommentTypo

        /// <summary>
        /// Demonstrates use of column default predicate settings
        /// </summary>
        [Test]
        public void DefaultPredicateTest()
        {
            var target = new SQLBuilder { Table = "T_X" };

            target.SetColumnDefaultPredicate("AND", "Bob", "MatchesText", "");
            target.SetColumnDefaultPredicate("OR", "Paul", "Equals", "");
            target.SetColumnDefaultPredicate("OR", "John", "NotEqual", "");
            target.SetColumnDefaultPredicate("AND", "Peggy Sue", "ContainsText", "");

            target.AddPredicateItem("Bob", "Your uncle");
            target.AddPredicateItem("Paul", "42");
            target.AddPredicateItem("John", "0");
            target.AddPredicateItem("Peggy Sue", "Your aunt");

            const string expected = "SELECT * FROM T_X WHERE Bob = 'Your uncle' AND [Peggy Sue] LIKE '%Your aunt%' AND (Paul = 42 OR NOT John = 0)";
            var actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for Table
        /// </summary>
        [Test]
        public void TableTest()
        {
            var target = new SQLBuilder { Table = "T_X" };
            const string expected = "SELECT * FROM T_X";
            var actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for Columns
        /// </summary>
        [Test]
        public void ColumnsTest()
        {
            var target = new SQLBuilder();

            Assert.That(target, Is.Not.Null);

            target.Table = "T_X";
            target.Columns = "Uno, Dos, Tres";
            const string expected = "SELECT Uno, Dos, Tres FROM T_X";
            var actual = target.BuildQuerySQL();

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for AddSortingItem
        /// </summary>
        [Test]
        public void AddSortingItemTest()
        {
            var target = new SQLBuilder();

            const string col = "Bob";
            const string dir = "ASC";
            target.AddSortingItem(col, dir);
            target.Table = "T_X";
            const string expected = "SELECT * FROM T_X ORDER BY Bob ASC";
            var actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// A test for AddPredicateItem
        /// </summary>
        [Test]
        public void AddPredicateItemTest()
        {
            var target = new SQLBuilder { Table = "T_X" };

            target.AddPredicateItem("AND", "Bob", "MatchesText", "Your uncle");
            var expected = "SELECT * FROM T_X WHERE Bob = 'Your uncle'";
            var actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));

            target.AddPredicateItem("AND", "Sue", "ContainsText", "Your aunt");
            expected = "SELECT * FROM T_X WHERE Bob = 'Your uncle' AND Sue LIKE '%Your aunt%'";
            actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));

            target.AddPredicateItem("OR", "Paul", "Equals", "42");
            target.AddPredicateItem("OR", "John", "NotEqual", "0");

            expected = "SELECT * FROM T_X WHERE Bob = 'Your uncle' AND Sue LIKE '%Your aunt%' AND (Paul = 42 OR NOT John = 0)";
            actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLInitiationTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Runtime parameters for query
            var runtimeParameters = new Dictionary<string, string>();
            const string testDB = "dms";
            runtimeParameters[":Database"] = testDB;
            runtimeParameters["Dataset"] = "sarc";

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.That(queryDefXML, Is.Not.Empty);

            var target = new SQLBuilder(queryDefXML, runtimeParameters);

            // SQLBuilder target = new SQLBuilder();
            // target.InitializeFromXML(queryDefXML, ref runtimeParameters);

            var specialArgs = target.SpecialArgs;

            Assert.Multiple(() =>
            {
                Assert.That(specialArgs[SQLBuilder.SERVER_NAME_KEY].ToLower(), Is.EqualTo(Globals.DMSServer.ToLower()));
                Assert.That(specialArgs[SQLBuilder.DATABASE_NAME_KEY], Is.EqualTo(testDB));
                Assert.That(target.SprocName, Is.Empty);
            });

            const string expected = "SELECT Dataset, Dataset_ID, Factor, Value FROM V_Custom_Factors_List_Report WHERE Dataset LIKE '%sarc%'";
            var actual = target.BuildQuerySQL();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLSprocInitiationTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Expected predefined parameter
            const string defParam = "@MinimumPMTQualityScore";
            const string defValue = "4";

            // Runtime parameters for query
            const string testParam = "@ExperimentFilter";
            const string testValue = "borked";
            var runtimeParameters = new Dictionary<string, string> { [testParam] = testValue };

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.That(queryDefXML, Is.Not.Empty);

            var target = new SQLBuilder(queryDefXML, runtimeParameters);
            Assert.That(target.SprocName, Is.EqualTo("GetMassTagsPlusPepProphetStats"));

            var sprocParams = target.SprocParameters;

            Assert.Multiple(() =>
            {
                Assert.That(sprocParams[testParam], Is.EqualTo(testValue));
                Assert.That(sprocParams[defParam], Is.EqualTo(defValue));
            });
        }

        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void XMLPredefineDescriptionsTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            // Get XML query definition by name
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("GetMassTagsPlusPepProphetStats");
            Assert.That(queryDefXML, Is.Not.Empty);

            var descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.Multiple(() =>
            {
                Assert.That(descriptions["@MinimumHighDiscriminantScore"], Is.Empty);
                Assert.That(descriptions["@MassCorrectionIDFilterList"], Is.EqualTo("Descriptive text for MassCorrectionIDFilterList"));
            });

            // Get XML query definition by name
            queryDefXML = ModuleDiscovery.GetQueryXMLDef("Factors_List_Report");
            Assert.That(queryDefXML, Is.Not.Empty);

            descriptions = SQLBuilder.GetDescriptionsFromXML(queryDefXML);

            Assert.Multiple(() =>
            {
                Assert.That(descriptions["Dataset_ID"], Is.Empty);
                Assert.That(descriptions["Dataset"], Is.EqualTo("Descriptive text for Dataset"));
                Assert.That(descriptions[":Description:"], Is.EqualTo("Get factors for selected datasets"));
            });
        }
    }
}
