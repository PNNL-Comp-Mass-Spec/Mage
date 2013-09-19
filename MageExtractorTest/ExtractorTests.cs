using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MageExtExtractionFilters;
using System.IO;
using System.Collections.ObjectModel;

namespace MageExtractorTest {

    class ExtractorTests {

        #region Member Variables 

        public event EventHandler<MageStatusEventArgs> OnMessageUpdated;

        private string mTestRootFolderPath = @"C:\Data\ExtractorTests";

         private string mTestCasesFolder = "TestCases";

        private string mJobListTestFileFolder = "JobListTestFiles";
        private string mJobListTestKnownGoodFolder = "JobListTestKnownGood";

        private string mJobInputResultsFolder = "JobResultsFolders";
        private string mExtractionResultsFromTestFolder = "ExtractionResultsFromTest";

        // test case columns
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

        public string TestRootFolderPath {
            get { return mTestRootFolderPath; }
            set { mTestRootFolderPath = value; }
        }

        #endregion

        #region Initialization


        #endregion
 
        public void RunTests() {
            //RunAllFindFilesTests();
            RunAllExtractionTests();
        }

        #region File Extraction Tests

        /// <summary>
        /// Execute several test cases that run extraction pipeline queue from job list to results.
        /// </summary>
        protected void RunAllExtractionTests() {
            // "Sequest Synopsis" "Sequest First Hits"  "X!Tandem First Protein"  "X!Tandem All Proteins"  "Inspect Synopsis"

            Console.WriteLine(string.Format("Extraction Tests Begin"));

            // get test cases to run
            string testCaseFilePath = Path.Combine(mTestRootFolderPath, Path.Combine(mTestCasesFolder, "LocalJobExtractionTestCases.txt")); // JobExtractionTestCases.txt
            SimpleSink cases = GetFileContentsToSink(testCaseFilePath);

            // run each case
            RunExtractionTestCases(cases);
            Console.WriteLine(string.Format("Extraction Tests Complete"));
        }

        public void RunExtractionTestCases(SimpleSink cases) {
            UpdateMessage(string.Format("Extraction Tests Begin"));
            string testResultsPath = Path.Combine(mTestRootFolderPath, mExtractionResultsFromTestFolder);

            foreach (string file in Directory.GetFiles(testResultsPath)) {
                File.Delete(file);
            }

            ExtractionType extractionParms = new ExtractionType();
            DestinationType destination = null;
			foreach (string[] testCase in cases.Rows)
			{

                // get job list to extract from
                string jobListFileName = testCase[JobListFileIdx].ToString();
                string jobListFilePath = Path.Combine(mTestRootFolderPath, Path.Combine(mJobListTestFileFolder, jobListFileName));
                SimpleSink jobList = GetFileContentsToSink(jobListFilePath);

                // adjust job list for local folders
                AdjustFolderPathForLocalFolders(jobList);

                // set up extraction parameters
                extractionParms.RType = ResultType.TypeList[testCase[ExtractionTypeIdx].ToString()];
                extractionParms.ResultFilterSetID = testCase[FilterSetIDIdx].ToString();
                extractionParms.MSGFCutoff = testCase[MSGFCutoffIdx].ToString();
                extractionParms.KeepAllResults = testCase[KeepAllIdx].ToString();

                // set up destination
                string type = testCase[DestinationTypeIdx].ToString();
                string outputPath = Path.Combine(testResultsPath, testCase[ContainerIdx].ToString());
                string item = testCase[NameIdx].ToString();
                destination = new DestinationType(type, outputPath, item);

                UpdateMessage(string.Format("Test Case:{0} ============", jobListFileName));
                TestExtractFromJobList(jobList, extractionParms, destination);
            }
            UpdateMessage(string.Format("Extraction Tests Complete"));
        }


        protected void TestExtractFromJobList(BaseModule jobList, ExtractionType extractionParms, DestinationType destination) {
            PipelineQueue pq = ExtractionPipelines.MakePipelineQueueToExtractFromJobList(jobList, extractionParms, destination);
            foreach (ProcessingPipeline p in pq.Pipelines.ToArray()) {
                ConnectPipelineToMessageHandler(p);
            }
            ConnectPipelineQueueToMessageHandler(pq);
            pq.RunRoot(null);
        }

        /// <summary>
        /// Substitute local folder tree for "[local]" token in folder column
        /// </summary>
        /// <param name="jobList"></param>
        private void AdjustFolderPathForLocalFolders(SimpleSink jobList) {
            // adjust job list for local folders
            int folderColIdx = jobList.ColumnIndex["Folder"];
            string localPath = Path.Combine(mTestRootFolderPath, mJobInputResultsFolder);
            for (int i = 0; i < jobList.Rows.Count; i++) {
                string folder = jobList.Rows[i][folderColIdx].ToString();
                jobList.Rows[i][folderColIdx] = folder.Replace("[local]", localPath);
            }
        }

        #endregion

        #region File Searching Tests

        protected void RunAllFindFilesTests() {
            // "Sequest Synopsis" "Sequest First Hits"  "X!Tandem First Protein"  "X!Tandem All Proteins"  "Inspect Synopsis"

            string testCaseFilePath = Path.Combine(mTestRootFolderPath, Path.Combine(mTestCasesFolder, "FileListTestCases.txt"));
            SimpleSink cases = GetFileContentsToSink(testCaseFilePath);

            ExtractionType extractionParms = new ExtractionType();
			foreach (string[] testCase in cases.Rows)
			{
                string jobListFile = testCase[0].ToString();
                extractionParms.RType = ResultType.TypeList[testCase[1].ToString()];
                TestFindFilesForJobList(jobListFile, extractionParms);
            }
        }

        /// <summary>
        /// given the name of a file containing a list of jobs, and an extraction type,
        /// get list of files that would be part of extraction.
        /// </summary>
        protected void TestFindFilesForJobList(string jobListFileName, ExtractionType extractionParms) {

            SimpleSink jobList = GetFileContentsToSink(Path.Combine(mTestRootFolderPath, Path.Combine(mJobListTestFileFolder, jobListFileName)));

            SimpleSink fileListSink = new SimpleSink();

            ProcessingPipeline p = ExtractionPipelines.MakePipelineToGetListOfFiles(new SinkWrapper(jobList), fileListSink, extractionParms);
            ConnectPipelineToMessageHandler(p);
            p.RunRoot(null);

            string goodFileName = Path.GetFileNameWithoutExtension(jobListFileName) + "_file_list.txt";
            SimpleSink goodValues = GetFileContentsToSink(Path.Combine(mTestRootFolderPath, Path.Combine(mJobListTestKnownGoodFolder, goodFileName)));
            string errorMsg = CompareSinks(fileListSink, goodValues);
            if (string.IsNullOrEmpty(errorMsg)) {
                Console.WriteLine(string.Format("Passed:{0}", jobListFileName));
            } else {
                Console.WriteLine(string.Format("ERROR:{0}", jobListFileName));
                Console.WriteLine(errorMsg);
            }
        }

        #endregion

        #region Utility Functions

        private SimpleSink GetFileContentsToSink(string path) {
            DelimitedFileReader reader = new DelimitedFileReader();
            reader.FilePath = path;
            SimpleSink sink = new SimpleSink();
            ProcessingPipeline.Assemble("FileContents", reader, sink).RunRoot(null);
            return sink;
        }

        private void WriteSinkContentsToFile(SimpleSink sink, string path) {
            DelimitedFileWriter writer = new DelimitedFileWriter();
            writer.FilePath = path;
            ProcessingPipeline.Assemble("SinkDump", sink, writer).RunRoot(null);
        }

        private static string CompareSinks(SimpleSink source, SimpleSink result) {
            StringBuilder mb = new StringBuilder();
            Collection<MageColumnDef> sourceCols = source.Columns;
            Collection<MageColumnDef> resultCols = result.Columns;
			Collection<string[]> sourceRows = source.Rows;
			Collection<string[]> resultRows = result.Rows;

            if (sourceCols.Count != resultCols.Count) {
                mb.AppendLine("column count does not match");
            }
            if (sourceRows.Count != resultRows.Count) {
                mb.AppendLine("Row count does not match");
            }

            for (int i = 0; i < sourceCols.Count; i++) {
                if (sourceCols[i].Name != resultCols[i].Name) {
                    mb.AppendLine("Column names do not match");
                }
            }

            for (int i = 0; i < sourceRows.Count; i++) {
                for (int j = 0; j < sourceCols.Count; j++) {
                    if (sourceRows[i][j].ToString() != resultRows[i][j].ToString()) {
                        mb.AppendLine(string.Format("Row {0} cell {1} content does not match", i, j));
                    }
                }
            }
            return mb.ToString();
        }


        #endregion

        #region Message Handling Functions

        private void ConnectPipelineToMessageHandler(ProcessingPipeline pipeline) {
            pipeline.OnStatusMessageUpdated += HandlePipelineMessage;
        }

        private void ConnectPipelineQueueToMessageHandler(PipelineQueue pipelineQueue) {
            pipelineQueue.OnPipelineStarted += HandlePipelineMessage;
        }

        private void HandlePipelineMessage(object sender, MageStatusEventArgs args) {
            UpdateMessage(args.Message);
        }

        private void UpdateMessage(string message) {
            if (OnMessageUpdated != null) {
                OnMessageUpdated(this, new MageStatusEventArgs(message));
            }
        }

        #endregion

    }
}
