using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MageDisplayLib;
using System.IO;
using System.Collections.ObjectModel;

namespace MageFileProcessor {

    /// <summary>
    /// This class contains several functions that build Mage pipelines 
    /// that supply data to the UI in response to user commands
    /// </summary>
    public class Pipelines {

        // class is not instantiated
        private Pipelines() {
        }

        /// <summary>
        /// pipeline to get list of jobs from DMS into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of jobs</param>
        /// <param name="queryDefXML">XML-formatted definition of query to use</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeJobQueryPipeline(ISinkModule sinkObject, string queryDefXML, Dictionary<string, string> runtimeParms) {

            // make source module and initialize from query def XML and runtime parameters 
            MSSQLReader rdr = new MSSQLReader(queryDefXML, runtimeParms);

            // build and wire pipeline
            return ProcessingPipeline.Assemble("JobQueryPipeline", rdr, sinkObject);
        }

        /// <summary>
        /// Pipeline to get list of files from results folders of jobs that are selected in list display
        /// and deliver the list to the files list display
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of folders to search</param>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeFileListPipeline(IBaseModule sourceObject, ISinkModule sinkObject, Dictionary<string, string> runtimeParms) {

            // create file filter module and initialize it
            FileListFilter fileFilter = new FileListFilter();
            fileFilter.SourceFolderColumnName = runtimeParms["SourceFolderColumnName"];
            fileFilter.FileColumnName = runtimeParms["FileColumnName"];
            fileFilter.OutputColumnList = runtimeParms["OutputColumnList"];
            fileFilter.FileNameSelector = runtimeParms["FileSelectors"];
            fileFilter.FileSelectorMode = runtimeParms["FileSelectionMode"];
            fileFilter.IncludeFilesOrFolders = runtimeParms["IncludeFilesOrFolders"];
            fileFilter.RecursiveSearch = runtimeParms["SearchInSubfolders"];
            fileFilter.SubfolderSearchName = runtimeParms["SubfolderSearchName"];

            // build and wire pipeline
            return ProcessingPipeline.Assemble("FileListPipeline", sourceObject, fileFilter, sinkObject);
        }

        /// <summary>
        /// pipeline to get selected list of files from local folder into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetLocalFileList(ISinkModule sinkObject, Dictionary<string, string> runtimeParms) {

            // make source module in pipeline to get list of files in local directory
            FileListFilter reader = new FileListFilter();
            reader.AddFolderPath(runtimeParms["Folder"]);
            reader.FileNameSelector = runtimeParms["FileNameFilter"];

			if (runtimeParms.ContainsKey("SearchInSubfolders"))
				reader.RecursiveSearch = runtimeParms["SearchInSubfolders"];

			if (runtimeParms.ContainsKey("SubfolderSearchName"))
				reader.SubfolderSearchName = runtimeParms["SubfolderSearchName"];

            reader.FileTypeColumnName = "Item";
            reader.FileColumnName = "Name";
            reader.SourceFolderColumnName = "Folder";
            reader.FileSizeColumnName = "File_Size_KB";
            reader.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text", reader.FileTypeColumnName, reader.FileColumnName, reader.FileSizeColumnName, reader.SourceFolderColumnName);
            reader.IncludeFilesOrFolders = "File";

            // build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToGetLocalFileList", reader, sinkObject);
        }

        /// <summary>
        /// Pipeline to read contents of manifest file
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetFilesFromManifest(ISinkModule sinkObject, Dictionary<string, string> runtimeParms) {
            string filePath = runtimeParms["ManifestFilePath"];
            string folderPath = Path.GetDirectoryName(filePath);

            // make source module in pipeline to get list of files in local directory
            DelimitedFileReader reader = new DelimitedFileReader();
            reader.FilePath = filePath;

            // filter module to add manifest file's folder as new column
            NullFilter filter = new NullFilter();
            string folderColName = "Manifest_Folder";
            filter.OutputColumnList = string.Format("Name, *, {0}|+|text", folderColName);
            filter.SetContext(new Dictionary<string, string>() { { folderColName, folderPath } });

            // build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToGetFilesFromManifest", reader, filter, sinkObject);
        }

        /// <summary>
        /// pipeline to copy files that are selected in the files list display to a local folder
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeFileCopyPipeline(IBaseModule sourceObject, Dictionary<string, string> runtimeParms) {

            // create file copy module and initialize it
            FileCopy copier = new FileCopy();
            string outputFolder = runtimeParms["OutputFolder"];
            copier.OutputFolderPath = outputFolder;
            copier.OutputColumnList = runtimeParms["OutputColumnList"];
            copier.ApplyPrefixToFileName = runtimeParms["ApplyPrefixToFileName"];
            copier.PrefixLeader = runtimeParms["PrefixLeader"];
            copier.ColumnToUseForPrefix = runtimeParms["ColumnToUseForPrefix"];

			if (runtimeParms["OverwriteExistingFiles"] == "Yes")
				copier.OverwriteExistingFiles = true;
			else
				copier.OverwriteExistingFiles = false;

            //
            copier.FileTypeColumnName = "Item";
            copier.SourceFolderColumnName = runtimeParms["SourceFolderColumnName"];
            copier.SourceFileColumnName = runtimeParms["SourceFileColumnName"];
            copier.OutputFileColumnName = runtimeParms["OutputFileColumnName"];

            // create a delimited file writer module to build manifest and initialize it
            DelimitedFileWriter writer = new DelimitedFileWriter();
            writer.FilePath = Path.Combine(outputFolder, runtimeParms["ManifestFileName"]);

            // build and wire pipeline
            return ProcessingPipeline.Assemble("FileCopyPipeline", sourceObject, copier, writer);
        }

        /// <summary>
        /// pipeline to filter contents of files that are selected in the files list display
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <param name="filterParms">Settings for parameters for filter module</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToFilterSelectedfiles(IBaseModule sourceObject, Dictionary<string, string> runtimeParms, Dictionary<string, string> filterParms) {

            // set up some parameter values
			string outputMode = runtimeParms["OutputMode"];
            string outputFolderPath = runtimeParms["OutputFolder"];
            string filterName = filterParms["SelectedFilterClassName"];
            filterParms.Remove("SelectedFilterClassName");

			if (string.IsNullOrEmpty(filterName))
				filterName = "All Pass";

            string reportFileName = string.Format("Runlog_{0}_{1:yyyy-MM-dd_hhmmss}.txt", filterName.Replace(" ", "_"), System.DateTime.Now);

            // make file sub-pipeline processing broker module 
            // to run a filter pipeline against files from list
            FileSubPipelineBroker broker = new FileSubPipelineBroker();
            broker.OutputFileName = runtimeParms["OutputFile"];
            broker.SourceFileColumnName = "Name";
            broker.SourceFolderColumnName = "Folder";
            broker.OutputFolderPath = outputFolderPath;

			if (filterName.ToLower() == "All Pass".ToLower())
				broker.FileFilterModuleName = "NullFilter";
			else
				broker.FileFilterModuleName = filterName;

            broker.SetFileFilterParameters(filterParms);
            if (outputMode == "SQLite_Output") {
                broker.DatabaseName = runtimeParms["DatabaseName"];
                broker.TableName = runtimeParms["TableName"];
                outputFolderPath = Path.GetDirectoryName(runtimeParms["DatabaseName"]);
            }

            // create a file writer module to output manifest file
            DelimitedFileWriter writer = new DelimitedFileWriter();
            writer.FilePath = Path.Combine(outputFolderPath, reportFileName);

            // build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToFilterSelectedfiles", sourceObject, broker, writer);
        }

    }
}
