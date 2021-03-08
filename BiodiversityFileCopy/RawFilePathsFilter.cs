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
            if (srcFilePath == null) throw new ArgumentNullException(nameof(srcFilePath));
            var sourceDirectory = outRow[SourceDirectoryIdx];
            var dataset = outRow[DatasetIdx];
            var datasetFile = dataset + ".RAW";
            srcFilePath = Path.Combine(sourceDirectory, datasetFile);
            var ogName = outRow[OrgNameIdx];
            destFilepath = Path.Combine(DestinationRootDirectoryPath, ogName, OutputSubdirectoryName, datasetFile);
            return true;
        }
    }
}
