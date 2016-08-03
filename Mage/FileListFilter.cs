using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using MyEMSLReader;

namespace Mage
{

    /// <summary>
    /// This module searches a list of folder paths for files and compares the file names
    /// against a set of file selection criteria and accumulates an internal list of files that pass,
    /// and outputs the selected files (and their folder path) via standard tablular output
    /// </summary>
    public class FileListFilter : FileListInfoBase
    {

        private enum FolderSearchMode
        {
            Files,
            Folders
        };

        #region Member Variables

        private bool mIncludeFiles;
        private bool mIncludeFolders;
        private bool mRecurseMyEMSL;

        private readonly List<string[]> mSearchSubfolders = new List<string[]>();

        private readonly SortedSet<string> mAccessDeniedSubfolders = new SortedSet<string>();

        #endregion

        #region "Constants"

        #endregion

        #region Properties

        /// <summary>
        /// List of folder paths to which the user did not have access
        /// </summary>
        public SortedSet<string> AccessDeniedSubfolders
        {
            get
            {
                return mAccessDeniedSubfolders;
            }
        }

        /// <summary>
        /// semi-colon delimited list of file matching patterns
        /// </summary>
        public string FileNameSelector { get; set; }

        /// <summary>
        /// how to use the file matching patterns ("FileSearch" or "RegEx")
        /// </summary>
        public string FileSelectorMode { get; set; }

        /// <summary>
        /// include files an/or folders in results
        /// ("File", "Folder", "IncludeFilesOrFolders")
        /// </summary>
        public string IncludeFilesOrFolders { get; set; }

        /// <summary>
        /// setting this property sets the file path to the internal file path buffer
        /// (necessary if Run will be called instead of processing via standard tabular input)
        /// </summary>
        public string FolderPath
        {
            get
            {
                return (mOutputBuffer.Count > 0) ? mOutputBuffer[0][2] : "";
            }
            set
            {
                mOutputBuffer.Clear();
                AddFolderPath(value);
            }
        }

        /// <summary>
        /// add a path to a folder to be searched
        /// (used when this module's "Run" method is to be called
        /// such as when it is installed as the root module in a pipeline)
        /// </summary>
        /// <param name="path"></param>
        public void AddFolderPath(string path)
        {
            var bufferUpdated = false;
            if (!string.IsNullOrEmpty(OutputColumnList))
            {
                var outputColumnDefs = OutputColumnList.Split(',').ToList();

                if (outputColumnDefs.Count > 0)
                {
                    var columnHeaders = new string[outputColumnDefs.Count];

                    for (var colIndex = 0; colIndex < outputColumnDefs.Count; colIndex++)
                    {
                        // break each column spec into fields
                        var colSpecFlds = outputColumnDefs[colIndex].Trim().Split('|');
                        var outputColName = colSpecFlds[0].Trim();

                        if (outputColName == SourceFolderColumnName)
                            columnHeaders[colIndex] = path;
                        else
                            columnHeaders[colIndex] = "";

                    }

                    mOutputBuffer.Add(columnHeaders);
                    bufferUpdated = true;
                }
            }

            if (!bufferUpdated)
                mOutputBuffer.Add(new string[] { "", "", "", "", path });  // Note: needs to have the same number of columns as OutputColumnList

        }

        /// <summary>
        /// do recursive file search
        /// </summary>
        public string RecursiveSearch { get; set; }

        /// <summary>
        /// folder name pattern used to restrict recursive search
        /// </summary>
        public string SubfolderSearchName { get; set; }


        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage file list filter module
        /// </summary>
        public FileListFilter()
        {
            FileSelectorMode = "FileSearch";
            IncludeFilesOrFolders = "File";
            RecursiveSearch = "No";
        }

        #endregion

        #region Search Functions

        /// <summary>
        /// Set up controls for scope of search
        /// </summary>
        protected override void SetupSearch()
        {

            mIncludeFiles = IncludeFilesOrFolders.Contains("File");
            mIncludeFolders = IncludeFilesOrFolders.Contains("Folder");

            mRecurseMyEMSL = false;

            // add subfolders if we are doing a recursive search
            if (RecursiveSearch == "Yes")
            {
                mRecurseMyEMSL = true;
                if (string.IsNullOrEmpty(SubfolderSearchName))
                {
                    SubfolderSearchName = "*";
                }
                foreach (var fields in mOutputBuffer)
                {
                    AddSearchSubfolders(fields);
                }
                mOutputBuffer.AddRange(mSearchSubfolders);
            }

        }

        /// <summary>
        /// Seach for files in the given folder
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subfolderInfo">Dictionary of found folders (input/output parameter)</param>
        /// <param name="folderPath">Folder path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        protected override void SearchOneFolder(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subfolderInfo,
            string folderPath,
            string datasetName)
        {
            SearchFolders(outputBufferRowIdx, fileInfo, subfolderInfo, folderPath, datasetName);
        }

        #endregion

        #region Private Functions


        private void SearchFolders(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subfolderInfo,
            string folderPath,
            string datasetName)
        {

            var foundFiles = new List<FileSystemInfo>();
            var foundSubFolders = new List<FileSystemInfo>();
            try
            {
                if (FileSelectorMode == "RegEx")
                {
                    if (mIncludeFiles)
                    {
                        if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundFiles = GetFileOrFolderNamesFromFolderByRegExMyEMSL(folderPath, FolderSearchMode.Files, datasetName);
                        else
                            foundFiles = GetFileOrFolderNamesFromFolderByRegEx(folderPath, FolderSearchMode.Files);
                    }
                    if (mIncludeFolders)
                    {
                        if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundSubFolders = GetFileOrFolderNamesFromFolderByRegExMyEMSL(folderPath, FolderSearchMode.Folders, datasetName);
                        else
                            foundSubFolders = GetFileOrFolderNamesFromFolderByRegEx(folderPath, FolderSearchMode.Folders);
                    }
                }
                else
                {
                    if (mIncludeFiles)
                    {
                        if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundFiles = GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(folderPath, FolderSearchMode.Files, datasetName);
                        else
                            foundFiles = GetFileOrFolderNamesFromFolderBySearchPattern(folderPath, FolderSearchMode.Files);
                    }
                    if (mIncludeFolders)
                    {
                        if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundSubFolders = GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(folderPath, FolderSearchMode.Folders, datasetName);
                        else
                            foundSubFolders = GetFileOrFolderNamesFromFolderBySearchPattern(folderPath, FolderSearchMode.Folders);
                    }
                }

                // Append new files in fileNames to fileInfo
                if (!(foundFiles == null || foundFiles.Count == 0))
                {
                    foreach (var entry in foundFiles)
                    {
                        var fileEntry = entry as FileInfo;
                        if (fileEntry != null && !fileInfo.ContainsKey(fileEntry.Name))
                            fileInfo.Add(fileEntry.Name, fileEntry);
                    }
                }

                // Append new subFolders in fileNames to subfolderInfo
                if (!(foundSubFolders == null || foundSubFolders.Count == 0))
                {
                    foreach (var entry in foundSubFolders)
                    {
                        var subfolderEntry = entry as DirectoryInfo;
                        if (subfolderEntry != null && !subfolderInfo.ContainsKey(subfolderEntry.Name))
                            subfolderInfo.Add(subfolderEntry.Name, subfolderEntry);
                    }
                }

            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is System.Security.SecurityException || e is ArgumentException || e is PathTooLongException || e is DirectoryNotFoundException || e is UnauthorizedAccessException)
                {
                    var msg = e.Message;

                    var pathExtractor = new Regex("Access to the path '(.+)' is denied", RegexOptions.IgnoreCase);
                    var pathMatch = pathExtractor.Match(e.Message);

                    if (pathMatch.Success)
                    {
                        var currentFolder = pathMatch.Groups[1].Value;
                        if (!mAccessDeniedSubfolders.Contains(currentFolder))
                        {
                            mAccessDeniedSubfolders.Add(currentFolder);
                            OnWarningMessage(new MageStatusEventArgs(e.Message));
                        }
                    }
                    else
                    {
                        OnWarningMessage(new MageStatusEventArgs(e.Message));
                    }
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

        /// <summary>
        /// add subdirectories to search list (used in recursive search mode)
        /// </summary>
        /// <param name="fields"></param>
        private void AddSearchSubfolders(string[] fields)
        {
            var path = fields[mFolderPathColIndx];
            if (path.StartsWith(MYEMSL_PATH_FLAG))
                return;

            var currentFolder = string.Copy(path);

            try
            {
                var di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    return;
                }

                if (!string.Equals(SubfolderSearchName, "*"))
                    currentFolder = path + @"\" + SubfolderSearchName;

                foreach (var sfDi in di.GetDirectories(SubfolderSearchName))
                {
                    currentFolder = string.Copy(sfDi.FullName);

                    var subfolderRow = (string[])fields.Clone();
                    var subfolderPath = Path.Combine(path, sfDi.Name);
                    subfolderRow[mFolderPathColIndx] = subfolderPath;
                    mSearchSubfolders.Add(subfolderRow);
                    AddSearchSubfolders(subfolderRow);
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (!mAccessDeniedSubfolders.Contains(currentFolder))
                {
                    mAccessDeniedSubfolders.Add(currentFolder);
                    OnWarningMessage(new MageStatusEventArgs(@"Access to the path '" + currentFolder + "' is denied."));
                }
            }

        }


        /// <summary>
        /// Get list of files from given directory using file selector list
        /// as file search patterns
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchMode"></param>
        /// <returns>List of file names</returns>
        private List<FileSystemInfo> GetFileOrFolderNamesFromFolderBySearchPattern(string path, FolderSearchMode searchMode)
        {
            var filteredFilesOrFolders = new Dictionary<string, FileSystemInfo>();

            var selectors = GetFileNameSelectors();

            var di = new DirectoryInfo(path);

            if (selectors.Count == 0)
            {
                // Get all files/subfolders in folder
                selectors.Add("*");
            }

            // get list of files for each selector
            foreach (var selector in selectors)
            {
                if (searchMode == FolderSearchMode.Files)
                {
                    foreach (var entry in di.GetFiles(selector))
                    {
                        filteredFilesOrFolders[entry.Name] = entry;
                    }
                }

                if (searchMode == FolderSearchMode.Folders)
                {
                    foreach (var entry in di.GetDirectories(selector))
                    {
                        filteredFilesOrFolders[entry.Name] = entry;
                    }
                }

            }

            // We used the dictionary keys for our file names to eliminate duplicates
            // Convert the values to a list of file system infos and return the list
            return filteredFilesOrFolders.Values.ToList();

        }

        private List<FileSystemInfo> GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(string folderPath, FolderSearchMode searchMode, string datasetName)
        {
            string subDir;
            string parentFolders;
            GetMyEMSLParentFoldersAndSubDir(folderPath, datasetName, out subDir, out parentFolders);

            var filteredFilesOrFolders = new Dictionary<string, FileSystemInfo>();

            var selectors = GetFileNameSelectors();

            if (selectors.Count == 0)
            {
                // Get all files/subfolders in folder
                selectors.Add("*");
            }

            // get list of files for each selector
            foreach (var selector in selectors)
            {
                var fiList = GetMyEMSLFilesOrFolders(searchMode, selector, datasetName, subDir, parentFolders);

                foreach (var entry in fiList)
                {
                    filteredFilesOrFolders[entry.Name] = entry;
                }
            }

            // We used the dictionary keys for our file names to eliminate duplicates
            // Convert the values to a list of file system infos and return the list
            return filteredFilesOrFolders.Values.ToList();

        }
        /// <summary>
        /// Get list of files from given directory using file selector list
        /// as RegEx patterns
        /// </summary>
        /// <param name="path">Folder path to get file from</param>
        /// <param name="searchMode"></param>
        /// <returns>List of file names</returns>
        private List<FileSystemInfo> GetFileOrFolderNamesFromFolderByRegEx(string path, FolderSearchMode searchMode)
        {
            var di = new DirectoryInfo(path);

            var fiList = new List<FileSystemInfo>();
            if (searchMode == FolderSearchMode.Files)
            {
                fiList.AddRange(di.GetFiles().ToList());
            }

            if (searchMode == FolderSearchMode.Folders)
            {
                fiList.AddRange(di.GetDirectories().ToList());
            }

            var fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

            return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
        }

        private List<FileSystemInfo> GetFileOrFolderNamesFromFolderByRegExMyEMSL(string folderPath, FolderSearchMode searchMode, string datasetName)
        {
            string subDir;
            string parentFolders;
            GetMyEMSLParentFoldersAndSubDir(folderPath, datasetName, out subDir, out parentFolders);

            const string fileSelector = "*";
            var fiList = GetMyEMSLFilesOrFolders(searchMode, fileSelector, datasetName, subDir, parentFolders);

            var fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

            return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
        }

        private List<FileSystemInfo> GetMyEMSLFilesOrFolders(
            FolderSearchMode searchMode,
            string fileSelector,
            string datasetName,
            string subDir,
            string parentFolders)
        {

            var fiList = new List<FileSystemInfo>();
            if (searchMode == FolderSearchMode.Files)
            {
                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(fileSelector, subDir, datasetName, mRecurseMyEMSL);
                foreach (var archiveFile in m_RecentlyFoundMyEMSLFiles)
                {
                    var encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentFolders, archiveFile.FileInfo.RelativePathWindows), archiveFile.FileID);
                    fiList.Add(new FileInfo(encodedFilePath));
                }
            }

            if (searchMode == FolderSearchMode.Folders)
            {
                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFolders(fileSelector, datasetName);

                foreach (var archiveFolder in m_RecentlyFoundMyEMSLFiles)
                {
                    fiList.Add(new DirectoryInfo(Path.Combine(parentFolders, archiveFolder.FileInfo.RelativePathWindows)));
                }
            }
            return fiList;
        }

        /// <summary>
        /// search files in folder and return list of files 
        /// whose names satisfy the selection criteria
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="fileNameRegExSpecs"></param>
        /// <returns></returns>
        private static List<FileSystemInfo> FilterFileNamesFromList(List<FileSystemInfo> fileList, List<Regex> fileNameRegExSpecs)
        {

            var filteredFilesOrFolders = new List<FileSystemInfo>(fileList.Count);

            // find files (or folders) that meet selection criteria.
            foreach (var fiEntry in fileList)
            {
                if (fileNameRegExSpecs.Count == 0)
                {
                    filteredFilesOrFolders.Add(fiEntry);
                }
                else
                {
                    foreach (var rx in fileNameRegExSpecs)
                    {
                        var m = rx.Match(fiEntry.Name);
                        if (m.Success)
                        {
                            filteredFilesOrFolders.Add(fiEntry);
                            break;
                        }
                    }
                }
            }
            return filteredFilesOrFolders;
        }

        /// <summary>
        /// Make list of regex objects from list of file selectors
        /// </summary>
        /// <returns></returns>
        private static List<Regex> GetRegexFileSelectors(IEnumerable<string> selectors)
        {
            var fileNameSpecs = new List<Regex>();
            foreach (var selector in selectors)
            {
                try
                {
                    var rx = new Regex(selector.Trim(), RegexOptions.IgnoreCase);
                    fileNameSpecs.Add(rx);
                }
                catch (Exception e)
                {
                    traceLog.Error(e.Message);
                    throw new MageException("Problem with file selector:" + e.Message);
                }
            }
            return fileNameSpecs;
        }

        /// <summary>
        /// get list of individual file selectors from selector list
        /// </summary>
        /// <returns></returns>
        private List<string> GetFileNameSelectors()
        {
            var selectorList = new List<string>();
            foreach (var selector in FileNameSelector.Split(';'))
            {
                selectorList.Add(selector.Trim());
            }
            return selectorList;
        }


        #endregion


    }
}
