using Mage;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for BaseModuleTest and is intended
    /// to contain all BaseModuleTest Unit Tests
    /// </summary>
    [TestFixture]
    public class BaseModuleTest
    {
        /// <summary>
        /// A test for OutputColumnList
        /// </summary>
        [Test]
        public void OutputColumnListTest()
        {
            var target = new BaseModule(); // TODO: Initialize to an appropriate value
            const string expected = "Test Value";
            target.OutputColumnList = expected;
            var actual = target.OutputColumnList;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for ModuleName
        /// </summary>
        [Test]
        public void ModuleNameTest()
        {
            var target = new BaseModule(); // TODO: Initialize to an appropriate value
            const string expected = "Test Value";
            target.ModuleName = expected;
            var actual = target.ModuleName;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for SetParameters
        /// </summary>
        [Test]
        public void SetParametersTest()
        {
            var target = new BaseModule(); // TODO: Initialize to an appropriate value
            const string key = "OutputColumnList";
            const string val = "Test Value";
            var parameters = new Dictionary<string, string> { { key, val } };
            target.SetParameters(parameters);
            Assert.AreEqual(val, target.OutputColumnList);
        }

        /// <summary>
        /// Test the column mapping feature
        /// </summary>
        [Test]
        public void MappedOutputColumnsOverrideColTypeTest()
        {
            var dGen = new DataGenerator
            {
                AddAdHocRow = new[] { "Larry", "Moe", "Curly" }
            };
            dGen.AddAdHocRow = new[] { "11", "22", "33" };

            var target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            var outColList = string.Join(", ", "Larry||varchar|256", "Morris|Moe|int", "Curly");
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
        [Test]
        public void MappedOutputColumnsWildcardTest()
        {
            const int rows = 11;
            const int cols = 7;
            const string insertedColumnName = "NewColName";
            var dGen = new DataGenerator(rows, cols);

            var target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            // Generate list of input column names that dGen will output
            var inColNames = new List<string>(DataGenerator.MakeSimulatedHeaderRow(cols));

            // Generate shuffled list of input column names as output column names
            var outColNames = new List<string> {
                inColNames[0],
                string.Format("{0}|+|text",
                insertedColumnName), "*"};

            // Insert new column and use wildcard for remainder of input columns

            var outColList = string.Join(", ", outColNames);
            target.OutputColumnList = outColList;

            dGen.Run(null);

            // Compare output column definitions from target module
            // against expected output column list

            // Make list of expected column names
            outColNames.Clear();
            for (var i = 0; i < inColNames.Count; i++)
            {
                outColNames.Add(inColNames[i]);
                if (i == 0)
                {
                    outColNames.Add(insertedColumnName);
                }
            }
            var actual = target.OutColDefs;
            Assert.AreEqual(outColNames.Count, actual.Count, "Number of output columns does not match.");
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(outColNames[i], actual[i].Name, "Output column name does not match");
            }

            // Compare output column position index
            // against expected position index
            var outColPos = new Dictionary<string, int>();
            for (var i = 0; i < outColNames.Count; i++)
            {
                outColPos[outColNames[i]] = i;
            }
            foreach (var item in target.OutColPos.Keys)
            {
                Assert.AreEqual(outColPos[item], target.OutColPos[item], "Column postion map index does not match");
            }
        }

        /// <summary>
        /// Test the basic column mapping feature
        /// </summary>
        [Test]
        public void MappedOutputColumnsTest()
        {
            const int rows = 11;
            const int cols = 7;
            var dGen = new DataGenerator(rows, cols);

            var target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            // Generate list of input column names that dGen will output
            var inColNames = new List<string>(DataGenerator.MakeSimulatedHeaderRow(cols));

            // Generate shuffled list of input column names as output column names
            var outColNames = new List<string>(inColNames);
            Shuffle(outColNames);
            var outColList = string.Join(", ", outColNames);
            var outColPos = new Dictionary<string, int>();
            for (var i = 0; i < outColNames.Count; i++)
            {
                outColPos[outColNames[i]] = i;
            }
            target.OutputColumnList = outColList;

            dGen.Run(null);

            // Compare output column definitions from target module
            // against expected output column list
            var actual = target.OutColDefs;
            Assert.AreEqual(inColNames.Count, actual.Count, "Number of output columns does not match.");
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(outColNames[i], actual[i].Name, "Output column name does not match");
            }

            // Compare output column position index
            // against expected position index
            foreach (var item in target.OutColPos.Keys)
            {
                Assert.AreEqual(outColPos[item], target.OutColPos[item], "Column postion map index does not match");
            }

            // Check remapped data rows
            var inputRows = target.Rows;
            var mappedRows = target.MappedRows;
            Assert.AreEqual(inputRows.Count, mappedRows.Count, "Input rows and mapped rows count does not match.");

            for (var i = 0; i < inputRows.Count; i++)
            {
                foreach (var colMap in target.OutToInPosMap)
                {
                    Assert.AreEqual(mappedRows[i][colMap.Key], inputRows[i][colMap.Value], "Mapped data does not match");
                }
            }
        }

        // Rearrange the order of items in the List
        private static void Shuffle(IList<string> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
