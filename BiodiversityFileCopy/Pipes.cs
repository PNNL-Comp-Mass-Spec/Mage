﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mage;

namespace BiodiversityFileCopy
{
    internal static class Pipes
    {
        // Ignore Spelling: dest, fasta, fht, mzid, mzml

        private static void ConnectPipelineToMessaging(ProcessingPipeline pl)
        {
            pl.OnWarningMessageUpdated += Logging.HandleLogMessage;
            pl.OnStatusMessageUpdated += Logging.HandleStatusMessage;
            pl.OnRunCompleted += Logging.HandleStatusMessage;
        }

        /// <summary>
        /// Copy files specified in the given file list.
        /// Source and destination file paths are contained in specified columns
        /// </summary>
        /// <param name="fileList">List containing source and destination file paths</param>
        /// <param name="sourceFilePathColName">Name of column that contains source path for file</param>
        /// <param name="destFilePathColName">Name of column that contains source path for file</param>
        /// <param name="doCopy">Actually copy the file</param>
        /// <param name="verbose">When true, log copy messages</param>
        /// <param name="checkSource"> </param>
        public static int CopyFiles(SimpleSink fileList, string sourceFilePathColName, string destFilePathColName, bool doCopy, bool verbose, bool checkSource)
        {
            var srcIdx = fileList.ColumnIndex[sourceFilePathColName];
            var destIdx = fileList.ColumnIndex[destFilePathColName];
            var numExist = 0;

            foreach (var row in fileList.Rows)
            {
                var destPath = row[destIdx];
                var sourcePath = row[srcIdx];
                try
                {
                    if (File.Exists(destPath))
                    {
                        FilteredMsg(string.Format("Exists: {0}", destPath), verbose);
                        numExist++;
                    }
                    else
                    {
                        if (checkSource)
                        {
                            CopyFile(doCopy, verbose, destPath, sourcePath);
                        }
                    }
                }
                catch (IOException e)
                {
                    Logging.LogError(e.Message);
                }
            }
            return numExist;
        }

        /// <summary>
        /// Copy a file
        /// </summary>
        /// <param name="doCopy">Copy if true, otherwise just log what would have happened</param>
        /// <param name="verbose">When true, log copy messages</param>
        /// <param name="destPath">Full path to copy file to</param>
        /// <param name="sourcePath">Full path to copy file from</param>
        private static void CopyFile(bool doCopy, bool verbose, string destPath, string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                Logging.LogError(string.Format("Source file missing {0}", sourcePath));
            }
            else
            {
                if (doCopy)
                {
                    // Make destination directory if necessary
                    EnsureDirectoryExists(destPath);
                    File.Copy(sourcePath, destPath, false);
                    FilteredMsg(string.Format("{2}: {0} -> {1}", sourcePath, destPath, "Copied:"), verbose);
                }
                else
                {
                    FilteredMsg(string.Format("{2}: {0} -> {1}", sourcePath, destPath, "Would copy:"), verbose);
                }
            }
        }

        private static void EnsureDirectoryExists(string destPath)
        {
            var destDir = Path.GetDirectoryName(destPath);
            if (destDir != null && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
        }

        private static void FilteredMsg(string msg, bool verbose)
        {
            if (verbose)
            {
                Logging.LogMsg(msg);
            }
        }

        /// <summary>
        /// Get list of datasets from given data package ID list
        /// </summary>
        public static SimpleSink GetDatasetsForDataPackages(string dataPackageId, Dictionary<string, string> orgLookup)
        {
            // var queryTemplate = @"SELECT Dataset_ID, Dataset, State, Folder As Directory, Data_Pkg_ID FROM V_Mage_Data_Package_Datasets WHERE Data_Pkg_ID IN (@)";
            const string queryTemplate = @"
SELECT DS.Dataset_ID ,
       DS.Dataset ,
       DS.State ,
       DS.Folder As Directory ,
       DS.Data_Pkg_ID AS Data_Package_ID,
       DFP.Dataset_Folder_Path ,
       DFP.Archive_Folder_Path ,
       DFP.MyEMSL_Path_Flag ,
       DFP.Instrument_Data_Purged
FROM V_Mage_Data_Package_Datasets AS DS
INNER JOIN V_Dataset_Folder_Paths AS DFP ON DS.Dataset_ID = DFP.Dataset_ID
WHERE DS.Data_Pkg_ID IN (@)
";
            return GetItemsForDataPackages(dataPackageId, orgLookup, queryTemplate);
        }

        public static SimpleSink GetJobsForDataPackages(string dataPackageId, Dictionary<string, string> orgLookup)
        {
            const string queryTemplate = @"
SELECT Job ,
       Results_Folder ,
       Folder as Directory ,
       Tool ,
       Organism_DB ,
       Dataset ,
       Dataset_ID ,
       Organism ,
       Data_Pkg_ID AS Data_Package_ID ,
       State
FROM V_Mage_Data_Package_Analysis_Jobs AS DPAJ
WHERE DPAJ.Data_Pkg_ID IN (@)
      AND State in ('Complete', 'No Export')";
            return GetItemsForDataPackages(dataPackageId, orgLookup, queryTemplate);
        }

        public static SimpleSink GetFastaFilesForDataPackages(string dataPackageId, Dictionary<string, string> orgLookup)
        {
            const string queryTemplate = @"
SELECT Organism,
       Organism_DB,
       dbo.get_fasta_file_path(Organism_DB, Organism) AS FASTA_Folder,
       Data_Pkg_ID
FROM (
    SELECT DISTINCT
           Organism,
           Organism_DB,
           Data_Pkg_ID
    FROM V_Mage_Data_Package_Analysis_Jobs
    WHERE State in ('Complete', 'No Export') AND
          Data_Pkg_ID IN (@)
) TX
                                        ";
            return GetItemsForDataPackages(dataPackageId, orgLookup, queryTemplate);
        }

        /// <summary>
        /// Retrieve list of items from given data packages according to given sql query
        /// </summary>
        /// <param name="dataPackageId"></param>
        /// <param name="orgLookup"></param>
        /// <param name="queryTemplate"></param>
        private static SimpleSink GetItemsForDataPackages(string dataPackageId, Dictionary<string, string> orgLookup, string queryTemplate)
        {
            var sql = queryTemplate.Replace("@", dataPackageId);

            // Typically prismdb2.emsl.pnl.gov and dms
            var dbr = new SQLReader
            {
                Server = Globals.DMSServer,
                Database = Globals.DMSDatabase,
                Username = Globals.DMSUser,
                Password = Globals.DMSUserPassword,
                SQLText = sql
            };

            var ogf = new AddOrganismNameFilter
            {
                OutputColumnList = "OG_Name|+|text, *",
                DataPackageIDColName = "Data_Pkg_ID",
                OrgNameColName = "OG_Name",
                OrganismLookup = orgLookup
            };

            var datasetList = new SimpleSink();

            var pl = ProcessingPipeline.Assemble("Get Package Datasets", dbr, ogf, datasetList);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return datasetList;
        }

        /// <summary>
        /// Return a dictionary for looking up assigned organism for given data package ID
        /// extracted from given sink object
        /// </summary>
        /// <param name="dataPackageList">Snk object containing information to be extracted</param>
        public static Dictionary<string, string> ExtractOrganismLookupFromSink(SimpleSink dataPackageList)
        {
            var orgLookup = new Dictionary<string, string>();
            var idIdx = dataPackageList.ColumnIndex["Package_ID"];
            var orgIdx = dataPackageList.ColumnIndex["OG_Name"];
            foreach (var row in dataPackageList.Rows.Where(row => !string.IsNullOrEmpty(row[idIdx])))
            {
                orgLookup[row[idIdx]] = row[orgIdx];
            }
            return orgLookup;
        }

        /// <summary>
        /// Extract list of data package IDs from given sink object
        /// </summary>
        /// <param name="dataPackageList">Sink object containing information to be extracted</param>
        public static List<string> ExtractIdListFromSink(SimpleSink dataPackageList)
        {
            var idIdx = dataPackageList.ColumnIndex["Package_ID"];
            var idList = (from row in dataPackageList.Rows where !string.IsNullOrEmpty(row[idIdx]) select row[idIdx]).ToList();
            idList.Sort();
            return idList;
        }

        /// <summary>
        /// Get list of data packages and the organism assigned for each
        /// </summary>
        /// <param name="dataPackageListFilePath"></param>
        public static SimpleSink GetDataPackageList(string dataPackageListFilePath)
        {
            var dataPackageList = new SimpleSink();

            var reader = new DelimitedFileReader { FilePath = dataPackageListFilePath };

            var filter = new NullFilter { OutputColumnList = "Package_ID|Datapk#, OG_Name" };

            var pl = ProcessingPipeline.Assemble("Get_Data_Package_List", reader, filter, dataPackageList);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return dataPackageList;
        }

        /// <summary>
        /// Add source and destination raw file paths to given dataset list
        /// </summary>
        /// <param name="datasetList"></param>
        /// <param name="outputRootDirectoryPath">Directory path at root of all output subdirectories</param>
        public static SimpleSink AddRawFilePaths(SimpleSink datasetList, string outputRootDirectoryPath)
        {
            // TBD: use file search as for mzid and mzml

            // Set up pipeline source
            var src = new SinkWrapper(datasetList);

            var rff = new RawFilePathsFilter();
            rff.SetDefaultProperties(outputRootDirectoryPath, "RAW");

            var dsl = new SimpleSink();

            var pl = ProcessingPipeline.Assemble("Add output directory path", src, rff, dsl);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return dsl;
        }

        /// <summary>
        /// Get list of MZML files for dataset directories imputed from job results directories
        /// Find cache file "*mzML.gz_CacheInfo.txt" in "Mz_Refinery_*" subdirectory
        /// Read path and add to dataset list .
        /// </summary>
        /// <param name="jobList"> </param>
        /// <param name="outputRootDirectoryPath"></param>
        public static SimpleSink AddMzmlFilePathsFromJobs(SimpleSink jobList, string outputRootDirectoryPath)
        {
            const string datasetDirectoryColName = "Dataset_Folder_Path";
            var jobs = AddParentDirectoryToJobList(jobList, datasetDirectoryColName);

            var src = new SinkWrapper(jobs);
            var lst = new SimpleSink();

            var flf = new FileListFilter();
            SetDefaultFileSearchFilterParameters(flf, "*CacheInfo.txt", "Yes", "M*");   // ("*mzML.gz_CacheInfo.txt", "Yes", "MZ_*") ("*mzML_CacheInfo.txt", "Yes", "MSXML_Gen_*");
            flf.SourceDirectoryColumnName = datasetDirectoryColName;

            var mzf = new MzmlFilePathsFilter();
            mzf.SetDefaultProperties(outputRootDirectoryPath, "MZML");
            mzf.SourceDirectoryPathColName = datasetDirectoryColName;   // Must match upstream setting

            var p1 = ProcessingPipeline.Assemble("Accumulate_File_Paths", src, flf, mzf);
            ConnectPipelineToMessaging(p1);
            p1.RunRoot(null);

            var p2 = ProcessingPipeline.Assemble("Select_Best_File_Paths", mzf, lst);
            ConnectPipelineToMessaging(p2);
            p2.RunRoot(null);
            return lst;
        }

        /// <summary>
        /// Get parent dataset directory path from job results directory path
        /// and return a new list with it added
        /// </summary>
        /// <param name="jobList">List of jobs to process</param>
        /// <param name="datasetDirectoryColName"></param>
        private static SimpleSink AddParentDirectoryToJobList(SimpleSink jobList, string datasetDirectoryColName)
        {
            // Set up pipeline source to only do rows with package IDs in whitelist
            var src = new SinkWrapper(jobList);

            // New filter to impute dataset directory from job results directory
            var dsf = new AddJobDatasetDirectoryFilter
            {
                OutputColumnList = datasetDirectoryColName + "|+|text, *",
                DatasetDirectoryColName = datasetDirectoryColName,
                JobResultsDirectoryColName = "Directory"
            };

            var lst = new SimpleSink();
            var addParentDirectoryPipeline = ProcessingPipeline.Assemble("Add_Parent_Directory", src, dsf, lst);
            ConnectPipelineToMessaging(addParentDirectoryPipeline);
            addParentDirectoryPipeline.RunRoot(null);
            return lst;
        }

        /// <summary>
        /// Get list of MSGF files for jobs in job list
        /// </summary>
        /// <param name="datasetList"></param>
        /// <param name="outputRootDirectoryPath"></param>
        public static SimpleSink AddMzidFilePaths(SimpleSink datasetList, string outputRootDirectoryPath)
        {
            var src = new SinkWrapper(datasetList);

            var flf = new FileListFilter();
            SetDefaultFileSearchFilterParameters(flf, "*mzid.gz", "No", "");

            var mzf = new SimpleFilePathsFilter();
            mzf.SetDefaultProperties(outputRootDirectoryPath, "MZID");

            var lst = new SimpleSink();

            var pl = ProcessingPipeline.Assemble("Add_Mzid_File_Paths", src, flf, mzf, lst);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return lst;
        }

        /// <summary>
        /// Get list of first hit files for jobs in job list
        /// </summary>
        /// <param name="datasetList"></param>
        /// <param name="outputRootDirectoryPath"></param>
        public static SimpleSink AddFhtFilePaths(SimpleSink datasetList, string outputRootDirectoryPath)
        {
            var src = new SinkWrapper(datasetList);

            var flf = new FileListFilter();
            SetDefaultFileSearchFilterParameters(flf, "*fht.txt", "No", "");

            var fhf = new SimpleFilePathsFilter();
            fhf.SetDefaultProperties(outputRootDirectoryPath, "MSGF_txt");

            var lst = new SimpleSink();

            var pl = ProcessingPipeline.Assemble("Add_Fht_File_Paths", src, flf, fhf, lst);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return lst;
        }

        /// <summary>
        /// Add source and destination fasta file paths to given fasta file list
        /// </summary>
        /// <param name="fastaFileList"></param>
        /// <param name="outputRootDirectoryPath">Directory path at root of all output subdirectories</param>
        public static SimpleSink AddFastaFilePaths(SimpleSink fastaFileList, string outputRootDirectoryPath)
        {
            var dsl = new SimpleSink();
            var src = new SinkWrapper(fastaFileList);

            var rff = new FastaFilePathsFilter();
            rff.SetDefaultProperties(outputRootDirectoryPath, "fasta");

            var pl = ProcessingPipeline.Assemble("Add output directory path", src, rff, dsl);
            ConnectPipelineToMessaging(pl);
            pl.RunRoot(null);
            return dsl;
        }

        /// <summary>
        /// Set up given FileListFilter module with default parameters
        /// that are common to all usages
        /// </summary>
        /// <param name="flf"></param>
        /// <param name="fileNameSelector"></param>
        /// <param name="recursiveSearch"></param>
        /// <param name="subdirectorySearchName"></param>
        private static void SetDefaultFileSearchFilterParameters(FileListFilter flf, string fileNameSelector, string recursiveSearch, string subdirectorySearchName)
        {
            flf.OutputColumnList = "Item|+|text, File|+|text, File_Size_KB|+|text, Directory, *";
            flf.SourceDirectoryColumnName = "Directory";            // The name of the input column that contains the directory path to search for files
            flf.FileSelectorMode = "FileSearch";                    // How to use the file matching patterns ("FileSearch" or "RegEx")
            flf.IncludeFilesOrDirectories = "File";                 // Include files an/or directories in results ("File", "Directory", "IncludeFilesOrDirectories")
            flf.RecursiveSearch = recursiveSearch;
            flf.FileNameSelector = fileNameSelector;                // Semicolon delimited list of file matching patterns
            flf.SubdirectorySearchName = subdirectorySearchName;    // Directory name pattern used to restrict recursive search
        }

        /// <summary>
        /// Extract Column From Sink
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="datasetList"></param>
        public static IEnumerable<string> ExtractColumnFromSink(string colName, SimpleSink datasetList)
        {
            return datasetList.Rows.Select(row => row[datasetList.ColumnIndex[colName]]);
        }
    }
}
