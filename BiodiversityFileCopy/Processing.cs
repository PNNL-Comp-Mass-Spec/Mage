using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Mage;

namespace BiodiversityFileCopy
{
    internal class Processing
    {
        // Ignore Spelling: fht, msgf

        public bool DoRawCopy { get; set; }
        public bool DoMZMLCopy { get; set; }
        public bool DoFASTACopy { get; set; }
        public bool DoMZIDCopy { get; set; }
        public bool DoFHTCopy { get; set; }

        public string OutputRootDirectoryPath { get; set; }
        public string InputFileRootDirectoryPath { get; set; }
        public string DataPackageListFile { get; set; }
        public string DataPackagesToProcess { get; set; }
        public bool Verbose { get; set; }
        public bool DoCopyFiles { get; set; }
        public bool DoSourceCheck { get; set; }

        public void ProcessDataPackages()
        {
            // Read list of data packages to process from external file
            var dataPkgList = Pipes.GetDataPackageList(Path.Combine(InputFileRootDirectoryPath, DataPackageListFile));
            if (dataPkgList.Rows.Count == 0)
            {
                Logging.LogError("The assigned organism file was missing or could not be read");
                return;
            }

            // Extract lookup table of organisms defined for data packages in list
            var orgLookup = Pipes.ExtractOrganismLookupFromSink(dataPkgList);

            // Set up subset of data packages to process
            // (defaults to all if not specified
            var subset = string.IsNullOrEmpty(DataPackagesToProcess) ? Pipes.ExtractIdListFromSink(dataPkgList) : DataPackagesToProcess.Split(',').ToList();

            // Process each data package in subset
            // (we choose to do one at a time at this point
            // even though pipelines can do multiples themselves)
            foreach (var dPkgId in subset)
            {
                var org = orgLookup[dPkgId].Substring(0, 5);
                var suffix = (!DoCopyFiles) ? "_check" : "";
                Logging.LogFileLabel = string.Format("-{0}-{1}{2}", dPkgId, org, suffix);
                Logging.LogMsg("----");
                Logging.LogMsg(string.Format("Processing Data Package {0} [{1}]", dPkgId, DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                Logging.LogMsg(string.Format("Program version: {0}", GetProgramVersionNumber()));

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

            // TBD: check job vs dataset coverage
            CheckJobsVsDatasets(packageDatasetIDs, packageDatasetIDForJobs);

            // Copy raw files for datasets (if enabled)
            if (DoRawCopy)
            {
                Logging.LogMsg(string.Format("Processing RAW files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                var rawFileList = Pipes.AddRawFilePaths(datasetList, OutputRootDirectoryPath);

                // Check mzML file coverage against datasets
                CheckFileCoverage(packageDatasetIDs, rawFileList, "Dataset_ID", "raw");

                var ne = Pipes.CopyFiles(rawFileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0} RAW files exist at destination out of {1} in list for {2} datasets ", ne, rawFileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy mzML files for datasets (if enabled)
            if (DoMZMLCopy)
            {
                Logging.LogMsg(string.Format("Processing mzML files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                var mzmlFileList = Pipes.AddMzmlFilePathsFromJobs(jobList, OutputRootDirectoryPath);
                // var fileList = Pipes.AddMzmlFilePathsFromDatasets(datasetList, dPkgId, OutputRootDirectoryPath);

                // Check mzML file coverage against datasets
                CheckFileCoverage(packageDatasetIDs, mzmlFileList, "Dataset_ID", "mzML");

                var ne = Pipes.CopyFiles(mzmlFileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0} mzML files exist at destination out of {1} in list for {2} datasets ", ne, mzmlFileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy *msgf.gz files for data package jobs
            if (DoMZIDCopy)
            {
                Logging.LogMsg(string.Format("Processing mzid.gz files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Add source and destination file paths for files
                var fileList = Pipes.AddMzidFilePaths(jobList, OutputRootDirectoryPath);

                // Check mzML file coverage against datasets
                CheckFileCoverage(packageDatasetIDs, fileList, "Dataset_ID", "msgf");

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0} mzid.gz files exist at destination out of {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }

            if (DoFHTCopy)
            {
                Logging.LogMsg(string.Format("Processing fht.txt files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Add source and destination file paths for files
                var fileList = Pipes.AddFhtFilePaths(jobList, OutputRootDirectoryPath);

                // Check mzML file coverage against datasets
                CheckFileCoverage(packageDatasetIDs, fileList, "Dataset_ID", "fht");

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0} fht.txt files exist at destination out of {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }

            // Copy *.fasta files for data package jobs
            if (DoFASTACopy)
            {
                Logging.LogMsg(string.Format("Processing fasta files [{0}]", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                // Get distinct list of fasta files from data package jobs
                var fastaList = Pipes.GetFastaFilesForDataPackages(dPkgId, orgLookup);

                // Add source and destination file paths for files
                var fileList = Pipes.AddFastaFilePaths(fastaList, OutputRootDirectoryPath);

                var ne = Pipes.CopyFiles(fileList, "SourceFilePath", "DestinationFilePath", DoCopyFiles, Verbose, DoSourceCheck);
                Logging.LogMsg(string.Format("{0} fasta files exist at destination out of {1} in list for {2} datasets ", ne, fileList.Rows.Count, packageDatasetIDs.Count));
            }
        }

        /// <summary>
        /// Check whether files match data package datasets.
        /// </summary>
        /// <param name="packageDatasetIDs">List of dataset IDs for data package</param>
        /// <param name="fileList">Sink object containing file list</param>
        /// <param name="datasetIDColName">Column in file list that contains dataset ID</param>
        /// <param name="fileLabel">File type label</param>
        private static void CheckFileCoverage(IReadOnlyCollection<string> packageDatasetIDs, SimpleSink fileList, string datasetIDColName, string fileLabel)
        {
            var fileDatasetIDs = new HashSet<string>(Pipes.ExtractColumnFromSink(datasetIDColName, fileList));
            var missingFileDatasetIDs = packageDatasetIDs.Except(fileDatasetIDs).ToList();
            var message = string.Format("{0} {1} files were found for {2} datasets ", fileDatasetIDs.Count, fileLabel, packageDatasetIDs.Count);

            Logging.LogMsg(message);

            if (missingFileDatasetIDs.Count > 0)
            {
                var datasetIDs = string.Join(",", missingFileDatasetIDs);
                var warningMessage = string.Format("The following datasets did not have {0} files: {1}", fileLabel, datasetIDs);
                Logging.LogWarning(warningMessage);
            }
        }

        /// <summary>
        /// Check that jobs and datasets in lists match each other
        /// </summary>
        private void CheckJobsVsDatasets(HashSet<string> pkgDatasetIDs, HashSet<string> pkgJobDatasetIDs)
        {
            var datasetsMissingJobs = pkgDatasetIDs.Except(pkgJobDatasetIDs).ToList();
            var jobsMissingDatasets = pkgJobDatasetIDs.Except(pkgDatasetIDs).ToList();

            if (datasetsMissingJobs.Count > 0)
            {
                var datasetIDs = string.Join(",", datasetsMissingJobs);
                var message = string.Format("There were no completed jobs in the data package for the following datasets {0}", datasetIDs);
                Logging.LogWarning(message);
            }

            if (jobsMissingDatasets.Count > 0)
            {
                var jobIDs = string.Join(",", jobsMissingDatasets);
                var message = string.Format("There were completed jobs for the following datasets that are not part of the data package {0}", jobIDs);
                Logging.LogWarning(message);
            }
        }
    }
}