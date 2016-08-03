using System;
using System.IO;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds input and output fasta file paths to output stream
  /// </summary>
  public class FastaFilePathsFilter : BaseFilePathsFilter
  {
    public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
    {
      if (srcFilePath == null) throw new ArgumentNullException(nameof(srcFilePath));
      var sourceFolder = outRow[SourceFldrIdx];
      var fastaFileName = outRow[DatasetIdx];
      srcFilePath = Path.Combine(sourceFolder, fastaFileName);
      var ogName = outRow[OrgNameIdx];
      destFilepath = Path.Combine(DestinationRootFolderPath, ogName, OutputSubfolderName, fastaFileName);
      return true;
    }

    public override void SetDefaultProperties(string outputRootFolderPath, string outputSubfolderName)
    {
      base.SetDefaultProperties(outputRootFolderPath, outputSubfolderName);
      SourceFolderPathColName = "FASTA_Folder";
      DatasetColName = "Organism DB";
      DataPackageIDColName = "Data_Package_ID";
      OrganismNameColumn = "OG_Name";
    }
  }

}


