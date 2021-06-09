using Mage;
using NUnit.Framework;
using System.Collections.Generic;

namespace MageUnitTests
{
    /// <summary>
    /// This is a test class for FileCopyTest and is intended
    /// to contain all FileCopyTest Unit Tests
    /// </summary>
    [TestFixture]
    public class FileCopyTest
    {
        /// <summary>
        /// <para>
        /// A class to provide access to private member variables of FileCopy.
        /// The original method, using private accessors, was deprecated in 2010
        /// </para>
        /// <para>
        /// Another option is to use PrivateObject, which requires
        /// Microsoft.VisualStudio.TestTools.UnitTesting and performs operations using reflection.
        /// </para>
        /// </summary>
        private class FileCopyExtractor : FileCopy
        {
            public Dictionary<string, int> InputColumnPosAccessor
            {
                get => InputColumnPos;
                set => InputColumnPos = value;
            }

            public string GetDestFileAccessor(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
            {
                return GetDestFile(sourceFile, fieldPos, fields);
            }
        }

        /// <summary>
        /// A test for GetDestFile
        /// </summary>
        [Test]
        public void GetDestFileTest()
        {
            var target = new FileCopyExtractor();
            const string sourceFile = "SourceFile";

            var fields = new[] { "FirstField", "SecondField", "ThirdField" };

            // Typical usage - ID column in column list and ApplyPrefixToFileName set to "Yes"
            const string idColNameTest1 = "Third";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "First", 0 }, { "Second", 1 }, { idColNameTest1, 2 } };
            target.ColumnToUseForPrefix = idColNameTest1;
            target.ApplyPrefixToFileName = "Yes";
            target.PrefixLeader = idColNameTest1;
            var fieldPosTest1 = target.InputColumnPosAccessor;
            var expectedTest1 = idColNameTest1 + "_" + fields[target.InputColumnPosAccessor[target.ColumnToUseForPrefix]] + "_" + sourceFile;
            var actualTest1 = target.GetDestFileAccessor(sourceFile, fieldPosTest1, fields);
            Assert.AreEqual(expectedTest1, actualTest1, "Typical usage");

            // ID column in column list and ApplyPrefixToFileName set to "No"
            const string idColNameTest2 = "Third";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "First", 0 }, { "Second", 1 }, { idColNameTest2, 2 } };
            target.ColumnToUseForPrefix = idColNameTest2;
            target.ApplyPrefixToFileName = "No";
            var fieldPosTest2 = target.InputColumnPosAccessor;

            const string expectedTest2 = sourceFile;
            var actualTest2 = target.GetDestFileAccessor(sourceFile, fieldPosTest2, fields);

            Assert.AreEqual(expectedTest2, actualTest2, "No prefix");

            // ID column NOT in column list and ApplyPrefixToFileName set to "Yes"
            const string idColNameTest3 = "Third";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "First", 0 }, { "Second", 1 }, { "ChoppedLiver", 2 } };
            target.ColumnToUseForPrefix = idColNameTest3;
            target.ApplyPrefixToFileName = "Yes";
            var fieldPosTest3 = target.InputColumnPosAccessor;

            const string expectedTest3 = "Tag_0_" + sourceFile;
            var actualTest3 = target.GetDestFileAccessor(sourceFile, fieldPosTest3, fields);

            Assert.AreEqual(expectedTest3, actualTest3, "Missing ID column");
        }

        /// <summary>
        /// A test for IDColumnName
        /// </summary>
        [Test]
        public void ColumnToUseForPrefixText()
        {
            var target = new FileCopy();
            const string expected = "Test Value";
            target.ColumnToUseForPrefix = expected;
            var actual = target.ColumnToUseForPrefix;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for OutputMode
        /// </summary>
        [Test]
        public void OutputModeTest()
        {
            var target = new FileCopy();
            const string expected = "Test Value";
            target.ApplyPrefixToFileName = expected;
            var actual = target.ApplyPrefixToFileName;
            Assert.AreEqual(expected, actual);
        }
    }
}
