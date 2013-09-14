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
		/// cache of files in MyEMSL for datasets
		/// </summary>
		/// <remarks>Initially does not have any datasets; add them as data is processed</remarks>
		protected DatasetListInfo m_MyEMSLDatastInfoCache = new DatasetListInfo();

		protected List<DatasetFolderOrFileInfo> m_RecentlyFoundMyEMSLFiles;

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

		protected string DetermineDatasetName(object[] bufferRow, string folderPath)
		{
			string datasetName = string.Empty;

			var datasetColNames = new List<string>();
			datasetColNames.Add(COLUMN_NAME_DATASET);
			datasetColNames.Add(COLUMN_NAME_DATASET_NAME);
			datasetColNames.Add(COLUMN_NAME_DATASET_NUM);

			int datasetColIndex=-1;
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
								
	}
}
