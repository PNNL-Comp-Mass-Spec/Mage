using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mage {
 
    /// <summary>
    /// delegate for a function that returns and output file name for a given input file name and parameters
    /// </summary>
    /// <param name="sourceFile">the original name of the file</param>
    /// <param name="fieldPos">index to the metadata field to be used in renaming</param>
    /// <param name="fields">list of metadata fields for original file</param>
    /// <returns></returns>
    public delegate string OutputFileNamer(string sourceFile, Dictionary<string, int> fieldPos, object[] fields);

    /// <summary>
    /// module that provides base functions for processing one or more input files 
    ///
    /// it expects to receive path information for files via its standard tabular input
    ///
    /// each row of standard tabular input will contain information for a single file 
    /// (parameters SourceFileColumnName and SourceFolderColumnName) define which
    /// columns in stardard input contain the folder and name of the input file.
    ///
    /// the OutputFolderPath parameter tells this module where to put results files
    /// 
    /// this module outputs a record of each file processed on stardard tabular output
    /// </summary>
	public class FileContentProcessor : FileProcessingBase
	{

        #region Member Variables

        // delegate that this module calls to get output file name
        private OutputFileNamer GetOutputFileName = null;

        #endregion

        #region Functions Available to Clients

        /// <summary>
        /// define a delegate function that will generate output file name 
        /// </summary>
        /// <param name="namer"></param>
        public void SetOutputFileNamer(OutputFileNamer namer) {
            GetOutputFileName = namer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// path to the folder into which the 
        /// processed input file contents will be saved as an output file
        /// (required by subclasses that create result files) 
        /// </summary>
        public string OutputFolderPath { get; set; }

        /// <summary>
        /// If this is not blank, it will be combined with the
        /// OutputFolderPath to generate the output file file.
        /// This will override use of any naming information
        /// from data row
        /// </summary>
        public string OutputFileName { get; set; }

        /// <summary>
        /// name of the column in the standard tabular input
        /// that contains the input folder path
        /// (optional - defaults to "Folder")
        /// </summary>
        public string SourceFolderColumnName { get; set; }

        /// <summary>
        /// name of the column in the standard tabular input
        /// that contains the input file name
        /// optional - defaults to "File")
        /// </summary>
        public string SourceFileColumnName { get; set; }

        /// <summary>
        /// name of the column in the standard tabular input
        /// that contains the input file type
        /// optional - defaults to blank)
        /// </summary>
        public string FileTypeColumnName { get; set; }

        /// <summary>
        /// the name of the output column that will contain the file name
        /// </summary>
        public string OutputFileColumnName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct new Mage file content processor module
        /// </summary>
        public FileContentProcessor() {
            SourceFolderColumnName = "Folder";
            FileTypeColumnName = "";
            SourceFileColumnName = "File";
            OutputFileColumnName = "File";
            OutputFileName = "";

            OutputColumnList = string.Format("{0}|+|text, {1}", OutputFileColumnName, SourceFolderColumnName);
            GetOutputFileName = GetDefaultOutputFileName;
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                bool concatenateOutput = !string.IsNullOrEmpty(OutputFileName);
                string sourceFolder = args.Fields[InputColumnPos[SourceFolderColumnName]].ToString();
                string sourceFile = args.Fields[InputColumnPos[SourceFileColumnName]].ToString();

				string fileType;
				bool sourceIsFolder = false;

				if (string.IsNullOrWhiteSpace(FileTypeColumnName))
				{
					if (System.IO.Directory.Exists(sourceFolder))
					{
						if (System.IO.File.Exists(Path.Combine(sourceFolder, sourceFile)))
							fileType = "file";
						else
						{
							UpdateStatus(this, new MageStatusEventArgs("FAILED->Cannot process folders with this processing mode", 1));
							OnWarningMessage(new MageStatusEventArgs("Cannot process folders with this processing mode"));
							System.Threading.Thread.Sleep(500);
							return;
						}
					}
					else
					{
						UpdateStatus(this, new MageStatusEventArgs("FAILED->Folder Not Found: " + sourceFolder, 1));
						OnWarningMessage(new MageStatusEventArgs("Copy failed->Folder Not Found: " + sourceFolder));
						System.Threading.Thread.Sleep(250);
						return;
					}
				}
				else
					fileType = args.Fields[InputColumnPos[FileTypeColumnName]].ToString();

				if (fileType == "folder")
					sourceIsFolder = true;

                string sourcePath;

				if (sourceIsFolder)
					sourcePath = sourceFolder;
				else
					sourcePath = Path.GetFullPath(Path.Combine(sourceFolder, sourceFile));

                string destFolder = OutputFolderPath;
				string destFile;
                string destPath;

				if (sourceFile == BaseModule.kNoFilesFound) {
					destFile = kNoFilesFound;
				} else 
				{
					if (concatenateOutput && !sourceIsFolder)
						destFile = OutputFileName;
					else
						destFile = GetOutputFileName(sourceFile, InputColumnPos, args.Fields);
				}

				destPath =  Path.GetFullPath(Path.Combine(destFolder, destFile));

                // package fields as dictionary
                Dictionary<string, string> context = new Dictionary<string, string>();
                foreach (KeyValuePair<string, int> colPos in InputColumnPos) {
					if (args.Fields[colPos.Value] == null)
						context.Add(colPos.Key, String.Empty);
					else
						context.Add(colPos.Key, args.Fields[colPos.Value].ToString());
                }

                // process file
                if (sourceFile == BaseModule.kNoFilesFound) {
					// skip
                } else
					if (string.IsNullOrEmpty(FileTypeColumnName) && fileType != "folder")
					{
						ProcessFile(sourceFile, sourcePath, destPath, context);
					} else {
						if (fileType == "file") {
							ProcessFile(sourceFile, sourcePath, destPath, context);
						}
						if (fileType == "folder") {
							ProcessFolder(sourceFile, sourcePath, destPath);
						}
					}

                object[] outRow = MapDataRow(args.Fields);
				
                int fileNameOutColIndx = OutputColumnPos[OutputFileColumnName];
                outRow[fileNameOutColIndx] = (concatenateOutput) ? sourceFile : destFile;

				// Strip off the MyEMSLID from the filename
				string newFilePath;
				Int64 myEMSLFileID = MyEMSLReader.DatasetInfoBase.ExtractMyEMSLFileID(outRow[fileNameOutColIndx].ToString(), out newFilePath);
				if (myEMSLFileID > 0)
					outRow[fileNameOutColIndx] = newFilePath;

				OnDataRowAvailable(new MageDataEventArgs(outRow));
            } else {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            // build lookup of column index by column name
            base.HandleColumnDef(sender, args);
            // end of column definitions from our source,
            // now tell our subscribers what columns to expect from us
            ExportColumnDefs();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// this function should be overriden by subclasses to do the actual processing
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="context"></param>
        protected virtual void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context) {
        }

        /// <summary>
        /// this function should be overriden by subclasses to do the actual processing
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        protected virtual void ProcessFolder(string sourceFile, string sourcePath, string destPath) {
        }

        #endregion

        #region Utility functions

        /// <summary>
        /// default output file renamer
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="fieldPos"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected string GetDefaultOutputFileName(string sourceFile, Dictionary<string, int> fieldPos, object[] fields) {
            return sourceFile;
        }

        #endregion

        #region Functions for Output Columns

        // tell our subscribers what columns to expect from us
        // which will be information about the files processed
        // pluss any input columns that are passed through to output
        private void ExportColumnDefs() {
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// update any interested listeners about our progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void UpdateStatus(object sender, MageStatusEventArgs args) {
            OnStatusMessageUpdated(args);
        }

        #endregion
    }
}
