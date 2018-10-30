using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MyEMSLReader;

namespace Mage
{
    /// <summary>
    /// This module looks up the file info for file paths passed into the base class (FileListInfoBase)
    /// </summary>
    public class FileListInfoLookup : FileListInfoBase
    {
        #region Member Variables

        private int mFileNameColIndex = -1;

        #endregion


        #region Properties

        /// <summary>
        /// The name of the input column that contains the filename
        /// </summary>
        public string SourceFileColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Mage file list filter module
        /// </summary>
        public FileListInfoLookup()
        {
        }

        #endregion


        #region Search Functions


        /// <summary>
        /// Set up indexes for row columns
        /// </summary>
        protected override void SetupSearch()
        {
            TryGetOutputColumnPos(SourceFileColumnName, out mFileNameColIndex);
        }

        /// <summary>
        /// Search for files in the given directory
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subdirectoryInfo">Dictionary of found directories (input/output parameter)</param>
        /// <param name="directoryPath">Directory path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        /// <remarks>This function only looks for the file in column mFileNameColIndex</remarks>
        protected override void SearchOneDirectory(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subdirectoryInfo,
            string directoryPath,
            string datasetName)
        {
            if (mFileNameColIndex < 0)
                return;

            var sourceFileName = mOutputBuffer[outputBufferRowIdx][mFileNameColIndex];
            GetFileInfo(outputBufferRowIdx, fileInfo, directoryPath, sourceFileName, datasetName);
        }

        #endregion

        #region Private Functions

        private void GetFileInfo(
            int outputBufferRowIdx,
            IDictionary<string, FileInfo> fileInfo,
            string directoryPath,
            string fileName,
            string datasetName)
        {

            try
            {
                FileInfo fiFile;

                if (directoryPath.StartsWith(MYEMSL_PATH_FLAG))
                    fiFile = GetFileInfoMyEMSL(directoryPath, fileName, datasetName);
                else
                    fiFile = GetFileInfoUNC(directoryPath, fileName);

                // Append new files in fileNames to fileInfo
                if (fiFile != null)
                {
                    if (!fileInfo.ContainsKey(fiFile.Name))
                        fileInfo.Add(fiFile.Name, fiFile);
                }

            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is System.Security.SecurityException || e is ArgumentException || e is PathTooLongException || e is DirectoryNotFoundException)
                {
                    var msg = e.Message;
                    ReportSearchErrorToOutput(outputBufferRowIdx, msg);
                }
                else if (e is IOException)
                {
                    throw new MageException("Process aborted:" + e.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        private FileInfo GetFileInfoUNC(string path, string fileName)
        {
            var fi = new FileInfo(Path.Combine(path, fileName));

            if (fi.Exists)
                return fi;
            else
                return null;
        }

        private FileInfo GetFileInfoMyEMSL(string directoryPath, string fileName, string datasetName)
        {

            GetMyEMSLParentDirectoriesAndSubDir(directoryPath, datasetName, out var subDir, out var parentDirectories);

            DatasetInfoBase.ExtractMyEMSLFileID(fileName, out var fileNameClean);

            m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(fileNameClean, subDir, datasetName, recurse: false);

            if (m_RecentlyFoundMyEMSLFiles.Count > 0)
            {
                var archiveFile = m_RecentlyFoundMyEMSLFiles.First();
                var encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentDirectories, archiveFile.FileInfo.RelativePathWindows), archiveFile.FileID);
                return new FileInfo(encodedFilePath);
            }

            return null;
        }

        #endregion

    }
}

