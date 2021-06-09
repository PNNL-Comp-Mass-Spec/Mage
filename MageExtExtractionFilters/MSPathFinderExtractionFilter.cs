using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    internal class MSPathFinderExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Mage

        #region "Enums"
        public enum MSPathFinderColumns
        {
            Peptide,
            Charge,
            Mass,
            SpecEValue,
            EValue,
            QValue,
            PepQValue,
            Scan,
            Protein
        }

        #endregion

        #region Member Variables

        // Working copy of MSPathFinder filter object

        // Indexes into the synopsis row field array
        private ColumnIndices mColumnIndices;
        private int peptideMassIndex;
        private int specEValueIndex;
        private int eValueIndex;
        private int qValueIndex;        // Aka FDR
        private int pepqValueIndex;     // Aka peptide-level FDR

        private MergeProteinData mProteinMerger;
        private bool mOutputAllProteins;
        private SortedSet<string> mDataWrittenRowTags;

        #endregion

        #region Properties

        public FilterMSPathFinderResults ResultChecker { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters()
        {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.MSPathFinder);
            mOutputAllProteins = mExtractionType.RType.ResultName == ResultType.MSPATHFINDER_SYN_ALL_PROTEINS;
            mDataWrittenRowTags = new SortedSet<string>();
        }

        public override void Prepare()
        {
            base.Prepare();
            InitializeParameters();
        }

        #endregion

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
                        for (var i = 0; i < rows.Count; i++)
                        {
                            var row = rows[i];

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
        /// <param name="vals"></param>
        /// <returns></returns>
        protected bool CheckFilter(string[] vals)
        {
            var accept = true;
            if (ResultChecker == null)
            {
                if (mFilterResultsColIdx >= 0)
                {
                    vals[mFilterResultsColIdx] = "Not Checked";
                }
            }
            else
            {
                var peptideSequence = GetColumnValue(vals, mColumnIndices.PeptideSequence, "");
                var chargeState = GetColumnValue(vals, mColumnIndices.ChargeState, 0);
                var peptideMass = GetColumnValue(vals, peptideMassIndex, -1d);
                var specEValue = GetColumnValue(vals, specEValueIndex, -1d);
                var eValue = GetColumnValue(vals, eValueIndex, -1d);
                var qValue = GetColumnValue(vals, qValueIndex, -1d);
                var pepQValue = GetColumnValue(vals, pepqValueIndex, -1d);

                var pass = ResultChecker.EvaluateMSPathFinder(peptideSequence, chargeState, peptideMass, specEValue, eValue, qValue, pepQValue);

                accept = pass || mKeepAllResults;
                if (mFilterResultsColIdx >= 0)
                {
                    vals[mFilterResultsColIdx] = (pass ? "Passed-" : "Failed-") + mExtractionType.ResultFilterSetID;
                }
            }

            if (accept)
            {
                mPassedRowsCounter++;
                OnDataRowAvailable(new MageDataEventArgs(vals));
            }

            return accept;
        }

        public static void DetermineFieldIndexes(Dictionary<string, int> columnHeaders, out Dictionary<MSPathFinderColumns, int> dctColumnMapping)
        {
            dctColumnMapping = new Dictionary<MSPathFinderColumns, int>
            {
                {MSPathFinderColumns.Scan, GetColumnIndex(columnHeaders, "Scan")},
                {MSPathFinderColumns.Peptide, GetColumnIndex(columnHeaders, "Sequence")},
                {MSPathFinderColumns.Charge, GetColumnIndex(columnHeaders, "Charge")},
                {MSPathFinderColumns.Mass, GetColumnIndex(columnHeaders, "Mass")},
                {MSPathFinderColumns.Protein, GetColumnIndex(columnHeaders, "ProteinName")},
                {MSPathFinderColumns.SpecEValue, GetColumnIndex(columnHeaders, "SpecEValue")},
                {MSPathFinderColumns.EValue, GetColumnIndex(columnHeaders, "EValue")},
                {MSPathFinderColumns.QValue, GetColumnIndex(columnHeaders, "QValue")},
                {MSPathFinderColumns.PepQValue, GetColumnIndex(columnHeaders, "PepQValue")}
            };
        }

        public static int GetColumnIndex(Dictionary<MSPathFinderColumns, int> columnPos, MSPathFinderColumns columnName)
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
            DetermineFieldIndexes(columnPos, out var dctColumnMapping);

            mColumnIndices.ScanNumber = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.Scan);
            mColumnIndices.PeptideSequence = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.Peptide);
            mColumnIndices.ChargeState = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.Charge);
            peptideMassIndex = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.Mass);
            mColumnIndices.Protein = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.Protein);

            specEValueIndex = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.SpecEValue);
            eValueIndex = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.EValue);

            qValueIndex = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.QValue);
            pepqValueIndex = GetColumnIndex(dctColumnMapping, MSPathFinderColumns.PepQValue);
        }

        /// <summary>
        /// Return a MSPathFinder filter object that is preset with filter criteria
        /// that is obtained (by means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterMSPathFinderResults MakeMSPathFinderResultChecker(string FilterSetID)
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

            // Create new MSGF+ filter object with retrieved filter criteria
            return new FilterMSPathFinderResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
