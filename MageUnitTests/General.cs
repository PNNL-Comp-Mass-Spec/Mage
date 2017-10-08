using System.IO;
using Mage;
using NUnit.Framework;

namespace MageUnitTests
{
    /// <summary>
    /// Summary description for General
    /// </summary>
    [TestFixture]
    public class General
    {
        public const string DATABASE_PATH_KEY = "Database";

        private const string REMOTE_UNIT_TEST_FOLDER = @"\\proto-2\UnitTest_Files\Mage";

        public static FileInfo GetTestFile(string testFilePath)
        {

            var localFile = new FileInfo(testFilePath);
            if (localFile.Exists)
                return localFile;

            var remoteFile = new FileInfo(Path.Combine(REMOTE_UNIT_TEST_FOLDER, localFile.Name));
            if (remoteFile.Exists)
                return remoteFile;

            if (localFile.Directory.Parent != null)
            {
                var parentFolderName = localFile.Directory.Parent.Name;

                var remoteFileAlt = new FileInfo(Path.Combine(REMOTE_UNIT_TEST_FOLDER, parentFolderName, localFile.Name));
                if (remoteFileAlt.Exists)
                    return remoteFileAlt;
            }

            Assert.Fail("Test file {0} not found locally or at {1}", testFilePath, REMOTE_UNIT_TEST_FOLDER);
            return new FileInfo(localFile.Name);
        }

        public static DirectoryInfo GetTestFolder(string testFolderPath)
        {

            var localFolder = new DirectoryInfo(testFolderPath);
            if (localFolder.Exists)
                return localFolder;

            var remoteFolder = new DirectoryInfo(Path.Combine(REMOTE_UNIT_TEST_FOLDER, localFolder.Name));
            if (remoteFolder.Exists)
                return remoteFolder;

            if (localFolder.Parent != null)
            {
                var parentFolderName = localFolder.Parent.Name;

                var remoteFolderAlt = new DirectoryInfo(Path.Combine(REMOTE_UNIT_TEST_FOLDER, parentFolderName, localFolder.Name));
                if (remoteFolderAlt.Exists)
                    return remoteFolderAlt;
            }

            Assert.Fail("Test folder {0} not found locally or at {1}", testFolderPath, REMOTE_UNIT_TEST_FOLDER);
            return new DirectoryInfo(localFolder.Name);
        }

        [Test]
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
