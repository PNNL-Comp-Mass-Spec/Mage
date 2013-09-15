using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MyEMSLReader;

namespace Mage
{
	public class FileProcessingBase : BaseModule
	{
		protected const string MYEMSL_PATH_FLAG = @"\\MyEMSL";

		protected const string COLUMN_NAME_DATASET = "Dataset";
		protected const string COLUMN_NAME_DATASET_NAME = "Dataset_Name";
		protected const string COLUMN_NAME_DATASET_NUM = "Dataset_Num";

		// protected const string COLUMN_NAME_DATASETID = "DatasetID";
		// protected const string COLUMN_NAME_DATASET_ID = "Dataset_ID";

		/// <summary>
		/// Cache of files stored in MyEMSL for datasets that the user searches for
		/// </summary>
		/// <remarks>Initially does not have any datasets; add them as data is processed</remarks>
		protected static DatasetListInfo m_MyEMSLDatasetInfoCache = new DatasetListInfo();
		protected bool m_MyEMSLEventsAttached = false;

		/// <summary>
		/// Recently found MyEMSL files
		/// </summary>
		/// <remarks>This list is cleared each time .FindFiles is called</remarks>
		protected static List<DatasetFolderOrFileInfo> m_RecentlyFoundMyEMSLFiles;

		/// <summary>
		/// All MyEMSL Files pass filters; keys are MyEMSL File IDs, values are the MyEMSL Info.  Items will be auto-purged from this list if the list grows to over 1 million records
		/// </summary>
		/// <remarks>This dictionary is used by the FileCopy class to determine the archived file info for a file in MyEMSL using MyEMSLFile ID</remarks>
		protected static Dictionary<Int64, DatasetFolderOrFileInfo> m_FilterPassingMyEMSLFiles;

		/// <summary>
		/// RegEx to extract the dataset name from a path of the form \\MyEMSL\VPro01\2013_3\QC_Shew_13_04d_500ng_10Sep13_Tiger_13-07-34
		/// The RegEx can also be used to determine the portion of a path that includes parent folders and the dataset folder
		/// </summary>
		private Regex mDatasetMatchStrict1 = new Regex(@"\\\\[^\\]+\\[^\\]+\\2[0-9][0-9][0-9]_[1-4]\\([^\\]+)", RegexOptions.Compiled);

		/// <summary>
		/// RegEx to extract the dataset name from a path of the form \\a2.emsl.pnl.gov\dmsarch\VPro01\2013_3\QC_Shew_13_04a_500ng_10Sep13_Tiger_13-07-36
		/// /// The RegEx can also be used to determine the portion of a path that includes parent folders and the dataset folder
		/// </summary>
		private Regex mDatasetMatchStrict2 = new Regex(@"\\\\[^\\]+\\[^\\]+\\[^\\]+\\2[0-9][0-9][0-9]_[1-4]\\([^\\]+)", RegexOptions.Compiled);

		/// <summary>
		/// RegEx to extract the dataset name from a path of the form \2013_3\QC_Shew_13_04f_500ng_10Sep13_Tiger_13-07-34
		/// /// The RegEx can also be used to determine the portion of a path that includes parent folders and the dataset folder
		/// </summary>
		private Regex mDatasetMatchLoose = new Regex(@"(^|\\)2[0-9][0-9][0-9]_[1-4]\\([^\\]+)", RegexOptions.Compiled);

		public FileProcessingBase()
		{
			if (!m_MyEMSLEventsAttached)
			{
				m_MyEMSLEventsAttached = true;
				m_MyEMSLDatasetInfoCache.ErrorEvent += new MessageEventHandler(m_MyEMSLDatasetInfoCache_ErrorEvent);
				m_MyEMSLDatasetInfoCache.MessageEvent += new MessageEventHandler(m_MyEMSLDatasetInfoCache_MessageEvent);
				m_MyEMSLDatasetInfoCache.ProgressEvent += new ProgressEventHandler(m_MyEMSLDatasetInfoCache_ProgressEvent);
			}
		}

		protected static void CacheFilterPassingFile(ArchivedFileInfo fileInfo)
		{
			if (fileInfo.FileID == 0)
				return;

			if (m_FilterPassingMyEMSLFiles == null)
				m_FilterPassingMyEMSLFiles = new Dictionary<long, DatasetFolderOrFileInfo>();

			DatasetFolderOrFileInfo fileInfoCached;
			if (m_FilterPassingMyEMSLFiles.TryGetValue(fileInfo.FileID, out fileInfoCached))
			{
				fileInfoCached = new DatasetFolderOrFileInfo(fileInfo.FileID, false, fileInfo);
			}
			else
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

		protected string DetermineDatasetName(object[] bufferRow, string folderPath)
		{
			string datasetName = string.Empty;

			var datasetColNames = new List<string>();
			datasetColNames.Add(COLUMN_NAME_DATASET);
			datasetColNames.Add(COLUMN_NAME_DATASET_NAME);
			datasetColNames.Add(COLUMN_NAME_DATASET_NUM);

			int datasetColIndex = -1;
			foreach (string datasetColName in datasetColNames)
			{
				if (TryGetOutputColumnPos(datasetColName, out datasetColIndex))
					break;
			}

			if (datasetColIndex >= 0)
			{
				datasetName = (string)bufferRow[datasetColIndex];
			}
			else
			{
				// Parse the folderPath with a RegEx to extract the dataset name
				var reMatch = mDatasetMatchStrict1.Match(folderPath);

				if (!reMatch.Success)
					reMatch = mDatasetMatchStrict2.Match(folderPath);

				if (!reMatch.Success)
					reMatch = mDatasetMatchLoose.Match(folderPath);

				if (reMatch.Success)
				{
					datasetName = reMatch.Groups[1].ToString();
				}
			}

			return datasetName;
		}

		protected string ExtractParentDatasetFolders(string folderPath)
		{
			string parentFolders = string.Empty;

			// Parse the folderPath with a RegEx to extract the parent folders
			var reMatch = mDatasetMatchStrict1.Match(folderPath);

			if (!reMatch.Success)
				reMatch = mDatasetMatchStrict2.Match(folderPath);

			if (!reMatch.Success)
				reMatch = mDatasetMatchLoose.Match(folderPath);

			if (reMatch.Success)
			{
				parentFolders = reMatch.ToString();
			}

			return parentFolders;
		}
		protected bool GetCachedArchivedFileInfo(Int64 myEMSLFileID, out ArchivedFileInfo fileInfo)
		{
			var fileInfoMatch = (from item in m_RecentlyFoundMyEMSLFiles where item.FileID == myEMSLFileID select item.FileInfo).ToList();

			if (fileInfoMatch.Count == 0)
			{
				fileInfo = null;
				return false;
			}
			else
			{
				fileInfo = fileInfoMatch.First();
				return true;
			}
		}

		public override void PostProcess()
		{
			if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count > 0)
			{
				if (m_MyEMSLDatasetInfoCache.FilesToDownload.Count == 1)
					OnStatusMessageUpdated(new MageStatusEventArgs("Downloading one file from MyEMSL"));
				else
					OnStatusMessageUpdated(new MageStatusEventArgs("Downloading " + m_MyEMSLDatasetInfoCache.FilesToDownload.Count + " files from MyEMSL"));

				m_MyEMSLDatasetInfoCache.ProcessDownloadQueue(".", Downloader.DownloadFolderLayout.SingleDataset);

				System.Threading.Thread.Sleep(10);
			}
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
			if (e.PercentComplete < 100)
				OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: 100% complete"));
			else
				OnStatusMessageUpdated(new MageStatusEventArgs("Downloading files from MyEMSL: " + e.PercentComplete.ToString("0.00") + "% complete"));
		}
		#endregion

	}


}
