using System;
using System.Collections.Generic;
using Mage;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Bases class for extraction filters
    /// Handles merging associated results (MSGF)
    /// </summary>
    public class ExtractionFilter : BaseModule
    {
        public const string ALL_PASS_CUTOFF = "All Pass";

        protected struct ColumnIndices
        {
            public int ScanNumber;
            public int ChargeState;
            public int PeptideSequence;
            public int Protein;
        }

        protected ExtractionType mExtractionType;

        protected bool mKeepAllResults = true;

        protected Dictionary<string, ResultType.MergeFile> mMergeFiles;

        protected string mResultsDirectoryPath = string.Empty;

        // Output column that will receive message about whether filter passed or failed
        protected string mFilterResultsColumnName = string.Empty;
        protected int mFilterResultsColIdx = -1;

        protected int mTotalRowsCounter;
        protected int mPassedRowsCounter;
        protected int mReportRowBlockSize = 1000;

        protected int mMinimumReportIntervalMsec = 500;
        protected DateTime mLastReportTimeUTC = DateTime.UtcNow;

        /// <summary>
        /// How many results were processed?
        /// </summary>
        public int ProcessedRows => mTotalRowsCounter;

        /// <summary>
        /// How many results passed?
        /// </summary>
        public int PassedRows => mPassedRowsCounter;

        /// <summary>
        /// Set extraction type parameters
        /// </summary>
        public ExtractionType ExtractionType
        {
            get => mExtractionType;
            set => mExtractionType = value;
        }

        /// <summary>
        /// Path to the directory containing the result files to be processed
        /// </summary>
        public string ResultsDirectoryPath
        {
            get => mResultsDirectoryPath;
            set => mResultsDirectoryPath = value;
        }

        /// <summary>
        /// Current analysis job that results belong to
        /// </summary>
        public string Job { get; set; }

        /// <summary>
        /// Set of associated files for the results files being processed
        /// </summary>
        public Dictionary<string, ResultType.MergeFile> MergeFiles
        {
            get => mMergeFiles;
            set => mMergeFiles = value;
        }

        private void InitializeParameters()
        {
            mTotalRowsCounter = 0;
            mPassedRowsCounter = 0;

            var context = new Dictionary<string, string> { { "Job", Job } };
            SetContext(context);
            mKeepAllResults = OptionEnabled(mExtractionType.KeepAllResults);
        }

        public override void Prepare()
        {
            base.Prepare();
            InitializeParameters();
        }

        protected string CreateRowTag(
            string[] values,
            bool includeProtein,
            ColumnIndices columnIndices)
        {
            var scanNumber = GetColumnValue(values, columnIndices.ScanNumber, -1);
            var chargeState = GetColumnValue(values, columnIndices.ChargeState, 0);
            var peptideSequence = GetColumnValue(values, columnIndices.PeptideSequence, string.Empty);
            var proteinName = GetColumnValue(values, columnIndices.Protein, string.Empty);

            if (includeProtein)
            {
                // Even though we are tracking proteins, we need to remove prefix and suffix residues to avoid reporting the same peptide mapped to the same protein in the same scan twice in the output file
                return scanNumber + "_" + chargeState + "_" + RemovePrefixAndSuffixResidues(peptideSequence) + "_" + proteinName;
            }

            var peptideSequenceMain = RemovePrefixAndSuffixResidues(peptideSequence);
            return scanNumber + "_" + chargeState + "_" + peptideSequenceMain;
        }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);

            if (OutputColumnDefs == null)
            {
                Cancel();
            }

            if (OutputColumnPos != null && OutputColumnPos.ContainsKey(mFilterResultsColumnName))
            {
                mFilterResultsColIdx = OutputColumnPos[mFilterResultsColumnName];
            }
        }

        protected string RemovePrefixAndSuffixResidues(string peptide)
        {
            if (peptide.StartsWith("..") && peptide.Length > 2)
                peptide = '.' + peptide.Substring(2);

            if (peptide.EndsWith("..") && peptide.Length > 2)
                peptide = peptide.Substring(0, peptide.Length - 2) + '.';

            var periodIndex1 = peptide.IndexOf('.');
            var periodIndex2 = peptide.IndexOf('.', periodIndex1 + 1);

            // See if strSequenceIn contains two periods
            if (periodIndex1 > -1 && periodIndex2 > -1 && periodIndex2 > periodIndex1 + 1)
            {
                // Sequence contains two periods with letters between the periods,
                // For example, A.BCDEFGHIJK.L or ABCD.BCDEFGHIJK.L
                // Extract out the text between the periods
                peptide = peptide.Substring(periodIndex1 + 1, periodIndex2 - periodIndex1 - 1);
            }

            return peptide;
        }

        protected void ReportProgress()
        {
            if (mTotalRowsCounter % mReportRowBlockSize == 0)
            {
                var msg = "Processed " + mTotalRowsCounter.ToString() + " total rows, passed " + mPassedRowsCounter.ToString();
                if (DateTime.UtcNow.Subtract(mLastReportTimeUTC).TotalMilliseconds >= mMinimumReportIntervalMsec)
                {
                    OnStatusMessageUpdated(new MageStatusEventArgs(msg));
                    mLastReportTimeUTC = DateTime.UtcNow;
                }
            }
        }
    }
}
