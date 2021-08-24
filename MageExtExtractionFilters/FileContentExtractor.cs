using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mage;
using MageExtContentFilters;
using MyEMSLReader;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Receives list of results files (and associated files) on standard tabular input
    /// and processes each one to individual or concatenated results
    /// </summary>
    public class FileContentExtractor : FileProcessingBase
    {
        // Ignore Spelling: Mage



        private int mCurrentFileCount;

        private FilterResultsBase mResultsChecker;

        private string mCurrentProgressText = string.Empty;



        /// <summary>
        /// Set of parameters specifying the user's choices for
        /// how the extraction should be performed
        /// </summary>
        public ExtractionType ExtractionParms { get; set; }

        /// <summary>
        /// Destination to which processed results will be delivered
        /// </summary>
        public DestinationType Destination { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input directory path
        /// (optional - defaults to "Directory"; previously was "Folder")
        /// </summary>
        public string SourceDirectoryColumnName { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input file name
        /// optional - defaults to "Name")
        /// </summary>
        public string SourceFileColumnName { get; set; }



        /// <summary>
        /// Column indexes
        /// </summary>
        private int mInputDirectoryIdx;
        private int mInputFileIdx;
        private int mJobIdx;

        private Dictionary<string, ResultType.MergeFile> mMergeFiles;

        private void PrecalculateColumnIndexes()
        {
            mInputDirectoryIdx = GetColumnIndex(InputColumnPos, SourceDirectoryColumnName);
            mInputFileIdx = GetColumnIndex(InputColumnPos, SourceFileColumnName);
            mJobIdx = GetColumnIndex(InputColumnPos, "Job");
        }

        private void PrecalculateMergeFileColumnIndexes()
        {
            mMergeFiles = new Dictionary<string, ResultType.MergeFile>();
            foreach (var mf in ExtractionParms.RType.MergeFileTypes)
            {
                if (InputColumnPos.Keys.Contains(mf.NameColumn))
                {
                    mf.ColumnIndex = InputColumnPos[mf.NameColumn];
                }
                mMergeFiles[mf.NameColumn] = mf;
            }
        }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            var columnDefs = OutputColumnDefs ?? InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
            PrecalculateColumnIndexes();
            PrecalculateMergeFileColumnIndexes();
            SetupCurrentResultsChecker();
        }

        /// <summary>
        /// Receives a data row representing a results file and one or more
        /// associated files that need to be merged into the output data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                var resultsDirectoryPath = args.Fields[mInputDirectoryIdx];
                var resultFileName = args.Fields[mInputFileIdx];
                string inputFilePath;

                var filesDownloadedFromMyEMSL = new List<string>();

                if (resultFileName == kNoFilesFound)
                    inputFilePath = string.Empty;
                else
                {
                    inputFilePath = Path.Combine(resultsDirectoryPath, resultFileName);

                    if (inputFilePath.StartsWith(MYEMSL_PATH_FLAG))
                    {
                        // We need to download the result file and any merge files from MyEMSL
                        // We'll download all of the files for the given dataset as a group
                        // These files should already have a MyEMSL File ID encoded in them

                        var filesToDownload = new List<string>
                        {
                            resultFileName
                        };

                        foreach (var mf in mMergeFiles.Values)
                        {
                            if (mf.ColumnIndex >= 0)
                            {
                                var mergeFileName = args.Fields[mf.ColumnIndex];
                                filesToDownload.Add(mergeFileName);
                            }
                        }

                        for (var i = 0; i < filesToDownload.Count; i++)
                        {
                            var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filesToDownload[i], out _);

                            if (myEMSLFileID <= 0)
                                throw new MageException("File does not have the MyEMSL FileID tag (" + DatasetInfoBase.MYEMSL_FILE_ID_TAG + "): " + filesToDownload[i]);

                            if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out var cachedFileInfo))
                            {
                                if (i == 0)
                                {
                                    // Override the result directory path
                                    resultsDirectoryPath = Path.Combine(Path.GetTempPath(), MAGE_TEMP_FILES_DIRECTORY, cachedFileInfo.FileInfo.Dataset);

                                    // Override the input file path
                                    inputFilePath = Path.Combine(resultsDirectoryPath, cachedFileInfo.FileInfo.Filename);
                                }

                                var filePathLocal = Path.Combine(resultsDirectoryPath, cachedFileInfo.FileInfo.Filename);

                                const bool unzipRequired = false;
                                m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(cachedFileInfo.FileID, cachedFileInfo.FileInfo, unzipRequired, filePathLocal);
                                filesDownloadedFromMyEMSL.Add(filePathLocal);
                            }
                            else
                                throw new MageException("Cannot download file since not in MyEMSL Memory Cache: " + filesToDownload[i]);
                        }

                        // Note that the target directory path will be ignored since we explicitly defined the destination file path when queuing the file
                        var success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(".", Downloader.DownloadLayout.SingleDataset);
                        if (!success)
                        {
                            var msg = "Failed to download Mage Extractor merge files from MyEMSL";
                            if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                                msg += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages.First();
                            else
                                msg += ": Unknown Error";

                            throw new MageException(msg);
                        }
                    }
                }

                // Add actual file name to merge file list
                foreach (var mf in mMergeFiles.Values)
                {
                    mf.MergeFileName = mf.ColumnIndex < 0 ? "" : args.Fields[mf.ColumnIndex];

                    var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(mf.MergeFileName, out var filePathClean);

                    if (myEMSLFileID > 0)
                        mf.MergeFileName = filePathClean;
                }

                if (resultFileName != kNoFilesFound)
                {
                    var job = args.Fields[mJobIdx];
                    SendProgressUpdate(job);

                    if (!File.Exists(inputFilePath))
                    {
                        OnWarningMessage(new MageStatusEventArgs("File not found for job " + job + ": " + inputFilePath));
                    }
                    else
                    {
                        // Build and run pipeline (in this thread) to process file contents
                        var resultsFileReader = new DelimitedFileReader
                        {
                            FilePath = inputFilePath
                        };

                        // Note that MSPathFinder jobs will not have an MSGF file, but we
                        // still need to include the MSGFExtractionFilter in the pipeline so that
                        // the data gets filtered
                        ExtractionFilter msgfFilter = new MSGFExtractionFilter
                        {
                            Job = job,
                            ResultsDirectoryPath = resultsDirectoryPath,
                            MergeFiles = mMergeFiles,
                            ExtractionType = ExtractionParms
                        };

                        var resultsFilter = ExtractionParms.RType.GetExtractionFilter(mResultsChecker);
                        resultsFilter.Job = job;
                        resultsFilter.ResultsDirectoryPath = resultsDirectoryPath;
                        resultsFilter.MergeFiles = mMergeFiles;
                        resultsFilter.ExtractionType = ExtractionParms;

                        var writer = Destination.GetDestinationWriterModule(job, inputFilePath);
                        var pipeline = ProcessingPipeline.Assemble("Extract File Contents", resultsFileReader, msgfFilter, resultsFilter, writer);

                        pipeline.OnWarningMessageUpdated += pipeline_OnWarningMessageUpdated;
                        pipeline.OnStatusMessageUpdated += pipeline_OnStatusMessageUpdated;

                        pipeline.RunRoot(null);

                        SendResultsUpdate(ExtractionParms.RType.ResultName, job, resultsFilter.ProcessedRows, resultsFilter.PassedRows);
                        SendResultsUpdate("MSGF", job, msgfFilter.ProcessedRows, msgfFilter.PassedRows);
                    }
                }

                if (filesDownloadedFromMyEMSL.Count > 0)
                {
                    foreach (var localFile in filesDownloadedFromMyEMSL)
                        DeleteFileIfLowDiskSpace(localFile);
                }
            }
            OnDataRowAvailable(args);
        }

        private void pipeline_OnStatusMessageUpdated(object sender, MageStatusEventArgs e)
        {
            if (string.IsNullOrEmpty(mCurrentProgressText))
                OnStatusMessageUpdated(e);
            else
                OnStatusMessageUpdated(new MageStatusEventArgs(mCurrentProgressText + ": " + e.Message));
        }

        private void pipeline_OnWarningMessageUpdated(object sender, MageStatusEventArgs e)
        {
            if (!e.Message.StartsWith("ProteinMerger did not find a match for row"))
                OnWarningMessage(e);
        }

        private void SendProgressUpdate(string job)
        {
            var cur = (++mCurrentFileCount).ToString();
            mCurrentProgressText = string.Format("Extracting results for job {0} ({1})", job, cur);
            OnStatusMessageUpdated(new MageStatusEventArgs(mCurrentProgressText));
        }

        private void SendResultsUpdate(string type, string job, int total, int passed)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(string.Format("{3} stats for job {0} Total:{1} Passed:{2}", job, total, passed, type)));
        }

        /// <summary>
        /// Set up an appropriate filter checker based on current result type
        /// </summary>
        private void SetupCurrentResultsChecker()
        {
            if (!string.Equals(ExtractionParms.ResultFilterSetID, ExtractionFilter.ALL_PASS_CUTOFF, StringComparison.OrdinalIgnoreCase))
            {
                mResultsChecker = ExtractionParms.RType.GetResultsChecker(ExtractionParms.ResultFilterSetID);

                // Write out the filter criteria as a tab-delimited text file
                mResultsChecker?.WriteCriteria(Path.Combine(Destination.ContainerPath, Destination.FilterCriteriaName));
            }
        }

        /// <summary>
        /// Get filter module to apply to file contents being extracted
        /// </summary>
        /// <param name="resultsDirectoryPath"></param>
        /// <param name="job"></param>
        private ExtractionFilter GetExtractionFilterModule(string resultsDirectoryPath, string job)
        {
            var resultsFilter = ExtractionParms.RType.GetExtractionFilter(mResultsChecker);
            resultsFilter.Job = job;
            resultsFilter.ResultsDirectoryPath = resultsDirectoryPath;
            resultsFilter.MergeFiles = mMergeFiles;
            resultsFilter.ExtractionType = ExtractionParms;

            return resultsFilter;
        }
    }
}
