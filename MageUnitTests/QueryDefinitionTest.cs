using Mage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MageUnitTests
{

    /// <summary>
    ///This is a test class for ProcessingPipelineTest and is intended
    ///to contain all ProcessingPipelineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueryDefinitionTest
    {

        /// <summary>
        ///A test for GetQueryXMLDef
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"..\..\..\TestItems\QueryDefinitions.xml")]
        public void GetQueryXMLDefTest()
        {
            var queryName = "Mage_Analysis_Jobs";
            var actual = ModuleDiscovery.GetQueryXMLDef(queryName);
            Assert.AreNotEqual("", actual);
        }
    }
}
