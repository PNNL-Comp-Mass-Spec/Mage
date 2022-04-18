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
        /// Directory where the Mage_Temp_Files directory should be created
        /// </summary>
        public string TempFilesContainerPath
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public MyEMSLSinkWrapper()
        {
            PredownloadMyEMSLFiles = true;
        }

        /// <summary>
        /// Constructor
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
            string downloadDirPath;

            if (string.IsNullOrEmpty(TempFilesContainerPath))
            {
                downloadDirPath = Path.GetTempPath();
            }
            else
            {
                downloadDirPath = TempFilesContainerPath;
            }

            var fiFileCheck = new FileInfo(downloadDirPath);
            if (fiFileCheck.Exists && fiFileCheck.Directory != null)
            {
                downloadDirPath = fiFileCheck.Directory.FullName;
            }

            downloadDirPath = Path.Combine(downloadDirPath, MAGE_TEMP_FILES_DIRECTORY);

            if ((mSink.ColumnIndex.TryGetValue("Directory", out var directoryColIndex) ||
                 mSink.ColumnIndex.TryGetValue("Folder", out directoryColIndex)) &&
                mSink.ColumnIndex.TryGetValue("Name", out var filenameColIndex))
            {
                for (var i = 0; i < mSink.Rows.Count; i++)
                {
                    if (UpdateSinkRowIfMyEMSLFile(i, filenameColIndex, directoryColIndex, downloadDirPath))
                        downloadMyEMSLFiles = true;
                }
            }
            else
            {
                directoryColIndex = -1;
                filenameColIndex = -1;
            }

            // Look for other entries in mSink with a MyEMSL File ID
            for (var i = 0; i < mSink.Rows.Count; i++)
            {
                for (var j = 0; j < mSink.Rows[i].Length; j++)
                {
                    if (j == filenameColIndex || j == directoryColIndex)
                        continue;

                    if (UpdateSinkRowIfMyEMSLFile(i, j, -1, downloadDirPath))
                        downloadMyEMSLFiles = true;
                }
            }

            if (downloadMyEMSLFiles)
            {
                var success = ProcessMyEMSLDownloadQueue(downloadDirPath, Downloader.DownloadLayout.DatasetNameAndSubdirectories);

                if (!success)
                {
                    string errorMessage;
                    if (m_MyEMSLDatasetInfoCache.ErrorMessages.Count > 0)
                        errorMessage ="Error downloading files using MyEMSL: " + m_MyEMSLDatasetInfoCache.ErrorMessages[0];
                    else
                        errorMessage = "Unknown error downloading files using MyEMSL";

                    var ex = ReportMageException(errorMessage);
                    throw ex;
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
        /// <param name="directoryColIndex">Optional index of the column that contains the directory for the file in column colIndex; -1 to ignore</param>
        /// <param name="downloadDirectoryPath">Download directory path</param>
        /// <returns>True if a MyEMSL File was found and the row was updated</returns>
        protected bool UpdateSinkRowIfMyEMSLFile(int rowIndex, int colIndex, int directoryColIndex, string downloadDirectoryPath)
        {
            var currentRow = mSink.Rows[rowIndex];

            if (currentRow[colIndex] == null)
                return false;

            if (directoryColIndex >= 0 && currentRow[directoryColIndex] == null)
                directoryColIndex = -1;

            var filePathWithID = currentRow[colIndex];

            if (filePathWithID == kNoFilesFound)
                return false;

            var myEMSLFileID = DatasetInfoBase.ExtractMyEMSLFileID(filePathWithID, out var filePathClean);

            if (myEMSLFileID <= 0)
                return false;

            if (m_FilterPassingMyEMSLFiles.TryGetValue(myEMSLFileID, out var cachedFileInfo))
            {
                m_MyEMSLDatasetInfoCache.AddFileToDownloadQueue(cachedFileInfo.FileInfo);

                var newFilePath = Path.Combine(downloadDirectoryPath, cachedFileInfo.FileInfo.Dataset, cachedFileInfo.FileInfo.RelativePathWindows);

                if (Path.IsPathRooted(filePathWithID))
                {
                    currentRow[colIndex] = newFilePath;
                }
                else
                {
                    currentRow[colIndex] = filePathClean;
                }

                if (directoryColIndex >= 0)
                    currentRow[directoryColIndex] = Path.GetDirectoryName(newFilePath);

                return true;
            }

            return false;
        }
    }
}
