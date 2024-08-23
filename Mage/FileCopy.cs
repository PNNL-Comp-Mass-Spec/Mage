using System;
using System.Collections.Generic;
using System.IO;
using MyEMSLReader;
using PRISM;

namespace Mage
{
    /// <summary>
    /// <para>
    /// This module copies one or more input files to an output directory
    /// </para>
    /// <para>
    /// Its FileContentProcessor base class provides the basic functionality
    /// </para>
    /// <para>
    /// The OutputMode parameter tells this module whether a prefix should be appended to
    /// each output file name to avoid name collisions when input files can come from
    /// more than one input directory
    /// </para>
    /// <para>
    /// If IDColumnName parameter is set, it specifies a column in the standard input data
    /// whose value should be used in the prefix. Otherwise, the prefix is generated.
    /// </para>
    /// </summary>
    public class FileCopy : FileContentProcessor
    {
        // Ignore Spelling: dest, downloader, Mage

        // Used to provide unique prefix for duplicate file names
        private int tagIndex;

        /// <summary>
        /// Name of column to be used for output file name prefix (optional)
        /// </summary>
        public string ColumnToUseForPrefix { get; set; }

        /// <summary>
        /// Whether to apply prefix to output file ("Yes" or "No")
        /// </summary>
        public string ApplyPrefixToFileName { set; get; }

        /// <summary>
        /// Literal text to apply as first part of prefix (optional)
        /// </summary>
        public string PrefixLeader { set; get; }

        /// <summary>
        /// Whether to overwrite existing files ("Yes" or "No")
        /// </summary>
        public bool OverwriteExistingFiles { set; get; }

        /// <summary>
        /// When true, open CacheInfo.txt files, read the file pointer, and copy the target file
        /// </summary>
        public bool ResolveCacheInfoFiles { set; get; }

        /// <summary>
        /// Construct a new Mage file copy module
        /// </summary>
        public FileCopy()
        {
            ColumnToUseForPrefix = string.Empty;
            ApplyPrefixToFileName = string.Empty;
            SetOutputFileNamer(GetDestFile);
            PrefixLeader = string.Empty;
            OverwriteExistingFiles = false;
            ResolveCacheInfoFiles = false;
        }

        /// <summary>
        /// Copy given file to output
        /// </summary>
        /// <param name="sourceFile">Name of input file</param>
        /// <param name="sourcePath">Directory with the input file</param>
        /// <param name="destPath">Target directory</param>
        /// <param name="context">Metadata associated with input file (used for column mapping)</param>
        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
            try
            {
                var bShowDoneMsg = true;
                var destPathSafe = GetFileSafeLongPath(destPath);

                if (sourcePath.StartsWith(MYEMSL_PATH_FLAG))
                {
                    var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(sourcePath);
                    DatasetInfoBase.ExtractMyEMSLFileID(destPathSafe, out var destPathClean);

                    if (!OverwriteExistingFiles && File.Exists(destPathClean))
                    {
                        UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping existing file: " + destPathClean, 0));
                        ReportMageWarning("WARNING->Skipping existing file: " + destPathClean);
                        System.Threading.Thread.Sleep(1);
                        bShowDoneMsg = false;
                    }
                    else
                    {
                        if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out var cachedFileInfo))
                        {
                            UpdateStatus(this, new MageStatusEventArgs("Queuing file for Download->" + sourceFile));

                            // Note: Explicitly defining the target path to save the file at using filePathLocal
                            const bool unzipRequired = false;
                            m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(myEMSLFileID, cachedFileInfo.FileInfo, unzipRequired, destPathClean);
                        }
                        else
                        {
                            UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping file since not in MyEMSL Memory Cache: " + destPathClean, 0));
                            ReportMageWarning("WARNING->Skipping file since not in MyEMSL Memory Cache: " + destPathClean);
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
                    var sourceFi = GetFileInfo(sourcePath);
                    if (OverwriteExistingFiles)
                    {
                        var bFileExists = File.Exists(destPathSafe);
                        sourceFi.CopyTo(destPathSafe, true);
                        if (bFileExists)
                        {
                            UpdateStatus(this, new MageStatusEventArgs("NOTE->Copy replaced existing file: " + destPath, 0));
                            System.Threading.Thread.Sleep(1);
                            bShowDoneMsg = false;
                        }
                    }
                    else
                    {
                        if (File.Exists(destPathSafe))
                        {
                            UpdateStatus(this, new MageStatusEventArgs("WARNING->Skipping existing file: " + destPath, 0));
                            ReportMageWarning("WARNING->Skipping existing file: " + destPath);
                            System.Threading.Thread.Sleep(1);
                            bShowDoneMsg = false;
                        }
                        else
                        {
                            sourceFi.CopyTo(destPathSafe, false);
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
                ReportMageWarning("Copy failed->File Not Found: " + sourcePath);
                System.Threading.Thread.Sleep(250);
            }
            catch (DirectoryNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->Directory Not Found: " + sourcePath, 1));
                ReportMageWarning("Copy failed->Directory Not Found: " + sourcePath);
                System.Threading.Thread.Sleep(250);
            }
            catch (IOException e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->I/O Exception: " + e.Message + " -- " + sourceFile, 1));
                ReportMageWarning("Copy failed->I/O Exception: " + e.Message + " -- " + sourceFile);
                System.Threading.Thread.Sleep(250);
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + sourceFile, 1));
                ReportMageWarning("Copy failed->" + e.Message + " -- " + sourceFile);
                System.Threading.Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Copy contents of given directory to the target
        /// </summary>
        /// <param name="sourcePath">Source directory path</param>
        /// <param name="destPath">Target directory path</param>
        protected override void ProcessDirectory(string sourcePath, string destPath)
        {
            var source = GetDirectoryInfo(sourcePath);
            var target = GetDirectoryInfo(destPath);

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
                    {
                        prefix = prefix.Replace(chInvalidChar, '_');
                    }
                }
                else
                {
                    prefix = "Tag_" + tagIndex++;
                }
                return prefix + "_" + sourceFile;
            }

            return sourceFile;
        }

        /// <summary>
        /// Copy source directory to target path
        /// </summary>
        /// <param name="source">Source directory</param>
        /// <param name="target">Target directory</param>
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
                ReportMageWarning("Directory copy failed->" + e.Message + " -- " + target.FullName);
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
                    var fiSafe = GetFileInfo(fi.FullName);
                    var destPath = Path.Combine(target.FullName, fi.Name);
                    var destSafe = GetFileSafeLongPath(destPath);

                    UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + fi.Name));
                    fiSafe.CopyTo(destSafe, true);
                    UpdateStatus(this, new MageStatusEventArgs("Done->" + fi.Name));
                }

                // Copy each subdirectory using recursion.
                foreach (var diSourceSubDir in source.GetDirectories())
                {
                    try
                    {
                        var diSourceSubDirSafe = GetDirectoryInfo(diSourceSubDir.FullName);
                        var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                        var nextTargetSubDirSafe = GetDirectoryInfo(nextTargetSubDir.FullName);
                        CopyAll(diSourceSubDirSafe, nextTargetSubDirSafe);
                    }
                    catch (Exception e)
                    {
                        UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + diSourceSubDir.Name, 1));
                        ReportMageWarning("Subdirectory copy failed->" + e.Message + " -- " + diSourceSubDir.FullName);
                        System.Threading.Thread.Sleep(250);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->File Not Found: " + sourceFile, 1));
                ReportMageWarning("Copy failed->File Not Found: " + sourcePath);
                System.Threading.Thread.Sleep(250);
            }
            catch (DirectoryNotFoundException)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->Directory Not Found: " + sourcePath, 1));
                ReportMageWarning("Copy failed->Directory Not Found: " + sourcePath);
                System.Threading.Thread.Sleep(250);
            }
            catch (IOException e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->I/O Exception: " + e.Message + " -- " + sourceFile, 1));
                ReportMageWarning("Copy failed->I/O Exception: " + e.Message + " -- " + sourceFile);
                System.Threading.Thread.Sleep(250);
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + sourceFile, 1));
                ReportMageWarning("Copy failed->" + e.Message + " -- " + sourceFile);
                System.Threading.Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Copy MyEMSL directory given by source to target
        /// </summary>
        /// <param name="sourceDirectory">Source directory</param>
        /// <param name="target">Target directory</param>
        private void CopyAllMyEMSL(FileSystemInfo sourceDirectory, DirectoryInfo target)
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
                ReportMageWarning("Directory copy failed->" + e.Message + " -- " + target.FullName);
                System.Threading.Thread.Sleep(250);
                return;
            }

            try
            {
                var datasetName = DetermineDatasetName(sourceDirectory.FullName);

                GetMyEMSLParentDirectoriesAndSubDir(sourceDirectory.FullName, datasetName, out var subDir, out _);

                var fileIDList = string.Empty;

                m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles("*", subDir, datasetName, fileIDList, true);

                if (m_RecentlyFoundMyEMSLFiles.Count == 0)
                {
                    return;
                }

                foreach (var archiveFile in m_RecentlyFoundMyEMSLFiles)
                {
                    if (!archiveFile.IsDirectory)
                        m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(archiveFile.FileInfo);
                }

                if (!string.IsNullOrEmpty(subDir))
                {
                    // The downloader will append the subdirectory name, thus use target.Parent
                    target = target.Parent ?? throw new NullReferenceException("parent directory of " + target.FullName + " is null");
                }

                var success = ProcessMyEMSLDownloadQueue(target.FullName, Downloader.DownloadLayout.SingleDataset);

                if (success) return;

                var message = "MyEMSL Download Error";
                if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                {
                    message += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages[0];
                }

                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + message, 1));
                ReportMageWarning(message);
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message, 1));
                ReportMageWarning("MyEMSL download failed->" + e.Message);
                System.Threading.Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Open CacheInfo.txt files, read the file pointer, and copy the target file to the destination directory
        /// </summary>
        /// <param name="cacheInfoFilePath"></param>
        /// <param name="overwriteExistingFiles"></param>
        /// <returns>
        /// True if success, false if an error, including that the remote file was not found
        /// Also returns false if the target file already exists in the destination directory and overwriteExistingFiles is false
        /// </returns>
        private bool ProcessCacheInfoFile(string cacheInfoFilePath, bool overwriteExistingFiles)
        {
            try
            {
                var cacheInfoFile = new FileInfo(cacheInfoFilePath);
                if (!cacheInfoFile.Exists)
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file not found: " + cacheInfoFilePath, 0));
                    ReportMageWarning("WARNING->CacheInfo file not found: " + cacheInfoFilePath);
                    return false;
                }

                if (cacheInfoFile.DirectoryName == null)
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file directory is null: " + cacheInfoFilePath, 0));
                    ReportMageWarning("WARNING->CacheInfo file directory is null: " + cacheInfoFilePath);
                    return false;
                }

                using var reader = new StreamReader(new FileStream(cacheInfoFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                if (reader.EndOfStream)
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file is empty: " + cacheInfoFilePath, 0));
                    ReportMageWarning("WARNING->CacheInfo file is empty: " + cacheInfoFilePath);
                    return false;
                }

                var remoteFilePath = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(remoteFilePath))
                {
                    UpdateStatus(this, new MageStatusEventArgs("WARNING->CacheInfo file is empty: " + cacheInfoFilePath, 0));
                    ReportMageWarning("WARNING->CacheInfo file is empty: " + cacheInfoFilePath);
                    return false;
                }

                var remoteFile = new FileInfo(remoteFilePath);
                if (!remoteFile.Exists)
                {
                    UpdateStatus(this, new MageStatusEventArgs("Remote file not found: " + remoteFilePath, 1));
                    ReportMageWarning("Remote file not found: " + remoteFilePath);
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
                        ReportMageWarning("WARNING->Skipping existing file: " + destPath);
                        return false;
                    }
                    UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + remoteFile.Name));
                    remoteFile.CopyTo(destPath, true);
                }
                else
                {
                    UpdateStatus(this, new MageStatusEventArgs("Start Copy->" + remoteFile.Name));
                    remoteFile.CopyTo(destPath, false);
                }

                return true;
            }
            catch (Exception e)
            {
                UpdateStatus(this, new MageStatusEventArgs("FAILED->" + e.Message + " -- " + cacheInfoFilePath, 1));
                ReportMageWarning("Cache info processing failed->" + e.Message + " -- " + cacheInfoFilePath);
                System.Threading.Thread.Sleep(250);
                return false;
            }
        }

        /// <summary>
        /// Obtain a DirectoryInfo object for the given path
        /// </summary>
        /// <remarks>If the path length is over 210 and not on Linux, converts the path to a Win32 long path</remarks>
        /// <param name="directoryPath"></param>
        private static DirectoryInfo GetDirectoryInfo(string directoryPath)
        {
            return directoryPath.Length >= NativeIOFileTools.FILE_PATH_LENGTH_THRESHOLD - 50 && !SystemInfo.IsLinux
                ? new DirectoryInfo(NativeIOFileTools.GetWin32LongPath(directoryPath))
                : new DirectoryInfo(directoryPath);
        }

        /// <summary>
        /// Obtain a FileInfo object for the given path
        /// </summary>
        /// <remarks>If the path length is over 260 and not on Linux, converts the path to a Win32 long path</remarks>
        /// <param name="filePath"></param>
        private static FileInfo GetFileInfo(string filePath)
        {
            return filePath.Length >= NativeIOFileTools.FILE_PATH_LENGTH_THRESHOLD && !SystemInfo.IsLinux
                ? new FileInfo(NativeIOFileTools.GetWin32LongPath(filePath))
                : new FileInfo(filePath);
        }

        /// <summary>
        /// Obtain a DirectoryInfo object for the given path
        /// </summary>
        /// <remarks>If the path length is over 210 and not on Linux, converts the path to a Win32 long path</remarks>
        /// <param name="directoryPath"></param>
        // ReSharper disable once UnusedMember.Local
        private static string GetDirectorySafeLongPath(string directoryPath)
        {
            return directoryPath.Length >= NativeIOFileTools.FILE_PATH_LENGTH_THRESHOLD - 50 && !SystemInfo.IsLinux
                ? NativeIOFileTools.GetWin32LongPath(directoryPath)
                : directoryPath;
        }

        /// <summary>
        /// Obtain a FileInfo object for the given path
        /// </summary>
        /// <remarks>If the path length is over 260 and not on Linux, converts the path to a Win32 long path</remarks>
        /// <param name="filePath"></param>
        private static string GetFileSafeLongPath(string filePath)
        {
            return filePath.Length >= NativeIOFileTools.FILE_PATH_LENGTH_THRESHOLD && !SystemInfo.IsLinux
                ? NativeIOFileTools.GetWin32LongPath(filePath)
                : filePath;
        }
    }
}
