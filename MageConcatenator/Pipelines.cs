using System;
using System.Collections.Generic;
using Mage;

namespace MageConcatenator
{
    /// <summary>
    /// This class contains several functions that build Mage pipelines
    /// that supply data to the UI in response to user commands
    /// </summary>
    public static class Pipelines
    {
        // Ignore Spelling: Mage

        public const string PIPELINE_GET_LOCAL_FILES = "PipelineToGetLocalFileList";

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
                reader.AddDirectoryPath(runtimeParams["Directory"]);
            else if (runtimeParams.ContainsKey("Folder"))
                reader.AddDirectoryPath(runtimeParams["Folder"]);
            else
                throw new Exception("Runtime params must have Directory or Folder");

            reader.FileNameSelector = runtimeParams["FileNameFilter"];

            if (runtimeParams.ContainsKey("FileSelectionMode"))
                reader.FileSelectorMode = runtimeParams["FileSelectionMode"];

            if (runtimeParams.ContainsKey("SearchInSubdirectories"))
                reader.RecursiveSearch = runtimeParams["SearchInSubdirectories"];

            if (runtimeParams.ContainsKey("SubdirectorySearchName"))
                reader.SubdirectorySearchName = runtimeParams["SubdirectorySearchName"];

            reader.FileTypeColumnName = FileListInfoBase.COLUMN_NAME_FILE_TYPE;			        // Item
            reader.FileColumnName = FileListInfoBase.COLUMN_NAME_FILE_NAME;				        // File
            reader.SourceDirectoryColumnName = FileListInfoBase.COLUMN_NAME_SOURCE_DIRECTORY;	// Directory
            reader.FileSizeColumnName = FileListInfoBase.COLUMN_NAME_FILE_SIZE;			        // File_Size_KB
            reader.FileDateColumnName = FileListInfoBase.COLUMN_NAME_FILE_DATE;			        // File_Date
            reader.OutputColumnList = string.Format("{0}|+|text, {1}|+|text, {2}|+|text, {3}|+|text, {4}|+|text", reader.FileTypeColumnName, reader.FileColumnName, reader.FileSizeColumnName, reader.FileDateColumnName, reader.SourceDirectoryColumnName);
            reader.IncludeFilesOrDirectories = "File";

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_LOCAL_FILES, reader, sinkObject);
        }
    }
}
