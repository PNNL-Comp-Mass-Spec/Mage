using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Mage
{

    /// <summary>
    /// Delegate for a client-supplied function that this module can call to build its sub-pipeline
    /// </summary>
    /// <param name="inputFilePath">path to input file that sub-pipeline should process</param>
    /// <param name="outputFilePath">path to output file that sub-pipeling should deliver results to</param>
    /// <param name="context">key/values pairs that sub-pipeline can use for column mapping</param>
    /// <returns></returns>
    public delegate ProcessingPipeline FileProcessingPipelineGenerator(string inputFilePath, string outputFilePath, Dictionary<string, string> context);

    /// <summary>
    /// module that creates and runs a Mage pipeline for one or more input files 
    ///
    /// it expects to receive path information for files via its standard tabular input
    /// (its FileContentProcessor base class provides the basic functionality)
    ///
    /// this module builds a filtering sub-pipeline to process each file 
    /// and runs that in the the same thread the module is currently running in
    ///
    /// there are two internally-defined file processing sub-pipelines, that have a delimited file
    /// reader module that reads rows from a file and passed them to a filter module, which passes
    /// its rows to a writer module.
    ///
    /// one of the internally-defined sub-pipelines uses a delimited file writer module, 
    /// and the other uses a SQLite writer.
    ///
    /// to use either of these internally-defined sub-pipelines, the client need only 
    /// supply the name of the filter module to be used (by setting the FileFilterModuleName property)
    /// if the DatabaseName property is set, the SQLite database sub-pipeline will be used
    /// otherwise the delimited file writer sub-pipeline is used
    /// 
    /// If the DatabaseName property is set, and the TableName property is set, all results go into
    /// that table.  If the TableName property is blank, results for each source file go into
    /// a separate table.  Table names will be equivalent to what the output file name would have been
    /// for the source file, minus the file extension.
    ///
    /// the sub-pipeline can also be supplied by the client by setting the FileProcessingPipelineGenerator
    /// delegate to call the client's pipeline generator function
    /// </summary>
    public class FileSubPipelineBroker : FileContentProcessor
    {

        #region Member Variables

        // running count of number of files processed
        private int mFileCount;

        // handle to the currently running sub-pipeline
        private ProcessingPipeline mPipeline;

        // delegate that this module calls to build sub-pipeline
        private FileProcessingPipelineGenerator ProcessingPipelineMaker;

        private Dictionary<string, string> mFileFilterParameters = new Dictionary<string, string>();

        private string mTableName = "";

        #endregion

        #region Functions Available to Clients

        /// <summary>
        /// define a delegate function that will be called by this module 
        /// to construct and run a file processing pipeline 
        /// for each file handled by this broker module
        /// </summary>
        /// <param name="maker"></param>
        public void SetPipelineMaker(FileProcessingPipelineGenerator maker)
        {
            ProcessingPipelineMaker = maker;
        }

        #endregion

        #region Properties

        /// <summary>
        /// name of filter module that is used when internally defined sub-pipeline is used
        /// </summary>
        public string FileFilterModuleName { get; set; }

        // 
        /// <summary>
        /// path to SQLite database file
        /// (parameter for SQLite database writer for internally defined sub-pipelines using SQLite Writer)
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// SQLite database table name
        /// (parameter for SQLite database writer for internally defined sub-pipelines using SQLite Writer)
        /// </summary>
        public string TableName
        {
            get { return mTableName; }
            set { mTableName = value.Trim(); }
        }

        /// <summary>
        /// Deal with file filter parameters as delimited string
        /// "key:value; key:value; 
        /// </summary>
        public string FileFilterParameters
        {
            get
            {
                var s = new List<string>();
                foreach (KeyValuePair<string, string> kv in mFileFilterParameters)
                {
                    s.Add(string.Format("{0}:{1}", kv.Key, kv.Value));
                }
                return string.Join(", ", s);
            }
            set
            {
                var parms = new Dictionary<string, string>();
                foreach (string def in value.Split(';'))
                {
                    string[] pair = def.Split(':');
                    parms.Add(pair[0].Trim(), pair[1].Trim());
                }
                SetFileFilterParameters(parms);
            }
        }

        /// <summary>
        /// (needs work)
        /// </summary>
        /// <param name="parms"></param>
        public void SetFileFilterParameters(Dictionary<string, string> parms)
        {
            mFileFilterParameters = parms;
        }


        /// <summary>
        /// (needs work)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFileFilterParameters()
        {
            return mFileFilterParameters;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage file subpipeline broker module
        /// </summary>
        public FileSubPipelineBroker()
        {
            // set up to use our own default sub-pipeline maker 
            //in case the client doesn't give us another one
            FileFilterModuleName = ""; // client must set this property to use internally defined sub-pipelines
            DatabaseName = ""; // client must set these properties to user internally-defined SQLiteWriter sub-pipeline
            TableName = "";
        }

        #endregion

        #region Overrides

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();

            if (!string.IsNullOrEmpty(FileFilterModuleName))
            {
                // optionally, set up our sub-pipeline generator delegate to use
                // an internally-defined sub-pipeline, according to module settings
                if (!string.IsNullOrEmpty(DatabaseName))
                {
                    ProcessingPipelineMaker = new FileProcessingPipelineGenerator(MakeDefaultSQLiteProcessingPipeline);
                }
                else
                {
                    ProcessingPipelineMaker = new FileProcessingPipelineGenerator(MakeDefaultFileProcessingPipeline);
                }
                // set up to use the file renaming function provided by the filter module
                SetupFileRenamer(FileFilterModuleName);
            }
        }

        /// <summary>
        /// this is called from the base class for each input file to be processed
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="context"></param>
        protected override void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
            if (ProcessingPipelineMaker != null)
            {
                mPipeline = ProcessingPipelineMaker(sourcePath, destPath, context);
                mPipeline.OnStatusMessageUpdated += UpdateStatus;
                mPipeline.RunRoot(null); // we are already in a pipeline thread - don't run sub-pipeline in a new one

                // sub-pipeline encountered fatal error, interrupt the main pipeline
                if (!string.IsNullOrEmpty(mPipeline.CompletionCode))
                {
                    throw new MageException(mPipeline.CompletionCode);
                }
                mFileCount++;
            }
        }

        #endregion

        #region Internally-Defined Processing Pipelines

        private ProcessingPipeline MakeDefaultFileProcessingPipeline(string inputFilePath, string outputFilePath, Dictionary<string, string> context)
        {
            var reader = new DelimitedFileReader();
            var filter = ProcessingPipeline.MakeModule(FileFilterModuleName) as BaseModule;
            var writer = new DelimitedFileWriter();

            filter.SetParameters(mFileFilterParameters);
            filter.SetContext(context);

            reader.FilePath = inputFilePath;
            writer.FilePath = outputFilePath;
            bool concatenateFiles = (!string.IsNullOrEmpty(OutputFileName)) && (mFileCount > 0);
            writer.Append = (concatenateFiles) ? "Yes" : "No";
            writer.Header = (concatenateFiles) ? "No" : "Yes";
            return ProcessingPipeline.Assemble("DefaultFileProcessingPipeline", reader, filter, writer);
        }

        private ProcessingPipeline MakeDefaultSQLiteProcessingPipeline(string inputFilePath, string outputFilePath, Dictionary<string, string> context)
        {
            var reader = new DelimitedFileReader();
            var filter = ProcessingPipeline.MakeModule(FileFilterModuleName) as BaseModule;
            var writer = new SQLiteWriter();

            filter.SetParameters(mFileFilterParameters);
            filter.SetContext(context);

            reader.FilePath = inputFilePath;
            string tableName = (!string.IsNullOrEmpty(TableName)) ? TableName : Path.GetFileNameWithoutExtension(inputFilePath);
            writer.DbPath = DatabaseName;
            writer.TableName = tableName;
            return ProcessingPipeline.Assemble("DefaultFileProcessingPipeline", reader, filter, writer);
        }

        #endregion


        /// <summary>
        /// wire the filter module's file renaming method to this broker module's delegate
        /// if such a renaming method is present
        /// </summary>
        /// <param name="filterModule"></param>
        protected void SetupFileRenamer(string filterModule)
        {
            var pipeline = new ProcessingPipeline("FileProcessingSubPipeline");
            pipeline.MakeModule(filterModule, FileFilterModuleName);
            var bm = (BaseModule)pipeline.GetModule(filterModule);

            var mi = bm.GetType().GetMethod("RenameOutputFile");
            if (mi != null)
            {
                var filterMod = (ContentFilter)bm;
                SetOutputFileNamer(new OutputFileNamer(filterMod.RenameOutputFile));
            }
        }
    }
}