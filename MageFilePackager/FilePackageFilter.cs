using System.Collections.Generic;
using Mage;

namespace MageFilePackager
{
    /// <summary>
    /// Cleans up input file search data and normalizes it for inclusion in manifest
    /// </summary>
    internal class FilePackageFilter : ContentFilter
    {
        // Indexes into the row field data array
        private int mSourceIdx;
        private int mPathIdx;
        private int mDirectoryIdx;
        private int mStoragePathIdx;
        private int mArchivePathIdx;
        private int mPurgedIdx;

        public static readonly string DataPackageShareRoot = @"\\protoapps\DataPkgs\";

        public static readonly Dictionary<string, string> PrefixList = new()
        {
                                                                      { "Job", @"\\agate.emsl.pnl.gov\dmsarch\"},
                                                                      { "Data_Package", @"\\agate.emsl.pnl.gov\archive\prismarch\DataPkgs\" },
                                                                      { "Dataset", @"\\agate.emsl.pnl.gov\dmsarch\" }
                                                                  };

        // Precalculate field indexes
        protected override void ColumnDefsFinished()
        {
            mDirectoryIdx = InputColumnPos["Directory"];
            mStoragePathIdx = InputColumnPos.ContainsKey("Storage_Path") ? InputColumnPos["Storage_Path"] : -1;
            mArchivePathIdx = InputColumnPos["Archive_Path"];
            mPurgedIdx = InputColumnPos.ContainsKey("Purged") ? InputColumnPos["Purged"] : -1;
            for (var i = 0; i < OutputColumnDefs.Count; i++)
            {
                var cd = OutputColumnDefs[i];
                if (cd.Name == "Source")
                    mSourceIdx = i;
                if (cd.Name == "Path")
                    mPathIdx = i;
            }
        }

        /// <summary>
        /// Filter each row
        /// </summary>
        /// <param name="values"></param>
        protected override bool CheckFilter(ref string[] values)
        {
            // Apply field mapping to output
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(values);

                // What kind DMS entity does the file belong to?
                var source = outRow[mSourceIdx];

                if (source == "Data_Package")
                {
                    // We don't have an actual archive path to work with
                    // - fake one from storage path
                    var directoryPath = values[mDirectoryIdx];
                    outRow[mPathIdx] = directoryPath.Replace(DataPackageShareRoot, "");
                }
                else
                {
                    // We have an actual archive path to work with
                    var archivePath = values[mDirectoryIdx];

                    // If it is not an archive path,
                    // replace the storage root path
                    // with the archive root path
                    var prefix = PrefixList.ContainsKey(source) ? PrefixList[source] : "";
                    var purged = values[mPurgedIdx];
                    if (purged == "0")
                    {
                        var storageRoot = values[mStoragePathIdx];
                        var archiveRoot = values[mArchivePathIdx];
                        archivePath = archivePath.Replace(storageRoot, archiveRoot);
                    }

                    // - remove the archive prefix
                    outRow[mPathIdx] = archivePath.Replace(prefix, "");
                }

                values = outRow;
            }
            return true;
        }
    }
}
