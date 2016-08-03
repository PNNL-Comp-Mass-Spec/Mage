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
        /// Folder name to use when caching MyEMSL files locally
        /// </summary>
        public const string MAGE_TEMP_FILES_FOLDER = "Mage_Temp_Files";

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
        protected static List<DatasetFolderOrFileInfo> m_RecentlyFoundMyEMSLFiles;

        /// <summary>
        /// All MyEMSL fils that pass filters; keys are MyEMSL File IDs, values are the MyEMSL Info.  Items will be auto-purged from this list if the list grows to over 1 million records
        /// </summary>
        /// <remarks>This dictionary is used by the FileCopy class to determine the archived file info for a file in MyEMSL using MyEMSLFile ID</remarks>
        protected static Dictionary<Int64, DatasetFolderOrFileInfo> m_FilterPassingMyEMSLFiles;

        /// <summary>
        /// RegEx to extract the dataset name from a path similar to these examples:
        ///   \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04
        ///   \\MyEMSL\VPro01\2013_3\QC_Shew_13_04d_500ng_10Sep13_Tiger_13-07-34
        ///   \\aurora.emsl.pnl.gov\archive\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
        ///   \\a2.emsl.pnl.gov\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
        /// The RegEx can also be used to determine the portion of a path that includes parent folders and the dataset folder
        /// </summary>
        private readonly Regex mDatasetMatchStrict = new Regex(@"\\\\[^\\]+(?:\\[^\\]+)?(?:\\[^\\]+)?\\[^\\]+\\2[0-9][0-9][0-9]_[1-4]\\(?<Dataset>[^\\]+)", RegexOptions.Compiled);

        /// <summary>
        /// RegEx to extract the dataset name from a path of the form \2013_3\QC_Shew_13_04f_500ng_10Sep13_Tiger_13-07-34
        /// /// The RegEx can also be used to determine the portion of a path that includes parent folders and the dataset folder
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
                m_MyEMSLDatasetInfoCache.ErrorEvent += m_MyEMSLDatasetInfoCache_ErrorEvent;
                m_MyEMSLDatasetInfoCache.MessageEvent += m_MyEMSLDatasetInfoCache_MessageEvent;
            }

            // As mentioned above, we need to attach this event every time the class is instantiated
            // This will result in duplicate calls to m_MyEMSLDatasetInfoCache_ProgressEvent by m_MyEMSLDatasetInfoCache.ProgressEvent but that doesn't hurt anything
            m_MyEMSLDatasetInfoCache.ProgressEvent += m_MyEMSLDatasetInfoCache_ProgressEvent;
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
                m_FilterPassingMyEMSLFiles = new Dictionary<long, DatasetFolderOrFileInfo>();


            DatasetFolderOrFileInfo fileInfoCached;
            if (!m_FilterPassingMyEMSLFiles.TryGetValue(fileInfo.FileID, out fileInfoCached))
            {
                m_FilterPassingMyEMSLFiles.Add(fileInfo.FileID, new DatasetFolderOrFileInfo(fileInfo.FileID, false, fileInfo));
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
        /// Determine the name of a dataset given a folder path
        /// </summary>
        /// <param name="folderPath">Folder path to examine</param>
        /// <returns>The dataset name if found; empty string if the dataset name could not be determined</returns>
        protected string DetermineDatasetName(string folderPath)
        {
            var datasetName = string.Empty;

            // Parse the folderPath with a RegEx to extract the dataset name
            var reMatch = mDatasetMatchStrict.Match(folderPath);

            if (!reMatch.Success)
                reMatch = mDatasetMatchLoose.Match(folderPath);

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
        /// <param name="folderPath">Folder path use if bufferRow does not contain a dataset column</param>
        /// <returns>The dataset name if found; empty string if the dataset name could not be determined</returns>
        protected string DetermineDatasetName(string[] bufferRow, string folderPath)
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
                datasetName = DetermineDatasetName(folderPath);
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
                string filePathClean;
                var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filePathRemote, out filePathClean);

                if (myEMSLFileID <= 0)
                    throw new MageException("MyEMSL File does not have the MyEMSL FileID tag (" + MyEMSLReader.DatasetInfoBase.MYEMSL_FILEID_TAG + "): " + filePathRemote);

                DatasetFolderOrFileInfo cachedFileInfo;
                if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out cachedFileInfo))
                {
                    filePathLocal = Path.Combine(Path.GetTempPath(), MAGE_TEMP_FILES_FOLDER, cachedFileInfo.FileInfo.Dataset, Path.GetFileName(filePathClean));

                    OnStatusMessageUpdated(new MageStatusEventArgs("Downloading file from MyEMSL: " + cachedFileInfo.FileInfo.RelativePathWindows));

                    // Note: Explicitly defining the target path to save the file at using filePathLocal
                    const bool unzipRequired = false;
                    m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(myEMSLFileID, cachedFileInfo.FileInfo, unzipRequired, filePathLocal);

                    // Note that the target folder path will be ignored since we explicitly defined the destination file path when queuing the file
                    var success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(".", Downloader.DownloadFolderLayout.SingleDataset);
                    if (!success)
                    {
                        var msg = "Failed to download file " + cachedFileInfo.FileInfo.RelativePathWindows + " from MyEMSL";
                        if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                            msg += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages.First();
                        else
                            msg += ": Unknown Error";

                        throw new MageException(msg);
                    }
                }
                else
                {
                    throw new MageException("Cannot download file since not in MyEMSL Memory Cache: " + filePathRemote);
                }
            }
            else
            {
                filePathLocal = filePathRemote;
            }

            return filePathLocal;
        }

        /// <summary>
        /// Examines the folder path to find the parent folders, including the dataset folder
        /// For example, given   \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04\SIC201307301439_Auto965355
        /// The path returned is \\proto-7\VOrbi05\2013_3\QC_Shew_13_02_500ng_4_HCD_26Jul13_Lynx_13-02-04
        /// </summary>
        /// <param name="folderPath">Path to examine</param>
        /// <returns>Parent folders, including the dataset folder</returns>
        protected string ExtractParentDatasetFolders(string folderPath)
        {
            var parentFolders = string.Empty;

            // Parse the folderPath with a RegEx to extract the parent folders
            var reMatch = mDatasetMatchStrict.Match(folderPath);

            if (!reMatch.Success)
                reMatch = mDatasetMatchLoose.Match(folderPath);

            if (reMatch.Success)
            {
                parentFolders = reMatch.ToString();
            }

            return parentFolders;
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
        /// Examines the path to determine the parent folders and possible subdirectory for a given dataset
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="datasetName"></param>
        /// <param name="subDir"></param>
        /// <param name="parentFolders"></param>
        protected void GetMyEMSLParentFoldersAndSubDir(string folderPath, string datasetName, out string subDir, out string parentFolders)
        {
            subDir = string.Empty;

            parentFolders = ExtractParentDatasetFolders(folderPath);
            if (string.IsNullOrEmpty(parentFolders))
            {
                parentFolders = MYEMSL_PATH_FLAG + @"\Instrument\2013_1\" + datasetName;
            }
            else
            {
                if (folderPath.Length > parentFolders.Length)
                {
                    subDir = folderPath.Substring(parentFolders.Length);
                    subDir = subDir.TrimStart('\\');
                }
                parentFolders = parentFolders.TrimEnd('\\');
            }
        }

        /// <summary>
        /// Called by pipeline container after pipeline execution has processed all of the data rows
        /// Download any queued MyEMSL files
        /// </summary>
        public override bool PostProcess()
        {
            // Note that the target folder path will most likely be ignored since explicit destination file paths were likely used when files were added to the queue
            var success = ProcessMyEMSLDownloadQueue(".", Downloader.DownloadFolderLayout.SingleDataset);
            return success;
        }

        /// <summary>
        /// Download queued MyEMSL files to the specified folder
        /// </summary>
        /// <param name="downloadFolderPath">Target folder path to save the files in</param>
        /// <param name="folderLayout">Folder layout to use</param>
        /// <returns>True if success (including an empty queue), false if an error</returns>
        /// <remarks>Any queued files that have explicit download paths will be downloaded to the explicit path and not downloadFolderPath</remarks>
        protected bool ProcessMyEMSLDownloadQueue(string downloadFolderPath, Downloader.DownloadFolderLayout folderLayout)
        {
            if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count == 0)
                return true;

            if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count == 1)
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading one file from MyEMSL"));
            else
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading " + m_MyEMSLDatasetInfoCache.FilesToDownload.Count + " files from MyEMSL"));

            var success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(downloadFolderPath, folderLayout);

            foreach (var errorMessage in m_MyEMSLDatasetInfoCache.ErrorMessages)
            {
                OnWarningMessage(new MageStatusEventArgs(errorMessage));
            }

            System.Threading.Thread.Sleep(10);

            return success;
        }

        #region "Event Handlers"


        void m_MyEMSLDatasetInfoCache_ErrorEvent(object sender, MessageEventArgs e)
        {
            OnWarningMessage(new MageStatusEventArgs("MyEMSL downloader: " + e.Message));
        }

        void m_MyEMSLDatasetInfoCache_MessageEvent(object sender, MessageEventArgs e)
        {
            if (!e.Message.Contains("Downloading ") && !e.Message.Contains("Overwriting ") && !e.Message.Contains("Skipping "))
            {
                if (e.Message.Contains("Warning,") || e.Message.Contains("Error ") || e.Message.Contains("Failure downloading") || e.Message.Contains("Failed to"))
                    OnWarningMessage(new MageStatusEventArgs("MyEMSL downloader: " + e.Message));
                else
                    OnStatusMessageUpdated(new MageStatusEventArgs("MyEMSL downloader: " + e.Message));
            }
        }

        void m_MyEMSLDatasetInfoCache_ProgressEvent(object sender, ProgressEventArgs e)
        {
            if (e.PercentComplete >= 100 - Single.Epsilon)
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: 100% complete"));
            else
                OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: " + e.PercentComplete.ToString("0.00") + "% complete"));
        }
        #endregion

    }


}
