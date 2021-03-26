using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
    internal class SimpleFilePathsFilter : BaseFilePathsFilter
    {
        protected int ItemIdx;
        protected int FileIdx;

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            ItemIdx = OutputColumnPos["Item"];// TBD: look up from property??
            FileIdx = OutputColumnPos["File"];// TBD: look up from property??
        }

        public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
        {
            // skip input rows that don't actually specify a file
            if (outRow[ItemIdx] == "file")
            {
                srcFilePath = Path.Combine(outRow[SourceDirectoryIdx], outRow[FileIdx]);
                var msgfFileName = outRow[FileIdx];
                var ogName = outRow[OrgNameIdx];
                destFilepath = Path.Combine(DestinationRootDirectoryPath, ogName, OutputSubdirectoryName, msgfFileName);
                return true;
            }
            return false;
        }
    }
}