using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Mage;

namespace BiodiversityFileCopy
{
    class Processing
    {
        public Boolean DoRawCopy { get; set; }
        public Boolean DoMZMLCopy { get; set; }
        public Boolean DoFASTACopy { get; set; }
        public Boolean DoMZIDCopy { get; set; }
        public Boolean DoFHTCopy { get; set; }

        public string OutputRootFolderPath { get; set; }
        public string InputFileRootFolderPath { get; set; }
        public string DataPackageListFile { get; set; }
        public string DataPkgsToProcess { get; set; }
        public Boolean Verbose { get; set; }
        public Boolean DoCopyFiles { get; set; }
        public Boolean DoSourceCheck { get; set; }

        public void ProcessDataPackages()
        {
            // Read list of data packages   to process from external file
            var dataPkgList = Pipes.GetDataPackageList(Path.Combine(InputFileRootFolderPath, DataPackageListFile));
            if (dataPkgList.Rows.Count == 0)
            {
                Logging.LogError("The   assigned organism   file was missing or could   not be read");
                return;
            }

            // Extract lookup   table   of organisms defined for data   packages in list
            var orgLookup = Pipes.ExtractOrganismLookupFromSink(dataPkgList);

            // Set up   subset of   data packages   to process
            // (defaults to all if not specfied
            var subset = string.IsNullOrEmpty(DataPkgsToProcess) ? Pipes.ExtracttIdListFromSink(dataPkgList) : DataPkgsToProcess.Split(',').ToList();

            // Process each data package in subset
            // (we choose   to do   one at a time   at this point
            // even though pipelines can do multiples   themselves)
            foreach (var dPkgId in subset)
            {
                var org = orgLookup[dPkgId].Substring(0, 5);
                var suffix = (!DoCopyFiles) ? "_check" : "";
                Logging.LogFileLabel = string.Format("-{0}-{1}{2}", dPkgId, org, suffix);
                Logging.LogMsg("----");
                Logging.LogMsg(string.Format("Processing Data   Package {0} [{1}]", dPkgId, DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                Logging.LogMsg(string.Format("Program   version: {0}", GetProgramVersionNumber()));

                if (!DoCopyFiles)
                {
                    Logging.LogMsg("--File copy is disabled--");
                }

                ProcessSelectedDataPackage(dPkgId, orgLookup);
            }
        }

        public string GetProgramVersionNumber()
        {
            var versionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return versionNumber;
        }

        /// <summary>
        /// Copy files for data packages in given package list
        /// </summary>
        /// <param name="dPkgId">list of data package IDs to process</param>
        /// <param name="orgLookup"></param>
        private void ProcessSelectedDataPackage(string dPkgId, Dictionary<string, string> orgLookup)
        {
            // Get list of datasets for data packages
            var datasetList = Pipes.GetDatasetsForDataPackages(dPkgId, orgLookup);
            var packageDatasetIDs = new HashSet<string>(Pipes.ExtractColumnFromSink("Dataset_ID", datasetList));

            // Get list of jobs for data packages
            var jobList = Pipes.GetJobsForDataPackages(dPkgId, orgLookup);
            var packageDatasetIDForJobs = new HashSet<string>(Pipes.ExtractColumnFromSink("Dataset_ID", jobList));

            // TBD: check   job vs dataset coverage
            CheckJobsVsDatasets(packageDatasetIDs, packageDatasetIDForJobs);

            // Copy raw files   for datasets (if enabled)
            if (DoRawCopy)
            {
                Logging.LogMsg(string.Format("Processing RAW files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                var rawFileList = Pipes.AddRawFilePaths(datasetList, OutputRootFolderPath);

                // Check mzML   file coverage   against datasets
                CheckFileCoverage(packageDatasetIDs, rawFileList, "Dataset_ID", "raw");

                var ne = Pipes.CopyFiles(rawFileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0}   RAW files   exist   at destination out of   {1} in list for {2} datasets ", ne, rawFileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy mzML files for datasets (if enabled)
            if (DoMZMLCopy)
            {
                Logging.LogMsg(string.Format("Processing mzML   files   [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                var mzmlFileList = Pipes.AddMzmlFilePathsFromJobs(jobList, OutputRootFolderPath);
                // var fileList =   Pipes.AddMzmlFilePathsFromDatasets(datasetList, dPkgId, OutputRootFolderPath);

                // Check mzML   file coverage   against datasets
                CheckFileCoverage(packageDatasetIDs, mzmlFileList, "Dataset_ID", "mzML");

                var ne = Pipes.CopyFiles(mzmlFileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0}   mzML files exist at destination out of {1} in   list for {2} datasets   ", ne, mzmlFileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy *msgf.gz files for data package jobs
            if (DoMZIDCopy)
            {
                Logging.LogMsg(string.Format("Processing mzid.gz files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Add source   and destination file paths for files
                var fileList = Pipes.AddMzidfFilePaths(jobList, OutputRootFolderPath);

                // Check mzML   file coverage   against datasets
                CheckFileCoverage(packageDatasetIDs, fileList, "Dataset_ID", "msgf");

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0}   mzid.gz files   exist   at destination out of   {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }

            if (DoFHTCopy)
            {
                Logging.LogMsg(string.Format("Processing fht.txt files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Add source   and destination file paths for files
                var fileList = Pipes.AddFhtFilePaths(jobList, OutputRootFolderPath);

                // Check mzML   file coverage   against datasets
                CheckFileCoverage(packageDatasetIDs, fileList, "Dataset_ID", "fht");

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0}   fht.txt files   exist   at destination out of   {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy *.fasta files   for data package jobs
            if (DoFASTACopy)
            {
                Logging.LogMsg(string.Format("Processing fasta files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Get distinct list of fasta   files   from data   package jobs
                var fastaList = Pipes.GetFastaFilesForDataPackages(dPkgId, orgLookup);

                // Add source   and destination file paths for files
                var fileList = Pipes.AddFastaFilePaths(fastaList, OutputRootFolderPath);

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0}   fasta   files   exist   at destination out of   {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }
        }

        /// <summary>
        /// Check whether files match data package datasets.
        /// </summary>
        /// <param name="packageDatasetIDs">List of dataset IDs for data package</param>
        /// <param name="fileList">Sink object containing file list</param>
        /// <param name="datasetIDColName">Column in file list that contains dataset ID</param>
        /// <param name="fileLabel">File type label</param>
        private static void CheckFileCoverage(HashSet<string> packageDatasetIDs, SimpleSink fileList, string datasetIDColName, string fileLabel)
        {
            var fileDatasetIDs = new HashSet<string>(Pipes.ExtractColumnFromSink(datasetIDColName, fileList));
            var missingFileDatasetIDs = packageDatasetIDs.Except(fileDatasetIDs).ToList();
            var sr = string.Format("{0} {2} files   were found for {1} datasets ", fileDatasetIDs.Count(),
                                                         packageDatasetIDs.Count(), fileLabel);
            Logging.LogMsg(sr);
            if (missingFileDatasetIDs.Any())
            {
                var sm = string.Join(",", missingFileDatasetIDs);
                var sx = string.Format("The following   datasets did not have   {1} files: {0}", sm, fileLabel);
                Logging.LogWarning(sx);
            }
        }

        /// <summary>
        /// Check that jobs and datasets in lists match each other
        /// </summary>
        private void CheckJobsVsDatasets(HashSet<string> pkgDatasetIDs, HashSet<string> pkgJobDatasetIDs)
        {
            var missingJobs = pkgDatasetIDs.Except(pkgJobDatasetIDs).ToList();
            var missingDatasets = pkgJobDatasetIDs.Except(pkgDatasetIDs).ToList();

            if (missingJobs.Any())
            {
                var sm = string.Join(",", missingJobs);
                var ss = string.Format("There   were no completed   jobs in the data package for the following datasets {0}", sm);
                Logging.LogWarning(ss);
            }
            if (missingDatasets.Any())
            {
                var sm = string.Join(",", missingDatasets);
                var ss = string.Format("There   were completed jobs for the following   datasets that   are not part of the data package {0}", sm);
                Logging.LogWarning(ss);
            }
        }
    }
}
