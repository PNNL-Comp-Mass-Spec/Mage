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
        /// A class to provide access to private member variables of FileCopy.
        /// Original method was using private accessors, deprecated starting in 2010
        /// Another option was using PrivateObject, which requires
        ///    Microsoft.VisualStudio.TestTools.UnitTesting and performs operations using reflection.
        /// </summary>
        private class FileCopyExtracter : FileCopy
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
            var target = new FileCopyExtracter();
            var sourceFile = "SourceFile";

            // Typical usage - ID column in column list and ApplyPrefixToFileName set to "Yes"
            var IDColName = "Tres";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "Uno", 0 }, { "Dos", 1 }, { IDColName, 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "Yes";
            target.PrefixLeader = IDColName;
            var fieldPos = target.InputColumnPosAccessor;
            var fields = new[] { "FirstField", "SecondField", "ThirdField" };
            var expected = IDColName + "_" + fields[target.InputColumnPosAccessor[target.ColumnToUseForPrefix]] + "_" + sourceFile;
            var actual = target.GetDestFileAccessor(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "Typical usage");

            // Typical usage - ID column in column list and ApplyPrefixToFileName set to "No"
            IDColName = "Tres";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "Uno", 0 }, { "Dos", 1 }, { IDColName, 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "No";
            fieldPos = target.InputColumnPosAccessor;
            fields = new[] { "FirstField", "SecondField", "ThirdField" };
            expected = sourceFile;
            actual = target.GetDestFileAccessor(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "No prefix");

            // ID column NOT in column list and ApplyPrefixToFileName set to "Yes"
            IDColName = "Tres";
            target.InputColumnPosAccessor = new Dictionary<string, int> { { "Uno", 0 }, { "Dos", 1 }, { "ChoppedLiver", 2 } };
            target.ColumnToUseForPrefix = IDColName;
            target.ApplyPrefixToFileName = "Yes";
            fieldPos = target.InputColumnPosAccessor;
            fields = new[] { "FirstField", "SecondField", "ThirdField" };
            expected = "Tag_0_" + sourceFile;
            actual = target.GetDestFileAccessor(sourceFile, fieldPos, fields);
            Assert.AreEqual(expected, actual, "Missing ID column");
        }

        /// <summary>
        /// A test for IDColumnName
        /// </summary>
        [Test]
        public void ColumnToUseForPrefixText()
        {
            var target = new FileCopy();
            var expected = "Test Value";
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
            var expected = "Test Value";
            target.ApplyPrefixToFileName = expected;
            var actual = target.ApplyPrefixToFileName;
            Assert.AreEqual(expected, actual);
        }
    }
}
