using System;
using System.IO;

namespace BiodiversityFileCopy
{
    /// <summary>
    /// Mage filter that adds input and output FASTA file paths to output stream
    /// </summary>
    public class FastaFilePathsFilter : BaseFilePathsFilter
    {
        public override bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath)
        {
            if (srcFilePath == null) throw new ArgumentNullException(nameof(srcFilePath));
            var sourceDirectory = outRow[SourceDirectoryIdx];
            var fastaFileName = outRow[DatasetIdx];
            srcFilePath = Path.Combine(sourceDirectory, fastaFileName);
            var ogName = outRow[OrgNameIdx];
            destFilepath = Path.Combine(DestinationRootDirectoryPath, ogName, OutputSubdirectoryName, fastaFileName);
            return true;
        }

        public override void SetDefaultProperties(string outputRootFolderPath, string outputSubfolderName)
        {
            base.SetDefaultProperties(outputRootFolderPath, OutputSubdirectoryName);
            SourceDirectoryPathColName = "FASTA_Folder";
            DatasetColName = "Organism DB";
            DataPackageIDColName = "Data_Package_ID";
            OrganismNameColumn = "OG_Name";
        }
    }
}


