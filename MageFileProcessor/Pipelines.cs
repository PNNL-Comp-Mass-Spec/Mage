using System.Collections.Generic;
using Mage;
using System.IO;
using System.Collections.ObjectModel;

namespace MageFileProcessor {

    /// <summary>
    /// This class contains several functions that build Mage pipelines 
    /// that supply data to the UI in response to user commands
    /// </summary>
    public class Pipelines
    {

        public const string PIPELINE_GET_DMS_FILES = "FileListPipeline";
        public const string PIPELINE_GET_LOCAL_FILES = "PipelineToGetLocalFileList";

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
            var rdr = new MSSQLReader(queryDefXML, runtimeParms);

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
            var fileFilter = new FileListFilter
            {
	            SourceFolderColumnName = runtimeParms["SourceFolderColumnName"],
	            FileColumnName = runtimeParms["FileColumnName"],
	            OutputColumnList = runtimeParms["OutputColumnList"],
	            FileNameSelector = runtimeParms["FileSelectors"],
	            FileSelectorMode = runtimeParms["FileSelectionMode"],
	            IncludeFilesOrFolders = runtimeParms["IncludeFilesOrFolders"],
	            RecursiveSearch = runtimeParms["SearchInSubfolders"],
	            SubfolderSearchName = runtimeParms["SubfolderSearchName"]
            };

	        // build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_DMS_FILES, sourceObject, fileFilter, sinkObject);
        }

        /// <summary>
        /// pipeline to get selected list of files from local folder into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetLocalFileList(ISinkModule sinkObject, Dictionary<string, string> runtimeParms) {

            // make source module in pipeline to get list of files in local directory
            var reader = new FileListFilter();
            reader.AddFolderPath(runtimeParms["Folder"]);
            reader.FileNameSelector = runtimeParms["FileNameFilter"];

            if (runtimeParms.ContainsKey("FileSelectionMode"))
				reader.FileSelectorMode = runtimeParms["FileSelectionMode"];

			if (runtimeParms.ContainsKey("SearchInSubfolders"))
				reader.RecursiveSearch = runtimeParms["SearchInSubfolders"];

			if (runtimeParms.ContainsKey("SubfolderSearchName"))
				reader.SubfolderSearchName = runtimeParms["SubfolderSearchName"];

			reader.FileTypeColumnName = FileListInfoBase.COLUMN_NAME_FILE_TYPE;			// Item
			reader.FileColumnName = FileListInfoBase.COLUMN_NAME_FILE_NAME;				// File
			reader.SourceFolderColumnName = FileListInfoBase.COLUMN_NAME_SOURCE_FOLDER;	// Folder
			reader.FileSizeColumnName = FileListInfoBase.COLUMN_NAME_FILE_SIZE;			// File_Size_KB
			reader.FileDateColumnName = FileListInfoBase.COLUMN_NAME_FILE_DATE;			// File_Date
            reader.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", reader.FileTypeColumnName, reader.FileColumnName, reader.FileSizeColumnName, reader.FileDateColumnName, reader.SourceFolderColumnName);
            reader.IncludeFilesOrFolders = "File";

            // build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_LOCAL_FILES, reader, sinkObject);
        }

        /// <summary>
        /// Pipeline to read contents of manifest file
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetFilesFromManifest(ISinkModule sinkObject, Dictionary<string, string> runtimeParms) {
            var filePath = runtimeParms["ManifestFilePath"];
            var folderPath = Path.GetDirectoryName(filePath);

            // make source module in pipeline to get list of files in local directory
            var reader = new DelimitedFileReader
            {
	            FilePath = filePath
            };

	        // filter module to add manifest file's folder as new column
            var filter = new NullFilter();
            const string folderColName = "Manifest_Folder";
            filter.OutputColumnList = string.Format("Name, *, {0}|+|text", folderColName);
            filter.SetContext(new Dictionary<string, string> { { folderColName, folderPath } });

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
			var outputFolder = runtimeParms["OutputFolder"];
			var copier = new FileCopy
			{
				OutputFolderPath = outputFolder,
				OutputColumnList = runtimeParms["OutputColumnList"],
				ApplyPrefixToFileName = runtimeParms["ApplyPrefixToFileName"],
				PrefixLeader = runtimeParms["PrefixLeader"],
				ColumnToUseForPrefix = runtimeParms["ColumnToUseForPrefix"]
			};

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
            var writer = new DelimitedFileWriter
            {
	            FilePath = Path.Combine(outputFolder, runtimeParms["ManifestFileName"])
            };

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
			var outputMode = runtimeParms["OutputMode"];
            var outputFolderPath = runtimeParms["OutputFolder"] ?? "";
            var filterName = filterParms["SelectedFilterClassName"];
            filterParms.Remove("SelectedFilterClassName");

			if (string.IsNullOrEmpty(filterName))
				filterName = "All Pass";

            var reportFileName = string.Format("Runlog_{0}_{1:yyyy-MM-dd_hhmmss}.txt", filterName.Replace(" ", "_"), System.DateTime.Now);
		
            // make file sub-pipeline processing broker module 
            // to run a filter pipeline against files from list
            var broker = new FileSubPipelineBroker
            {
	            OutputFileName = runtimeParms["OutputFile"],
	            SourceFileColumnName = "Name",
	            SourceFolderColumnName = "Folder",
	            OutputFolderPath = outputFolderPath
            };

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
            var writer = new DelimitedFileWriter
            {
	            FilePath = Path.Combine(outputFolderPath ?? "", reportFileName)
            };

	        // build and wire pipeline
			return ProcessingPipeline.Assemble("PipelineToFilterSelectedfiles", sourceObject, broker, writer);
        }



		/// <summary>
		/// Build and return Mage pipeline queue to extract contents of results files
		/// for jobs given in jobList according to parameters defined in mExtractionParms 
		/// and deliver output according to mDestination.  Also create metadata file for jobList.
		/// </summary>
		public static PipelineQueue MakePipelineQueueToPreProcessThenFilterFiles(
			BaseModule sourceObject, 
			Dictionary<string, string> runtimeParms, 
			Dictionary<string, string> filterParms)
		{
			var outputMode = runtimeParms["OutputMode"];
			var outputFolderPath = runtimeParms["OutputFolder"];

			var pipelineQueue = new PipelineQueue();

			//ProcessingPipeline pxjm = MakePipelineToExportJobMetadata(sourceObject, outputMode, outputFolderPath);
			//pipelineQueue.Add(pxjm);

			// buffer module to accumulate aggregated file list
			var fileList = new SimpleSink();

			// search job results folders for list of results files to process
			// and accumulate into buffer module
			var plof = MakePipelineToGetListOfFiles(sourceObject, fileList, runtimeParms);
			pipelineQueue.Add(plof);

			var sinkWrapper = new MyEMSLSinkWrapper(fileList)
			{
				TempFilesContainerPath = outputFolderPath,
				PredownloadMyEMSLFiles = true
			};

			var pxfl = MakePipelineToFilterSelectedfiles(sinkWrapper, runtimeParms, filterParms);
			pipelineQueue.Add(pxfl);

			return pipelineQueue;
		}

		/*
		/// <summary>
		/// Make a Mage pipeline to dump contents of job list 
		/// as metadata file or db table
		/// </summary>
		/// <param name="jobList"></param>
		/// <returns></returns>
		public static ProcessingPipeline MakePipelineToExportJobMetadata(BaseModule jobList, string outputMode, string outputFolderPath)
		{
			if (outputMode == )
			{
				broker.DatabaseName = runtimeParms["DatabaseName"];
				broker.TableName = runtimeParms["TableName"];
				outputFolderPath = Path.GetDirectoryName(runtimeParms["DatabaseName"]);
			}


			BaseModule writer = null;
			switch (outputMode)
			{
				case "SQLite_Output"
					SQLiteWriter sw = new SQLiteWriter();
					sw.DbPath = destination.ContainerPath;
					sw.TableName = destination.MetadataName;
					writer = sw;
					break;
				case DestinationType.Types.File_Output:
					DelimitedFileWriter dw = new DelimitedFileWriter();
					dw.FilePath = Path.Combine(destination.ContainerPath, destination.MetadataName);
					writer = dw;
					break;
			}
			ProcessingPipeline filePipeline = ProcessingPipeline.Assemble("Job Metadata", jobList, writer);
			return filePipeline;
		}
		 */

		/// <summary>
		/// Make a Mage pipeline to lookup file info for the source files
		/// Results are accumulated into the given sink module
		/// </summary>
		/// <param name="jobListSource">Mage module that contains list of files</param>
		/// <param name="fileListSink">Mage module to accumulate list of results into</param>
		/// <param name="runtimeParms">Runtime parameters</param>
		/// <returns></returns>
		public static ProcessingPipeline MakePipelineToGetListOfFiles(BaseModule jobListSource, BaseModule fileListSink, Dictionary<string, string> runtimeParms)
		{
			var outputFolder = runtimeParms["OutputFolder"];

			var modules = new Collection<object>
			{
				jobListSource
			};

			var fileFinder = new FileListInfoLookup
			{
				SourceFileColumnName = "Name",
				SourceFolderColumnName = "Folder",
				FileColumnName = "Name"
			};

            string valueOverride;
            if (runtimeParms.TryGetValue("SourceFileColumnName", out valueOverride))
            {
                fileFinder.SourceFileColumnName = valueOverride;
                fileFinder.FileColumnName = valueOverride;
            }

            if (runtimeParms.TryGetValue("SourceFolderColumnName", out valueOverride))
                fileFinder.SourceFolderColumnName = valueOverride;

			fileFinder.OutputColumnList = string.Format("Job, Item|{0}, Folder, Name|{1}, File_Size_KB|{2}, File_Date|{3}", 
				FileListInfoBase.COLUMN_NAME_FILE_TYPE, 
				fileFinder.FileColumnName, 
				FileListInfoBase.COLUMN_NAME_FILE_SIZE, 
				FileListInfoBase.COLUMN_NAME_FILE_DATE);

			modules.Add(fileFinder);
			modules.Add(fileListSink);

			// create a delimited file writer module to build manifest and initialize it
			var writer = new DelimitedFileWriter
			{
				FilePath = Path.Combine(outputFolder, runtimeParms["ManifestFileName"]),
				OutputColumnList = "*"
			};
			modules.Add(writer);

			var filePipeline = ProcessingPipeline.Assemble("Search For Files", modules);
			return filePipeline;
		}
    }
}
