using System;
using System.Collections.Generic;
using Mage;
using System.IO;
using System.Collections.ObjectModel;

namespace MageFileProcessor
{

    /// <summary>
    /// This class contains several functions that build Mage pipelines
    /// that supply data to the UI in response to user commands
    /// </summary>
    public static class Pipelines
    {

        public const string PIPELINE_GET_DMS_FILES = "FileListPipeline";
        public const string PIPELINE_GET_LOCAL_FILES = "PipelineToGetLocalFileList";

        // No constructor; this class is never instantiated

        /// <summary>
        /// Pipeline to get list of jobs from DMS into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of jobs</param>
        /// <param name="queryDefXML">XML-formatted definition of query to use</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeJobQueryPipeline(ISinkModule sinkObject, string queryDefXML, Dictionary<string, string> runtimeParms)
        {

            // Make source module and initialize from query def XML and runtime parameters
            var rdr = new MSSQLReader(queryDefXML, runtimeParms);

            // Build and wire pipeline
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
        public static ProcessingPipeline MakeFileListPipeline(IBaseModule sourceObject, ISinkModule sinkObject, Dictionary<string, string> runtimeParms)
        {

            // Create file filter module and initialize it
            var fileFilter = new FileListFilter
            {
                SourceFolderColumnName = GetRuntimeParam(runtimeParms, "SourceFolderColumnName"),
                FileColumnName = GetRuntimeParam(runtimeParms, "FileColumnName"),
                OutputColumnList = GetRuntimeParam(runtimeParms, "OutputColumnList"),
                FileNameSelector = GetRuntimeParam(runtimeParms, "FileSelectors"),
                FileSelectorMode = GetRuntimeParam(runtimeParms, "FileSelectionMode"),
                IncludeFilesOrFolders = GetRuntimeParam(runtimeParms, "IncludeFilesOrFolders"),
                RecursiveSearch = GetRuntimeParam(runtimeParms, "SearchInSubfolders"),
                SubfolderSearchName = GetRuntimeParam(runtimeParms, "SubfolderSearchName")
            };

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_DMS_FILES, sourceObject, fileFilter, sinkObject);
        }

        /// <summary>
        /// Pipeline to get selected list of files from local folder into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetLocalFileList(ISinkModule sinkObject, Dictionary<string, string> runtimeParms)
        {

            // Make source module in pipeline to get list of files in local directory
            var reader = new FileListFilter();
            reader.AddFolderPath(GetRuntimeParam(runtimeParms, "Folder"));
            reader.FileNameSelector = GetRuntimeParam(runtimeParms, "FileNameFilter");

            if (runtimeParms.ContainsKey("FileSelectionMode"))
                reader.FileSelectorMode = GetRuntimeParam(runtimeParms, "FileSelectionMode");

            if (runtimeParms.ContainsKey("SearchInSubfolders"))
                reader.RecursiveSearch = GetRuntimeParam(runtimeParms, "SearchInSubfolders");

            if (runtimeParms.ContainsKey("SubfolderSearchName"))
                reader.SubfolderSearchName = GetRuntimeParam(runtimeParms, "SubfolderSearchName");

            reader.FileTypeColumnName = FileListInfoBase.COLUMN_NAME_FILE_TYPE;         // Item
            reader.FileColumnName = FileListInfoBase.COLUMN_NAME_FILE_NAME;             // File
            reader.SourceFolderColumnName = FileListInfoBase.COLUMN_NAME_SOURCE_FOLDER; // Folder
            reader.FileSizeColumnName = FileListInfoBase.COLUMN_NAME_FILE_SIZE;         // File_Size_KB
            reader.FileDateColumnName = FileListInfoBase.COLUMN_NAME_FILE_DATE;			// File_Date
            reader.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", reader.FileTypeColumnName, reader.FileColumnName, reader.FileSizeColumnName, reader.FileDateColumnName, reader.SourceFolderColumnName);
            reader.IncludeFilesOrFolders = "File";

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_LOCAL_FILES, reader, sinkObject);
        }

        /// <summary>
        /// Pipeline to read contents of manifest file
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetFilesFromManifest(ISinkModule sinkObject, Dictionary<string, string> runtimeParms)
        {
            var filePath = GetRuntimeParam(runtimeParms, "ManifestFilePath");
            var folderPath = Path.GetDirectoryName(filePath);

            // Make source module in pipeline to get list of files in local directory
            var reader = new DelimitedFileReader
            {
                FilePath = filePath
            };

            // Filter module to add manifest file's folder as new column
            var filter = new NullFilter();
            const string folderColName = "Manifest_Folder";
            filter.OutputColumnList = string.Format("Name, *, {0}|+|text", folderColName);
            filter.SetContext(new Dictionary<string, string> { { folderColName, folderPath } });

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToGetFilesFromManifest", reader, filter, sinkObject);
        }

        /// <summary>
        /// Pipeline to copy files that are selected in the files list display to a local folder
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeFileCopyPipeline(IBaseModule sourceObject, Dictionary<string, string> runtimeParms)
        {

            // Create file copy module and initialize it
            var outputFolder = GetRuntimeParam(runtimeParms, "OutputFolder");
            var copier = new FileCopy
            {
                OutputFolderPath = outputFolder,
                OutputColumnList = GetRuntimeParam(runtimeParms, "OutputColumnList"),
                ApplyPrefixToFileName = GetRuntimeParam(runtimeParms, "ApplyPrefixToFileName"),
                PrefixLeader = GetRuntimeParam(runtimeParms, "PrefixLeader"),
                ColumnToUseForPrefix = GetRuntimeParam(runtimeParms, "ColumnToUseForPrefix")
            };

            if (string.Equals(GetRuntimeParam(runtimeParms, "OverwriteExistingFiles"), "Yes", StringComparison.OrdinalIgnoreCase))
                copier.OverwriteExistingFiles = true;
            else
                copier.OverwriteExistingFiles = false;

            if (string.Equals(GetRuntimeParam(runtimeParms, "ResolveCacheInfoFiles"), "Yes", StringComparison.OrdinalIgnoreCase))
                copier.ResolveCacheInfoFiles = true;
            else
                copier.ResolveCacheInfoFiles = false;

            copier.FileTypeColumnName = "Item";
            copier.SourceFolderColumnName = GetRuntimeParam(runtimeParms, "SourceFolderColumnName");
            copier.SourceFileColumnName = GetRuntimeParam(runtimeParms, "SourceFileColumnName");
            copier.OutputFileColumnName = GetRuntimeParam(runtimeParms, "OutputFileColumnName");

            // Create a delimited file writer module to build manifest and initialize it
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputFolder, GetRuntimeParam(runtimeParms, "ManifestFileName"))
            };

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("FileCopyPipeline", sourceObject, copier, writer);
        }

        /// <summary>
        /// Pipeline to filter contents of files that are selected in the files list display
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParms">Settings for parameters for modules in the pipeline</param>
        /// <param name="filterParms">Settings for parameters for filter module</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToFilterSelectedfiles(IBaseModule sourceObject, Dictionary<string, string> runtimeParms, Dictionary<string, string> filterParms)
        {

            // Set up some parameter values
            var outputMode = GetRuntimeParam(runtimeParms, "OutputMode");
            var outputFolderPath = GetRuntimeParam(runtimeParms, "OutputFolder") ?? "";
            var filterName = filterParms["SelectedFilterClassName"];
            filterParms.Remove("SelectedFilterClassName");

            if (string.IsNullOrEmpty(filterName))
                filterName = "All Pass";

            var reportFileName = string.Format("Runlog_{0}_{1:yyyy-MM-dd_hhmmss}.txt", filterName.Replace(" ", "_"), System.DateTime.Now);

            // Make file sub-pipeline processing broker module
            // to run a filter pipeline against files from list
            var broker = new FileSubPipelineBroker
            {
                OutputFileName = GetRuntimeParam(runtimeParms, "OutputFile"),
                SourceFileColumnName = "Name",
                SourceFolderColumnName = "Folder",
                OutputFolderPath = outputFolderPath
            };

            if (filterName.ToLower() == "All Pass".ToLower())
                broker.FileFilterModuleName = "NullFilter";
            else
                broker.FileFilterModuleName = filterName;

            broker.SetFileFilterParameters(filterParms);
            if (outputMode == "SQLite_Output")
            {
                broker.DatabaseName = GetRuntimeParam(runtimeParms, "DatabaseName");
                broker.TableName = GetRuntimeParam(runtimeParms, "TableName");
                outputFolderPath = Path.GetDirectoryName(GetRuntimeParam(runtimeParms, "DatabaseName"));
            }

            // Create a file writer module to output manifest file
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputFolderPath ?? "", reportFileName)
            };

            // Build and wire pipeline
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
            var outputMode = GetRuntimeParam(runtimeParms, "OutputMode");
            var outputFolderPath = GetRuntimeParam(runtimeParms, "OutputFolder");

            var pipelineQueue = new PipelineQueue();

            // ProcessingPipeline pxjm = MakePipelineToExportJobMetadata(sourceObject, outputMode, outputFolderPath);
            // pipelineQueue.Add(pxjm);

            // Buffer module to accumulate aggregated file list
            var fileList = new SimpleSink();

            // Search job results folders for list of results files to process
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
                broker.DatabaseName = GetRuntimeParam(runtimeParms, "DatabaseName");
                broker.TableName = GetRuntimeParam(runtimeParms, "TableName");
                outputFolderPath = Path.GetDirectoryName(GetRuntimeParam(runtimeParms, "DatabaseName"));
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
            var outputFolder = GetRuntimeParam(runtimeParms, "OutputFolder");

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

            if (runtimeParms.TryGetValue("SourceFileColumnName", out var valueOverride))
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

            // Create a delimited file writer module to build manifest and initialize it
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputFolder, GetRuntimeParam(runtimeParms, "ManifestFileName")),
                OutputColumnList = "*"
            };
            modules.Add(writer);

            var filePipeline = ProcessingPipeline.Assemble("Search For Files", modules);
            return filePipeline;
        }

        /// <summary>
        /// Return the value for the given runtime parameter
        /// </summary>
        /// <param name="runtimeParams">Runtime parameters</param>
        /// <param name="keyName">Parameter to find</param>
        /// <returns>The value for the parameter</returns>
        /// <remarks>Raises an exception if the runtimeParams dictionary does not have the desired key</remarks>
        private static string GetRuntimeParam(IReadOnlyDictionary<string, string> runtimeParams, string keyName)
        {
            if (!runtimeParams.ContainsKey(keyName))
                throw new Exception("runtimeParams does not contain key " + keyName);

            return runtimeParams[keyName];
        }

    }
}
