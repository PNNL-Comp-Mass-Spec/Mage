using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MyEMSLReader;

namespace Mage
{
    /// <summary>
    /// This modules accepts a list of files on standard tabular input
    /// looks for an associated file for each one and adds that file's
    /// name to standard output stream
    /// </summary>
    public class AddAssociatedFile : FileProcessingBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddAssociatedFile()
        {
            SourceDirectoryColumnName = "Directory";
            SourceFileColumnName = "Name";
        }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input directory path
        /// (optional - defaults to "Directory"; previously defaulted to "Folder")
        /// </summary>
        public string SourceDirectoryColumnName { get; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input directory path
        /// </summary>
        [Obsolete("Use SourceDirectoryColumnName")]
        public string SourceFolderColumnName => SourceDirectoryColumnName;

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input file name
        /// optional - defaults to "Name")
        /// </summary>
        public string SourceFileColumnName { get; }

        /// <summary>
        /// Name of the output column that receives name of associated file
        /// </summary>
        public string ColumnToReceiveAssocFileName { get; set; }

        /// <summary>
        /// Associated file name replacement pattern
        /// </summary>
        public string AssocFileNameReplacementPattern
        {
            get => string.Format("{0}|{1}", mSourceFileNameFragment, mAssociatedFileNameFragment);
            set
            {
                var fields = value.Split('|');
                mSourceFileNameFragment = fields[0];
                mAssociatedFileNameFragment = fields[1];
            }
        }

        private int mInputDirectoryIdx;
        private int mInputFileIdx;
        private int mAssocFileIdx;

        /// <summary>
        /// Source file name that we are matching
        /// </summary>
        /// <remarks>May have a series of suffixes, separated by semicolons</remarks>
        private string mSourceFileNameFragment = string.Empty;
        private string mAssociatedFileNameFragment = string.Empty;

        /// <summary>
        /// Handle the column defs defined by args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            var columnDefs = OutputColumnDefs ?? InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));

            mInputDirectoryIdx = InputColumnPos[SourceDirectoryColumnName];
            mInputFileIdx = InputColumnPos[SourceFileColumnName];
            mAssocFileIdx = InputColumnPos[ColumnToReceiveAssocFileName];
        }

        /// <summary>
        /// Handle the data row defined by args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                var directoryPathSpec = args.Fields[mInputDirectoryIdx];
                var resultFileName = args.Fields[mInputFileIdx];

                var assocFileName = string.Empty;
                var sourceFileNameParts = mSourceFileNameFragment.Split(';');
                foreach (var sourceFileNamePart in sourceFileNameParts)
                {
                    if (resultFileName.IndexOf(sourceFileNamePart, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        assocFileName = ReplaceEx(resultFileName, sourceFileNamePart, mAssociatedFileNameFragment);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(assocFileName))
                {
                    // Match not found; this is unexpected
                    assocFileName = ReplaceEx(resultFileName, mSourceFileNameFragment, mAssociatedFileNameFragment);
                }

                if (resultFileName.Contains("_msgfdb") && assocFileName.Contains("_msgfplus"))
                {
                    // Auto-switch from _msgfplus to _msgfdb
                    assocFileName = assocFileName.Replace("_msgfplus", "_msgfdb");
                }

                if (assocFileName == kNoFilesFound)
                {
                    args.Fields[mAssocFileIdx] = string.Empty;
                }
                else
                {
                    // directoryPathSpec may contain multiple directories, separated by a vertical bar
                    // If that is the case, then we'll search for files in each directory, preferentially using files in the directory listed first
                    var directoryPaths = new List<string>();
                    if (directoryPathSpec.Contains('|'))
                    {
                        directoryPaths = directoryPathSpec.Split('|').ToList();
                    }
                    else
                    {
                        directoryPaths.Add(directoryPathSpec);
                    }

                    foreach (var resultsDirectoryPath in directoryPaths)
                    {
                        if (resultsDirectoryPath.StartsWith(MYEMSL_PATH_FLAG))
                        {
                            var datasetName = DetermineDatasetName(resultsDirectoryPath);

                            GetMyEMSLParentDirectoriesAndSubDir(resultsDirectoryPath, datasetName, out var subDir, out _);

                            DatasetInfoBase.ExtractMyEMSLFileID(assocFileName, out var assocFileNameClean);

                            m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(assocFileNameClean, subDir, datasetName, recurse: false);
                            if (m_RecentlyFoundMyEMSLFiles.Count > 0)
                            {
                                var archiveFileInfo = m_RecentlyFoundMyEMSLFiles[0];
                                var encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(archiveFileInfo.FileInfo.Filename, archiveFileInfo.FileID);
                                args.Fields[mAssocFileIdx] = encodedFilePath;

                                CacheFilterPassingFile(archiveFileInfo.FileInfo);
                            }
                        }
                        else if (File.Exists(Path.Combine(resultsDirectoryPath, assocFileName)))
                        {
                            args.Fields[mAssocFileIdx] = assocFileName;
                            break;
                        }
                        else
                        {
                            args.Fields[mAssocFileIdx] = string.Empty;
                        }
                    }
                }

                OnDataRowAvailable(args);
            }
        }
    }
}
