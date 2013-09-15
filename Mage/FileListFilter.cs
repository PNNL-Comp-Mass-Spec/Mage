using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using MyEMSLReader;

namespace Mage
{

	/// <summary>
	/// this module searches a list of folder paths for files and compares the file names
	/// against a set of file selection criteria and accumulates an internal list of files that pass,
	/// and outputs the selected files (and their folder path) via standard tablular output
	///
	/// this module can receive the list of source folders either via its HandleDataRow listener
	/// (it will accumulate the list into an internal file path buffer and then use it to look for files)
	/// or it may be run as a source module after one or more source folders are specified by
	/// setting the "FolderPath" property/parameter
	///
	/// this module uses output column definitions
	/// the internal defaults will provide a functional minimum even if the 
	/// "OutputColumnList" property is not set by the client, but if it is
	/// it must include a new column definition for the column specified by the "FileColumnName" property
	/// </summary>
	public class FileListFilter : FileProcessingBase
	{
		private static readonly ILog traceLog = LogManager.GetLogger("TraceLog");

		private enum FolderSearchMode { Files, Folders };

		#region Member Variables

		// buffer that accumulates a row of output fields for each input row
		// received via standard tabular input or via the "FolderPath" property
		// it includes the folder path column to be searched for files
		// so it also functions as an internal file path buffer 
		private List<object[]> mOutputBuffer = new List<object[]>();

		// these are used by the file/subfolder search logic
		private int mPathColIndx = -1;
		private int mFileNameOutColIndx = -1;
		private int mFileSizeOutColIndx = -1;
		private int mFileDateOutColIndx = -1;
		private int mFileTypeOutColIndex = -1;
		private bool mIncludeFiles;
		private bool mIncludeFolders;
		private List<object[]> mSearchSubfolders = new List<object[]>();

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

		/// <summary>
		/// semi-colon delimited list of file matching patterns
		/// </summary>
		public string FileNameSelector { get; set; }

		/// <summary>
		/// how to use the file matching patterns ("FileSearch" or "RegEx")
		/// </summary>
		public string FileSelectorMode { get; set; }

		/// <summary>
		/// include files an/or folders in results
		/// ("File", "Folder", "IncludeFilesOrFolders")
		/// </summary>
		public string IncludeFilesOrFolders { get; set; }

		/// <summary>
		/// setting this property sets the file path to the internal file path buffer
		/// (necessary if Run will be called instead of processing via standard tabular input)
		/// </summary>
		public string FolderPath
		{
			get
			{
				return (mOutputBuffer.Count > 0) ? mOutputBuffer[0][2].ToString() : "";
			}
			set
			{
				mOutputBuffer.Clear();
				AddFolderPath(value);
			}
		}

		/// <summary>
		/// add a path to a folder to be searched
		/// (used when this module's "Run" method is to be called
		/// such as when it is installed as the root module in a pipeline)
		/// </summary>
		/// <param name="path"></param>
		public void AddFolderPath(string path)
		{
			mOutputBuffer.Add(new object[] { "", "", "", path });  // Note: needs to have the same number of columns as OutputColumnList
		}

		/// <summary>
		/// do recursive file search
		/// </summary>
		public string RecursiveSearch { get; set; }

		/// <summary>
		/// folder name pattern used to restrict recursive search
		/// </summary>
		public string SubfolderSearchName { get; set; }


		#endregion

		#region Constructors

		/// <summary>
		/// construct a new Mage file list filter module
		/// </summary>
		public FileListFilter()
		{
			FileTypeColumnName = COLUMN_NAME_FILE_TYPE;				// Item
			FileColumnName = COLUMN_NAME_FILE_NAME;					// File
			FileSizeColumnName = COLUMN_NAME_FILE_SIZE;				// File_Size_KB
			FileDateColumnName = COLUMN_NAME_FILE_DATE;				// File_Date
			SourceFolderColumnName = COLUMN_NAME_SOURCE_FOLDER;		// Folder
			OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", FileTypeColumnName, FileColumnName, FileSizeColumnName, FileDateColumnName, SourceFolderColumnName);
			FileSelectorMode = "RegEx";
			IncludeFilesOrFolders = "File";
			RecursiveSearch = "No";
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

		#region Private Functions

		/// <summary>
		/// go through each folder that we accumulated in our internal buffer
		/// and search the folder for files that satisfy file name filter,
		/// and output them to our listeners
		/// </summary>
		private void SearchFoldersAndOutputFiles()
		{
			// set up indexes for row columns
			TryGetOutputColumnPos(SourceFolderColumnName, out mPathColIndx);
			TryGetOutputColumnPos(FileColumnName, out mFileNameOutColIndx);
			TryGetOutputColumnPos(FileSizeColumnName, out mFileSizeOutColIndx);
			TryGetOutputColumnPos(FileDateColumnName, out mFileDateOutColIndx);

			if (string.IsNullOrEmpty(FileTypeColumnName))
				mFileTypeOutColIndex = -1;
			else
				TryGetOutputColumnPos(FileTypeColumnName, out mFileTypeOutColIndex);

			// set up controls for scope of search
			mIncludeFiles = IncludeFilesOrFolders.Contains("File");
			mIncludeFolders = IncludeFilesOrFolders.Contains("Folder");

			bool recurseMyEMSL = false;

			// add subfolders if we are doing a recursive search
			if (RecursiveSearch == "Yes")
			{
				recurseMyEMSL = true;
				if (string.IsNullOrEmpty(SubfolderSearchName))
				{
					SubfolderSearchName = "*";
				}
				foreach (object[] fields in mOutputBuffer)
				{
					AddSearchSubfolders(fields);
				}
				mOutputBuffer.AddRange(mSearchSubfolders);
			}

			var dctRowDatasets = new Dictionary<int, string>();
			bool searchMyEMSL = false;

			// Determine the dataset name to use for each row in mOutputBuffer
			for (int outputBufferRowIdx = 0; outputBufferRowIdx < mOutputBuffer.Count; outputBufferRowIdx++)
			{
				string folderPathSpec = (string)mOutputBuffer[outputBufferRowIdx][mPathColIndx];

				string datasetName = DetermineDatasetName(mOutputBuffer[outputBufferRowIdx], folderPathSpec);

				if (folderPathSpec.Contains(MYEMSL_PATH_FLAG))
				{
					searchMyEMSL = true;

					if (string.IsNullOrEmpty(datasetName))
						throw new MageException("Unable to determine dataset name for row " + (outputBufferRowIdx + 1).ToString() + ", file " + folderPathSpec);
				}
			
				dctRowDatasets.Add(outputBufferRowIdx, datasetName);
			}

			if (searchMyEMSL)
			{
				foreach (var datasetName in dctRowDatasets.Values.Distinct())
					m_MyEMSLDatasetInfoCache.AddDataset(datasetName);
			}
				

			// go through each folder that we accumulated in our internal buffer
			for (int outputBufferRowIdx = 0; outputBufferRowIdx < mOutputBuffer.Count; outputBufferRowIdx++)
			{
				if (Abort)
				{
					ReportProcessingAborted();
					break;
				}

				string folderPathSpec = (string)mOutputBuffer[outputBufferRowIdx][mPathColIndx];
				List<string> folderPaths = new List<string>();

				// folderPathSpec may contain multiple folders, separated by a vertical bar
				// If that is the case, then we'll search for files in each folder, preferentially using files in the folder listed first
				if (folderPathSpec.Contains('|'))
				{
					folderPaths = folderPathSpec.Split('|').ToList<string>();
				}
				else
				{
					folderPaths.Add(folderPathSpec);
				}

				// This dictionary holds filename and file system info for each file that is found
				Dictionary<string, FileInfo> fileInfo = new Dictionary<string, FileInfo>(StringComparer.CurrentCultureIgnoreCase);

				// This dictionary holds subfolder name file system info for each subfolder that is found
				Dictionary<string, DirectoryInfo> subfolderInfo = new Dictionary<string, DirectoryInfo>(StringComparer.CurrentCultureIgnoreCase);

				string datasetName = dctRowDatasets[outputBufferRowIdx];

				foreach (string folderPath in folderPaths)
				{
					if (Abort)
					{
						ReportProcessingAborted();
						break;
					}

					traceLog.Debug("FileListFilter: Searching folder " + folderPath);
					UpdateStatus("FileListFilter: Searching folder " + folderPath);

					SearchFolders(outputBufferRowIdx, fileInfo, subfolderInfo, folderPath, recurseMyEMSL, datasetName);

				}

				// inform our subscribers of what we found
				if ((fileInfo == null || fileInfo.Count == 0) && (subfolderInfo == null || subfolderInfo.Count == 0))
				{
					ReportNothingFound(outputBufferRowIdx);
				}
				else
				{
					foreach (KeyValuePair<string, FileInfo> entry in fileInfo)
					{
						string fileName = string.Empty;
						string folderPath = string.Empty;
						string fileSizeKB = string.Empty;
						string fileDate = string.Empty;

						if (entry.Value.DirectoryName.StartsWith(MYEMSL_PATH_FLAG))
						{
							Int64 myEMSLFileID = DatasetInfo.ExtractMyEMSLFileID(entry.Value.FullName);

							if (myEMSLFileID == 0)
								throw new MageException("Encoded MyEMSL File ID not found in " + entry.Value.FullName);

							ArchivedFileInfo archiveFileInfo;

							if (!GetCachedArchivedFileInfo(myEMSLFileID, out archiveFileInfo))
								throw new MageException("Cached ArchiveFileInfo does not contain MyEMSL File ID " + myEMSLFileID);

							fileName = MyEMSLReader.DatasetInfo.AppendMyEMSLFileID(archiveFileInfo.Filename, myEMSLFileID);
							var fiMyEMSLFile = new FileInfo(MYEMSL_PATH_FLAG + "\\" + archiveFileInfo.PathWithInstrumentAndDatasetWindows);
							folderPath = fiMyEMSLFile.Directory.FullName;
							fileSizeKB = FileSizeBytesToString(archiveFileInfo.FileSizeBytes);
							fileDate = archiveFileInfo.SubmissionTime;

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
					foreach (KeyValuePair<string, DirectoryInfo> entry in subfolderInfo)
					{
						string subfolderName = entry.Key;
						string folderPath = entry.Value.FullName;
						ReportSubfolderFound(outputBufferRowIdx, folderPath, subfolderName);
					}
				}
			}
			// inform our subscribers that all data has been sent
			OnDataRowAvailable(new MageDataEventArgs(null));
		}

		private void SearchFolders(
			int outputBufferRowIdx, 
			Dictionary<string, FileInfo> fileInfo, 
			Dictionary<string, DirectoryInfo> 
			subfolderInfo, string folderPath,
			bool recurseMyEMSL,
		    string datasetName)
		{

			List<FileSystemInfo> foundFiles = new List<FileSystemInfo>();
			List<FileSystemInfo> foundSubFolders = new List<FileSystemInfo>();
			try
			{
				if (FileSelectorMode == "RegEx")
				{
					if (mIncludeFiles)
					{
						if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
							foundFiles = GetFileOrFolderNamesFromFolderByRegExMyEMSL(folderPath, FolderSearchMode.Files, datasetName, recurseMyEMSL);
						else
							foundFiles = GetFileOrFolderNamesFromFolderByRegEx(folderPath, FolderSearchMode.Files);
					}
					if (mIncludeFolders)
					{
						if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
							foundSubFolders = GetFileOrFolderNamesFromFolderByRegExMyEMSL(folderPath, FolderSearchMode.Folders, datasetName, recurseMyEMSL);
						else
							foundSubFolders = GetFileOrFolderNamesFromFolderByRegEx(folderPath, FolderSearchMode.Folders);
					}
				}
				else
				{
					if (mIncludeFiles)
					{
						if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
							foundFiles = GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(folderPath, FolderSearchMode.Files, datasetName, recurseMyEMSL);
						else
							foundFiles = GetFileOrFolderNamesFromFolderBySearchPattern(folderPath, FolderSearchMode.Files);
					}
					if (mIncludeFolders)
					{
						if (folderPath.StartsWith(MYEMSL_PATH_FLAG))
							foundSubFolders = GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(folderPath, FolderSearchMode.Folders, datasetName, recurseMyEMSL);
						else
							foundSubFolders = GetFileOrFolderNamesFromFolderBySearchPattern(folderPath, FolderSearchMode.Folders);
					}
				}

				// Append new files in fileNames to fileInfo
				if (!(foundFiles == null || foundFiles.Count == 0))
				{
					foreach (FileInfo entry in foundFiles)
					{
						if (!fileInfo.ContainsKey(entry.Name))
							fileInfo.Add(entry.Name, entry);
					}
				}

				// Append new subFolders in fileNames to subfolderInfo
				if (!(foundSubFolders == null || foundSubFolders.Count == 0))
				{
					foreach (DirectoryInfo entry in foundSubFolders)
					{
						if (!subfolderInfo.ContainsKey(entry.Name))
							subfolderInfo.Add(entry.Name, entry);
					}
				}

			}
			catch (Exception e)
			{
				if (e is ArgumentNullException || e is System.Security.SecurityException || e is ArgumentException || e is PathTooLongException || e is DirectoryNotFoundException)
				{
					string msg = e.Message;
					ReportSearchErrorToOutput(outputBufferRowIdx, msg);
				}
				else if (e is IOException)
				{
					throw new MageException("Process aborted:" + e.Message);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// add subdirectories to search list (used in recursive search mode)
		/// </summary>
		/// <param name="fields"></param>
		private void AddSearchSubfolders(object[] fields)
		{
			string path = (string)fields[mPathColIndx];
			if (path.StartsWith(MYEMSL_PATH_FLAG))
				return;

			DirectoryInfo di = new DirectoryInfo(path);
			if (di.Exists)
			{
				foreach (DirectoryInfo sfDi in di.GetDirectories(SubfolderSearchName))
				{
					object[] subfolderRow = (object[])fields.Clone();
					string subfolderPath = Path.Combine(path, sfDi.Name);
					subfolderRow[mPathColIndx] = subfolderPath;
					mSearchSubfolders.Add(subfolderRow);
					AddSearchSubfolders(subfolderRow);
				}
			}
		}

		private string FileSizeBytesToString(Int64 sizeBytes)
		{
			if (sizeBytes == 0)
				return "0";

			double fileSizeKB = sizeBytes / 1024.0;

			if (fileSizeKB < 10)
				return fileSizeKB.ToString("0.000");

			if (fileSizeKB < 100)
				return fileSizeKB.ToString("0.00");

			if (fileSizeKB < 1000)
				return fileSizeKB.ToString("0.0");

			return fileSizeKB.ToString("0");

		}

		/// <summary>
		/// Get list of files from given directory using file selector list
		/// as file search patterns
		/// </summary>
		/// <param name="path"></param>
		/// <param name="searchMode"></param>
		/// <returns>List of file names</returns>
		private List<FileSystemInfo> GetFileOrFolderNamesFromFolderBySearchPattern(string path, FolderSearchMode searchMode)
		{
			Dictionary<string, FileSystemInfo> filteredFilesOrFolders = new Dictionary<string, FileSystemInfo>();

			List<string> selectors = GetFileNameSelectors();

			FileSystemInfo[] fiList = null;
			DirectoryInfo di = new DirectoryInfo(path);

			if (selectors.Count == 0)
			{
				// Get all files/subfolders in folder
				selectors.Add("*");
			}

			// get list of files for each selector
			foreach (string selector in selectors)
			{
				if (searchMode == FolderSearchMode.Files)
				{
					fiList = di.GetFiles(selector);						
				}
				if (searchMode == FolderSearchMode.Folders)
				{
					fiList = di.GetDirectories(selector);
				}
				foreach (FileSystemInfo fi in fiList)
				{
					filteredFilesOrFolders[fi.Name] = fi;
				}
			}
			
			// We used the dictionary keys for our file names to eliminate duplicates
			// Convert the values to a list of file system infos and return the list
			return filteredFilesOrFolders.Values.ToList<FileSystemInfo>();

		}

		private List<FileSystemInfo> GetFileOrFolderNamesFromFolderBySearchPatternMyEMSL(string path, FolderSearchMode searchMode, string datasetName, bool recurse)
		{
			string subDir;
			string parentFolders;
			GetMyEMSLParentFoldersAndSubDir(path, datasetName, out subDir, out parentFolders);
			
			var filteredFilesOrFolders = new Dictionary<string, FileSystemInfo>();

			List<string> selectors = GetFileNameSelectors();
			
			if (selectors.Count == 0)
			{
				// Get all files/subfolders in folder
				selectors.Add("*");
			}

			// get list of files for each selector
			foreach (string selector in selectors)
			{
				var fiList = GetMyEMSLFilesOrFolders(searchMode, selector, datasetName, recurse, subDir, parentFolders);
			
				foreach (FileSystemInfo fi in fiList)
				{
					filteredFilesOrFolders[fi.Name] = fi;
				}
			}
			
			// We used the dictionary keys for our file names to eliminate duplicates
			// Convert the values to a list of file system infos and return the list
			return filteredFilesOrFolders.Values.ToList<FileSystemInfo>();

		}
		/// <summary>
		/// Get list of files from given directory using file selector list
		/// as RegEx patterns
		/// </summary>
		/// <param name="path">Folder path to get file from</param>
		/// <param name="searchMode"></param>
		/// <returns>List of file names</returns>
		private List<FileSystemInfo> GetFileOrFolderNamesFromFolderByRegEx(string path, FolderSearchMode searchMode)
		{
			DirectoryInfo di = new DirectoryInfo(path);

			var fiList = new List<FileSystemInfo>(); 
			if (searchMode == FolderSearchMode.Files)
			{
				foreach (var file in di.GetFiles().ToList())
					fiList.Add(file);
			}

			if (searchMode == FolderSearchMode.Folders)
			{
				foreach (var directory in di.GetDirectories().ToList())
					fiList.Add(directory);
			}
		
			List<Regex> fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

			return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
		}

		private List<FileSystemInfo> GetFileOrFolderNamesFromFolderByRegExMyEMSL(string path, FolderSearchMode searchMode, string datasetName, bool recurse)
		{
			string subDir;
			string parentFolders;
			GetMyEMSLParentFoldersAndSubDir(path, datasetName, out subDir, out parentFolders);

			string fileSelector = "*";
			var fiList = GetMyEMSLFilesOrFolders(searchMode, fileSelector, datasetName, recurse, subDir, parentFolders);

			List<Regex> fileNameRegExSpecs = GetRegexFileSelectors(GetFileNameSelectors());

			return FilterFileNamesFromList(fiList, fileNameRegExSpecs);
		}

		private List<FileSystemInfo> GetMyEMSLFilesOrFolders(
			FolderSearchMode searchMode, 
			string fileSelector, 
			string datasetName, 
			bool recurse, 
			string subDir, 
			string parentFolders)
		{

			var fiList = new List<FileSystemInfo>();
			if (searchMode == FolderSearchMode.Files)
			{
				m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(fileSelector, subDir, datasetName, recurse);
				foreach (var archiveFile in m_RecentlyFoundMyEMSLFiles)
				{
					string encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentFolders, archiveFile.FileInfo.RelativePathWindows), archiveFile.FileID);
					fiList.Add(new FileInfo(encodedFilePath));
				}
			}

			if (searchMode == FolderSearchMode.Folders)
			{
				m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFolders(fileSelector, datasetName);

				foreach (var archiveFolder in m_RecentlyFoundMyEMSLFiles)
				{
					string encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(Path.Combine(parentFolders, archiveFolder.FileInfo.RelativePathWindows), archiveFolder.FileID);
					fiList.Add(new DirectoryInfo(encodedFilePath));
				}
			}
			return fiList;
		}

		private void GetMyEMSLParentFoldersAndSubDir(string path, string datasetName, out string subDir, out string parentFolders)
		{
			subDir = string.Empty;

			parentFolders = ExtractParentDatasetFolders(path);
			if (string.IsNullOrEmpty(parentFolders))
			{
				parentFolders = MYEMSL_PATH_FLAG + @"\Instrument\2013_1\" + datasetName;
			}
			else
			{
				if (path.Length > parentFolders.Length)
				{
					subDir = path.Substring(parentFolders.Length);
					subDir = subDir.TrimStart('\\');
				}
				parentFolders = parentFolders.TrimEnd('\\');
			}
		}


		/// <summary>
		/// search files in folder and return list of files 
		/// whose names satisfy the selection criteria
		/// </summary>
		/// <param name="fileList"></param>
		/// <param name="fileNameRegExSpecs"></param>
		/// <returns></returns>
		private static List<FileSystemInfo> FilterFileNamesFromList(List<FileSystemInfo> fileList, List<Regex> fileNameRegExSpecs)
		{

			List<FileSystemInfo> filteredFilesOrFolders = new List<FileSystemInfo>(fileList.Count);

			// find files (or folders) that meet selection criteria.
			foreach (FileSystemInfo fiEntry in fileList)
			{
				if (fileNameRegExSpecs.Count == 0)
				{
					filteredFilesOrFolders.Add(fiEntry);
				}
				else
				{
					foreach (Regex rx in fileNameRegExSpecs)
					{
						Match m = rx.Match(fiEntry.Name);
						if (m.Success)
						{
							filteredFilesOrFolders.Add(fiEntry);
							break;
						}
					}
				}
			}
			return filteredFilesOrFolders;
		}

		/// <summary>
		/// Make list of regex objects from list of file selectors
		/// </summary>
		/// <returns></returns>
		private static List<Regex> GetRegexFileSelectors(List<string> selectors)
		{
			List<Regex> fileNameSpecs = new List<Regex>();
			foreach (string selector in selectors)
			{
				try
				{
					Regex rx = new Regex(selector.Trim(), RegexOptions.IgnoreCase);
					fileNameSpecs.Add(rx);
				}
				catch (Exception e)
				{
					traceLog.Error(e.Message);
					throw new MageException("Problem with file selector:" + e.Message);
				}
			}
			return fileNameSpecs;
		}

		/// <summary>
		/// get list of individual file selectors from selector list
		/// </summary>
		/// <returns></returns>
		private List<string> GetFileNameSelectors()
		{
			List<string> selectorList = new List<string>();
			foreach (string selector in FileNameSelector.Split(';'))
			{
				selectorList.Add(selector.Trim());
			}
			return selectorList;
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
			object[] outRow = (object[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
			if (mFileTypeOutColIndex > -1)
			{
				outRow[mFileTypeOutColIndex] = "folder";
				outRow[mPathColIndx] = folderPath;
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
			object[] outRow = (object[])mOutputBuffer[outputBufferRowIdx].Clone(); // yes, we do want a shallow copy
			if (mFileTypeOutColIndex > -1)
			{
				outRow[mFileTypeOutColIndex] = "file";
				outRow[mPathColIndx] = folderPath;
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
		private void ReportSearchErrorToOutput(int outputBufferRowIdx, string msg)
		{
			mOutputBuffer[outputBufferRowIdx][mFileNameOutColIndx] = "--Error: " + msg;
			traceLog.Error(msg);
			OnDataRowAvailable(new MageDataEventArgs(mOutputBuffer[outputBufferRowIdx]));
		}

		#endregion

	}
}
