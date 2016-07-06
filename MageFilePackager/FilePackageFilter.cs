using System.Collections.Generic;
using Mage;

namespace MageFilePackager {

    // cleans up input file search data and normalizes it for inclusion in manifest
    class FilePackageFilter : ContentFilter {

        // Indexes into the row field data array
        private int _sourceIdx;
        private int _pathIdx;
        private int _folderIdx;
        private int _storagePathIdx;
        private int _archivePathIdx;
        private int _purgedIdx;

        public readonly static string DataPackageShareRoot = @"\\protoapps\DataPkgs\";

        public readonly static Dictionary<string, string> PrefixList = new Dictionary<string, string> {
                                                                      { "Job", @"\\aurora.emsl.pnl.gov\archive\dmsarch\"},
                                                                      { "Data_Package", @"\\aurora.emsl.pnl.gov\archive\prismarch\DataPkgs\" },
                                                                      { "Dataset", @"\\aurora.emsl.pnl.gov\archive\dmsarch\" }
                                                                  };

        // Precalulate field indexes
        protected override void ColumnDefsFinished() {
            _folderIdx = InputColumnPos["Folder"];
            _storagePathIdx = (InputColumnPos.ContainsKey("Storage_Path")) ? InputColumnPos["Storage_Path"] : -1;
            _archivePathIdx = InputColumnPos["Archive_Path"];
            _purgedIdx = (InputColumnPos.ContainsKey("Purged")) ? InputColumnPos["Purged"] : -1;
            for (int i = 0; i < OutputColumnDefs.Count; i++) {
                MageColumnDef cd = OutputColumnDefs[i];
                if (cd.Name == "Source") _sourceIdx = i;
                if (cd.Name == "Path") _pathIdx = i;
            }
        }

        /// <summary>
        /// Filter each row
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
		protected override bool CheckFilter(ref string[] vals)
		{

            // apply field mapping to output
            if (OutputColumnDefs != null) {
				string[] outRow = MapDataRow(vals);

                // what kind DMS entity does the file belong to?
                string source = outRow[_sourceIdx];

                if (source == "Data_Package") {
                    // we don't have an actual archive path to work with
                    // - fake one from storage path
                    string folderPath = vals[_folderIdx];
                    outRow[_pathIdx] = folderPath.Replace(DataPackageShareRoot, "");
                } else {
                    // we have an actual archive path to work with
                    string archivePath = vals[_folderIdx];

                    // if it is not an archive path, 
                    // replace the storage root path 
                    // with the archive root path
                    string prefix = (PrefixList.ContainsKey(source)) ? PrefixList[source] : "";
                    string purged = vals[_purgedIdx];
                    if (purged == "0") {
						string storageRoot = vals[_storagePathIdx];
                        string archiveRoot = vals[_archivePathIdx];
                        archivePath = archivePath.Replace(storageRoot, archiveRoot);
                    }

                    // - remove the archive prefix
                    outRow[_pathIdx] = archivePath.Replace(prefix, "");
                }

                vals = outRow;
            }
            return true;
        }

    }
}
