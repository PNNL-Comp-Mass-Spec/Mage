using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mage;

namespace MageUnitTests
{
    /// <summary>
    /// Summary description for General
    /// </summary>
    [TestClass]
    public class General
    {

        [TestMethod()]
        public void DataGeneratorTest()
        {
            var dGen = new DataGenerator();
            var cols = 4;
            var rows = 17;
            dGen.Rows = rows;
            dGen.Cols = cols;
            var sink = new SimpleSink();
            dGen.ColumnDefAvailable += sink.HandleColumnDef;
            dGen.DataRowAvailable += sink.HandleDataRow;
            dGen.Run(null);
            Assert.AreEqual(rows, sink.Rows.Count);
            Assert.AreEqual(cols, sink.Columns.Count);
        }

        public static void CompareSinks(SimpleSink source, SimpleSink result)
        {
            var sourceCols = source.Columns;
            var resultCols = result.Columns;
            var sourceRows = source.Rows;
            var resultRows = result.Rows;

            Assert.AreEqual(sourceCols.Count, resultCols.Count);
            Assert.AreEqual(sourceRows.Count, resultRows.Count);

            for (var i = 0; i < sourceCols.Count; i++)
            {
                Assert.AreEqual(sourceCols[i].Name, resultCols[i].Name, "Column names do not match");
            }

            for (var i = 0; i < sourceRows.Count; i++)
            {
                for (var j = 0; j < sourceCols.Count; j++)
                {
                    Assert.AreEqual(sourceRows[i][j], resultRows[i][j], string.Format("Row {0} cell {1} content does not match", i, j));
                }
            }
        }

    }
}
