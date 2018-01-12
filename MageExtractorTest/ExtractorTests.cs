using System;
using System.Text;
using Mage;
using MageExtExtractionFilters;
using System.IO;

namespace MageExtractorTest
{

    class ExtractorTests
    {

        #region Member Variables

        public event EventHandler<MageStatusEventArgs> OnMessageUpdated;

        private string mTestRootFolderPath = @"C:\Data\ExtractorTests";

        private string mTestCasesFolder = "TestCases";

        private string mJobListTestFileFolder = "JobListTestFiles";
        private string mJobListTestKnownGoodFolder = "JobListTestKnownGood";

        private string mJobInputResultsFolder = "JobResultsFolders";
        private string mExtractionResultsFromTestFolder = "ExtractionResultsFromTest";

        // Test case columns
        int JobListFileIdx = 0;
        int ExtractionTypeIdx = 1;
        int FilterSetIDIdx = 2;
        int MSGFCutoffIdx = 3;
        int KeepAllIdx = 4;
        int DestinationTypeIdx = 5;
        int ContainerIdx = 6;
        int NameIdx = 7;

        #endregion

        #region Properties

        public string TestRootFolderPath
        {
            get => mTestRootFolderPath;
            set => mTestRootFolderPath = value;
        }

        #endregion

        #region Initialization


        #endregion

        public void RunTests()
        {
            // RunAllFindFilesTests();
            RunAllExtractionTests();
        }

        #region File Extraction Tests

        /// <summary>
        /// Execute several test cases that run extraction pipeline queue from job list to results.
        /// </summary>
        protected void RunAllExtractionTests()
        {
            var testRootFolder = new DirectoryInfo(mTestRootFolderPath);
            if (!testRootFolder.Exists)
            {
                UpdateMessage("Root test folder not found; cannot run tests: " + testRootFolder.FullName);
                return;
            }

            // "Sequest Synopsis" "Sequest First Hits"  "X!Tandem First Protein"  "X!Tandem All Proteins"  "Inspect Synopsis"

            Console.WriteLine("Extraction Tests Begin");

            // Get test cases to run
            var testCaseFilePath = Path.Combine(testRootFolder.FullName, Path.Combine(mTestCasesFolder, "LocalJobExtractionTestCases.txt")); // JobExtractionTestCases.txt
            var cases = GetFileContentsToSink(testCaseFilePath);

            // Run each case
            RunExtractionTestCases(cases);
            Console.WriteLine("Extraction Tests Complete");
        }

        public void RunExtractionTestCases(SimpleSink cases)
        {
            var testRootFolder = new DirectoryInfo(mTestRootFolderPath);
            if (!testRootFolder.Exists)
            {
                UpdateMessage("Root test folder not found; cannot run tests: " + testRootFolder.FullName);
                return;
            }

            UpdateMessage("Extraction Tests Begin");
            var testResultsPath = Path.Combine(testRootFolder.FullName, mExtractionResultsFromTestFolder);

            foreach (var file in Directory.GetFiles(testResultsPath))
            {
                File.Delete(file);
            }

            var extractionParms = new ExtractionType();
            foreach (var testCase in cases.Rows)
            {

                // Get job list to extract from
                var jobListFileName = testCase[JobListFileIdx];
                var jobListFilePath = Path.Combine(testRootFolder.FullName, Path.Combine(mJobListTestFileFolder, jobListFileName));
                var jobList = GetFileContentsToSink(jobListFilePath);

                // Adjust job list for local folders
                AdjustFolderPathForLocalFolders(jobList);

                // Set up extraction parameters
                extractionParms.RType = ResultType.TypeList[testCase[ExtractionTypeIdx]];
                extractionParms.ResultFilterSetID = testCase[FilterSetIDIdx];
                extractionParms.MSGFCutoff = testCase[MSGFCutoffIdx];
                extractionParms.KeepAllResults = testCase[KeepAllIdx];

                // Set up destination
                var type = testCase[DestinationTypeIdx];
                var outputPath = Path.Combine(testResultsPath, testCase[ContainerIdx]);
                var item = testCase[NameIdx];
                var destination = new DestinationType(type, outputPath, item);

                UpdateMessage(string.Format("Test Case:{0} ============", jobListFileName));
                TestExtractFromJobList(jobList, extractionParms, destination);
            }
            UpdateMessage("Extraction Tests Complete");
        }


        protected void TestExtractFromJobList(BaseModule jobList, ExtractionType extractionParms, DestinationType destination)
        {
            var pq = ExtractionPipelines.MakePipelineQueueToExtractFromJobList(jobList, extractionParms, destination);
            foreach (var p in pq.Pipelines.ToArray())
            {
                ConnectPipelineToMessageHandler(p);
            }
            ConnectPipelineQueueToMessageHandler(pq);
            pq.RunRoot(null);
        }

        /// <summary>
        /// Substitute local folder tree for "[local]" token in folder column
        /// </summary>
        /// <param name="jobList"></param>
        private void AdjustFolderPathForLocalFolders(SimpleSink jobList)
        {
            // Adjust job list for local folders
            var folderColIdx = jobList.ColumnIndex["Folder"];
            var localPath = Path.Combine(mTestRootFolderPath, mJobInputResultsFolder);
            for (var i = 0; i < jobList.Rows.Count; i++)
            {
                var folder = jobList.Rows[i][folderColIdx];
                jobList.Rows[i][folderColIdx] = folder.Replace("[local]", localPath);
            }
        }

        #endregion

        #region File Searching Tests

        protected void RunAllFindFilesTests()
        {
            var testRootFolder = new DirectoryInfo(mTestRootFolderPath);
            if (!testRootFolder.Exists)
            {
                UpdateMessage("Root test folder not found; cannot run tests: " + testRootFolder.FullName);
                return;
            }

            // "Sequest Synopsis" "Sequest First Hits"  "X!Tandem First Protein"  "X!Tandem All Proteins"  "Inspect Synopsis"

            var testCaseFilePath = Path.Combine(testRootFolder.FullName, Path.Combine(mTestCasesFolder, "FileListTestCases.txt"));
            var cases = GetFileContentsToSink(testCaseFilePath);

            var extractionParms = new ExtractionType();
            foreach (var testCase in cases.Rows)
            {
                var jobListFile = testCase[0];
                extractionParms.RType = ResultType.TypeList[testCase[1]];
                TestFindFilesForJobList(jobListFile, extractionParms);
            }
        }

        /// <summary>
        /// Given the name of a file containing a list of jobs, and an extraction type,
        /// Get list of files that would be part of extraction.
        /// </summary>
        protected void TestFindFilesForJobList(string jobListFileName, ExtractionType extractionParms)
        {
            var testRootFolder = new DirectoryInfo(mTestRootFolderPath);
            if (!testRootFolder.Exists)
            {
                UpdateMessage("Root test folder not found; cannot run tests: " + testRootFolder.FullName);
                return;
            }

            var jobList = GetFileContentsToSink(Path.Combine(testRootFolder.FullName, Path.Combine(mJobListTestFileFolder, jobListFileName)));

            var fileListSink = new SimpleSink();

            var p = ExtractionPipelines.MakePipelineToGetListOfFiles(new SinkWrapper(jobList), fileListSink, extractionParms);
            ConnectPipelineToMessageHandler(p);
            p.RunRoot(null);

            var goodFileName = Path.GetFileNameWithoutExtension(jobListFileName) + "_file_list.txt";
            var goodValues = GetFileContentsToSink(Path.Combine(testRootFolder.FullName, Path.Combine(mJobListTestKnownGoodFolder, goodFileName)));
            var errorMsg = CompareSinks(fileListSink, goodValues);
            if (string.IsNullOrEmpty(errorMsg))
            {
                Console.WriteLine("Passed:{0}", jobListFileName);
            }
            else
            {
                Console.WriteLine("ERROR:{0}", jobListFileName);
                Console.WriteLine(errorMsg);
            }
        }

        #endregion

        #region Utility Functions

        private SimpleSink GetFileContentsToSink(string path)
        {
            var reader = new DelimitedFileReader();
            reader.FilePath = path;
            var sink = new SimpleSink();
            ProcessingPipeline.Assemble("FileContents", reader, sink).RunRoot(null);
            return sink;
        }

        private void WriteSinkContentsToFile(SimpleSink sink, string path)
        {
            var writer = new DelimitedFileWriter();
            writer.FilePath = path;
            ProcessingPipeline.Assemble("SinkDump", sink, writer).RunRoot(null);
        }

        private static string CompareSinks(SimpleSink source, SimpleSink result)
        {
            var mb = new StringBuilder();
            var sourceCols = source.Columns;
            var resultCols = result.Columns;
            var sourceRows = source.Rows;
            var resultRows = result.Rows;

            if (sourceCols.Count != resultCols.Count)
            {
                mb.AppendLine("column count does not match");
            }
            if (sourceRows.Count != resultRows.Count)
            {
                mb.AppendLine("Row count does not match");
            }

            for (var i = 0; i < sourceCols.Count; i++)
            {
                if (sourceCols[i].Name != resultCols[i].Name)
                {
                    mb.AppendLine("Column names do not match");
                }
            }

            for (var i = 0; i < sourceRows.Count; i++)
            {
                for (var j = 0; j < sourceCols.Count; j++)
                {
                    if (sourceRows[i][j] != resultRows[i][j])
                    {
                        mb.AppendLine(string.Format("Row {0} cell {1} content does not match", i, j));
                    }
                }
            }
            return mb.ToString();
        }


        #endregion

        #region Message Handling Functions

        private void ConnectPipelineToMessageHandler(ProcessingPipeline pipeline)
        {
            pipeline.OnStatusMessageUpdated += HandlePipelineMessage;
        }

        private void ConnectPipelineQueueToMessageHandler(PipelineQueue pipelineQueue)
        {
            pipelineQueue.OnPipelineStarted += HandlePipelineMessage;
        }

        private void HandlePipelineMessage(object sender, MageStatusEventArgs args)
        {
            UpdateMessage(args.Message);
        }

        private void UpdateMessage(string message)
        {
            if (OnMessageUpdated != null)
            {
                OnMessageUpdated(this, new MageStatusEventArgs(message));
            }
        }

        #endregion

    }
}
