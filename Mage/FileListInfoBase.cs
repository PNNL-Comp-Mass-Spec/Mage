using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using log4net;
using MyEMSLReader;

namespace Mage
{
    /// <summary>
    /// This module serves as the base class for either FileListFilter or FileListInfoLookup
    /// 
    /// If using FileListFilter, then pass in a list of folder paths to be searched
    /// If using FileListInfoLookup, then pass in a list of file paths whose file information should be determined
    ///
    /// This module can receive the list of source folders either via its HandleDataRow listener
    /// (it will accumulate the list into an internal file path buffer and then use it to look for files)
    /// or it may be run as a source module after one or more source folders are specified by
    /// setting the "FolderPath" property/parameter
    ///
    /// This module uses output column definitions
    /// the internal defaults will provide a functional minimum even if the 
    /// "OutputColumnList" property is not set by the client, but if it is
    /// it must include a new column definition for the column specified by the "FileColumnName" property
    /// </summary>
    public abstract class FileListInfoBase : FileProcessingBase
    {
        /// <summary>
        /// Trace logger
        /// </summary>
        protected static readonly ILog traceLog = LogManager.GetLogger("TraceLog");

        #region Member Variables

        /// <summary>
        /// Buffer that accumulates a row of output fields for each input row
        /// received via standard tabular input or via the "FolderPath" property
        /// It includes the folder path column to be searched for files
        /// so it also functions as an internal file path buffer 
        /// </summary>
        protected readonly List<string[]> mOutputBuffer = new List<string[]>();

        // these are used by the file/subfolder search logic

        /// <summary>
        /// Column index that has the folder path
        /// </summary>
        protected int mFolderPathColIndx = -1;

        private int mFileNameOutColIndx = -1;
        private int mFileSizeOutColIndx = -1;
        private int mFileDateOutColIndx = -1;
        private int mFileTypeOutColIndex = -1;

        #endregion

        #region "Constants"

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
        /// Default column name for the source folder column
        /// </summary>
        public const string COLUMN_NAME_SOURCE_FOLDER = "Folder";

        #endregion

        #region Properties

        /// <summary>
        /// the name of the input column that contains the folder path to search for files
        /// </summary>
        public string SourceFolderColumnName { get; set; }

        /// <summary>
        /// name of output column that will receive filename
        /// </summary>
        public string FileColumnName { get; set; }

        /// <summary>
        /// name of output column that will receive file size (bytes)
        /// </summary>
        public string FileSizeColumnName { get; set; }

        /// <summary>
        /// name of output column that will receive file date
        /// </summary>
        public string FileDateColumnName { get; set; }

        /// <summary>
        /// the name of the output column that will contain the file type
        /// </summary>
        public string FileTypeColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage file list filter module
        /// </summary>
        protected FileListInfoBase()
        {
            FileTypeColumnName = COLUMN_NAME_FILE_TYPE;             // Item
            FileColumnName = COLUMN_NAME_FILE_NAME;                 // File
            FileSizeColumnName = COLUMN_NAME_FILE_SIZE;             // File_Size_KB
            FileDateColumnName = COLUMN_NAME_FILE_DATE;             // File_Date
            SourceFolderColumnName = COLUMN_NAME_SOURCE_FOLDER;     // Folder
            OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", FileTypeColumnName, FileColumnName, FileSizeColumnName, FileDateColumnName, SourceFolderColumnName);
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// called when this module functions as source module
        /// (requires that optional property FolderPath be set)
        /// </summary>
        /// <param name="state"></param>
        public override void Run(object state)
        {
            SetupOutputColumns();
            ExportColumnDefs();
            SearchFoldersAndOutputFiles();
        }


        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// 
        /// receive storage folder path as column in data row, 
        /// and save it and the ID column value to our local folder path buffer
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
                // if we have subscribers, do the file lookup and tell them about it
                SearchFoldersAndOutputFiles();
            }
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            // build lookup of column index by column name
            base.HandleColumnDef(sender, args);

            // end of column definitions from our source,
            // now tell our subscribers what columns to expect from us
            ExportColumnDefs();
        }

        #endregion

        #region Functions for Output Columns

        /// <summary>
        /// tell our subscribers what columns to expect from us
        /// which will be an internal (filename) column
        /// followed by any input columns that are passed through to output
        /// </summary>
        private void ExportColumnDefs()
        {
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
        }

        #endregion

        #region Search Functions

        /// <summary>
        /// Initialize variables prior to starting the search
        /// </summary>
        protected abstract void SetupSearch();

        /// <summary>
        /// Seach for files in the given folder
        /// </summary>
        /// <param name="outputBufferRowIdx">Row index in mOutputBuffer to examine</param>
        /// <param name="fileInfo">Dictionary of found files (input/output parameter)</param>
        /// <param name="subfolderInfo">Dictionary of found folders (input/output parameter)</param>
        /// <param name="folderPath">Folder path to examine</param>
        /// <param name="datasetName">Dataset name</param>
        protected abstract void SearchOneFolder(
            int outputBufferRowIdx,
            Dictionary<string, FileInfo> fileInfo,
            Dictionary<string, DirectoryInfo> subfolderInfo,
            string folderPath,
            string datasetName);

        #endregion

        #region Private Functions

        private void SearchFoldersAndOutputFiles()
        {

            // set up indexes for row columns
            TryGetOutputColumnPos(SourceFolderColumnName, out mFolderPathColIndx);
            TryGetOutputColumnPos(FileColumnName, out mFileNameOutColIndx);
            TryGetOutputColumnPos(FileSizeColumnName, out mFileSizeOutColIndx);
            TryGetOutputColumnPos(FileDateColumnName, out mFileDateOutColIndx);

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
                var folderPathSpec = string.Empty;
                if (mFolderPathColIndx < mOutputBuffer[outputBufferRowIdx].Length)
                    folderPathSpec = mOutputBuffer[outputBufferRowIdx][mFolderPathColIndx];

                var datasetName = DetermineDatasetName(mOutputBuffer[outputBufferRowIdx], folderPathSpec);

                if (folderPathSpec.Contains(MYEMSL_PATH_FLAG))
                {
                    searchMyEMSL = true;

                    if (string.IsNullOrEmpty(datasetName))
                        throw new MageException("Unable to determine dataset name for row " + (outputBufferRowIdx + 1) + ", file " + folderPathSpec);
                }

                dctRowDatasets.Add(outputBufferRowIdx, datasetName);
            }

            if (searchMyEMSL)
            {
                foreach (var datasetName in dctRowDatasets.Values.Distinct())
                    m_MyEMSLDatasetInfoCache.AddDataset(datasetName);
            }


            // go through each folder that we accumulated in our internal buffer
            for (var outputBufferRowIdx = 0; outputBufferRowIdx < mOutputBuffer.Count; outputBufferRowIdx++)
            {
                if (Abort)
                {
                    ReportProcessingAborted();
                    break;
                }

                var folderPathSpec = mOutputBuffer[outputBufferRowIdx][mFolderPathColIndx];
                var folderPaths = new List<string>();

                // folderPathSpec may contain multiple folders, separated by a vertical bar
                // If that is the case, then we'll search for files in each folder, preferentially using files in the folder listed first
                if (folderPathSpec.Contains('|'))
                {
                    folderPaths = folderPathSpec.Split('|').ToList();
                }
                else
                {
                    folderPaths.Add(folderPathSpec);
                }

                // This dictionary holds filename and file system info for each file that is found
                var fileInfo = new Dictionary<string, FileInfo>(StringComparer.CurrentCultureIgnoreCase);

                // This dictionary holds subfolder name file system info for each subfolder that is found
                var subfolderInfo = new Dictionary<string, DirectoryInfo>(StringComparer.CurrentCultureIgnoreCase);

                var datasetName = dctRowDatasets[outputBufferRowIdx];

                foreach (var folderPath in folderPaths)
                {
                    if (Abort)
                    {
                        ReportProcessingAborted();
                        break;
                    }

                    traceLog.Debug("FileListFilter: Searching folder " + folderPath);
                    UpdateStatus("FileListFilter: Searching folder " + folderPath);

                    SearchOneFolder(outputBufferRowIdx, fileInfo, subfolderInfo, folderPath, datasetName);

                }

                // inform our subscribers of what we found
                if ((fileInfo.Count == 0) && (subfolderInfo.Count == 0))
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
                        string folderPath;
                        string fileSizeKB;
                        string fileDate;

                        if (entry.Value.DirectoryName.StartsWith(MYEMSL_PATH_FLAG))
                        {
                            var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(entry.Value.FullName);

                            if (myEMSLFileID == 0)
                                throw new MageException("Encoded MyEMSL File ID not found in " + entry.Value.FullName);

                            ArchivedFileInfo archiveFileInfo;

                            if (!GetCachedArchivedFileInfo(myEMSLFileID, out archiveFileInfo))
                                throw new MageException("Cached ArchiveFileInfo does not contain MyEMSL File ID " + myEMSLFileID);

                            fileName = DatasetInfoBase.AppendMyEMSLFileID(archiveFileInfo.Filename, myEMSLFileID);
                            var fiMyEMSLFile = new FileInfo(MYEMSL_PATH_FLAG + "\\" + archiveFileInfo.PathWithInstrumentAndDatasetWindows);
                            folderPath = fiMyEMSLFile.Directory.FullName;
                            fileSizeKB = FileSizeBytesToString(archiveFileInfo.FileSizeBytes);
                            fileDate = archiveFileInfo.SubmissionTimeODBC12hr;

                            CacheFilterPassingFile(archiveFileInfo);
                        }
                        else
                        {
                            fileName = entry.Key;
                            folderPath = entry.Value.DirectoryName;
                            fileSizeKB = FileSizeBytesToString(entry.Value.Length);
                            fileDate = entry.Value.LastWriteTime.ToString("yyyy-MM-dd hh:mm:ss tt");
                        }
                        ReportFileFound(outputBufferRowIdx, folderPath, fileName, fileSizeKB, fileDate);
                    }
                    foreach (var entry in subfolderInfo)
                    {
                        var subfolderName = entry.Key;
                        var folderPath = entry.Value.FullName;
                        ReportSubfolderFound(outputBufferRowIdx, folderPath, subfolderName);
                    }
                }
            }

            // inform our subscribers that all data has been sent
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        private string FileSizeBytesToString(Int64 sizeBytes)
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

        #endregion

        #region File/Subfolder Result Reporting

        /// <summary>
        /// report a found subfolder to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        /// <param name="folderPath"></param>
        /// <param name="subfolderName"></param>
        private void ReportSubfolderFound(int outputBufferRowIdx, string folderPath, string subfolderName)
        {
            var outRow = (string[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
            if (mFileTypeOutColIndex > -1)
            {
                outRow[mFileTypeOutColIndex] = "folder";
                outRow[mFolderPathColIndx] = folderPath;
            }
            outRow[mFileNameOutColIndx] = subfolderName;
            //if (mFileSizeOutColIndx > -1)
            //    outRow[mFileSizeOutColIndx] = 0;
            OnDataRowAvailable(new MageDataEventArgs(outRow));
        }

        /// <summary>
        /// report a found file to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <param name="fileSizeKB"></param>
        /// /// <param name="fileDate"></param>
        private void ReportFileFound(int outputBufferRowIdx, string folderPath, string fileName, string fileSizeKB, string fileDate)
        {
            var outRow = (string[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
            if (mFileTypeOutColIndex > -1)
            {
                outRow[mFileTypeOutColIndex] = "file";
                outRow[mFolderPathColIndx] = folderPath;
            }
            outRow[mFileNameOutColIndx] = fileName;
            if (mFileSizeOutColIndx > -1)
                outRow[mFileSizeOutColIndx] = fileSizeKB;

            if (mFileDateOutColIndx > -1)
                outRow[mFileDateOutColIndx] = fileDate;

            OnDataRowAvailable(new MageDataEventArgs(outRow));
        }

        /// <summary>
        /// report nothing found to output
        /// </summary>
        /// <param name="outputBufferRowIdx">Index to row in mOutputBuffer</param>
        private void ReportNothingFound(int outputBufferRowIdx)
        {
            if (mFileTypeOutColIndex > -1)
            {
                mOutputBuffer[outputBufferRowIdx][mFileTypeOutColIndex] = "";
            }
            // output record that says we didn't find any files
            mOutputBuffer[outputBufferRowIdx][mFileNameOutColIndx] = BaseModule.kNoFilesFound;
            OnDataRowAvailable(new MageDataEventArgs(mOutputBuffer[outputBufferRowIdx]));
        }

        /// <summary>
        /// output record that says we had problem accessing files
        /// </summary>
        /// <param name="outputBufferRowIdx"></param>
        /// <param name="msg"></param>
        protected void ReportSearchErrorToOutput(int outputBufferRowIdx, string msg)
        {
            mOutputBuffer[outputBufferRowIdx][mFileNameOutColIndx] = "--Error: " + msg;
            traceLog.Error(msg);
            OnDataRowAvailable(new MageDataEventArgs(mOutputBuffer[outputBufferRowIdx]));
        }

        #endregion

    }
}
