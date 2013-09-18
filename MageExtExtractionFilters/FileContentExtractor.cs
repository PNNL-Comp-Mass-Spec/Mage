using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.IO;
using MageExtContentFilters;
using MyEMSLReader;

namespace MageExtExtractionFilters {

    /// <summary>
    /// Receives list of results files (and associated files) on standard tabular input
    /// and processes each one to individual or concatented results
    /// </summary>
	public class FileContentExtractor : FileProcessingBase
	{

        #region Member Variables

        private int mCurrentFileCount = 0;

        private ExtractionType mExtractionType = null;

        private DestinationType mDestination = null;

        private FilterResultsBase mResultsChecker = null;

		private string mCurrentProgressText = "";

        #endregion

        #region Propertires

        /// <summary>
        /// Set of parameters specifying the user's choices for
        /// how the extraction should be performed
        /// </summary>
        public ExtractionType ExtractionParms {
            get { return mExtractionType; }
            set { mExtractionType = value; }
        }

        /// <summary>
        /// Destination to which processed results will be delivered
        /// </summary>
        public DestinationType Destination {
            get { return mDestination; }
            set { mDestination = value; }
        }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input folder path
        /// (optional - defaults to "Folder")
        /// </summary>
        public string SourceFolderColumnName { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input file name
        /// optional - defaults to "Name")
        /// </summary>
        public string SourceFileColumnName { get; set; }


        #endregion

        /// <summary>
        /// Column indexes
        /// </summary>
        private int mInputFolderIdx;
        private int mInputFileIdx;
        private int mJobIdx;

        private Dictionary<string, ResultType.MergeFile> mMergeFiles;

        private void PrecalculateColumnIndexes() {
			mInputFolderIdx = GetColumnIndex(InputColumnPos, SourceFolderColumnName);
			mInputFileIdx = GetColumnIndex(InputColumnPos, SourceFileColumnName);
			mJobIdx = GetColumnIndex(InputColumnPos, "Job");
        }

        private void PrecalculateMergeFileColumnIndexes() {
            mMergeFiles = new Dictionary<string, ResultType.MergeFile>();
            foreach (ResultType.MergeFile mf in mExtractionType.RType.MergeFileTypes) {
                if (InputColumnPos.Keys.Contains(mf.NameColumn)) {
                    mf.ColumnIndx = InputColumnPos[mf.NameColumn];
                }
                mMergeFiles[mf.NameColumn] = mf;
            }
        }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            List<MageColumnDef> columnDefs = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
            PrecalculateColumnIndexes();
            PrecalculateMergeFileColumnIndexes();
            SetupCurrentResultsChecker();
        }

        /// <summary>
        /// receives a data row representing a results file and one or more 
        /// associated files that need to be merged into the output data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                string resultFolderPath = args.Fields[mInputFolderIdx].ToString();
                string resultFileName = args.Fields[mInputFileIdx].ToString();
                string inputFilePath;

				List<string> filesDownloadedFromMyEMSL = new List<string>();

				if (resultFileName == BaseModule.kNoFilesFound)
					inputFilePath = "";
				else
				{
					inputFilePath = Path.Combine(resultFolderPath, resultFileName);

					if (inputFilePath.StartsWith(MYEMSL_PATH_FLAG))
					{
						// We need to download the result file and any merge files from MyEMSL
						// We'll download all of the files for the given dataset as a group
						// These files should already have a MyEMSL File ID encoded in them

						var filesToDownload = new List<string>();

						filesToDownload.Add(resultFileName);

						foreach (ResultType.MergeFile mf in mMergeFiles.Values)
						{
							if (mf.ColumnIndx >= 0)
							{
								string mergeFileName = args.Fields[mf.ColumnIndx].ToString();
								filesToDownload.Add(mergeFileName);
							}
						}

						for (int i = 0; i < filesToDownload.Count; i++)
						{
							string filePathClean;
							Int64 myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filesToDownload[i], out filePathClean);

							if (myEMSLFileID <= 0)
								throw new MageException("File does not have the MyEMSL FileID tag (" + MyEMSLReader.DatasetInfoBase.MYEMSL_FILEID_TAG + "): " + filesToDownload[i]);

							DatasetFolderOrFileInfo cachedFileInfo;
							if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out cachedFileInfo))
							{
								if (i == 0)
								{
									// Override the result folder path
									resultFolderPath = Path.Combine(Path.GetTempPath(), MAGE_TEMP_FILES_FOLDER, cachedFileInfo.FileInfo.Dataset);

									// Override the input file path
									inputFilePath = Path.Combine(resultFolderPath, cachedFileInfo.FileInfo.Filename);
								}

								string filePathLocal = Path.Combine(resultFolderPath, cachedFileInfo.FileInfo.Filename);

								bool unzipRequired = false;
								m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(cachedFileInfo.FileID, cachedFileInfo.FileInfo, unzipRequired, filePathLocal);
								filesDownloadedFromMyEMSL.Add(filePathLocal);

							}
							else
								throw new MageException("Cannot download file since not in MyEMSL Memory Cache: " + filesToDownload[i]);
								
						}
					
						// Note that the target folder path will be ignored since we explicitly defined the destination file path when queuing the file
						bool success = m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(".", Downloader.DownloadFolderLayout.SingleDataset);
						if (!success)
						{
							string msg = "Failed to download Mage Extractor merge files from MyEMSL";
							if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
								msg += ": " + m_MyEMSLDatasetInfoCache.ErrorMessages.First();
							else
								msg += ": Unknown Error";

							throw new MageException(msg);
						}

					}
				}

                // add actual file name to merge file list
                foreach (ResultType.MergeFile mf in mMergeFiles.Values) {
                    mf.MergeFileName = (mf.ColumnIndx < 0) ? "" : args.Fields[mf.ColumnIndx].ToString();

					string filePathClean;
					Int64 myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(mf.MergeFileName, out filePathClean);

					if (myEMSLFileID > 0)
						mf.MergeFileName = filePathClean;
                }

                if (resultFileName != BaseModule.kNoFilesFound) {
                    string job = args.Fields[mJobIdx].ToString();
                    SendProgressUpdate(job);

					if (!File.Exists(inputFilePath))
					{
						OnWarningMessage(new MageStatusEventArgs("File not found for job " + job + ": " + inputFilePath));
					}
					else
					{
						// build and run pipeline (in this thread) to process file contents
						DelimitedFileReader resultsFileReader = new DelimitedFileReader();
						resultsFileReader.FilePath = inputFilePath;

						ExtractionFilter msgfFilter = new MSGFExtractionFilter();
						msgfFilter.Job = job;
						msgfFilter.ResultFolderPath = resultFolderPath;
						msgfFilter.MergeFiles = mMergeFiles;
						msgfFilter.ExtractionType = mExtractionType;

						ExtractionFilter resultsFilter = mExtractionType.RType.GetExtractionFilter(mResultsChecker);
						resultsFilter.Job = job;
						resultsFilter.ResultFolderPath = resultFolderPath;
						resultsFilter.MergeFiles = mMergeFiles;
						resultsFilter.ExtractionType = mExtractionType;

						BaseModule writer = mDestination.GetDestinationWriterModule(job, inputFilePath);
						var pipeline = ProcessingPipeline.Assemble("Extract File Contents", resultsFileReader, msgfFilter, resultsFilter, writer);

						pipeline.OnWarningMessageUpdated += new EventHandler<MageStatusEventArgs>(pipeline_OnWarningMessageUpdated);
						pipeline.OnStatusMessageUpdated += new EventHandler<MageStatusEventArgs>(pipeline_OnStatusMessageUpdated);

						pipeline.RunRoot(null);

						SendResultsUpdate(mExtractionType.RType.ResultName, job, resultsFilter.ProcessedRows, resultsFilter.PassedRows);
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

		void pipeline_OnStatusMessageUpdated(object sender, MageStatusEventArgs e)
		{
			if (string.IsNullOrEmpty(mCurrentProgressText))
				OnStatusMessageUpdated(e);
			else
				OnStatusMessageUpdated(new MageStatusEventArgs(mCurrentProgressText + ": " + e.Message));
		}

		void pipeline_OnWarningMessageUpdated(object sender, MageStatusEventArgs e)
		{
			if (!e.Message.StartsWith("ProteinMerger did not find a match for row"))
				OnWarningMessage(e);
		}


        private void SendProgressUpdate(string job) {
            string cur = (++mCurrentFileCount).ToString();
			mCurrentProgressText = string.Format("Extracting results for job {0} ({1})", job, cur);
            OnStatusMessageUpdated(new MageStatusEventArgs(mCurrentProgressText));
        }

        private void SendResultsUpdate(string type, string job, int total, int passed) {
            OnStatusMessageUpdated(new MageStatusEventArgs(string.Format("{3} stats for job {0} Total:{1} Passed:{2}", job, total, passed, type)));
        }


        /// <summary>
        /// Set up an appropriate filter checker based on current result type
        /// </summary>
        private void SetupCurrentResultsChecker() {
            if (mExtractionType.ResultFilterSetID.ToLower() != "All Pass".ToLower()) {
                mResultsChecker = mExtractionType.RType.GetResultsChecker(mExtractionType.ResultFilterSetID);

                if (mResultsChecker != null) {
                    // Write out the filter criteria as a tab-delimited text file
                    mResultsChecker.WriteCriteria(System.IO.Path.Combine(Destination.ContainerPath, Destination.FilterCriteriaName));
                }
            }
        }

        /// <summary>
        /// Get filter module to apply to file contents being extracted
        /// </summary>
        /// <param name="assocScoreLookup"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        private ExtractionFilter GetExtractionFilterModule(string resultsFolderPath, string job) {
            ExtractionFilter resultsFilter = null;
            resultsFilter = mExtractionType.RType.GetExtractionFilter(mResultsChecker);
            resultsFilter.Job = job;
            resultsFilter.ResultFolderPath = resultsFolderPath;
            resultsFilter.MergeFiles = mMergeFiles;
            resultsFilter.ExtractionType = mExtractionType;

            return resultsFilter;
        }

    }
}
