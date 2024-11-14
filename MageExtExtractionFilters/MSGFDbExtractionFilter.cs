using System;
using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    [Obsolete("Deprecated in 2024")]
    public class MSGFDbExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Mage

        public enum MSGFDBColumns
        {
            Peptide,
            Charge,
            MH,
            MSGF_SpecProb,
            PValueOrEValue,
            FDROrQValue,
            PepFDROrPepQValue,
            MSGFDB_SpecProbOrEValue,
            Rank_MSGFDB_SpecProbOrEValue,
            Scan,
            Protein
        }

        // Indexes into the synopsis row field array
        private ColumnIndices mColumnIndices;
        private int peptideMassIndex;
        private int msgfDbSpecEValueIndex;		        // MSGFDB_SpecProb for MSGFDB, MSGFDB_SpecEValue for MS-GF+
        private int rankMSGFDbSpecProbIndex = -1;		// Rank_MSGFDB_SpecProb for MSGFDB, Rank_MSGFDB_SpecEValue for MS-GF+

        private int eValueIndex;					    // PValue for MSGFDB,          EValue for MS-GF+

        // Note that FDR and PepFDR may not be present
        private int FDRIndex = -1;						// FDR for MSGFDB,             QValue for MS-GF+
        private int pepFDRIndex = -1;					// PepFDR for MSGFDB,          PepQValue for MS-GF+

        private int msgfSpecProbIndex = -1;             // Spectral Probability from MSGF; for MS-GF+ this column will have identical values the data in msgfDbSpecEValueIndex

        private MergeProteinData mProteinMerger;
        private bool mOutputAllProteins;
        private SortedSet<string> mDataWrittenRowTags;

        public FilterMSGFDbResults ResultChecker { get; set; }

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters()
        {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.MSGFPlus);
            mOutputAllProteins = mExtractionType.RType.ResultName == ResultType.MSGFDB_SYN_ALL_PROTEINS;
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

            // List<MageColumnDef> columnDefs = (OutputColumnDefs != null) ? OutputColumnDefs : InputColumnDefs;
            // OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
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
                    var mergeSuccess = mProteinMerger.MergeFirstProtein(outRow, out var warningMessage);
                    if (!mergeSuccess)
                    {
                        // mProteinMerger does not have a record of this row's ResultID
                        // This is perfectly acceptable when processing a _syn.txt file
                        // because, for each scan/charge combo in the _syn.txt file,
                        // the syn_ResultToSeqMap.txt only contains an entry for the first
                        // occurrence of each peptide (for a given scan/charge)
                        //
                        // We will log a warning below if the failure of the merge is actually an issue
                    }

                    var sScanChargePeptide = CreateRowTag(outRow, includeProtein: false, columnIndices: mColumnIndices);
                    if (!mDataWrittenRowTags.Contains(sScanChargePeptide))
                    {
                        if (!mergeSuccess)
                        {
                            OnWarningMessage(
                                new MageStatusEventArgs("ProteinMerger reports " + warningMessage + " for row " + mTotalRowsCounter));
                        }

                        if (string.IsNullOrWhiteSpace(outRow[mColumnIndices.Protein]))
                        {
                            OnWarningMessage(
                                new MageStatusEventArgs("Empty protein name for row " + mTotalRowsCounter));
                        }

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
                var specEValue = GetColumnValue(values, msgfDbSpecEValueIndex, -1d);
                var eValue = GetColumnValue(values, eValueIndex, -1d);
                var FDR = GetColumnValue(values, FDRIndex, -1d);
                var PepFDR = GetColumnValue(values, pepFDRIndex, -1d);
                var msgfSpecProb = GetColumnValue(values, msgfSpecProbIndex, -1d);
                var rankMSGFDbSpecProb = GetColumnValue(values, rankMSGFDbSpecProbIndex, -1);

                var pass = ResultChecker.EvaluateMSGFDB(peptideSequence, chargeState, peptideMass, specEValue, eValue, FDR, PepFDR, msgfSpecProb, rankMSGFDbSpecProb);

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

        public static void DetermineFieldIndexes(Dictionary<string, int> columnHeaders, out Dictionary<MSGFDBColumns, int> columnMapping)
        {
            var msgfPlus = false;

            columnMapping = new Dictionary<MSGFDBColumns, int>
            {
                {MSGFDBColumns.Scan, GetColumnIndex(columnHeaders, "Scan")},
                {MSGFDBColumns.Peptide, GetColumnIndex(columnHeaders, "Peptide")},
                {MSGFDBColumns.Charge, GetColumnIndex(columnHeaders, "Charge")},
                {MSGFDBColumns.MH, GetColumnIndex(columnHeaders, "MH")},
                {MSGFDBColumns.Protein, GetColumnIndex(columnHeaders, "Protein")}
            };

            var columnIndex = GetColumnIndex(columnHeaders, "MSGFDB_SpecProb");

            // MS-GF+ has MSGFDB_SpecEValue instead of MSGFDB_SpecProb; need to check for this
            if (columnIndex < 0)
            {
                columnIndex = GetColumnIndex(columnHeaders, "MSGFDB_SpecEValue");
                if (columnIndex >= 0)
                    msgfPlus = true;
            }

            columnMapping.Add(MSGFDBColumns.MSGFDB_SpecProbOrEValue, columnIndex);

            // Note that FDR and PepFDR (QValue and PepQValue in MS-GF+) may not be present in MSGFDB results
            if (msgfPlus)
            {
                columnMapping.Add(MSGFDBColumns.PValueOrEValue, GetColumnIndex(columnHeaders, "EValue"));
                columnMapping.Add(MSGFDBColumns.FDROrQValue, GetColumnIndex(columnHeaders, "QValue"));
                columnMapping.Add(MSGFDBColumns.PepFDROrPepQValue, GetColumnIndex(columnHeaders, "PepQValue"));
                columnMapping.Add(MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue, GetColumnIndex(columnHeaders, "Rank_MSGFDB_SpecEValue"));
            }
            else
            {
                columnMapping.Add(MSGFDBColumns.PValueOrEValue, GetColumnIndex(columnHeaders, "PValue"));
                columnMapping.Add(MSGFDBColumns.FDROrQValue, GetColumnIndex(columnHeaders, "FDR"));
                columnMapping.Add(MSGFDBColumns.PepFDROrPepQValue, GetColumnIndex(columnHeaders, "PepFDR"));
                columnMapping.Add(MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue, GetColumnIndex(columnHeaders, "Rank_MSGFDB_SpecProb"));
            }

            columnMapping.Add(MSGFDBColumns.MSGF_SpecProb, GetColumnIndex(columnHeaders, "MSGF_SpecProb"));
        }

        public static int GetMSGFDBColumnIndex(Dictionary<MSGFDBColumns, int> columnPos, MSGFDBColumns columnName)
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

            mColumnIndices.ScanNumber = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.Scan);
            mColumnIndices.PeptideSequence = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.Peptide);
            mColumnIndices.ChargeState = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.Charge);
            peptideMassIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.MH);
            mColumnIndices.Protein = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.Protein);

            msgfDbSpecEValueIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.MSGFDB_SpecProbOrEValue);
            rankMSGFDbSpecProbIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.Rank_MSGFDB_SpecProbOrEValue);

            eValueIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.PValueOrEValue);

            // Note that FDR and PepFDR may not be present
            FDRIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.FDROrQValue);
            pepFDRIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.PepFDROrPepQValue);

            msgfSpecProbIndex = GetMSGFDBColumnIndex(columnMapping, MSGFDBColumns.MSGF_SpecProb);
        }

        /// <summary>
        /// Return an MSGFDB filter object that is preset with filter criteria
        /// that is obtained (by means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterMSGFDbResults MakeMSGFDbResultChecker(string FilterSetID)
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
            return new FilterMSGFDbResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
