using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.IO;
using MyEMSLReader;

namespace Mage {

    /// <summary>
    /// This modules accepts a list of files on standard tabular input
    /// looks for an associated file for each one and adds that file's
    /// name to standard ouput stream
    /// </summary>
	public class AddAssociatedFile : FileProcessingBase
	{

        #region Member Variables

        #endregion

        #region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
        public AddAssociatedFile() {
            SourceFolderColumnName = "Folder";
            SourceFileColumnName = "Name";
        }

        #endregion

        #region Properites

        /// <summary>
        /// name of the column in the standard tabular input
        /// that contains the input folder path
        /// (optional - defaults to "Folder")
        /// </summary>
        public string SourceFolderColumnName { get; set; }

        /// <summary>
        /// name of the column in the standard tabular input
        /// that contains the input file name
        /// optional - defaults to "Name")
        /// </summary>
        public string SourceFileColumnName { get; set; }

        /// <summary>
        /// name of the output column that receives name of associated file
        /// </summary>
        public string ColumnToReceiveAssocFileName { get; set; }

        /// <summary>
        /// associated file name replacement pattern
        /// </summary>
        public string AssocFileNameReplacementPattern { 
            get {
                return string.Format("{0}|{1}", mSourceFileNameFragment, mAssociatedFileNameFragment);
            }
            set {
                string[] flds = value.Split('|'); 
                mSourceFileNameFragment = flds[0];
                mAssociatedFileNameFragment = flds[1];
            }
        }

        #endregion

        private int mInputFolderIdx;
        private int mInputFileIdx;
        private int mAssocFileIdx;

        private string mSourceFileNameFragment = "";
        private string mAssociatedFileNameFragment = "";

        /// <summary>
        /// Handle the column defs defined by args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            List<MageColumnDef> columnDefs = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));

            mInputFolderIdx = InputColumnPos[SourceFolderColumnName];
            mInputFileIdx = InputColumnPos[SourceFileColumnName];
            mAssocFileIdx = InputColumnPos[ColumnToReceiveAssocFileName];
        }

        /// <summary>
        /// Handle the data row defined by args
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                string folderPathSpec = args.Fields[mInputFolderIdx].ToString();
                string resultFileName = args.Fields[mInputFileIdx].ToString();
                string assocFileName = base.ReplaceEx(resultFileName, mSourceFileNameFragment, mAssociatedFileNameFragment);

                if (assocFileName == kNoFilesFound)
                {
                    args.Fields[mAssocFileIdx] = "";
                }
                else 
                {
                    // folderPathSpec may contain multiple folders, separated by a vertical bar
                    // If that is the case, then we'll search for files in each folder, preferentially using files in the folder listed first
                    List<string> folderPaths = new List<string>();
                    if (folderPathSpec.Contains('|')) {
                        folderPaths = folderPathSpec.Split('|').ToList<string>();
                    }
                    else {
                        folderPaths.Add(folderPathSpec);
                    }

                    foreach (string resultFolderPath in folderPaths)
                    {
						if (resultFolderPath.StartsWith(MYEMSL_PATH_FLAG))
						{
							string subDir;
							string parentFolders;
							string datasetName = DetermineDatasetName(resultFolderPath);

							GetMyEMSLParentFoldersAndSubDir(resultFolderPath, datasetName, out subDir, out parentFolders);

							string assocFileNameClean;
							DatasetInfoBase.ExtractMyEMSLFileID(assocFileName, out assocFileNameClean);

							m_RecentlyFoundMyEMSLFiles = m_MyEMSLDatasetInfoCache.FindFiles(assocFileNameClean, subDir, datasetName, recurse: false);
							if (m_RecentlyFoundMyEMSLFiles.Count > 0)
							{
								var archiveFileInfo = m_RecentlyFoundMyEMSLFiles[0];
								string encodedFilePath = DatasetInfoBase.AppendMyEMSLFileID(archiveFileInfo.FileInfo.Filename, archiveFileInfo.FileID);
								args.Fields[mAssocFileIdx] = encodedFilePath;

								CacheFilterPassingFile(archiveFileInfo.FileInfo);
								
							}
						}
						else if (File.Exists(Path.Combine(resultFolderPath, assocFileName)))
                        {
                            args.Fields[mAssocFileIdx] = assocFileName;
                            break;
                        }
                        else
                        {
                            args.Fields[mAssocFileIdx] = "";
                        }
                    }
                }

                OnDataRowAvailable(args);
            }
        }

    }
}
