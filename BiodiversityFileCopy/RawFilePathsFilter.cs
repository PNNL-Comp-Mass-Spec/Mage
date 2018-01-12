using System;
using System.IO;


namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds input and output raw file paths to output stream
  /// </summary>
  public class RawFilePathsFilter : BaseFilePathsFilter
  {
    public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
    {
      if (srcFilePath == null) throw new ArgumentNullException("srcFilePath");
      var sourceFolder = outRow[SourceFldrIdx];
      var dataset = outRow[DatasetIdx];
      var datasetFile = dataset + ".RAW";
      srcFilePath = Path.Combine(sourceFolder, datasetFile);
      var ogName = outRow[OrgNameIdx];
      destFilepath = Path.Combine(DestinationRootFolderPath, ogName, OutputSubfolderName, datasetFile);
      return true;
    }
  }

}
