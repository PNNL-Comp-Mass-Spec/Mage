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
        // Ignore Spelling: varchar

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
            Assert.That(actual, Is.EqualTo(expected));
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
            Assert.That(actual, Is.EqualTo(expected));
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
            Assert.That(target.OutputColumnList, Is.EqualTo(val));
        }

        /// <summary>
        /// Test the column mapping feature
        /// </summary>
        [Test]
        public void MappedOutputColumnsOverrideColTypeTest()
        {
            var dGen = new DataGenerator
            {
                AddAdHocRow = ["Larry", "Moe", "Curly"]
            };
            dGen.AddAdHocRow = ["11", "22", "33"];

            var target = new TestModule();
            dGen.ColumnDefAvailable += target.HandleColumnDef;
            dGen.DataRowAvailable += target.HandleDataRow;

            target.OutputColumnList = string.Join(", ", "Larry||varchar|256", "Morris|Moe|int", "Curly");

            dGen.Run(null);

            Assert.Multiple(() =>
            {
                Assert.That(target.OutColDefs[0].Name, Is.EqualTo("Larry"), "First column name mapping did not match");
                Assert.That(target.OutColDefs[0].DataType, Is.EqualTo("varchar"), "First column type mapping did not match");
                Assert.That(target.OutColDefs[0].Size, Is.EqualTo("256"), "First column size mapping did not match");

                Assert.That(target.OutColDefs[1].Name, Is.EqualTo("Morris"), "Second column name mapping did not match");
                Assert.That(target.OutColDefs[1].DataType, Is.EqualTo("int"), "Second column type mapping did not match");
                Assert.That(target.OutColDefs[1].Size, Is.Empty, "Second column size mapping did not match");

                Assert.That(target.OutColDefs[2].Name, Is.EqualTo("Curly"), "Third column name mapping did not match");
                Assert.That(target.OutColDefs[2].DataType, Is.EqualTo("text"), "Third column type mapping did not match");
                Assert.That(target.OutColDefs[2].Size, Is.EqualTo("10"), "Third column size mapping did not match");
            });
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
            Assert.That(actual, Has.Count.EqualTo(outColNames.Count), "Number of output columns does not match.");
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.That(actual[i].Name, Is.EqualTo(outColNames[i]), "Output column name does not match");
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
                Assert.That(target.OutColPos[item], Is.EqualTo(outColPos[item]), "Column position map index does not match");
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
            Assert.That(actual, Has.Count.EqualTo(inColNames.Count), "Number of output columns does not match.");
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.That(actual[i].Name, Is.EqualTo(outColNames[i]), "Output column name does not match");
            }

            // Compare output column position index
            // against expected position index
            foreach (var item in target.OutColPos.Keys)
            {
                Assert.That(target.OutColPos[item], Is.EqualTo(outColPos[item]), "Column position map index does not match");
            }

            // Check remapped data rows
            var inputRows = target.Rows;
            var mappedRows = target.MappedRows;
            Assert.That(mappedRows, Has.Count.EqualTo(inputRows.Count), "Input rows and mapped rows count does not match.");

            for (var i = 0; i < inputRows.Count; i++)
            {
                foreach (var colMap in target.OutToInPosMap)
                {
                    Assert.That(inputRows[i][colMap.Value], Is.EqualTo(mappedRows[i][colMap.Key]), "Mapped data does not match");
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

                // Swap values
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
