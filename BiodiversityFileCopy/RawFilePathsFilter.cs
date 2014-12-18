using System;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds input and output raw file paths to output stream 
  /// </summary>
  public class RawFilePathsFilter : AddFilePathsFilter
  {
    public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
    {
      if (srcFilePath == null) throw new ArgumentNullException("srcFilePath");
      string sourceFolder = outRow[SourceFldrIdx];
      string dataset = outRow[DatasetIdx];
      srcFilePath = string.Format(@"{0}\{1}.RAW", sourceFolder, dataset);
      var ogName = outRow[OrgNameIdx];
      destFilepath = string.Format(@"{0}{1}\RAW\{2}.RAW", DestinationRootFolderPath, ogName, dataset);
      return true;
    }
  }

}
