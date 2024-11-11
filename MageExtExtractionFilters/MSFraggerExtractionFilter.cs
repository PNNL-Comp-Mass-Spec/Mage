using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    internal class MSFraggerExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Fragger, Mage

        public enum MSFraggerColumns
        {
            Peptide,
            Charge,
            Mass,
            EValue,
            Hyperscore,
            PeptideProphetProbability,
            QValue,
            Scan,
            Protein
        }

        // Indexes into the synopsis row field array
        private ColumnIndices mColumnIndices;
        private int peptideMassIndex;
        private int eValueIndex;
        private int hyperscoreIndex;
        private int peptideProphetProbabilityIndex;
        private int qValueIndex;                                // Aka FDR

        private MergeProteinData mProteinMerger;
        private bool mOutputAllProteins;
        private SortedSet<string> mDataWrittenRowTags;

        public FilterMSFraggerResults ResultChecker { get; set; }

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters()
        {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.MSGFPlus);
            mOutputAllProteins = mExtractionType.RType.ResultName == ResultType.MSFRAGGER_SYN_ALL_PROTEINS;
            mDataWrittenRowTags = new SortedSet<string>();
        }

        public override void Prepare()
        {
            base.Prepare();
            InitializeParameters();
        }

        /// <summary>
        /// Set up output column specifications to provide fields to receive merged protein fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            mFilterResultsColumnName = "Passed_Filter";
            OutputColumnList = "Job, Passed_Filter|+|text, *, Cleavage_State|+|text, Terminus_State|+|text";
            base.HandleColumnDef(sender, args);

            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));

            PrecalculateFieldIndexes();

            if (mProteinMerger != null)
            {
                mProteinMerger.GetProteinLookupData(mMergeFiles["ResultToSeqMap"], mMergeFiles["SeqToProteinMap"], mResultsDirectoryPath);
                mProteinMerger.LookupColumn = OutputColumnPos[mExtractionType.RType.ResultIDColName];
                mProteinMerger.SetMergeSourceColIndexes(OutputColumnPos);
            }
        }

        /// <summary>
        /// Process a single result record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                var outRow = MapDataRow(args.Fields);

                if (!mOutputAllProteins)
                {
                    if (!mProteinMerger.MergeFirstProtein(outRow, out var warningMessage))
                    {
                        OnWarningMessage(
                            new MageStatusEventArgs("ProteinMerger reports " + warningMessage + " for row " + mTotalRowsCounter));
                    }

                    var sScanChargePeptide = CreateRowTag(outRow, includeProtein: false, columnIndices: mColumnIndices);
                    if (!mDataWrittenRowTags.Contains(sScanChargePeptide))
                    {
                        mDataWrittenRowTags.Add(sScanChargePeptide);
                        CheckFilter(outRow);
                    }
                }
                else
                {
                    var rows = mProteinMerger.MergeAllProteins(outRow, out var matchFound);
                    if (rows == null)
                    {
                        // Either the peptide only maps to one protein, or the ProteinMerger did not find a match for the row
                        var sScanChargePeptideProtein = CreateRowTag(outRow, includeProtein: true, columnIndices: mColumnIndices);
                        if (!mDataWrittenRowTags.Contains(sScanChargePeptideProtein))
                        {
                            mDataWrittenRowTags.Add(sScanChargePeptideProtein);
                            CheckFilter(outRow);
                            if (!matchFound)
                            {
                                OnWarningMessage(
                                    new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
                            }
                        }
                    }
                    else
                    {
                        foreach (var row in rows)
                        {
                            var sScanChargePeptideProtein = CreateRowTag(row, includeProtein: true, columnIndices: mColumnIndices);
                            if (!mDataWrittenRowTags.Contains(sScanChargePeptideProtein))
                            {
                                mDataWrittenRowTags.Add(sScanChargePeptideProtein);
                                CheckFilter(row);
                            }
                        }
                    }
                }

                ++mTotalRowsCounter;
                ReportProgress();
            }
            else
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        /// <summary>
        /// Evaluate result against filter (if one is active)
        /// and annotate the appropriate column in the result (if one is specified)
        /// and pass on result if it passed the filter
        /// </summary>
        /// <param name="values"></param>
        protected bool CheckFilter(string[] values)
        {
            var accept = true;
            if (ResultChecker == null)
            {
                if (mFilterResultsColIdx >= 0)
                {
                    values[mFilterResultsColIdx] = "Not Checked";
                }
            }
            else
            {
                var peptideSequence = GetColumnValue(values, mColumnIndices.PeptideSequence, "");
                var chargeState = GetColumnValue(values, mColumnIndices.ChargeState, 0);
                var peptideMass = GetColumnValue(values, peptideMassIndex, -1d);

                var eValue = GetColumnValue(values, eValueIndex, -1d);
                var hyperscore = GetColumnValue(values, hyperscoreIndex, -1d);
                var peptideProphetProbability = GetColumnValue(values, peptideProphetProbabilityIndex, -1d);
                var qValue = GetColumnValue(values, qValueIndex, -1d);

                var pass = ResultChecker.EvaluateMSFragger(peptideSequence, chargeState, peptideMass, eValue, hyperscore, peptideProphetProbability, qValue);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0)
                {
                    values[mFilterResultsColIdx] = (pass ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
            }

            if (accept)
            {
                mPassedRowsCounter++;
                OnDataRowAvailable(new MageDataEventArgs(values));
            }

            return accept;
        }

        public static void DetermineFieldIndexes(Dictionary<string, int> columnHeaders, out Dictionary<MSFraggerColumns, int> columnMapping)
        {
            columnMapping = new Dictionary<MSFraggerColumns, int>
            {
                {MSFraggerColumns.Scan, GetColumnIndex(columnHeaders, "Scan")},
                {MSFraggerColumns.Peptide, GetColumnIndex(columnHeaders, "Sequence")},
                {MSFraggerColumns.Charge, GetColumnIndex(columnHeaders, "Charge")},
                {MSFraggerColumns.Mass, GetColumnIndex(columnHeaders, "Mass")},
                {MSFraggerColumns.Protein, GetColumnIndex(columnHeaders, "ProteinName")},
                {MSFraggerColumns.EValue, GetColumnIndex(columnHeaders, "EValue")},
                {MSFraggerColumns.Hyperscore, GetColumnIndex(columnHeaders, "Hyperscore")},
                {MSFraggerColumns.PeptideProphetProbability, GetColumnIndex(columnHeaders, "PeptideProphetProbability")},
                {MSFraggerColumns.QValue, GetColumnIndex(columnHeaders, "QValue")}
            };
        }

        public static int GetColumnIndex(Dictionary<MSFraggerColumns, int> columnPos, MSFraggerColumns columnName)
        {
            if (columnPos.TryGetValue(columnName, out var value))
                return value;

            return -1;
        }

        /// <summary>
        /// Set up indexes into row fields array based on column name
        /// (saves time when referencing result columns later)
        /// </summary>
        private void PrecalculateFieldIndexes()
        {
            if (string.IsNullOrEmpty(OutputColumnList))
            {
                PrecalculateFieldIndexes(InputColumnPos);
            }
            else
            {
                PrecalculateFieldIndexes(OutputColumnPos);
            }
        }

        private void PrecalculateFieldIndexes(Dictionary<string, int> columnPos)
        {
            DetermineFieldIndexes(columnPos, out var columnMapping);

            mColumnIndices.ScanNumber = GetColumnIndex(columnMapping, MSFraggerColumns.Scan);
            mColumnIndices.PeptideSequence = GetColumnIndex(columnMapping, MSFraggerColumns.Peptide);
            mColumnIndices.ChargeState = GetColumnIndex(columnMapping, MSFraggerColumns.Charge);
            peptideMassIndex = GetColumnIndex(columnMapping, MSFraggerColumns.Mass);
            mColumnIndices.Protein = GetColumnIndex(columnMapping, MSFraggerColumns.Protein);

            eValueIndex = GetColumnIndex(columnMapping, MSFraggerColumns.EValue);
            hyperscoreIndex = GetColumnIndex(columnMapping, MSFraggerColumns.Hyperscore);
            peptideProphetProbabilityIndex = GetColumnIndex(columnMapping, MSFraggerColumns.PeptideProphetProbability);
            qValueIndex = GetColumnIndex(columnMapping, MSFraggerColumns.QValue);
        }

        /// <summary>
        /// Return an MSFragger filter object that is preset with filter criteria
        /// that is obtained (by means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterMSFraggerResults MakeMSFraggerResultChecker(string FilterSetID)
        {
            var queryDefXML = ModuleDiscovery.GetQueryXMLDef("Extraction_Filter_Set_List");
            var runtimeParms = new Dictionary<string, string>
            {
                { "Filter_Set_ID", FilterSetID }
            };

            var reader = new SQLReader(queryDefXML, runtimeParms);

            // Create Mage module to receive query results
            var filterCriteria = new SimpleSink();

            // Build pipeline and run it
            var pipeline = ProcessingPipeline.Assemble("GetFilterCriteria", reader, filterCriteria);
            pipeline.RunRoot(null);

            // Create new MS-GF+ filter object with retrieved filter criteria
            return new FilterMSFraggerResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
