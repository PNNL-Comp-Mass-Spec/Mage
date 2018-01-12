using System.Collections.Generic;
using Mage;

namespace MageFilePackager
{
    internal static class Pipelines
    {
        // Class is not instantiated

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
                SourceFolderColumnName = runtimeParms["SourceFolderColumnName"],
                FileColumnName = runtimeParms["FileColumnName"],
                OutputColumnList = runtimeParms["OutputColumnList"],
                FileNameSelector = runtimeParms["FileSelectors"],
                FileSelectorMode = runtimeParms["FileSelectionMode"],
                IncludeFilesOrFolders = runtimeParms["IncludeFilesOrFolders"],
                RecursiveSearch = runtimeParms["SearchInSubfolders"],
                SubfolderSearchName = runtimeParms["SubfolderSearchName"]
            };

            // Filter to remove "not found" rows
            var cleanupFilter = new AfterSearchFilter();

            // Build and wire pipeline
            return ProcessingPipeline.Assemble("FileListPipeline", sourceObject, fileFilter, cleanupFilter, sinkObject);
        }

    }
}
