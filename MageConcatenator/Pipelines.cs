using System.Collections.Generic;
using Mage;

namespace MageConcatenator
{

    /// <summary>
    /// This class contains several functions that build Mage pipelines
    /// that supply data to the UI in response to user commands
    /// </summary>
    public class Pipelines
    {

        public const string PIPELINE_GET_LOCAL_FILES = "PipelineToGetLocalFileList";

        // Class is not instantiated
        private Pipelines()
        {
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

            // Build and wire pipeline
            return ProcessingPipeline.Assemble(PIPELINE_GET_LOCAL_FILES, reader, sinkObject);
        }

    }
}
