using System;
using System.IO;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds input and output fasta file paths to output stream
  /// </summary>
  public class FastaFilePathsFilter : AddFilePathsFilter
  {
    public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
    {
      if (srcFilePath == null) throw new ArgumentNullException("srcFilePath");
      string sourceFolder = outRow[SourceFldrIdx];
      string fastaFileName = outRow[DatasetIdx];
      srcFilePath = Path.Combine(sourceFolder, fastaFileName);
      var ogName = outRow[OrgNameIdx];
      destFilepath = string.Format(@"{0}{1}\fasta\{2}", DestinationRootFolderPath, ogName, fastaFileName);
      return true;
    }
  }

}
