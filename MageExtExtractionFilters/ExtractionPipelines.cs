using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Mage;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Builds Mage pipelines and pipeline queues for extracting DMS analysis job
    /// results file content
    /// </summary>
    public static class ExtractionPipelines
    {
        public static PipelineQueue MakePipelineQueueToExtractFromFileList(SimpleSink fileList, ExtractionType extractionParms, DestinationType destination)
        {
            var pipelineQueue = new PipelineQueue();

            var fileListExporterPipeline = MakePipelineToExportFileList(new MyEMSLSinkWrapper(fileList), destination);
            pipelineQueue.Add(fileListExporterPipeline);

            // Extract contents of list of files
            var fileContentExtractorPipeline = MakePipelineToExtractFileContents(new SinkWrapper(fileList), extractionParms, destination);
            pipelineQueue.Add(fileContentExtractorPipeline);

            return pipelineQueue;
        }

        /// <summary>
        /// Build and return Mage pipeline queue to extract contents of results files
        /// for jobs given in jobList according to parameters defined in mExtractionParms
        /// and deliver output according to mDestination.  Also create metadata file for jobList.
        /// </summary>
        public static PipelineQueue MakePipelineQueueToExtractFromJobList(BaseModule jobList, ExtractionType extractionParms, DestinationType destination)
        {
            var pipelineQueue = new PipelineQueue();

            var jobMetadataExportPipeline = MakePipelineToExportJobMetadata(jobList, destination);
            pipelineQueue.Add(jobMetadataExportPipeline);

            // Buffer module to accumulate aggregated file list
            var fileList = new SimpleSink();

            // Search job results directories for list of results files to process
            // and accumulate into buffer module
            var fileListPipeline = MakePipelineToGetListOfFiles(jobList, fileList, extractionParms);
            pipelineQueue.Add(fileListPipeline);

            var fileExportPipeline = MakePipelineToExportFileList(new MyEMSLSinkWrapper(fileList), destination);
            pipelineQueue.Add(fileExportPipeline);

            // Extract contents of list of files
            var fileContentExtractorPipeline = MakePipelineToExtractFileContents(new SinkWrapper(fileList), extractionParms, destination);
            pipelineQueue.Add(fileContentExtractorPipeline);

            return pipelineQueue;
        }

        /// <summary>
        /// Make a Mage pipeline to get list of files for list of jobs
        /// contained in given source module and accumulate
        /// into given sink module
        /// </summary>
        /// <param name="jobListSource">Mage module that contains list of jobs to search</param>
        /// <param name="fileListSink">Mage module to accumulate list of results (and MSGF) files into</param>
        /// <param name="extractionParms">Extraction parameters</param>
        /// <returns></returns>
        public static ProcessingPipeline MakePipelineToGetListOfFiles(BaseModule jobListSource, BaseModule fileListSink, ExtractionType extractionParms)
        {
            var modules = new Collection<object>
            {
                jobListSource
            };

            var fileFinder = new FileListFilter
            {
                IncludeFilesOrDirectories = "File",
                FileSelectorMode = FileListFilter.FILE_SELECTOR_REGEX,
                FileColumnName = "Name",
                FileNameSelector = extractionParms.RType.ResultsFileNamePattern
            };

            fileFinder.OutputColumnList = string.Format(
                "Job, {0}|+|text, Directory, {1}|+|text, {2}|+|text, {3}|+|text",
                FileListInfoBase.COLUMN_NAME_FILE_TYPE,
                fileFinder.FileColumnName,
                FileListInfoBase.COLUMN_NAME_FILE_SIZE,
                FileListInfoBase.COLUMN_NAME_FILE_DATE);

            modules.Add(fileFinder);

            foreach (var mf in extractionParms.RType.MergeFileTypes)
            {
                var assocFinder = new AddAssociatedFile
                {
                    // Note that .ResultsFileNamePattern may have a series of filename patterns separated by semicolons
                    AssocFileNameReplacementPattern = string.Format("{0}|{1}", extractionParms.RType.ResultsFileNamePattern, mf.FileNameTag),
                    ColumnToReceiveAssocFileName = mf.NameColumn
                };
                fileFinder.OutputColumnList += string.Format(", {0}|+|text", mf.NameColumn);
                modules.Add(assocFinder);
            }

            modules.Add(fileListSink);

            var filePipeline = ProcessingPipeline.Assemble("Search For Files", modules);
            return filePipeline;
        }

        /// <summary>
        /// Make pipeline to extract contents of files given in list
        /// and add it to the queue
        /// </summary>
        public static ProcessingPipeline MakePipelineToExtractFileContents(BaseModule fileListSource, ExtractionType extractionParms, DestinationType destination)
        {
            var extractor = new FileContentExtractor
            {
                ExtractionParms = extractionParms,
                Destination = destination,
                SourceDirectoryColumnName = "Directory",
                SourceFileColumnName = "Name"
            };

            var filePipeline = ProcessingPipeline.Assemble("Process Files", fileListSource, extractor);
            return filePipeline;
        }

        /// <summary>
        ///  Check that the tool type for jobs selected for extraction
        ///  are correct and consistent with extraction type
        /// </summary>
        /// <param name="jobList"></param>
        /// <param name="toolCol"></param>
        /// <param name="extractionParams">Extraction parameters</param>
        /// <returns></returns>
        public static string CheckJobResultType(BaseModule jobList, string toolCol, ExtractionType extractionParams)
        {
            var msg = string.Empty;
            var toolList = new SimpleSink();
            ProcessingPipeline.Assemble("GetToolList", jobList, toolList).RunRoot(null);
            var tools = new Dictionary<string, int>();

            foreach (var row in toolList.Rows)
            {
                var idx = toolList.ColumnIndex[toolCol];
                var tool = GetBaseTool(row[idx]);
                if (tools.ContainsKey(tool))
                {
                    tools[tool] = tools[tool] + 1;
                }
                else
                {
                    tools[tool] = 1;
                }
            }
            if (tools.Count > 1)
            {
                var toolRollup = "(" + string.Join(", ", tools.Keys) + ")";
                msg = "Cannot work on a mix of job types " + toolRollup + Environment.NewLine + "Select only Sequest, X!Tandem, Inspect, or MSGFPlus jobs";
            }
            else
            {
                var filterName = extractionParams.RType.Filter;
                var toolName = tools.Keys.ElementAt(0);
                if (string.Equals(filterName, toolName, StringComparison.OrdinalIgnoreCase))
                {
                    return msg;
                }

                if (string.Equals(toolName, "msgfplus", StringComparison.OrdinalIgnoreCase) && filterName.StartsWith("msgfplus", StringComparison.OrdinalIgnoreCase))
                {
                    // MSGF+ results can be processed by two different extractors: msgfdb and msgfdbFHT
                    return msg;
                }

                msg = string.Format("Result type chosen for extraction ({0}) is not correct for selected jobs ({1})", filterName, toolName);
            }
            return msg;
        }

        /// <summary>
        /// Get base tool name from raw tool name.
        /// Uses hueristic that all common tools begin
        /// with the same prefix set off by underscore character.
        /// </summary>
        /// <param name="rawTool"></param>
        /// <returns></returns>
        public static string GetBaseTool(string rawTool)
        {
            var tool = rawTool.ToLower();
            var idx = rawTool.IndexOf('_');
            if (idx >= 0)
            {
                tool = tool.Substring(0, idx);
            }
            return tool;
        }

        private static string GetContainerDirectory(DestinationType destination)
        {
            var containerPath = destination.ContainerPath;
            if (containerPath.EndsWith(".db3", StringComparison.OrdinalIgnoreCase))
            {
                var containerFile = new FileInfo(containerPath);
                if (containerFile.Directory != null)
                {
                    containerPath = containerFile.Directory.FullName;
                }
            }
            return containerPath;
        }

        /// <summary>
        /// Make a Mage pipeline to dump contents of job list
        /// as metadata file or db table
        /// </summary>
        /// <param name="jobList"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static ProcessingPipeline MakePipelineToExportJobMetadata(BaseModule jobList, DestinationType destination)
        {
            BaseModule writer = null;
            switch (destination.Type)
            {
                case DestinationType.Types.SQLite_Output:
                    var sw = new SQLiteWriter
                    {
                        DbPath = destination.ContainerPath,
                        TableName = destination.MetadataName
                    };
                    writer = sw;
                    break;
                case DestinationType.Types.File_Output:
                    var dw = new DelimitedFileWriter
                    {
                        FilePath = Path.Combine(destination.ContainerPath, destination.MetadataName)
                    };
                    writer = dw;
                    break;
            }
            var filePipeline = ProcessingPipeline.Assemble("Job Metadata", jobList, writer);
            return filePipeline;
        }

        /// <summary>
        /// Make a Mage pipeline to dump contents of file list
        /// as file or db table
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static ProcessingPipeline MakePipelineToExportFileList(MyEMSLSinkWrapper fileList, DestinationType destination)
        {
            BaseModule writer = null;
            switch (destination.Type)
            {
                case DestinationType.Types.SQLite_Output:
                    var sw = new SQLiteWriter
                    {
                        DbPath = destination.ContainerPath,
                        TableName = destination.FileListName
                    };
                    writer = sw;
                    break;
                case DestinationType.Types.File_Output:
                    var dw = new DelimitedFileWriter
                    {
                        FilePath = Path.Combine(destination.ContainerPath, destination.FileListName)
                    };
                    writer = dw;
                    break;
            }

            fileList.TempFilesContainerPath = GetContainerDirectory(destination);
            fileList.PredownloadMyEMSLFiles = true;

            var filePipeline = ProcessingPipeline.Assemble("Job Metadata", fileList, writer);
            return filePipeline;
        }
    }
}
