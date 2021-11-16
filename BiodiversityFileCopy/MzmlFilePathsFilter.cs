using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
    /// <summary>
    /// Mage filter that adds input and output mzML file paths to output stream
    /// </summary>
    public class MzmlFilePathsFilter : BaseFilePathsFilter
    {
        // Ignore Spelling: Mage

        private int ItemIdx;
        private int FileIdx;

        public Dictionary<string, MzMLPath> MzMLPaths { get; } = new Dictionary<string, MzMLPath>();

        private int _datasetIdIdx;
        private const string MzmlGenPattern = "MSXML_Gen";

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            ItemIdx = OutputColumnPos["Item"];
            FileIdx = OutputColumnPos["File"];
            _datasetIdIdx = OutputColumnPos["Dataset_ID"];
        }

        public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
        {
            // Look at tool for row
            // if it is refinery and dataset is in refinery set, output it
            // if it is not refinery and dataset is in not refinery set, output it.

            // Skip input rows that don't actually specify a file
            if (outRow[ItemIdx] == "file")
            {
                // Save mzML cache file path for dataset based on tool
                // to internal buffer
                var datasetId = outRow[_datasetIdIdx];
                var cacheFilePath = Path.Combine(outRow[SourceDirectoryIdx], outRow[FileIdx]);
                var gen = cacheFilePath.Contains(MzmlGenPattern);

                if (!MzMLPaths.ContainsKey(datasetId))
                {
                    MzMLPaths[datasetId] = new MzMLPath();
                }
                if (gen)
                {
                    MzMLPaths[datasetId].GenPath = cacheFilePath;
                    MzMLPaths[datasetId].GenRow = outRow;
                }
                else
                {
                    MzMLPaths[datasetId].RefineryPath = cacheFilePath;
                    MzMLPaths[datasetId].RefRow = outRow;
                }
            }
            return false; // This is a sink module which accumulates to its own internal buffer - not appropriate to pass rows to output stream
        }

        /// <summary>
        /// Iterate through all saved rows and set up the correct
        /// mzML source and destination file paths
        /// </summary>
        private IEnumerable<string[]> CorrectFilePaths()
        {
            var savedRows = new List<string[]>();
            foreach (var kv in MzMLPaths)
            {
                string[] row = null;

                var q = kv.Value;
                if (q == null)
                    continue;

                if (q.RefineryPath != null)
                {
                    row = q.RefRow;
                    row[SourceFileIdx] = q.RefineryPath;
                }
                else if (q.GenPath != null)
                {
                    row = q.GenRow;
                    row[SourceFileIdx] = q.GenPath;
                }

                Debug.Assert(row != null, "row is null in MzmlFilePathsFilter.CorrectFilePaths");

                var cacheFilePath = Path.Combine(row[SourceDirectoryIdx], row[FileIdx]);

                var cacheFileLines = File.ReadAllLines(cacheFilePath);

                if (cacheFileLines.Length == 0)
                    continue;

                var srcFilePath = cacheFileLines[0].Trim();

                var mzmlFileName = Path.GetFileName(srcFilePath);
                if (string.IsNullOrEmpty(mzmlFileName))
                    continue;

                var ogName = row[OrgNameIdx];
                var destFilepath = Path.Combine(DestinationRootDirectoryPath, ogName, OutputSubdirectoryName, mzmlFileName);
                row[SourceFileIdx] = srcFilePath;
                row[DestFileIdx] = destFilepath;

                savedRows.Add(row);
            }
            return savedRows;
        }

        /// <summary>
        /// Serve contents of paths collection
        /// </summary>
        /// <param name="state"></param>
        public override void Run(object state)
        {
            var rows = CorrectFilePaths();
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
            foreach (var row in rows)
            {
                if (Abort) break;
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        /// <summary>
        /// Possible paths for mzML cache
        /// </summary>
        public class MzMLPath
        {
            public string GenPath { get; set; }
            public string[] GenRow { get; set; }
            public string RefineryPath { get; set; }
            public string[] RefRow { get; set; }
        }
    }
}
