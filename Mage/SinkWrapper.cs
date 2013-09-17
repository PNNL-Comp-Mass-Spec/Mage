using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mage;
using MyEMSLReader;

namespace Mage
{

	/// <summary>
	/// This class is a thin wrapper around a Mage SimpleSink object
	/// and acts as a source module to serve it content
	/// </summary>
	public class SinkWrapper : FileProcessingBase
	{

		private SimpleSink mSink = null;

		/// <summary>
		/// constructor
		/// </summary>
		public SinkWrapper()
		{
		}

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="sink"></param>
		public SinkWrapper(SimpleSink sink)
		{
			mSink = sink;
		}

		public override void Prepare()
		{
			base.Prepare();

			// Retrieve any MyEMSL files tracked by mSink
			// Only retrieve files for which the MyEMSL FileID is known (the file will have an @ sign)
			// Update the paths in mSink to point to the temporary path to which the files were downloaded

			bool downloadMyEMSLFiles = false;
			string downloadFolderPath;

			if (Context == null || !Context.TryGetValue(DESTINATION_CONTAINER_PATH, out downloadFolderPath))
			{
				downloadFolderPath = Path.GetTempPath();
			}

			var fiFileCheck = new FileInfo(downloadFolderPath);
			if (fiFileCheck.Exists)
			{
				downloadFolderPath = fiFileCheck.Directory.FullName;
			}

			downloadFolderPath = Path.Combine(downloadFolderPath, MAGE_TEMP_FILES_FOLDER);

			for (int i = 0; i < mSink.Rows.Count; i++)
			{
				object[] currentRow = mSink.Rows[i];
				bool rowUpdated = false;

				for (int j = 0; j < currentRow.Length; j++)
				{
					if (currentRow[j] == null)
						continue;

					string filePathWithID = currentRow[j].ToString();
					string filePathClean;

					Int64 myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filePathWithID, out filePathClean);

					if (myEMSLFileID <= 0)
						continue;

					DatasetFolderOrFileInfo cachedFileInfo;
					if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out cachedFileInfo))
					{
						m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(cachedFileInfo.FileInfo);
						downloadMyEMSLFiles = true;

						string newFilePath = Path.Combine(downloadFolderPath, cachedFileInfo.FileInfo.Dataset, cachedFileInfo.FileInfo.RelativePathWindows);

						if (Path.IsPathRooted(filePathWithID))
						{
							currentRow[j] = newFilePath;
						}
						else
						{
							// This column only contains a filename; not a full path
							// The previous column likely contains the MyEMSL folder path; update it if it does
							if (j > 0)
							{
								string filePathPrevious = currentRow[j - 1].ToString();
								if (filePathPrevious.StartsWith(MYEMSL_PATH_FLAG))
								{
									currentRow[j - 1] = Path.GetDirectoryName(newFilePath);
								}
							}
							currentRow[j] = filePathClean;
						}
						rowUpdated = true;
					}
				}

				if (rowUpdated)
				{
					mSink.UpdateSavedRowData(i, currentRow);
				}
			}

			if (downloadMyEMSLFiles)
			{
				bool success = ProcessMyEMSLDownloadQueue(downloadFolderPath, Downloader.DownloadFolderLayout.DatasetNameAndSubFolders);

				if (!success)
				{
					if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
						throw new MageException("Error downloading files using MyEMSL: " + m_MyEMSLDatasetInfoCache.ErrorMessages.First());
					else
						throw new MageException("Unknown error downloading files using MyEMSL");
				}
			}

		}

		/// <summary>
		/// Serve contents of SimpleSink object that we are wrapped around
		/// </summary>
		/// <param name="state"></param>
		public override void Run(object state)
		{
			OnColumnDefAvailable(new MageColumnEventArgs(mSink.Columns.ToArray()));
			foreach (object[] row in mSink.Rows)
			{
				if (Abort) break;
				OnDataRowAvailable(new MageDataEventArgs(row));
			}
			OnDataRowAvailable(new MageDataEventArgs(null));
		}

	}
}
