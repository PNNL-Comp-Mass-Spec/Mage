using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MyEMSLReader;
using PRISM.Logging;

namespace Mage
{
    /// <summary>
    /// <para>
    /// This module serves as the base class for either FileListFilter or FileListInfoLookup
    /// </para>
    /// <para>
    /// If using FileListFilter, pass in a list of directory paths to be searched
    /// </para>
    /// <para>
    /// If using FileListInfoLookup, pass in a list of file paths whose file information should be determined
    /// </para>
    /// <para>
    /// This module can receive the list of source directories via either its HandleDataRow listener
    /// (it will accumulate the list into an internal file path buffer and then use it to look for files)
    /// or it may be run as a source module after one or more source directories are specified by
    /// setting the "DirectoryPath" property/parameter
    /// </para>
    /// <para>
    /// This module uses output column definitions
    /// the internal defaults will provide a functional minimum even if the
    /// "OutputColumnList" property is not set by the client, but if it is
    /// it must include a new column definition for the column specified by the "FileColumnName" property
    /// </para>
    /// </summary>
    public abstract class FileListInfoBase : FileProcessingBase
    {
        // Ignore Spelling: Mage, hh:mm:ss tt, yyyy-MM-dd

        /// <summary>
        /// Trace logger
        /// </summary>
        protected static readonly FileLogger traceLogFileList = new(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        /// <summary>
        /// Buffer that accumulates a row of output fields for each input row
        /// received via standard tabular input or via the "DirectoryPath" property
        /// It includes the directory path column to be searched for files
        /// so it also functions as an internal file path buffer
        /// </summary>
        protected readonly List<string[]> mOutputBuffer = new();

        // these are used by the file/subdirectory search logic

        /// <summary>
        /// Column index that has the directory path
        /// </summary>
        protected int mDirectoryPathColIndex = -1;

        private int mFileNameOutColIndex = -1;
        private int mFileSizeOutColIndex = -1;
        private int mFileDateOutColIndex = -1;
        private int mFileTypeOutColIndex = -1;

        /// <summary>
        /// Default column name for the item type column
        /// </summary>
        public const string COLUMN_NAME_FILE_TYPE = "Item";

        /// <summary>
        /// Default column name for the file name column
        /// </summary>
        public const string COLUMN_NAME_FILE_NAME = "File";

        /// <summary>
        /// Default column name for the file size column
        /// </summary>
        public const string COLUMN_NAME_FILE_SIZE = "File_Size_KB";

        /// <summary>
        /// Default column name for the file date column
        /// </summary>
        public const string COLUMN_NAME_FILE_DATE = "File_Date";

        /// <summary>
        /// Default column name for the source directory column
        /// </summary>
        public const string COLUMN_NAME_SOURCE_DIRECTORY = "Directory";

        /// <summary>
        /// The name of the input column that contains the directory path to search for files
        /// </summary>
        public string SourceDirectoryColumnName { get; set; }

        /// <summary>
        /// Name of output column that will receive filename
        /// </summary>
        public string FileColumnName { get; set; }

        /// <summary>
        /// Name of output column that will receive file size (bytes)
        /// </summary>
        public string FileSizeColumnName { get; set; }

        /// <summary>
        /// Name of output column that will receive file date
        /// </summary>
        public string FileDateColumnName { get; set; }

        /// <summary>
        /// The name of the output column that will contain the file type
        /// </summary>
        public string FileTypeColumnName { get; set; }

        /// <summary>
        /// Construct a new Mage file list filter module
        /// </summary>
        protected FileListInfoBase()
        {
            FileTypeColumnName = COLUMN_NAME_FILE_TYPE;                     // Item
            FileColumnName = COLUMN_NAME_FILE_NAME;                         // File
            FileSizeColumnName = COLUMN_NAME_FILE_SIZE;                     // File_Size_KB
            FileDateColumnName = COLUMN_NAME_FILE_DATE;                     // File_Date
            SourceDirectoryColumnName = COLUMN_NAME_SOURCE_DIRECTORY;       // Directory
            OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", FileTypeColumnName, FileColumnName, FileSizeColumnName, FileDateColumnName, SourceDirectoryColumnName);
        }

        /// <summary>
        /// Called when this module functions as source module
        /// (requires that optional property DirectoryPath be set)
        /// </summary>
        /// <param name="state"></param>
        public override void Run(object state)
        {
            SetupOutputColumns();
            ExportColumnDefs();
            SearchDirectoriesAndOutputFiles();
        }

        /// <summary>
        /// <para>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </para>
        /// <para>
        /// Receive storage directory path as column in data row,
        /// and save it and the ID column value to our local directory path buffer
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                mOutputBuffer.Add(MapDataRow(args.Fields));
            }
            else
            {
                // If we have subscribers, do the file lookup and tell them about it
                SearchDirectoriesAndOutputFiles();
            }
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            // Build lookup of column index by column name
            base.HandleColumnDef(sender, args);
            // End of column definitions from our source,

            // Now tell our subscribers what columns to expect from us
            ExportColumnDefs();
        }

        // Methods for output columns

        /// <summary>
        /// Tell our subscribers what columns to expect from us
        /// which will be an internal (filename) column
        /// followed by any input columns that are passed through to output
        /// </summary>
        private void ExportColumnDefs()
        {
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
        }

        // Search Methods

        /// <summary>
        /// Initialize variables prior to starting the search
        /// </summary>
        protected abstract void SetupSearch();

        /// <summary>
        /// Search for files in the given directory
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subdirectoryInfo">Dictionary of found directories (input/output parameter)</param>
        /// <param name="directoryPath">Directory path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        protected abstract void SearchOneDirectory(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subdirectoryInfo,
            string directoryPath,
            string datasetName);

        private void SearchDirectoriesAndOutputFiles()
        {
            // Set up indexes for row columns
            var directoryColDefined = TryGetOutputColumnPos(SourceDirectoryColumnName, out mDirectoryPathColIndex);
            if (!directoryColDefined && SourceDirectoryColumnName.Equals("Directory", StringComparison.OrdinalIgnoreCase))
            {
                // Column name not found
                // Possibly auto-switch from Directory to Folder
                TryGetOutputColumnPos("Folder", out mDirectoryPathColIndex);
            }

            if (mDirectoryPathColIndex < 0)
            {
                const string errorMessage = "SearchDirectoriesAndOutputFiles: Unable to determine the index of the Directory column";
                var ex = ReportMageException(errorMessage);
                throw ex;
            }

            TryGetOutputColumnPos(FileColumnName, out mFileNameOutColIndex);
            TryGetOutputColumnPos(FileSizeColumnName, out mFileSizeOutColIndex);
            TryGetOutputColumnPos(FileDateColumnName, out mFileDateOutColIndex);

            if (string.IsNullOrEmpty(FileTypeColumnName))
                mFileTypeOutColIndex = -1;
            else
                TryGetOutputColumnPos(FileTypeColumnName, out mFileTypeOutColIndex);

            SetupSearch();

            var dctRowDatasets = new Dictionary<int, string>();
            var searchMyEMSL = false;

            // Determine the dataset name to use for each row in mOutputBuffer
            for (var outputBufferRowIdx = 0; outputBufferRowIdx < mOutputBuffer.Count; outputBufferRowIdx++)
            {
                var directoryPathSpec = string.Empty;
                if (mDirectoryPathColIndex < mOutputBuffer[outputBufferRowIdx].Length)
                    directoryPathSpec = mOutputBuffer[outputBufferRowIdx][mDirectoryPathColIndex];

                var datasetName = DetermineDatasetName(mOutputBuffer[outputBufferRowIdx], directoryPathSpec);

                if (directoryPathSpec.Contains(MYEMSL_PATH_FLAG))
                {
                    searchMyEMSL = true;

                    if (string.IsNullOrEmpty(datasetName))
                    {
                        var errorMessage = "Unable to determine dataset name for row " + (outputBufferRowIdx + 1) + ", file " + directoryPathSpec;
                        var ex = ReportMageException(errorMessage);
                        throw ex;
                    }
                }

                dctRowDatasets.Add(outputBufferRowIdx, datasetName);
            }

            if (searchMyEMSL)
            {
                foreach (var datasetName in dctRowDatasets.Values.Distinct())
                    m_MyEMSLDatasetInfoCache.AddDataset(datasetName);
            }

            // Go through each directory that we accumulated in our internal buffer
            for (var outputBufferRowIdx = 0; outputBufferRowIdx < mOutputBuffer.Count; outputBufferRowIdx++)
            {
                if (Abort)
                {
                    ReportProcessingAborted();
                    break;
                }

                var directoryPathSpec = mOutputBuffer[outputBufferRowIdx][mDirectoryPathColIndex];
                List<string> directoryPaths;

                // directoryPathSpec may contain multiple directories, separated by a vertical bar
                // If that is the case, then we'll search for files in each directory, preferentially using files in the directory listed first
                if (directoryPathSpec.Contains('|'))
                {
                    directoryPaths = directoryPathSpec.Split('|').ToList();
                }
                else
                {
                    directoryPaths = new List<string> {directoryPathSpec};
                }

                // This dictionary holds filename and file system info for each file that is found
                var fileInfo = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);

                // This dictionary holds subdirectory name file system info for each subdirectory that is found
                var subdirectoryInfo = new Dictionary<string, DirectoryInfo>(StringComparer.OrdinalIgnoreCase);

                var datasetName = dctRowDatasets[outputBufferRowIdx];

                foreach (var directoryPath in directoryPaths)
                {
                    if (Abort)
                    {
                        ReportProcessingAborted();
                        break;
                    }

                    traceLogFileList.Debug("FileListFilter: Searching directory " + directoryPath);
                    UpdateStatus("FileListFilter: Searching directory " + directoryPath);

                    SearchOneDirectory(outputBufferRowIdx, fileInfo, subdirectoryInfo, directoryPath, datasetName);
                }

                // Inform our subscribers of what we found
                if ((fileInfo.Count == 0) && (subdirectoryInfo.Count == 0))
                {
                    ReportNothingFound(outputBufferRowIdx);
                }
                else
                {
                    // When searching the archive, we may find files that start with x_
                    // We want to skip those if there is a corresponding file that does not start with x_

                    var candidatesForRemoval = (from item in fileInfo where item.Key.StartsWith("x_") select item).ToList();

                    foreach (var candidate in candidatesForRemoval)
                    {
                        if (candidate.Key.Length < 3)
                            continue;

                        var nameToFind = candidate.Key.Substring(2);

                        if (fileInfo.ContainsKey(nameToFind))
                        {
                            fileInfo.Remove(candidate.Key);
                        }
                    }

                    foreach (var entry in fileInfo)
                    {
                        string fileName;
                        string directoryPath;
                        string fileSizeKB;
                        string fileDate;

                        if (entry.Value.DirectoryName != null && entry.Value.DirectoryName.StartsWith(MYEMSL_PATH_FLAG))
                        {
                            var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(entry.Value.FullName);

                            if (myEMSLFileID == 0)
                            {
                                var errorMessage = "Encoded MyEMSL File ID not found in " + entry.Value.FullName;
                                var ex = ReportMageException(errorMessage);
                                throw ex;
                            }

                            if (!GetCachedArchivedFileInfo(myEMSLFileID, out var archiveFileInfo))
                            {
                                var errorMessage = "Cached ArchiveFileInfo does not contain MyEMSL File ID " + myEMSLFileID;
                                var ex = ReportMageException(errorMessage);
                                throw ex;
                            }

                            fileName = DatasetInfoBase.AppendMyEMSLFileID(archiveFileInfo.Filename, myEMSLFileID);
                            var fiMyEMSLFile = new FileInfo(MYEMSL_PATH_FLAG + "\\" + archiveFileInfo.PathWithInstrumentAndDatasetWindows);
                            if (fiMyEMSLFile.Directory != null)
                                directoryPath = fiMyEMSLFile.Directory.FullName;
                            else
                            {
                                directoryPath = string.Empty;
                            }
                            fileSizeKB = FileSizeBytesToString(archiveFileInfo.FileSizeBytes);
                            fileDate = archiveFileInfo.SubmissionTimeODBC12hr;

                            CacheFilterPassingFile(archiveFileInfo);
                        }
                        else
                        {
                            fileName = entry.Key;
                            directoryPath = entry.Value.DirectoryName;
                            fileSizeKB = FileSizeBytesToString(entry.Value.Length);
                            fileDate = entry.Value.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                        }
                        ReportFileFound(outputBufferRowIdx, directoryPath, fileName, fileSizeKB, fileDate);
                    }

                    foreach (var entry in subdirectoryInfo)
                    {
                        var subdirectoryName = entry.Key;
                        var directoryPath = entry.Value.FullName;
                        ReportSubdirectoryFound(outputBufferRowIdx, directoryPath, subdirectoryName);
                    }
                }
            }

            // Inform our subscribers that all data has been sent
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        private string FileSizeBytesToString(long sizeBytes)
        {
            if (sizeBytes == 0)
                return "0";

            var fileSizeKB = sizeBytes / 1024.0;

            if (fileSizeKB < 10)
                return fileSizeKB.ToString("0.000");

            if (fileSizeKB < 100)
                return fileSizeKB.ToString("0.00");

            if (fileSizeKB < 1000)
                return fileSizeKB.ToString("0.0");

            return fileSizeKB.ToString("0");
        }

        private void UpdateStatus(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        /// <summary>
        /// Report a found subdirectory to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        /// <param name="directoryPath"></param>
        /// <param name="subdirectoryName"></param>
        private void ReportSubdirectoryFound(int outputBufferRowIdx, string directoryPath, string subdirectoryName)
        {
            var outRow = (string[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
            if (mFileTypeOutColIndex > -1)
            {
                outRow[mFileTypeOutColIndex] = "directory";
                outRow[mDirectoryPathColIndex] = directoryPath;
            }
            outRow[mFileNameOutColIndex] = subdirectoryName;
            // if (mFileSizeOutColIndex > -1)
            //    outRow[mFileSizeOutColIndex] = 0;
            OnDataRowAvailable(new MageDataEventArgs(outRow));
        }

        /// <summary>
        /// Report a found file to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSizeKB"></param>
        /// <param name="fileDate"></param>
        private void ReportFileFound(int outputBufferRowIdx, string directoryPath, string fileName, string fileSizeKB, string fileDate)
        {
            var outRow = (string[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
            if (mFileTypeOutColIndex > -1)
            {
                outRow[mFileTypeOutColIndex] = "file";
                outRow[mDirectoryPathColIndex] = directoryPath;
            }
            outRow[mFileNameOutColIndex] = fileName;
            if (mFileSizeOutColIndex > -1)
                outRow[mFileSizeOutColIndex] = fileSizeKB;

            if (mFileDateOutColIndex > -1)
                outRow[mFileDateOutColIndex] = fileDate;

            OnDataRowAvailable(new MageDataEventArgs(outRow));
        }

        /// <summary>
        /// Report nothing found to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        private void ReportNothingFound(int outputBufferRowIdx)
        {
            if (mFileTypeOutColIndex > -1)
            {
                mOutputBuffer[outputBufferRowIdx][mFileTypeOutColIndex] = string.Empty;
            }
            // Output record that says we didn't find any files
            mOutputBuffer[outputBufferRowIdx][mFileNameOutColIndex] = kNoFilesFound;
            OnDataRowAvailable(new MageDataEventArgs(mOutputBuffer[outputBufferRowIdx]));
        }

        /// <summary>
        /// Output record that says we had problem accessing files
        /// </summary>
        /// <param name="outputBufferRowIdx"></param>
        /// <param name="msg"></param>
        protected void ReportSearchErrorToOutput(int outputBufferRowIdx, string msg)
        {
            mOutputBuffer[outputBufferRowIdx][mFileNameOutColIndex] = "--Error: " + msg;
            traceLogFileList.Error(msg);
            OnDataRowAvailable(new MageDataEventArgs(mOutputBuffer[outputBufferRowIdx]));
        }
    }
}
