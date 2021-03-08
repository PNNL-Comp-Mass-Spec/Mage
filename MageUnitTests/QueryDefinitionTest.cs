using Mage;
using NUnit.Framework;
namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for ProcessingPipelineTest and is intended
    /// to contain all ProcessingPipelineTest Unit Tests
    /// </summary>
    [TestFixture]
    public class QueryDefinitionTest
    {
        /// <summary>
        /// A test for GetQueryXMLDef
        /// </summary>
        [Test]
        [TestCase(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void GetQueryXMLDefTest(string queryDefinitionsPath)
        {
            var queryDefsFile = General.GetTestFile(queryDefinitionsPath);

            var queryName = "Mage_Analysis_Jobs";
            ModuleDiscovery.QueryDefinitionFileName = queryDefsFile.FullName;

            var actual = ModuleDiscovery.GetQueryXMLDef(queryName);
            Assert.AreNotEqual("", actual);
        }
    }
}
