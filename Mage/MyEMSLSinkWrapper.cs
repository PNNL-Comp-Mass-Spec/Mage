using System.IO;
using System.Linq;
using MyEMSLReader;

namespace Mage
{
    /// <summary>
    /// This class is a thin wrapper around a Mage SimpleSink object
    /// and acts as a source module to serve it content
    /// It also supports downloading files from MyEMSL
    /// </summary>
    public class MyEMSLSinkWrapper : FileProcessingBase
    {

        private readonly SimpleSink mSink;

        /// <summary>
        /// Set to True to download and cache locally any MyEMSL files
        /// </summary>
        /// <remarks>It is more efficient to pre-download the files; the only reason you would not want to do this is if you're low on free disk space</remarks>
        public bool PredownloadMyEMSLFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Folder where the Mage_Temp_Files folder should be created
        /// </summary>
        public string TempFilesContainerPath
        {
            get;
            set;
        }
        /// <summary>
        /// constructor
        /// </summary>
        public MyEMSLSinkWrapper()
        {
            PredownloadMyEMSLFiles = true;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sink"></param>
        public MyEMSLSinkWrapper(SimpleSink sink)
        {
            mSink = sink;
            PredownloadMyEMSLFiles = true;
        }

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();

            if (mSink == null)
                return;

            if (!PredownloadMyEMSLFiles)
                return;

            // Retrieve any MyEMSL files tracked by mSink
            // Only retrieve files for which the MyEMSL FileID is known (the file will have an @ sign)
            // Update the paths in mSink to point to the temporary path to which the files were downloaded

            var downloadMyEMSLFiles = false;
            string downloadFolderPath;

            if (string.IsNullOrEmpty(TempFilesContainerPath))
            {
                downloadFolderPath = Path.GetTempPath();
            }
            else
            {
                downloadFolderPath = TempFilesContainerPath;
            }

            var fiFileCheck = new FileInfo(downloadFolderPath);
            if (fiFileCheck.Exists && fiFileCheck.Directory != null)
            {
                downloadFolderPath = fiFileCheck.Directory.FullName;
            }

            downloadFolderPath = Path.Combine(downloadFolderPath, MAGE_TEMP_FILES_FOLDER);

            // Look for the Folder and Name columns
            int folderColIndex;
            int filenameColIndex;
            if (mSink.ColumnIndex.TryGetValue("Folder", out folderColIndex) &&
                mSink.ColumnIndex.TryGetValue("Name", out filenameColIndex))
            {
                for (var i = 0; i < mSink.Rows.Count; i++)
                {
                    if (UpdateSinkRowIfMyEMSLFile(i, filenameColIndex, folderColIndex, downloadFolderPath))
                        downloadMyEMSLFiles = true;
                }
            }
            else
            {
                folderColIndex = -1;
                filenameColIndex = -1;
            }

            // Look for other entries in mSink with a MyEMSL File ID
            for (var i = 0; i < mSink.Rows.Count; i++)
            {
                for (var j = 0; j < mSink.Rows[i].Length; j++)
                {
                    if (j == filenameColIndex || j == folderColIndex)
                        continue;

                    if (UpdateSinkRowIfMyEMSLFile(i, j, -1, downloadFolderPath))
                        downloadMyEMSLFiles = true;
                }
            }

            if (downloadMyEMSLFiles)
            {
                var success = ProcessMyEMSLDownloadQueue(downloadFolderPath, Downloader.DownloadFolderLayout.DatasetNameAndSubFolders);

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
            foreach (var row in mSink.Rows)
            {
                if (Abort)
                    break;
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

        /// <summary>
        /// Checks for a MyEMSL File ID in the entry at row rowIndex, column colIndex in mSink.Rows()
        /// </summary>
        /// <param name="rowIndex">Index of row in mSink.Rows to examine</param>
        /// <param name="colIndex">Index of the column to examine</param>
        /// <param name="folderColIndex">Optional index of the column that contains the folder for the file in column colIndex; -1 to ignore</param>
        /// <param name="downloadFolderPath">Download folder path</param>
        /// <returns>True if a MyEMSL File was found and the row was updated</returns>
        protected bool UpdateSinkRowIfMyEMSLFile(int rowIndex, int colIndex, int folderColIndex, string downloadFolderPath)
        {
            var currentRow = mSink.Rows[rowIndex];

            if (currentRow[colIndex] == null)
                return false;

            if (folderColIndex >= 0 && currentRow[folderColIndex] == null)
                folderColIndex = -1;

            var filePathWithID = currentRow[colIndex];

            if (filePathWithID == kNoFilesFound)
                return false;

            string filePathClean;

            var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filePathWithID, out filePathClean);

            if (myEMSLFileID <= 0)
                return false;

            DatasetFolderOrFileInfo cachedFileInfo;
            if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out cachedFileInfo))
            {
                m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(cachedFileInfo.FileInfo);

                var newFilePath = Path.Combine(downloadFolderPath, cachedFileInfo.FileInfo.Dataset, cachedFileInfo.FileInfo.RelativePathWindows);

                if (Path.IsPathRooted(filePathWithID))
                {
                    currentRow[colIndex] = newFilePath;
                }
                else
                {
                    currentRow[colIndex] = filePathClean;
                }

                if (folderColIndex >= 0)
                    currentRow[folderColIndex] = Path.GetDirectoryName(newFilePath);

                return true;
            }

            return false;
        }

    }
}
