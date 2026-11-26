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
        // Ignore Spelling: Mage

        public const string DATABASE_PATH_KEY = "Database";

        private const string REMOTE_UNIT_TEST_DIRECTORY = @"\\proto-2\UnitTest_Files\Mage";

        public static FileInfo GetTestFile(string testFilePath)
        {
            var localFile = new FileInfo(testFilePath);
            if (localFile.Exists)
                return localFile;

            var remoteFile = new FileInfo(Path.Combine(REMOTE_UNIT_TEST_DIRECTORY, localFile.Name));
            if (remoteFile.Exists)
                return remoteFile;

            if (localFile.Directory?.Parent != null)
            {
                var parentDirectoryName = localFile.Directory.Parent.Name;

                var remoteFileAlt = new FileInfo(Path.Combine(REMOTE_UNIT_TEST_DIRECTORY, parentDirectoryName, localFile.Name));
                if (remoteFileAlt.Exists)
                    return remoteFileAlt;
            }

            Assert.Fail(string.Format("Test file {0} not found locally or at {1}", testFilePath, REMOTE_UNIT_TEST_DIRECTORY));
            return new FileInfo(localFile.Name);
        }

        public static DirectoryInfo GetTestDirectory(string testDirectoryPath)
        {
            var localDirectory = new DirectoryInfo(testDirectoryPath);
            if (localDirectory.Exists)
                return localDirectory;

            var remoteDirectory = new DirectoryInfo(Path.Combine(REMOTE_UNIT_TEST_DIRECTORY, localDirectory.Name));
            if (remoteDirectory.Exists)
                return remoteDirectory;

            if (localDirectory.Parent != null)
            {
                var parentDirectoryName = localDirectory.Parent.Name;

                var remoteDirectoryAlt = new DirectoryInfo(Path.Combine(REMOTE_UNIT_TEST_DIRECTORY, parentDirectoryName, localDirectory.Name));
                if (remoteDirectoryAlt.Exists)
                    return remoteDirectoryAlt;
            }

            Assert.Fail(string.Format("Test DIRECTORY {0} not found locally or at {1}", testDirectoryPath, REMOTE_UNIT_TEST_DIRECTORY));
            return new DirectoryInfo(localDirectory.Name);
        }

        [Test]
        public void DataGeneratorTest()
        {
            var dGen = new DataGenerator();
            const int cols = 4;
            const int rows = 17;
            dGen.Rows = rows;
            dGen.Cols = cols;
            var sink = new SimpleSink();
            dGen.ColumnDefAvailable += sink.HandleColumnDef;
            dGen.DataRowAvailable += sink.HandleDataRow;
            dGen.Run(null);

            Assert.Multiple(() =>
            {
                Assert.That(sink.Rows, Has.Count.EqualTo(rows));
                Assert.That(sink.Columns, Has.Count.EqualTo(cols));
            });
        }

        public static void CompareSinks(SimpleSink source, SimpleSink result)
        {
            var sourceCols = source.Columns;
            var resultCols = result.Columns;
            var sourceRows = source.Rows;
            var resultRows = result.Rows;

            Assert.Multiple(() =>
            {
                Assert.That(resultCols, Has.Count.EqualTo(sourceCols.Count));
                Assert.That(resultRows, Has.Count.EqualTo(sourceRows.Count));
            });

            for (var i = 0; i < sourceCols.Count; i++)
            {
                Assert.That(resultCols[i].Name, Is.EqualTo(sourceCols[i].Name), "Column names do not match");
            }

            for (var i = 0; i < sourceRows.Count; i++)
            {
                for (var j = 0; j < sourceCols.Count; j++)
                {
                    Assert.That(resultRows[i][j], Is.EqualTo(sourceRows[i][j]), string.Format("Row {0} cell {1} content does not match", i, j));
                }
            }
        }

    }
}
