using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MyEMSLReader;

namespace Mage
{

    /// <summary>
    /// This module copies one or more input files to an output folder
    ///
    /// Its FileContentProcessor base class provides the basic functionality
    ///
    /// The OutputMode parameter tells this module whether or not to append a prefix to
    /// each output file name to avoid name collisions when input files can come from
    /// more than one input folder
    ///
    /// If IDColumnName parameter is set, it specifies a column in the standard input data
    /// whose value should be used in the prefix.  Otherwise the prefix is generated.
    /// </summary>
    public class FileCopy : FileContentProcessor
    {

        #region Member Variables

        // Used to provide unique prefix for duplicate file names
        private int tagIndex;

        #endregion

        #region Properties


        /// <summary>
        /// Name of column to be used for output file name prefix (optional)
        /// </summary>
        public string ColumnToUseForPrefix { get; set; }

        /// <summary>
        /// Whether or not to apply prefix to output file ("Yes" or "No")
        /// </summary>
        public string ApplyPrefixToFileName { set; get; }

        /// <summary>
        /// Literal text to apply as first part of prefix (optional)
        /// </summary>
        public string PrefixLeader { set; get; }

        /// <summary>
        /// Whether or not to overwrite existing files ("Yes" or "No")
        /// </summary>
        public bool OverwriteExistingFiles { set; get; }

        /// <summary>
        /// Whether or not to open CacheInfo.txt files, read the file pointer, and copy the target file
        /// </summary>
        public bool ResolveCacheInfoFiles { set; get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Mage file copy module
        /// </summary>
        public FileCopy()
        {
            ColumnToUseForPrefix = "";
            ApplyPrefixToFileName = "";
            SetOutputFileNamer(GetDestFile);
            PrefixLeader = "";
            OverwriteExistingFiles = false;
            ResolveCacheInfoFiles = false;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Copy given file to output
        /// </summary>
        /// <param name="sourceFile">name of input file</param>
        /// <param name="sourcePath">containing folder for input file</param>
        /// <param name="destPath">containing folder for output file</param>
        /// <param name="context">metadata associated with input file (used for column mapping)</param>
        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
            try
            {
                var bShowDoneMsg = true;

                if (sourcePath.StartsWith(MYEMSL_PATH_FLAG))
                {
                    var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(sourcePath);
                    string destPathClean;
                    DatasetInfoBase.ExtractMyEMSLFileID(destPath, out destPathClean);

                    if (!OverwriteExistingFiles && File.Exists(destPathClean))
                    {
                        UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping existing file: " + destPathClean, 0));
                        OnWarningMessage(new MageStatusEventArgs("WARNING->Skipping existing file: " + destPathClean));
                        System.Threading.Thread.Sleep(1);
                        bShowDoneMsg = false;
                    }
                    else
                    {
                        DatasetDirectoryOrFileInfo cachedFileInfo;
                        if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out cachedFileInfo))
                        {
                            UpdateStatus(this, new MageStatusEventArgs("Queuing file for Download->" + sourceFile));

                            // Note: Explicitly defining the target path to save the file at using filePathLocal
                            const bool unzipRequired = false;
                            m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(myEMSLFileID, cachedFileInfo.FileInfo, unzipRequired, destPathClean);
                        }
                        else
                        {
                            UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping file since not in MyEMSL Memory Cache: " + destPathClean, 0));
                            OnWarningMessage(new MageStatusEventArgs("WARNING->Skipping file since not in MyEMSL Memory Cache: " + destPathClean));
                            System.Threading.Thread.Sleep(1);
                            bShowDoneMsg = false;
                        }

                    }

                    if (ResolveCacheInfoFiles && destPathClean.EndsWith("CacheInfo.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        var fileCopied = ProcessCacheInfoFile(destPathClean, OverwriteExistingFiles);
                        if (!fileCopied)
                            bShowDoneMsg = false;
                    }
                }
                else
                {
                    UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + sourceFile));
                    if (OverwriteExistingFiles)
                    {
                        var bFileExists = File.Exists(destPath);
                        File.Copy(sourcePath, destPath, true);
                        if (bFileExists)
                        {
                            UpdateStatus(this, new MageStatusEventArgs("NOTE->Copy replaced existing file: " + destPath, 0));
                            System.Threading.Thread.Sleep(1);
                            bShowDoneMsg = false;
                        }
                    }
                    else
                    {
                        if (File.Exists(destPath))
                        {
                            UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping existing file: " + destPath, 0));
                            OnWarningMessage(new MageStatusEventArgs("WARNING->Skipping existing file: " + destPath));
                            System.Threading.Thread.Sleep(1);
                            bShowDoneMsg = false;
                        }
                        else
                        {
                            File.Copy(sourcePath, destPath, false);
                        }
                    }

                    if (ResolveCacheInfoFiles && destPath.EndsWith("CacheInfo.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        var fileCopied = ProcessCacheInfoFile(destPath, OverwriteExistingFiles);
                        if (!fileCopied)
                            bShowDoneMsg = false;
                    }
                }

                if (bShowDoneMsg)
                    UpdateStatus(this, new MageStatusEventArgs("Done->" + sourceFile));

            }
            catch (FileNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->File Not Found: " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->File Not Found: " + sourcePath));
                System.Threading.Thread.Sleep(250);
            }
            catch (DirectoryNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->Folder Not Found: " + sourcePath, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->Folder Not Found: " + sourcePath));
                System.Threading.Thread.Sleep(250);
            }
            catch (IOException e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->I/O Exception: " + e.Message + " -- " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->I/O Exception: " + e.Message + " -- " + sourceFile));
                System.Threading.Thread.Sleep(250);
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->" + e.Message + " -- " + sourceFile));
                System.Threading.Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Copy given folder to output
        /// </summary>
        /// <param name="sourcePath">input folder</param>
        /// <param name="destPath">output folder</param>
        protected override void ProcessFolder(string sourcePath, string destPath)
        {

            var source = new DirectoryInfo(sourcePath);
            var target = new DirectoryInfo(destPath);

            if (sourcePath.StartsWith(MYEMSL_PATH_FLAG))
                CopyAllMyEMSL(source, target);
            else
                CopyAll(source, target);

        }

        /// <summary>
        /// Determine the name to be used for the destination file
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="fieldPos"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected string GetDestFile(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
        {
            if (OptionEnabled(ApplyPrefixToFileName))
            {
                string prefix;
                if (InputColumnPos.ContainsKey(ColumnToUseForPrefix))
                {
                    var leader = (!string.IsNullOrEmpty(PrefixLeader)) ? PrefixLeader + "_" : "";
                    prefix = leader + fields[fieldPos[ColumnToUseForPrefix]];

                    // Replace any invalid characters with an underscore
                    foreach (var chInvalidChar in Path.GetInvalidFileNameChars())
                        prefix = prefix.Replace(chInvalidChar, '_');

                }
                else
                {
                    prefix = "Tag_" + (tagIndex++);
                }
                return prefix + "_" + sourceFile;
            }

            return sourceFile;
        }

        /// <summary>
        /// Copy folder given by source to target
        /// </summary>
        /// <param name="source">Path to folder to be copied</param>
        /// <param name="target">Path that folder will be copied to</param>
        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {

            var sourceFile = "??";
            var sourcePath = "??";

            try
            {
                // Check if the target directory exists, if not, create it.
                if (!target.Exists)
                {
                    target.Create();
                }
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + target.Name, 1));
                OnWarningMessage(new MageStatusEventArgs("Directory copy failed->" + e.Message + " -- " + target.FullName));
                System.Threading.Thread.Sleep(250);
                return;
            }

            try
            {
                // Copy each file into its new directory.
                foreach (var fi in source.GetFiles())
                {
                    sourceFile = fi.Name;
                    sourcePath = fi.FullName;

                    UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + fi.Name));
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                    UpdateStatus(this, new MageStatusEventArgs("Done->" + fi.Name));
                }

                // Copy each subdirectory using recursion.
                foreach (var diSourceSubDir in source.GetDirectories())
                {
                    try
                    {
                        var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                        CopyAll(diSourceSubDir, nextTargetSubDir);

                    }
                    catch (Exception e)
                    {
                        UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + diSourceSubDir.Name, 1));
                        OnWarningMessage(new MageStatusEventArgs("Subdirectory copy failed->" + e.Message + " -- " + diSourceSubDir.FullName));
                        System.Threading.Thread.Sleep(250);
                    }

                }

            }
            catch (FileNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->File Not Found: " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->File Not Found: " + sourcePath));
                System.Threading.Thread.Sleep(250);
            }
            catch (DirectoryNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->Folder Not Found: " + sourcePath, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->Folder Not Found: " + sourcePath));
                System.Threading.Thread.Sleep(250);
            }
            catch (IOException e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->I/O Exception: " + e.Message + " -- " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->I/O Exception: " + e.Message + " -- " + sourceFile));
                System.Threading.Thread.Sleep(250);
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + sourceFile, 1));
                OnWarningMessage(new MageStatusEventArgs("Copy failed->" + e.Message + " -- " + sourceFile));
                System.Threading.Thread.Sleep(250);
            }

        }

        /// <summary>
        /// Copy MyEMSL folder given by source to target
        /// </summary>
        /// <param name="sourceFolder">Path to folder to be copied</param>
        /// <param name="target">Path that folder will be copied to</param>
        private void CopyAllMyEMSL(DirectoryInfo sourceFolder, DirectoryInfo target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            try
            {
                // Check if the target directory exists, if not, create it.
                if (!target.Exists)
                {
                    target.Create();
                }
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + target.Name, 1));
                OnWarningMessage(new MageStatusEventArgs("Directory copy failed->" + e.Message + " -- " + target.FullName));
                System.Threading.Thread.Sleep(250);
                return;
            }

            try
            {
                // Download the files
                string subDir;
                string parentFolders;
                var datasetName = DetermineDatasetName(sourceFolder.FullName);

                GetMyEMSLParentFoldersAndSubDir(sourceFolder.FullName, datasetName, out subDir, out parentFolders);

                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles("*", subDir, datasetName, true);

                if (m_RecentlyFoundMyEMSLFiles.Count > 0)
                {
                    foreach (var archiveFile in m_RecentlyFoundMyEMSLFiles)
                    {
                        if (!archiveFile.IsDirectory)
                            m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(archiveFile.FileInfo);
                    }

                    if (!string.IsNullOrEmpty(subDir))
                    {
                        if (target.Parent == null)
                            throw new NullReferenceException("parent directory of " + target.FullName + " is null");

                        // The downloader will append the subfolder name, thus use target.Parent
                        target = target.Parent;
                    }

                    var success = ProcessMyEMSLDownloadQueue(target.FullName, Downloader.DownloadLayout.SingleDataset);

                    if (success) return;

                    var message = "MyEMSL Download Error";
                    if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                    {
                        message += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages.First();
                    }

                    UpdateStatus(this, new MageStatusEventArgs("FAILED->" + message, 1));
                    OnWarningMessage(new MageStatusEventArgs(message));
                }

            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message, 1));
                OnWarningMessage(new MageStatusEventArgs("MyEMSL download failed->" + e.Message));
                System.Threading.Thread.Sleep(250);
            }
        }


        /// <summary>
        /// Open CacheInfo.txt files, read the file pointer, and copy the target file to the destination folder
        /// </summary>
        /// <param name="cacheInfoFilePath"></param>
        /// <param name="overwriteExistingFiles"></param>
        /// <returns>
        /// True if success, false if an error, including that the remote file was not found
        /// Also returns false if the target file already exists in the destination folder and overwriteExistingFiles is false
        /// </returns>
        private bool ProcessCacheInfoFile(string cacheInfoFilePath, bool overwriteExistingFiles)
        {
            try
            {
                var cacheInfoFile = new FileInfo(cacheInfoFilePath);
                if (!cacheInfoFile.Exists)
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file not found: " + cacheInfoFilePath, 0));
                    OnWarningMessage(new MageStatusEventArgs("WARNING->CacheInfo file not found: " + cacheInfoFilePath));
                    return false;
                }

                if (cacheInfoFile.DirectoryName == null)
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file directory is null: " + cacheInfoFilePath, 0));
                    OnWarningMessage(new MageStatusEventArgs("WARNING->CacheInfo file directory is null: " + cacheInfoFilePath));
                    return false;
                }

                using (var reader = new StreamReader(new FileStream(cacheInfoFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    if (reader.EndOfStream)
                    {
                        UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file is empty: " + cacheInfoFilePath, 0));
                        OnWarningMessage(new MageStatusEventArgs("WARNING->CacheInfo fileis empty: " + cacheInfoFilePath));
                        return false;
                    }

                    var remoteFilePath = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(remoteFilePath))
                    {
                        UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file is empty: " + cacheInfoFilePath, 0));
                        OnWarningMessage(new MageStatusEventArgs("WARNING->CacheInfo fileis empty: " + cacheInfoFilePath));
                        return false;
                    }

                    var remoteFile = new FileInfo(remoteFilePath);
                    if (!remoteFile.Exists)
                    {
                        UpdateStatus(this, new MageStatusEventArgs("Remote file not found: " + remoteFilePath, 1));
                        OnWarningMessage(new MageStatusEventArgs("Remote file not found: " + remoteFilePath));
                        return false;
                    }

                    var destPath = Path.Combine(cacheInfoFile.DirectoryName, remoteFile.Name);

                    if (File.Exists(destPath))
                    {
                        if (overwriteExistingFiles)
                        {
                            UpdateStatus(this, new MageStatusEventArgs("NOTE->Copy replaced existing file: " + destPath, 0));
                        }
                        else
                        {
                            UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping existing file: " + destPath, 0));
                            OnWarningMessage(new MageStatusEventArgs("WARNING->Skipping existing file: " + destPath));
                            return false;
                        }
                        UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + remoteFile.Name));
                        File.Copy(remoteFile.FullName, destPath, true);

                    }
                    else
                    {
                        UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + remoteFile.Name));
                        File.Copy(remoteFile.FullName, destPath, false);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + cacheInfoFilePath, 1));
                OnWarningMessage(new MageStatusEventArgs("Cache info processing failed->" + e.Message + " -- " + cacheInfoFilePath));
                System.Threading.Thread.Sleep(250);
                return false;
            }
        }

        #endregion

    }
}
