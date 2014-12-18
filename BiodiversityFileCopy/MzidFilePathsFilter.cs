using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds input and output mzid.gz file paths to output stream 
  /// </summary>
  public class MzidFilePathsFilter : AddFilePathsFilter
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
      if (outRow[ItemIdx] == "file") // skip input rows that don't actually specify a file
                {
        srcFilePath = Path.Combine(outRow[SourceFldrIdx], outRow[FileIdx]);
        var msgfFileName = outRow[FileIdx];
        var ogName = outRow[OrgNameIdx];
        //destFilepath = string.Format(@"{0}{1}\MZID\{2}", DestinationRootFolderPath, ogName, msgfFileName);
        destFilepath = Path.Combine(DestinationRootFolderPath, ogName, "MZID", msgfFileName);
        return true;
      }
      return false;
    }
  }

}
