using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MyEMSLReader;

namespace Mage
{
    /// <summary>
    /// This class adds support for downloading files from MyEMSL
    /// </summary>
    public class FileProcessingBase : BaseModule
    {
        /// <summary>
        /// Directory name to use when caching MyEMSL files locally
        /// </summary>
        public const string MAGE_TEMP_FILES_DIRECTORY = "Mage_Temp_Files";

        /// <summary>
        /// Text to look for when determining if a file is stored in MyEMSL
        /// </summary>
        protected const string MYEMSL_PATH_FLAG = @"\\MyEMSL";

        /// <summary>
        /// Column name with the Dataset Name
        /// </summary>
        protected const string COLUMN_NAME_DATASET = "Dataset";

        /// <summary>
        /// Alternative column name with the Dataset Name
        /// </summary>
        protected const string COLUMN_NAME_DATASET_NAME = "Dataset_Name";

        /// <summary>
        /// Alternative column name with the Dataset Name
        /// </summary>
        protected const string COLUMN_NAME_DATASET_NUM = "Dataset_Num";

        // ReSharper disable once CommentTypo
        // protected const string COLUMN_NAME_DATASETID = "DatasetID";
        // protected const string COLUMN_NAME_DATASET_ID = "Dataset_ID";

        /// <summary>
        /// Cache of files stored in MyEMSL for datasets that the user searches for
        /// </summary>
        /// <remarks>Initially does not have any datasets; add them as data is processed</remarks>
        protected static readonly DatasetListInfo m_MyEMSLDatasetInfoCache = new DatasetListInfo();

        /// <summary>
        /// Set to true once events have been attached to m_MyEMSLDatasetInfoCache
        /// </summary>
        protected static bool m_MyEMSLEventsAttached;

        /// <summary>
        /// Recently found MyEMSL files
        /// </summary>
        /// <remarks>This list is cleared each time .FindFiles is called</remarks>
        protected static List<DatasetDirectoryOrFileInfo> m_RecentlyFoundMyEMSLFiles;

        /// <summary>
        /// All MyEMSL files that pass filters; keys are MyEMSL File IDs, values are the MyEMSL Info.  Items will be auto-purged from this list if the list grows to over 1 million records
        /// </summary>
        /// <remarks>This dictionary is used by the FileCopy class to determine the archived file info for a file in MyEMSL using MyEMSLFile ID</remarks>
        protected static Dictionary<Int64, DatasetDirectoryOrFileInfo> m_FilterPassingMyEMSLFiles;

        /// <summary>
        /// RegEx to extract the dataset name from a path similar to these examples:
        ///   \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04
        ///   \\MyEMSL\VPro01\2013_3\QC_Shew_13_04d_500ng_10Sep13_Tiger_13-07-34
        ///   \\adms.emsl.pnl.gov\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
        ///   \\aurora.emsl.pnl.gov\archive\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
        ///   \\a2.emsl.pnl.gov\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
        /// The RegEx can also be used to determine the portion of a path that includes parent directories and the dataset directory
        /// </summary>
        private readonly Regex mDatasetMatchStrict = new Regex(@"\\\\[^\\]+(?:\\[^\\]+)?(?:\\[^\\]+)?\\[^\\]+\\2[0-9][0-9][0-9]_[1-4]\\(?<Dataset>[^\\]+)", RegexOptions.Compiled);

        /// <summary>
        /// RegEx to extract the dataset name from a path of the form \2013_3\QC_Shew_13_04f_500ng_10Sep13_Tiger_13-07-34
        /// he RegEx can also be used to determine the portion of a path that includes parent directories and the dataset directory
        /// </summary>
        private readonly Regex mDatasetMatchLoose = new Regex(@"(^|\\)2[0-9][0-9][0-9]_[1-4]\\(?<Dataset>[^\\]+)", RegexOptions.Compiled);

        /// <summary>
        /// Constructor
        /// </summary>
        protected FileProcessingBase()
        {
            if (!m_MyEMSLEventsAttached)
            {
                // We only want to attach these events once since m_MyEMSLDatasetInfoCache is static
                // However, I have found that if I only attach the Progress Event once, the GUI form does not display the progress messages (even though they are, in fact, being actively handled by this class)
                m_MyEMSLEventsAttached = true;
                m_MyEMSLDatasetInfoCache.ErrorEvent += MyEMSLDatasetInfoCache_ErrorEvent;
                m_MyEMSLDatasetInfoCache.StatusEvent += MyEMSLDatasetInfoCache_MessageEvent;
            }

            // As mentioned above, we need to attach this event every time the class is instantiated
            // This will result in duplicate calls to m_MyEMSLDatasetInfoCache_ProgressEvent by m_MyEMSLDatasetInfoCache.ProgressEvent but that doesn't hurt anything
            m_MyEMSLDatasetInfoCache.ProgressUpdate += MyEMSLDatasetInfoCache_ProgressEvent;
        }

        /// <summary>
        /// Store the archive file info for a file in m_FilterPassingMyEMSLFiles
        /// </summary>
        /// <param name="fileInfo">MyEMSL file info</param>
        /// <remarks>Useful for retrieving MyEMSL file info at a future time using MyEMSL File ID</remarks>
        protected static void CacheFilterPassingFile(ArchivedFileInfo fileInfo)
        {
            if (fileInfo.FileID == 0)
                return;

            if (m_FilterPassingMyEMSLFiles == null)
                m_FilterPassingMyEMSLFiles = new Dictionary<long, DatasetDirectoryOrFileInfo>();

            if (!m_FilterPassingMyEMSLFiles.TryGetValue(fileInfo.FileID, out var fileInfoCached))
            {
                m_FilterPassingMyEMSLFiles.Add(fileInfo.FileID, new DatasetDirectoryOrFileInfo(fileInfo.FileID, false, fileInfo));
            }

            if (m_FilterPassingMyEMSLFiles.Count > 1000000)
            {
                // Remove any entries over 3 hours old
                var fileIDsToRemove = (from item in m_FilterPassingMyEMSLFiles
                                       where DateTime.UtcNow.Subtract(item.Value.CacheDateUTC).TotalMinutes > 180
                                       select item.Key).ToList();

                foreach (var fileID in fileIDsToRemove)
                    m_FilterPassingMyEMSLFiles.Remove(fileID);
            }
        }

        /// <summary>
        /// Delete the given file is the disk free space is less than 500 MB
        /// </summary>
        /// <param name="filePath">File to delete</param>
        protected void DeleteFileIfLowDiskSpace(string filePath)
        {
            DeleteFileIfLowDiskSpace(filePath, 500);
        }

        /// <summary>
        /// Delete the given file is the disk free space is less than the specified threshold
        /// </summary>
        /// <param name="filePath">File to delete</param>
        /// <param name="freeSpaceThresholdMB">Disk space threshold, in MB</param>
        protected void DeleteFileIfLowDiskSpace(string filePath, int freeSpaceThresholdMB)
        {
            try
            {
                // Abort if the file is on a remote share (it's too hard to determine disk free space)
                if (filePath.StartsWith(@"\\"))
                    return;

                var fiFile = new FileInfo(filePath);

                // Make sure the file exists
                if (!fiFile.Exists)
                    return;

                if (fiFile.FullName.Length < 3)
                    return;

                // The first 3 characters of the full path should be the drive letter, a colon, and a slash
                // We'll get an exception if they're not
                var currentDrive = new DriveInfo(fiFile.FullName.Substring(0, 3));

                // Determine the drive free space, in MB
                var freeSpaceMB = currentDrive.AvailableFreeSpace / 1024.0 / 1024;

                if (freeSpaceThresholdMB < 100)
                    freeSpaceThresholdMB = 100;

                if (freeSpaceMB < freeSpaceThresholdMB)
                    fiFile.Delete();
            }
            catch
            {
                // Ignore errors here
            }
        }

        /// <summary>
        /// Determine the name of a dataset given a directory path
        /// </summary>
        /// <param name="directoryPath">Directory path to examine</param>
        /// <returns>The dataset name if found; empty string if the dataset name could not be determined</returns>
        protected string DetermineDatasetName(string directoryPath)
        {
            var datasetName = string.Empty;

            // Parse the directoryPath with a RegEx to extract the dataset name
            var reMatch = mDatasetMatchStrict.Match(directoryPath);

            if (!reMatch.Success)
                reMatch = mDatasetMatchLoose.Match(directoryPath);

            if (reMatch.Success)
            {
                datasetName = reMatch.Groups["Dataset"].Value;
            }

            return datasetName;
        }

        /// <summary>
        /// Determine the name using the Dataset column in a given row of data
        /// </summary>
        /// <param name="bufferRow">Row of data to examine; should contain a dataset column in the OutputColumnPos object</param>
        /// <param name="directoryPath">Directory path use if bufferRow does not contain a dataset column</param>
        /// <returns>The dataset name if found; empty string if the dataset name could not be determined</returns>
        protected string DetermineDatasetName(string[] bufferRow, string directoryPath)
        {
            string datasetName;

            var datasetColNames = new List<string>
            {
                COLUMN_NAME_DATASET,
                COLUMN_NAME_DATASET_NAME,
                COLUMN_NAME_DATASET_NUM
            };

            var datasetColIndex = -1;
            foreach (var datasetColName in datasetColNames)
            {
                if (TryGetOutputColumnPos(datasetColName, out datasetColIndex))
                    break;
            }

            if (datasetColIndex >= 0 && datasetColIndex < bufferRow.Length)
            {
                datasetName = bufferRow[datasetColIndex];
            }
            else
            {
                datasetName = DetermineDatasetName(directoryPath);
            }

            return datasetName;
        }

        /// <summary>
        /// Download the given file if it is in MyEMSL
        /// </summary>
        /// <param name="filePathRemote">File path to examine</param>
        /// <returns>The local file path to which the file was downloaded; if not in MyEMSL, then returns the original path</returns>
        /// <remarks>it is better to add several files to the download queue using AddFileToDownloadQueue then download them in bulk using ProcessMyEMSLDownloadQueue</remarks>
        protected string DownloadFileIfRequired(string filePathRemote)
        {
            string filePathLocal;

            if (filePathRemote.StartsWith(MYEMSL_PATH_FLAG))
            {
                var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filePathRemote, out var filePathClean);

                if (myEMSLFileID <= 0)
                {
                    var errorMessage = "MyEMSL File does not have the MyEMSL FileID tag (" + DatasetInfoBase.MYEMSL_FILE_ID_TAG + "): " + filePathRemote;
                    var ex = ReportMageException(errorMessage);
                    throw ex;
                }

                if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out var cachedFileInfo))
                {
                    filePathLocal = Path.Combine(Path.GetTempPath(), MAGE_TEMP_FILES_DIRECTORY, cachedFileInfo.FileInfo.Dataset, Path.GetFileName(filePathClean));

                    OnStatusMessageUpdated(new MageStatusEventArgs("Downloading file from MyEMSL: " + cachedFileInfo.FileInfo.RelativePathWindows));

                    // Note: Explicitly defining the target path to save the file at using filePathLocal
                    const bool unzipRequired = false;
                    m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(myEMSLFileID, cachedFileInfo.FileInfo, unzipRequired, filePathLocal);

                    // Note that the target directory path will be ignored since we explicitly defined the destination file path when queuing the file
                    var success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(".", Downloader.DownloadLayout.SingleDataset);
                    if (!success)
                    {
                        var errorMessage = "Failed to download file " + cachedFileInfo.FileInfo.RelativePathWindows + " from MyEMSL";
                        if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                            errorMessage += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages.First();
                        else
                            errorMessage += ": Unknown Error";

                        var ex = ReportMageException(errorMessage);
                        throw ex;
                    }
                }
                else
                {
                    var errorMessage = "Cannot download file since not in MyEMSL Memory Cache: " + filePathRemote;
                    var ex = ReportMageException(errorMessage);
                    throw ex;
                }
            }
            else
            {
                filePathLocal = filePathRemote;
            }

            return filePathLocal;
        }

        /// <summary>
        /// Examines the directory path to find the parent directories, including the dataset directory
        /// For example, given   \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04\SIC201307301439_Auto965355
        /// The path returned is \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04
        /// </summary>
        /// <param name="directoryPath">Path to examine</param>
        /// <returns>Parent directories, including the dataset directory</returns>
        protected string ExtractParentDatasetDirectories(string directoryPath)
        {
            var parentDirectories = string.Empty;

            // Parse the directoryPath with a RegEx to extract the parent directories
            var reMatch = mDatasetMatchStrict.Match(directoryPath);

            if (!reMatch.Success)
                reMatch = mDatasetMatchLoose.Match(directoryPath);

            if (reMatch.Success)
            {
                parentDirectories = reMatch.ToString();
            }

            return parentDirectories;
        }

        /// <summary>
        /// Looks for given MyEMSL file id in m_RecentlyFoundMyEMSLFiles and m_FilterPassingMyEMSLFiles
        /// </summary>
        /// <param name="myEMSLFileID">MyEMSL File ID to find</param>
        /// <param name="fileInfo">Output: File Info object</param>
        /// <returns>True if success, false if an error</returns>
        protected bool GetCachedArchivedFileInfo(Int64 myEMSLFileID, out ArchivedFileInfo fileInfo)
        {
            var fileInfoMatch = (from item in m_RecentlyFoundMyEMSLFiles where item.FileID == myEMSLFileID select item.FileInfo).ToList();

            if (fileInfoMatch.Count == 0)
            {
                fileInfoMatch = (from item in m_FilterPassingMyEMSLFiles where item.Key == myEMSLFileID select item.Value.FileInfo).ToList();
            }

            if (fileInfoMatch.Count == 0)
            {
                fileInfo = null;
                return false;
            }

            fileInfo = fileInfoMatch.First();
            return true;
        }

        /// <summary>
        /// Examines the path to determine the parent directories and possible subdirectory for a given dataset
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="datasetName"></param>
        /// <param name="subDir"></param>
        /// <param name="parentDirectories"></param>
        protected void GetMyEMSLParentDirectoriesAndSubDir(string directoryPath, string datasetName, out string subDir, out string parentDirectories)
        {
            subDir = string.Empty;

            parentDirectories = ExtractParentDatasetDirectories(directoryPath);
            if (string.IsNullOrEmpty(parentDirectories))
            {
                parentDirectories = MYEMSL_PATH_FLAG + @"\Instrument\2013_1\" + datasetName;
            }
            else
            {
                if (directoryPath.Length > parentDirectories.Length)
                {
                    subDir = directoryPath.Substring(parentDirectories.Length);
                    subDir = subDir.TrimStart('\\');
                }
                parentDirectories = parentDirectories.TrimEnd('\\');
            }
        }

        /// <summary>
        /// Called by pipeline container after pipeline execution has processed all of the data rows
        /// Download any queued MyEMSL files
        /// </summary>
        public override bool PostProcess()
        {
            // Note that the target directory path will most likely be ignored since
            // explicit destination file paths were likely used when files were added to the queue
            var success = ProcessMyEMSLDownloadQueue(".", Downloader.DownloadLayout.SingleDataset);
            return success;
        }

        /// <summary>
        /// Download queued MyEMSL files to the specified directory
        /// </summary>
        /// <param name="downloadDirectoryPath">Target directory path to save the files in</param>
        /// <param name="directoryLayout">Directory layout to use</param>
        /// <returns>True if success (including an empty queue), false if an error</returns>
        /// <remarks>Any queued files that have explicit download paths will be downloaded to the explicit path and not downloadDirectoryPath</remarks>
        protected bool ProcessMyEMSLDownloadQueue(string downloadDirectoryPath, Downloader.DownloadLayout directoryLayout)
        {
            if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count == 0)
                return true;

            if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count == 1)
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading one file from MyEMSL"));
            else
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading " + m_MyEMSLDatasetInfoCache.FilesToDownload.Count + " files from MyEMSL"));

            var success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(downloadDirectoryPath, directoryLayout);

            foreach (var errorMessage in m_MyEMSLDatasetInfoCache.ErrorMessages)
            {
                ReportMageWarning(errorMessage);
            }

            System.Threading.Thread.Sleep(10);

            return success;
        }

        #region "Event Handlers"

        void MyEMSLDatasetInfoCache_ErrorEvent(string message, Exception ex)
        {
            ReportMageWarning("MyEMSL downloader: " + message);
        }

        void MyEMSLDatasetInfoCache_MessageEvent(string message)
        {
            if (!message.Contains("Downloading ") && !message.Contains("Overwriting ") && !message.Contains("Skipping "))
            {
                if (message.Contains("Warning,") || message.Contains("Error ") || message.Contains("Failure downloading") || message.Contains("Failed to"))
                    ReportMageWarning("MyEMSL downloader: " + message);
                else
                    OnStatusMessageUpdated(new MageStatusEventArgs("MyEMSL downloader: " + message));
            }
        }

        void MyEMSLDatasetInfoCache_ProgressEvent(string progressMessage, float percentComplete)
        {
            if (percentComplete >= 100 - Single.Epsilon)
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: 100% complete"));
            else
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: " + percentComplete.ToString("0.00") + "% complete"));
        }
        #endregion

    }
}
