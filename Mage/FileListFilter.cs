﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using MyEMSLReader;

// ReSharper disable UnusedMember.Global

namespace Mage
{
    /// <summary>
    /// This module searches a list of directory paths for files and compares the file names
    /// against a set of file selection criteria and accumulates an internal list of files that pass,
    /// and outputs the selected files (and their directory path) via standard tabular output
    /// </summary>
    public class FileListFilter : FileListInfoBase
    {
        // Ignore Spelling: Mage, Regex

        /// <summary>
        /// Normal file search (glob-based)
        /// </summary>
        public const string FILE_SELECTOR_NORMAL = "FileSearch";

        /// <summary>
        /// Regex-based file search
        /// </summary>
        public const string FILE_SELECTOR_REGEX = "RegEx";

        private enum DirectorySearchMode
        {
            Files,
            Directories
        };

        private bool mIncludeFiles;
        private bool mIncludeDirectories;
        private bool mRecurseMyEMSL;

        /// <summary>
        /// List of subdirectories to search when RecursiveSearch is enabled ("Search in subdirectories")
        /// </summary>
        private readonly List<string[]> mSearchSubdirectories = new();

        /// <summary>
        /// List of directory paths to which the user did not have access
        /// </summary>
        public SortedSet<string> AccessDeniedSubdirectories { get; } = new();

        /// <summary>
        /// Semicolon delimited list of file matching patterns
        /// </summary>
        public string FileNameSelector { get; set; }

        /// <summary>
        /// File matching pattern type: "FileSearch" or "RegEx"
        /// </summary>
        public string FileSelectorMode { get; set; }

        /// <summary>
        /// Include files and/or directories in results
        /// Allowed values: "File", "Directory", "IncludeFilesOrDirectories" (previously "Folder" and "IncludeFilesOrFolders")
        /// </summary>
        public string IncludeFilesOrDirectories { get; set; }

        /// <summary>
        /// Setting this property sets the file path to the internal file path buffer
        /// (necessary if Run will be called instead of processing via standard tabular input)
        /// </summary>
        public string DirectoryPath
        {
            get => (mOutputBuffer.Count > 0) ? mOutputBuffer[0][2] : "";
            set
            {
                mOutputBuffer.Clear();
                AddDirectoryPath(value);
            }
        }

        /// <summary>
        /// Add a path to a directory to be searched
        /// (used when this module's "Run" method is to be called
        /// such as when it is installed as the root module in a pipeline)
        /// </summary>
        /// <param name="path"></param>
        public void AddDirectoryPath(string path)
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
                        // Break each column spec into searchResult
                        var colSpecFields = outputColumnDefs[colIndex].Trim().Split('|');
                        var outputColName = colSpecFields[0].Trim();

                        if (outputColName == SourceDirectoryColumnName)
                            columnHeaders[colIndex] = path;
                        else
                            columnHeaders[colIndex] = string.Empty;
                    }

                    mOutputBuffer.Add(columnHeaders);
                    bufferUpdated = true;
                }
            }

            if (!bufferUpdated)
            {
                // Note: needs to have the same number of columns as OutputColumnList
                mOutputBuffer.Add(new[] { string.Empty, string.Empty, string.Empty, string.Empty, path });
            }
        }

        /// <summary>
        /// Do recursive file search
        /// </summary>
        public string RecursiveSearch { get; set; }

        /// <summary>
        /// Directory name pattern used to restrict recursive search
        /// </summary>
        public string SubdirectorySearchName { get; set; }

        /// <summary>
        /// Construct a new Mage file list filter module
        /// </summary>
        public FileListFilter()
        {
            FileSelectorMode = "FileSearch";
            IncludeFilesOrDirectories = "File";
            RecursiveSearch = "No";
        }

        // Search Methods

        /// <summary>
        /// Set up controls for scope of search
        /// </summary>
        protected override void SetupSearch()
        {
            mIncludeFiles = IncludeFilesOrDirectories.Contains("File");

            if (IncludeFilesOrDirectories.Contains("Directory") || IncludeFilesOrDirectories.Contains("Directories"))
                mIncludeDirectories = true;
            else if (IncludeFilesOrDirectories.Contains("Folder"))
                mIncludeDirectories = true;
            else
                mIncludeDirectories = false;

            var filterSpec = string.Empty;
            var selectors = GetFileNameSelectors();

            if (selectors.Count > 0)
            {
                filterSpec = selectors[0];
            }

            // Update directory paths if the File Name Selector starts with ..\
            foreach (var searchResult in mOutputBuffer)
            {
                var path = searchResult[mDirectoryPathColIndex];
                if (path.StartsWith(MYEMSL_PATH_FLAG))
                {
                    // MyEMSL does not support relative file name filters
                    continue;
                }

                var directoryPathToSearch = HandleRelativePathFilter(path, filterSpec);

                if (!string.Equals(directoryPathToSearch, searchResult[mDirectoryPathColIndex]))
                {
                    // Update the stored path
                    searchResult[mDirectoryPathColIndex] = directoryPathToSearch;
                }
            }

            mRecurseMyEMSL = false;

            if (!OptionEnabled(RecursiveSearch))
            {
                return;
            }

            // Recursive search: add subdirectories
            mRecurseMyEMSL = true;
            if (string.IsNullOrEmpty(SubdirectorySearchName))
            {
                SubdirectorySearchName = "*";
            }

            foreach (var searchResult in mOutputBuffer)
            {
                var path = searchResult[mDirectoryPathColIndex];
                if (path.StartsWith(MYEMSL_PATH_FLAG))
                {
                    // Recursive searching is handled differently for MyEMSL
                    continue;
                }

                AddSearchSubdirectories(searchResult);
            }
            mOutputBuffer.AddRange(mSearchSubdirectories);
        }

        /// <summary>
        /// Search for files in the given directory
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subdirectoryInfo">Dictionary of found directories (input/output parameter)</param>
        /// <param name="directoryPath">Directory path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        protected override void SearchOneDirectory(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subdirectoryInfo,
            string directoryPath,
            string datasetName)
        {
            SearchDirectories(outputBufferRowIdx, fileInfo, subdirectoryInfo, directoryPath, datasetName);
        }

        private void SearchDirectories(
            int outputBufferRowIdx,
            IDictionary<string, FileInfo> fileInfo,
            IDictionary<string, DirectoryInfo> subdirectoryInfo,
            string directoryPath,
            string datasetName)
        {
            var foundFiles = new List<FileSystemInfo>();
            var foundSubdirectories = new List<FileSystemInfo>();

            try
            {
                if (FileSelectorMode == FILE_SELECTOR_REGEX)
                {
                    if (mIncludeFiles)
                    {
                        if (directoryPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundFiles = GetFileOrDirNamesFromDirectoryByRegExMyEMSL(directoryPath, DirectorySearchMode.Files, datasetName);
                        else
                            foundFiles = GetFileOrDirNamesFromDirectoryByRegEx(directoryPath, DirectorySearchMode.Files);
                    }
                    if (mIncludeDirectories)
                    {
                        if (directoryPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundSubdirectories = GetFileOrDirNamesFromDirectoryByRegExMyEMSL(directoryPath, DirectorySearchMode.Directories, datasetName);
                        else
                            foundSubdirectories = GetFileOrDirNamesFromDirectoryByRegEx(directoryPath, DirectorySearchMode.Directories);
                    }
                }
                else
                {
                    if (mIncludeFiles)
                    {
                        if (directoryPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundFiles = GetFileOrDirNamesFromDirectoryBySearchPatternMyEMSL(directoryPath, DirectorySearchMode.Files, datasetName);
                        else
                            foundFiles = GetFileOrDirNamesFromDirectoryBySearchPattern(directoryPath, DirectorySearchMode.Files);
                    }
                    if (mIncludeDirectories)
                    {
                        if (directoryPath.StartsWith(MYEMSL_PATH_FLAG))
                            foundSubdirectories = GetFileOrDirNamesFromDirectoryBySearchPatternMyEMSL(directoryPath, DirectorySearchMode.Directories, datasetName);
                        else
                            foundSubdirectories = GetFileOrDirNamesFromDirectoryBySearchPattern(directoryPath, DirectorySearchMode.Directories);
                    }
                }

                // Append new files in fileNames to fileInfo
                if (!(foundFiles == null || foundFiles.Count == 0))
                {
                    foreach (var entry in foundFiles)
                    {
                        if (entry is FileInfo fileEntry && !fileInfo.ContainsKey(fileEntry.Name))
                            fileInfo.Add(fileEntry.Name, fileEntry);
                    }
                }

                // Append new subdirectories in fileNames to subdirectoryInfo
                if (!(foundSubdirectories == null || foundSubdirectories.Count == 0))
                {
                    foreach (var entry in foundSubdirectories)
                    {
                        if (entry is DirectoryInfo subdirectoryEntry && !subdirectoryInfo.ContainsKey(subdirectoryEntry.Name))
                            subdirectoryInfo.Add(subdirectoryEntry.Name, subdirectoryEntry);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException or System.Security.SecurityException or ArgumentException or PathTooLongException or DirectoryNotFoundException or UnauthorizedAccessException)
                {
                    var msg = e.Message;

                    var pathExtractor = new Regex("Access to the path '(.+)' is denied", RegexOptions.IgnoreCase);
                    var pathMatch = pathExtractor.Match(e.Message);

                    if (pathMatch.Success)
                    {
                        var currentDirectory = pathMatch.Groups[1].Value;
                        if (!AccessDeniedSubdirectories.Contains(currentDirectory))
                        {
                            AccessDeniedSubdirectories.Add(currentDirectory);
                            ReportMageWarning(e.Message);
                        }
                    }
                    else
                    {
                        ReportMageWarning(e.Message);
                    }
                    ReportSearchErrorToOutput(outputBufferRowIdx, msg);
                }
                else if (e is IOException)
                {
                    var errorMessage = "Process aborted: " + e.Message;
                    var ex = ReportMageException(errorMessage, e);
                    throw ex;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Add subdirectories to search list (used in recursive search mode)
        /// </summary>
        /// <param name="searchResult">Search result row. Column at index mDirectoryPathColIndex will have a directory path </param>
        private void AddSearchSubdirectories(string[] searchResult)
        {
            var path = searchResult[mDirectoryPathColIndex];
            if (path.StartsWith(MYEMSL_PATH_FLAG))
                return;

            var currentDirectoryPath = string.Copy(path);

            try
            {
                var currentDirectory = new DirectoryInfo(path);
                if (!currentDirectory.Exists)
                {
                    return;
                }

                if (!string.Equals(SubdirectorySearchName, "*"))
                {
                    // Update currentDirectory so it can be used in the Catch block if the user encounters access denied
                    currentDirectoryPath = Path.Combine(path, SubdirectorySearchName);
                }

                foreach (var subdirectory in currentDirectory.GetDirectories(SubdirectorySearchName))
                {
                    currentDirectoryPath = string.Copy(subdirectory.FullName);

                    var subdirectoryRow = (string[])searchResult.Clone();
                    var subdirectoryPath = Path.Combine(path, subdirectory.Name);
                    subdirectoryRow[mDirectoryPathColIndex] = subdirectoryPath;
                    mSearchSubdirectories.Add(subdirectoryRow);
                    AddSearchSubdirectories(subdirectoryRow);
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (!AccessDeniedSubdirectories.Contains(currentDirectoryPath))
                {
                    AccessDeniedSubdirectories.Add(currentDirectoryPath);
                    ReportMageWarning("Access to the path '" + currentDirectoryPath + "' is denied.");
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
        private List<FileSystemInfo> GetFileOrDirNamesFromDirectoryBySearchPattern(string path, DirectorySearchMode searchMode)
        {
            var filteredFilesOrDirectories = new Dictionary<string, FileSystemInfo>();

            var selectors = GetFileNameSelectors();

            var di = new DirectoryInfo(path);

            if (selectors.Count == 0)
            {
                // Get all files/subdirectories in directory
                selectors.Add("*");
            }

            // Get list of files for each selector
            foreach (var selector in selectors)
            {
                string updatedSelector;

                if (FileSelectorMode == FILE_SELECTOR_REGEX)
                {
                    // Regex search mode; use the selector as-is since relative paths are not supported for RegEx mode
                    updatedSelector = selector;
                }
                else
                {
                    // Remove any relative path specifiers from the selector
                    // For example, update  ..\*.mzml  to be  *.mzml
                    updatedSelector = ScrubRelativePathText(selector);
                }

                if (searchMode == DirectorySearchMode.Files)
                {
                    foreach (var entry in di.GetFiles(updatedSelector))
                    {
                        filteredFilesOrDirectories[entry.Name] = entry;
                    }
                }

                if (searchMode == DirectorySearchMode.Directories)
                {
                    foreach (var entry in di.GetDirectories(updatedSelector))
                    {
                        filteredFilesOrDirectories[entry.Name] = entry;
                    }
                }
            }

            // We used the dictionary keys for our file names to eliminate duplicates
            // Convert the values to a list of file system infos and return the list
            return filteredFilesOrDirectories.Values.ToList();
        }

        private List<FileSystemInfo> GetFileOrDirNamesFromDirectoryBySearchPatternMyEMSL(string directoryPath, DirectorySearchMode searchMode, string datasetName)
        {
            GetMyEMSLParentDirectoriesAndSubDir(directoryPath, datasetName, out var subDir, out var parentDirectories);

            var filteredFilesOrDirectories = new Dictionary<string, FileSystemInfo>();

            var selectors = GetFileNameSelectors();

            if (selectors.Count == 0)
            {
                // Get all files/subdirectories in directory
                selectors.Add("*");
            }

            // Get list of files for each selector
            foreach (var selector in selectors)
            {
                var fiList = GetMyEMSLFilesOrDirectories(searchMode, selector, datasetName, subDir, parentDirectories);

                foreach (var entry in fiList)
                {
                    filteredFilesOrDirectories[entry.Name] = entry;
                }
            }

            // We used the dictionary keys for our file names to eliminate duplicates
            // Convert the values to a list of file system infos and return the list
            return filteredFilesOrDirectories.Values.ToList();
        }

        /// <summary>
        /// Get list of files from given directory using file selector list
        /// as RegEx patterns
        /// </summary>
        /// <param name="path">Directory path to get file from</param>
        /// <param name="searchMode"></param>
        /// <returns>List of file names</returns>
        private List<FileSystemInfo> GetFileOrDirNamesFromDirectoryByRegEx(string path, DirectorySearchMode searchMode)
        {
            var di = new DirectoryInfo(path);

            var fiList = new List<FileSystemInfo>();
            if (searchMode == DirectorySearchMode.Files)
            {
                fiList.AddRange(di.GetFiles().ToList());
            }

            if (searchMode == DirectorySearchMode.Directories)
            {
                fiList.AddRange(di.GetDirectories().ToList());
            }

            var fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

            return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
        }

        private List<FileSystemInfo> GetFileOrDirNamesFromDirectoryByRegExMyEMSL(string directoryPath, DirectorySearchMode searchMode, string datasetName)
        {
            GetMyEMSLParentDirectoriesAndSubDir(directoryPath, datasetName, out var subDir, out var parentDirectories);

            const string fileDialog = "*";
            var fiList = GetMyEMSLFilesOrDirectories(searchMode, fileDialog, datasetName, subDir, parentDirectories);

            var fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

            return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
        }

        private List<FileSystemInfo> GetMyEMSLFilesOrDirectories(
            DirectorySearchMode searchMode,
            string fileDialog,
            string datasetName,
            string subDir,
            string parentDirectories)
        {
            var fiList = new List<FileSystemInfo>();
            var fileIDList = string.Empty;

            if (searchMode == DirectorySearchMode.Files)
            {
                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(fileDialog, subDir, datasetName, fileIDList, mRecurseMyEMSL);

                foreach (var archiveFile in m_RecentlyFoundMyEMSLFiles)
                {
                    var encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentDirectories, archiveFile.FileInfo.RelativePathWindows), archiveFile.FileID);
                    fiList.Add(new FileInfo(encodedFilePath));
                }
            }

            if (searchMode == DirectorySearchMode.Directories)
            {
                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindDirectories(fileDialog, datasetName);

                foreach (var archiveDirectory in m_RecentlyFoundMyEMSLFiles)
                {
                    fiList.Add(new DirectoryInfo(Path.Combine(parentDirectories, archiveDirectory.FileInfo.RelativePathWindows)));
                }
            }
            return fiList;
        }

        /// <summary>
        /// Search files in directory and return list of files
        /// whose names satisfy the selection criteria
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="fileNameRegExSpecs"></param>
        private static List<FileSystemInfo> FilterFileNamesFromList(IReadOnlyCollection<FileSystemInfo> fileList, IReadOnlyCollection<Regex> fileNameRegExSpecs)
        {
            var filteredFilesOrDirectories = new List<FileSystemInfo>(fileList.Count);

            // Find files (or directories) that meet selection criteria.
            foreach (var fiEntry in fileList)
            {
                if (fileNameRegExSpecs.Count == 0)
                {
                    filteredFilesOrDirectories.Add(fiEntry);
                }
                else
                {
                    foreach (var rx in fileNameRegExSpecs)
                    {
                        var m = rx.Match(fiEntry.Name);
                        if (m.Success)
                        {
                            filteredFilesOrDirectories.Add(fiEntry);
                            break;
                        }
                    }
                }
            }
            return filteredFilesOrDirectories;
        }

        /// <summary>
        /// Make list of regex objects from list of file selectors
        /// </summary>
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
                    traceLogFileList.Error(e.Message);
                    throw new MageException("Problem with file selector: " + e.Message);
                }
            }
            return fileNameSpecs;
        }

        /// <summary>
        /// Get list of individual file selectors from selector list
        /// </summary>
        public List<string> GetFileNameSelectors()
        {
            var selectorList = new List<string>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var selector in FileNameSelector.Split(';'))
            {
                selectorList.Add(selector.Trim());
            }

            return selectorList;
        }

        /// <summary>
        /// Return the directory to search, depending on whether filterSpec starts with ..\
        /// </summary>
        /// <param name="directoryToSearch">Directory path from the search results</param>
        /// <param name="filterSpec">FileSelector</param>
        /// <returns>Directory path to search</returns>
        private string HandleRelativePathFilter(string directoryToSearch, string filterSpec)
        {
            if (string.IsNullOrWhiteSpace(filterSpec) || !filterSpec.StartsWith(@"..\"))
            {
                return directoryToSearch;
            }

            var di = new DirectoryInfo(directoryToSearch);

            while (filterSpec.StartsWith(@"..\"))
            {
                if (di?.Parent == null)
                    break;

                di = di.Parent;
                filterSpec = filterSpec.Substring(3);
            }

            return di.FullName;
        }

        /// <summary>
        /// Remove any relative path specs in front of the selector
        /// Those should have been used already when determining which directories to search
        /// </summary>
        /// <param name="selector"></param>
        private string ScrubRelativePathText(string selector)
        {
            if (!selector.StartsWith(@"..\"))
            {
                return selector;
            }

            while (selector.StartsWith(@"..\"))
            {
                selector = selector.Substring(3);
            }
            return selector;
        }
    }
}
