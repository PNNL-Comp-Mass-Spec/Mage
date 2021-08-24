using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Mage;

namespace MageFileProcessor
{
    /// <summary>
    /// This class contains several functions that build Mage pipelines
    /// that supply data to the UI in response to user commands
    /// </summary>
    public static class Pipelines
    {
        // Ignore Spelling: Mage

        public const string PIPELINE_GET_DMS_FILES = "FileListPipeline";
        public const string PIPELINE_GET_LOCAL_FILES = "PipelineToGetLocalFileList";

        // No constructor; this class is never instantiated

        /// <summary>
        /// Pipeline to get list of jobs from DMS into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of jobs</param>
        /// <param name="queryDefXML">XML-formatted definition of query to use</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeJobQueryPipeline(ISinkModule sinkObject, string queryDefXML, Dictionary<string, string> runtimeParams)
        {
            // Make source module and initialize from query def XML and runtime parameters
            var rdr = new SQLReader(queryDefXML, runtimeParams);

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("JobQueryPipeline", rdr, sinkObject);
        }

        /// <summary>
        /// Pipeline to get list of files from results directories of jobs that are selected in list display
        /// and deliver the list to the files list display
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of directories to search</param>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeFileListPipeline(IBaseModule sourceObject, ISinkModule sinkObject, Dictionary<string, string> runtimeParams)
        {
            // Create file filter module and initialize it
            var fileFilter = new FileListFilter
            {
                SourceDirectoryColumnName = GetRuntimeParam(runtimeParams, "SourceDirectoryColumnName"),
                FileColumnName = GetRuntimeParam(runtimeParams, "FileColumnName"),
                OutputColumnList = GetRuntimeParam(runtimeParams, "OutputColumnList"),
                FileNameSelector = GetRuntimeParam(runtimeParams, "FileSelectors"),
                FileSelectorMode = GetRuntimeParam(runtimeParams, "FileSelectionMode"),
                IncludeFilesOrDirectories = GetRuntimeParam(runtimeParams, "IncludeFilesOrDirectories"),
                RecursiveSearch = GetRuntimeParam(runtimeParams, "SearchInSubdirectories"),
                SubdirectorySearchName = GetRuntimeParam(runtimeParams, "SubdirectorySearchName")
            };

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_DMS_FILES, sourceObject, fileFilter, sinkObject);
        }

        /// <summary>
        /// Pipeline to get selected list of files from local directory into list display
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetLocalFileList(ISinkModule sinkObject, Dictionary<string, string> runtimeParams)
        {
            // Make source module in pipeline to get list of files in local directory
            var reader = new FileListFilter();

            if (runtimeParams.ContainsKey("Directory"))
                reader.AddDirectoryPath(GetRuntimeParam(runtimeParams, "Directory"));
            else if (runtimeParams.ContainsKey("Folder"))
                reader.AddDirectoryPath(GetRuntimeParam(runtimeParams, "Folder"));

            reader.FileNameSelector = GetRuntimeParam(runtimeParams, "FileNameFilter");

            if (runtimeParams.ContainsKey("FileSelectionMode"))
                reader.FileSelectorMode = GetRuntimeParam(runtimeParams, "FileSelectionMode");

            if (runtimeParams.ContainsKey("SearchInSubdirectories"))
                reader.RecursiveSearch = GetRuntimeParam(runtimeParams, "SearchInSubdirectories");

            if (runtimeParams.ContainsKey("SubdirectorySearchName"))
                reader.SubdirectorySearchName = GetRuntimeParam(runtimeParams, "SubdirectorySearchName");

            reader.FileTypeColumnName = FileListInfoBase.COLUMN_NAME_FILE_TYPE;                 // Item
            reader.FileColumnName = FileListInfoBase.COLUMN_NAME_FILE_NAME;                     // File
            reader.SourceDirectoryColumnName = FileListInfoBase.COLUMN_NAME_SOURCE_DIRECTORY;   // Directory
            reader.FileSizeColumnName = FileListInfoBase.COLUMN_NAME_FILE_SIZE;                 // File_Size_KB
            reader.FileDateColumnName = FileListInfoBase.COLUMN_NAME_FILE_DATE;			        // File_Date
            reader.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", reader.FileTypeColumnName, reader.FileColumnName, reader.FileSizeColumnName, reader.FileDateColumnName, reader.SourceDirectoryColumnName);
            reader.IncludeFilesOrDirectories = "File";

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_LOCAL_FILES, reader, sinkObject);
        }

        /// <summary>
        /// Pipeline to read contents of manifest file
        /// </summary>
        /// <param name="sinkObject">External ISinkModule object that will receive list of files found</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToGetFilesFromManifest(ISinkModule sinkObject, Dictionary<string, string> runtimeParams)
        {
            var filePath = GetRuntimeParam(runtimeParams, "ManifestFilePath");
            var directoryPath = Path.GetDirectoryName(filePath);

            // Make source module in pipeline to get list of files in local directory
            var reader = new DelimitedFileReader
            {
                FilePath = filePath
            };

            // Filter module to add manifest file's directory as new column
            var filter = new NullFilter();
            const string manifestDirColName = "Manifest_Directory";
            filter.OutputColumnList = string.Format("Name, *, {0}|+|text", manifestDirColName);
            filter.SetContext(new Dictionary<string, string> { { manifestDirColName, directoryPath } });

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToGetFilesFromManifest", reader, filter, sinkObject);
        }

        /// <summary>
        /// Pipeline to copy files that are selected in the files list display to a local directory
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakeFileCopyPipeline(IBaseModule sourceObject, Dictionary<string, string> runtimeParams)
        {
            // Create file copy module and initialize it
            var outputDirectory = GetRuntimeParam(runtimeParams, "OutputDirectory");

            var copier = new FileCopy
            {
                OutputDirectoryPath = outputDirectory,
                OutputColumnList = GetRuntimeParam(runtimeParams, "OutputColumnList"),
                ApplyPrefixToFileName = GetRuntimeParam(runtimeParams, "ApplyPrefixToFileName"),
                PrefixLeader = GetRuntimeParam(runtimeParams, "PrefixLeader"),
                ColumnToUseForPrefix = GetRuntimeParam(runtimeParams, "ColumnToUseForPrefix"),
                OverwriteExistingFiles = string.Equals(GetRuntimeParam(runtimeParams, "OverwriteExistingFiles"), "Yes", StringComparison.OrdinalIgnoreCase),
                ResolveCacheInfoFiles = string.Equals(GetRuntimeParam(runtimeParams, "ResolveCacheInfoFiles"), "Yes", StringComparison.OrdinalIgnoreCase),
                FileTypeColumnName = "Item",
                SourceDirectoryColumnName = GetRuntimeParam(runtimeParams, "SourceDirectoryColumnName"),
                SourceFileColumnName = GetRuntimeParam(runtimeParams, "SourceFileColumnName"),
                OutputFileColumnName = GetRuntimeParam(runtimeParams, "OutputFileColumnName")
            };

            // Create a delimited file writer module to build manifest and initialize it
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputDirectory, GetRuntimeParam(runtimeParams, "ManifestFileName"))
            };

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("FileCopyPipeline", sourceObject, copier, writer);
        }

        /// <summary>
        /// Pipeline to filter contents of files that are selected in the files list display
        /// </summary>
        /// <param name="sourceObject">Module that will supply list of files</param>
        /// <param name="runtimeParams">Settings for parameters for modules in the pipeline</param>
        /// <param name="filterParms">Settings for parameters for filter module</param>
        /// <returns>Mage pipeline</returns>
        public static ProcessingPipeline MakePipelineToFilterSelectedFiles(IBaseModule sourceObject, Dictionary<string, string> runtimeParams, Dictionary<string, string> filterParms)
        {
            // Set up some parameter values
            var outputMode = GetRuntimeParam(runtimeParams, "OutputMode");
            var outputDirectoryPath = GetRuntimeParam(runtimeParams, "OutputDirectory") ?? string.Empty;
            var filterName = filterParms["SelectedFilterClassName"];
            filterParms.Remove("SelectedFilterClassName");

            if (string.IsNullOrEmpty(filterName))
                filterName = "All Pass";

            var reportFileName = string.Format("RunLog_{0}_{1:yyyy-MM-dd_hhmmss}.txt", filterName.Replace(" ", "_"), DateTime.Now);

            // Make file sub-pipeline processing broker module
            // to run a filter pipeline against files from list
            var broker = new FileSubPipelineBroker
            {
                OutputFileName = GetRuntimeParam(runtimeParams, "OutputFile"),
                SourceFileColumnName = "Name",
                SourceDirectoryColumnName = "Directory",
                OutputDirectoryPath = outputDirectoryPath
            };

            if (string.Equals(filterName, "All Pass", StringComparison.OrdinalIgnoreCase))
                broker.FileFilterModuleName = "NullFilter";
            else
                broker.FileFilterModuleName = filterName;

            broker.SetFileFilterParameters(filterParms);
            if (outputMode == "SQLite_Output")
            {
                broker.DatabaseName = GetRuntimeParam(runtimeParams, "DatabaseName");
                broker.TableName = GetRuntimeParam(runtimeParams, "TableName");
                outputDirectoryPath = Path.GetDirectoryName(GetRuntimeParam(runtimeParams, "DatabaseName"));
            }

            // Create a file writer module to output manifest file
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputDirectoryPath ?? string.Empty, reportFileName)
            };

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("PipelineToFilterSelectedFiles", sourceObject, broker, writer);
        }

        /// <summary>
        /// Build and return Mage pipeline queue to extract contents of results files
        /// for jobs given in jobList according to parameters defined in mExtractionParms
        /// and deliver output according to mDestination.  Also create metadata file for jobList.
        /// </summary>
        public static PipelineQueue MakePipelineQueueToPreProcessThenFilterFiles(
            BaseModule sourceObject,
            Dictionary<string, string> runtimeParams,
            Dictionary<string, string> filterParms)
        {
            // var outputMode = GetRuntimeParam(runtimeParams, "OutputMode");
            var outputDirectoryPath = GetRuntimeParam(runtimeParams, "OutputDirectory");

            var pipelineQueue = new PipelineQueue();

            // ProcessingPipeline exportMetadataPipeline = MakePipelineToExportJobMetadata(sourceObject, outputMode, outputDirectoryPath);
            // pipelineQueue.Add(exportMetadataPipeline);

            // Buffer module to accumulate aggregated file list
            var fileList = new SimpleSink();

            // Search job results directories for list of results files to process
            // and accumulate into buffer module
            var listOfFilesPipeline = MakePipelineToGetListOfFiles(sourceObject, fileList, runtimeParams);
            pipelineQueue.Add(listOfFilesPipeline);

            var sinkWrapper = new MyEMSLSinkWrapper(fileList)
            {
                TempFilesContainerPath = outputDirectoryPath,
                PredownloadMyEMSLFiles = true
            };

            var filterSelectedFilesPipeline = MakePipelineToFilterSelectedFiles(sinkWrapper, runtimeParams, filterParms);
            pipelineQueue.Add(filterSelectedFilesPipeline);

            return pipelineQueue;
        }

        /*
        /// <summary>
        /// Make a Mage pipeline to dump contents of job list
        /// as metadata file or db table
        /// </summary>
        /// <param name="jobList"></param>
        public static ProcessingPipeline MakePipelineToExportJobMetadata(BaseModule jobList, string outputMode, string outputDirectoryPath)
        {
            if (outputMode == )
            {
                broker.DatabaseName = GetRuntimeParam(runtimeParams, "DatabaseName");
                broker.TableName = GetRuntimeParam(runtimeParams, "TableName");
                outputDirectoryPath = Path.GetDirectoryName(GetRuntimeParam(runtimeParams, "DatabaseName"));
            }


            BaseModule writer = null;
            switch (outputMode)
            {
                case "SQLite_Output":

                    writer = new SQLiteWriter
                    {
                        DbPath = destination.ContainerPath,
                        TableName = destination.MetadataName
                    };
                    break;
                case DestinationType.Types.File_Output:
                    writer = new DelimitedFileWriter
                    {
                        FilePath = Path.Combine(destination.ContainerPath, destination.MetadataName)
                    };
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
        /// <param name="runtimeParams">Runtime parameters</param>
        public static ProcessingPipeline MakePipelineToGetListOfFiles(BaseModule jobListSource, BaseModule fileListSink, Dictionary<string, string> runtimeParams)
        {
            var outputDirectory = GetRuntimeParam(runtimeParams, "OutputDirectory");

            var modules = new Collection<object>
            {
                jobListSource
            };

            var fileFinder = new FileListInfoLookup
            {
                SourceFileColumnName = "Name",
                SourceDirectoryColumnName = "Directory",
                FileColumnName = "Name"
            };

            if (runtimeParams.TryGetValue("SourceFileColumnName", out var valueOverride))
            {
                fileFinder.SourceFileColumnName = valueOverride;
                fileFinder.FileColumnName = valueOverride;
            }

            if (runtimeParams.TryGetValue("SourceDirectoryColumnName", out valueOverride))
                fileFinder.SourceDirectoryColumnName = valueOverride;

            fileFinder.OutputColumnList = string.Format("Job, Item|{0}, Directory, Name|{1}, File_Size_KB|{2}, File_Date|{3}",
                FileListInfoBase.COLUMN_NAME_FILE_TYPE,
                fileFinder.FileColumnName,
                FileListInfoBase.COLUMN_NAME_FILE_SIZE,
                FileListInfoBase.COLUMN_NAME_FILE_DATE);

            modules.Add(fileFinder);
            modules.Add(fileListSink);

            // Create a delimited file writer module to build manifest and initialize it
            var writer = new DelimitedFileWriter
            {
                FilePath = Path.Combine(outputDirectory, GetRuntimeParam(runtimeParams, "ManifestFileName")),
                OutputColumnList = "*"
            };
            modules.Add(writer);

            var filePipeline = ProcessingPipeline.Assemble("Search For Files", modules);
            return filePipeline;
        }

        /// <summary>
        /// Return the value for the given runtime parameter
        /// </summary>
        /// <remarks>Raises an exception if the runtimeParams dictionary does not have the desired key</remarks>
        /// <param name="runtimeParams">Runtime parameters</param>
        /// <param name="keyName">Parameter to find</param>
        /// <returns>The value for the parameter</returns>
        private static string GetRuntimeParam(IReadOnlyDictionary<string, string> runtimeParams, string keyName)
        {
            if (!runtimeParams.ContainsKey(keyName))
                throw new Exception("runtimeParams does not contain key " + keyName);

            return runtimeParams[keyName];
        }
    }
}
