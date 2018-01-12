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

        private int mFileNameColIndx = -1;

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
            TryGetOutputColumnPos(SourceFileColumnName, out mFileNameColIndx);
        }

        /// <summary>
        /// Seach for files in the given folder
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subfolderInfo">Dictionary of found folders (input/output parameter)</param>
        /// <param name="folderPath">Folder path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        /// <remarks>This function only looks for the file in column mFileNameColIndx</remarks>
        protected override void SearchOneFolder(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subfolderInfo,
            string folderPath,
            string datasetName)
        {
            if (mFileNameColIndx < 0)
                return;

            var sourceFileName = mOutputBuffer[outputBufferRowIdx][mFileNameColIndx];
            GetFileInfo(outputBufferRowIdx, fileInfo, folderPath, sourceFileName, datasetName);
        }

        #endregion

        #region Private Functions

        private void GetFileInfo(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            string folderPath,
            string fileName,
            string datasetName)
        {

            try
            {
                FileInfo fiFile;

                if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
                    fiFile = GetFileInfoMyEMSL(folderPath, fileName, datasetName);
                else
                    fiFile = GetFileInfoUNC(folderPath, fileName);

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

        private FileInfo GetFileInfoMyEMSL(string folderPath, string fileName, string datasetName)
        {
            string subDir;
            string parentFolders;

            GetMyEMSLParentFoldersAndSubDir(folderPath, datasetName, out subDir, out parentFolders);

            string fileNameClean;
            DatasetInfoBase.ExtractMyEMSLFileID(fileName, out fileNameClean);

            m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(fileNameClean, subDir, datasetName, recurse: false);

            if (m_RecentlyFoundMyEMSLFiles.Count > 0)
            {
                var archiveFile = m_RecentlyFoundMyEMSLFiles.First();
                var encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentFolders, archiveFile.FileInfo.RelativePathWindows), archiveFile.FileID);
                return new FileInfo(encodedFilePath);
            }

            return null;
        }

        #endregion

    }
}

