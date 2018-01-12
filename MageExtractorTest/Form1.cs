using System;
using System.IO;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;
using PRISM.Logging;


namespace MageExtractorTest {

    public partial class Form1 : Form {


        #region Initialization

        public Form1() {
            InitializeComponent();
            SavedState.SetupConfigFiles("MageExtractor");
            var logFilePath = Path.Combine(SavedState.DataDirectory, "log.txt");
            const bool appendDateToBaseName = false;
            FileLogger.ChangeLogFileBaseName(logFilePath, appendDateToBaseName);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e) {
            var mode = DisplaySourceMode.Selected;
            RunTests(mode);
        }
        private void button2_Click(object sender, EventArgs e) {
            var mode = DisplaySourceMode.All;
            RunTests(mode);
        }
        private void RunTests(DisplaySourceMode mode) {
            BaseModule display = new GVPipelineSource(gridViewDisplayControl1, mode);
            var testCases = new SimpleSink();
            ProcessingPipeline.Assemble("TestCases", display, testCases).RunRoot(null);

            textBox1.Clear();

            var et = new ExtractorTests {TestRootFolderPath = TestRootPathCtl.Text};
            et.OnMessageUpdated += HandleMessage;
            et.RunExtractionTestCases(testCases);
        }

        #region Message Handlers

        private delegate void MessageHandler(string message);

        private void HandleMessage(object sender, MageStatusEventArgs args) {
            if (textBox1.InvokeRequired) {
                MessageHandler ncb = DisplayMessage;
                textBox1.Invoke(ncb, args.Message);
            } else {
                DisplayMessage(args.Message);
            }
        }

        private void DisplayMessage(string Message) {
            textBox1.Text += Message + Environment.NewLine;
            textBox1.Update();
        }

        #endregion

        #region Weird XML pipeline testing stuff

        private void label1_Click(object sender, EventArgs e) {
            TestXMLBuiltImportPipelines();
 //           TestCodeBuiltImportPipelines();
        }

        private
        string mXMLForPipelineToImportToFile = @"
<pipeline name='Test_Pipeline'>
    <module type='FileListFilter'>
        <param name='FolderPath'><![CDATA[C:\Data\syn2]]></param>
        <param name='FileNameSelector'>_syn.txt</param>
        <param name='FileTypeColumnName'>Item</param>
        <param name='FileColumnName'>Name</param>
        <param name='SourceFolderColumnName'>Folder</param>
        <param name='OutputColumnList'>Item|+|text, Name|+|text, Folder|+|text</param>
        <param name='IncludeFilesOrFolders'>File</param>
    </module>
    <module type='FileSubPipelineBroker'>
        <param name='FileFilterModuleName'>NullFilter</param>
        <param name='SourceFileColumnName'>Name</param>
        <param name='SourceFolderColumnName'>Folder</param>
        <param name='FileFilterParameters'>OutputColumnList:Name|+|text, *</param>
        <param name='OutputFolderPath'><![CDATA[C:\Data\syn2\]]></param>
        <param name='OutputFileName'>junk.txt</param>
        <param name='DatabaseName'></param>
        <param name='TableName'></param>
    </module>
</pipeline>
";
        private
        string mXMLForPipelineToImportToSQLite = @"
<pipeline name='Test_Pipeline'>
    <module type='FileListFilter'>
        <param name='FolderPath'><![CDATA[C:\Data\syn2]]></param>
        <param name='FileNameSelector'>_syn.txt</param>

        <param name='FileTypeColumnName'>Item</param>
        <param name='FileColumnName'>Name</param>
        <param name='SourceFolderColumnName'>Folder</param>
        <param name='OutputColumnList'>Item|+|text, Name|+|text, Folder|+|text</param>
        <param name='IncludeFilesOrFolders'>File</param>
    </module>
    <module type='FileSubPipelineBroker'>
        <param name='FileFilterParameters'>OutputColumnList:Name|+|text, *</param>
        <param name='OutputFolderPath'></param>
        <param name='OutputFileName'></param>
        <param name='DatabaseName'><![CDATA[C:\Data\syn2\junk.db3]]></param>
        <param name='TableName'>t_test</param>

        <param name='SourceFolderColumnName'>Folder</param>
        <param name='FileFilterModuleName'>NullFilter</param>
        <param name='SourceFileColumnName'>Name</param>
    </module>
</pipeline>
";
        private void TestXMLBuiltImportPipelines() {
            ProcessingPipeline.Assemble(mXMLForPipelineToImportToFile).RunRoot(null);
            ProcessingPipeline.Assemble(mXMLForPipelineToImportToSQLite).RunRoot(null);
        }

        private void TestCodeBuiltImportPipelines() {
            var pipeline = ImportContentsOfFiles(@"C:\Data\syn2", "_syn.txt", "Name|+|text, *", @"C:\Data\syn2\junk.db3", "t_bob", "database");
            pipeline.RunRoot(null);
            pipeline = ImportContentsOfFiles(@"C:\Data\syn2", "_syn.txt", "Name|+|text, *", @"C:\Data\syn2\", "junk.txt", "file");
            pipeline.RunRoot(null);
        }

        private ProcessingPipeline ImportContentsOfFiles(string sourceFolderPath, string fileNameSelector, string columnMap, string containerPath, string name, string destinationType) {
            // Make source module in pipeline
            // to get filtered list of files in local directory
            var reader = new FileListFilter
            {
                FolderPath = sourceFolderPath,
                FileNameSelector = fileNameSelector,
                FileTypeColumnName = "Item",
                FileColumnName = "Name",
                SourceFolderColumnName = "Folder",
                OutputColumnList = "Item|+|text, Name|+|text, Folder|+|text",
                IncludeFilesOrFolders = "File"
            };

            // Make file sub-pipeline processing broker module
            // to run a filter pipeline against files from list
            // and extract their contents
            var broker = new FileSubPipelineBroker
            {
                FileFilterModuleName = "NullFilter",
                SourceFileColumnName = "Name",
                SourceFolderColumnName = "Folder",
                FileFilterParameters = "OutputColumnList:" + columnMap
            };

            // Output extracted filed contents
            // to SQLite or delimited file(s)
            if (destinationType == "database") {
                broker.OutputFolderPath = "";
                broker.OutputFileName = "";
                broker.DatabaseName = containerPath;
                broker.TableName = name;
            }
            if (destinationType == "file") {
                broker.OutputFolderPath = containerPath;
                broker.OutputFileName = name;
                broker.DatabaseName = "";
                broker.TableName = "";
            }

            // Build pipeline from modules
            return ProcessingPipeline.Assemble("Import_Files", reader, broker);
        }

        // Export SQLite query to file

        // Do extraction

        #endregion
    }
}
