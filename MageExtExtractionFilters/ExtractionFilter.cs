using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.IO;

namespace MageExtExtractionFilters {

    /// <summary>
    /// Bases class for extraction filters
    /// Handles merging associated results (MSGF)
    /// </summary>
    public class ExtractionFilter : BaseModule {

        #region Member Variables

        protected ExtractionType mExtractionType = null;

        protected bool mKeepAllResults = true;

        protected Dictionary<string, ResultType.MergeFile> mMergeFiles;

        protected string mResultFolderPath = "";


        // Output column that will receive message about whether filter passed or failed
        protected string mFilterResultsColumnName = "";
        protected int mFilterResultsColIdx = -1;

        protected int mTotalRowsCounter = 0;
        protected int mPassedRowsCounter = 0;
        protected int mReportRowBlockSize = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// How many results were processed?
        /// </summary>
        public int ProcessedRows { get { return mTotalRowsCounter; } }

        /// <summary>
        /// How many results passed?
        /// </summary>
        public int PassedRows { get { return mPassedRowsCounter; } }

        /// <summary>
        /// Set extraction type parameters
        /// </summary>
        public ExtractionType ExtractionType {
            get { return mExtractionType; }
            set { mExtractionType = value; }
        }

        /// <summary>
        /// Path to the folder containing the result files to be processed
        /// </summary>
        public string ResultFolderPath {
            get { return mResultFolderPath; }
            set { mResultFolderPath = value; }
        }

        /// <summary>
        /// Current analysis job that results belong to
        /// </summary>
        public string Job { get; set; }

        /// <summary>
        /// Set of associated files for the results files being processed
        /// </summary>
        public Dictionary<string, ResultType.MergeFile> MergeFiles {
            get { return mMergeFiles; }
            set { mMergeFiles = value; }
        }

        #endregion

        #region Constructors

        #endregion

        #region Initialization

        private void InitializeParameters() {
            mTotalRowsCounter = 0;
            mPassedRowsCounter = 0;

            Dictionary<string, string> context = new Dictionary<string, string>() { { "Job", Job } };
            SetContext(context);
            mKeepAllResults = (mExtractionType.KeepAllResults == "Yes");
        }

        public override void Prepare() {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);

            if (OutputColumnDefs == null) {
                Cancel();
            }

            if (OutputColumnPos != null && OutputColumnPos.ContainsKey(mFilterResultsColumnName)) {
                mFilterResultsColIdx = OutputColumnPos[mFilterResultsColumnName];
            }
        }


        protected void ReportProgress() {
            if (mTotalRowsCounter % mReportRowBlockSize == 0) {
                string msg = "Processed " + mTotalRowsCounter.ToString() + " total rows, passed " + mPassedRowsCounter.ToString();
                OnStatusMessageUpdated(new MageStatusEventArgs(msg));
            }
        }
    }
}
