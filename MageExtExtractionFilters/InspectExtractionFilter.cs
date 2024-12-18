﻿using System;
using System.Collections.Generic;
using Mage;
using MageExtContentFilters;

namespace MageExtExtractionFilters
{
    [Obsolete("Deprecated in 2024")]
    public class InspectExtractionFilter : ExtractionFilter
    {
        // Ignore Spelling: Mage

        // Indexes into the synopsis row field array
        private int peptideSequenceIndex;
        private int chargeStateIndex;
        private int peptideMassIndex;
        private int mqScoreIndex;
        private int totalPRMScoreIndex;
        private int fScoreIndex;
        private int pValueIndex;

        private int msgfSpecProbIndex = -1;
        private int rankTotalPRMScoreIndex = -1;

        private MergeProteinData mProteinMerger;
        private bool mOutputAllProteins;

        public FilterInspectResults ResultChecker { get; set; }

        /// <summary>
        /// Read contents of associated protein files and build lookup tables and indexes
        /// </summary>
        private void InitializeParameters()
        {
            // ResultType.MergeFile mfMap = mMergeFiles["ResultToSeqMap"];
            // ResultType.MergeFile mfProt = mMergeFiles["SeqToProteinMap"];
            mProteinMerger = new MergeProteinData(MergeProteinData.MergeModeConstants.MSGFPlus);
            mOutputAllProteins = mExtractionType.RType.ResultName == ResultType.INSPECT_SYN_ALL_PROTEINS;
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
                    if (!mProteinMerger.MergeFirstProtein(outRow, out var warningMessage))
                    {
                        OnWarningMessage(
                            new MageStatusEventArgs("ProteinMerger reports " + warningMessage + " for row " + mTotalRowsCounter));
                    }

                    CheckFilter(outRow);
                }
                else
                {
                    var rows = mProteinMerger.MergeAllProteins(outRow, out var matchFound);
                    if (rows == null)
                    {
                        CheckFilter(outRow);
                        if (!matchFound)
                            OnWarningMessage(new MageStatusEventArgs("ProteinMerger did not find a match for row " + mTotalRowsCounter));
                    }
                    else
                    {
                        foreach (var row in rows)
                        {
                            CheckFilter(row);
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
                var peptideSequence = GetColumnValue(values, peptideSequenceIndex, "");
                var chargeState = GetColumnValue(values, chargeStateIndex, 0);
                var peptideMass = GetColumnValue(values, peptideMassIndex, -1d);

                var MQScore = GetColumnValue(values, mqScoreIndex, -1d);
                var TotalPRMScore = GetColumnValue(values, totalPRMScoreIndex, -1d);
                var FScore = GetColumnValue(values, fScoreIndex, -1d);
                var PValue = GetColumnValue(values, pValueIndex, -1d);

                var msgfSpecProb = GetColumnValue(values, msgfSpecProbIndex, -1d);
                var rankTotalPRMScore = GetColumnValue(values, rankTotalPRMScoreIndex, -1);

                var pass = ResultChecker.EvaluateInspect(peptideSequence, chargeState, peptideMass, MQScore, TotalPRMScore, FScore, PValue, msgfSpecProb, rankTotalPRMScore);

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
            peptideSequenceIndex = GetColumnIndex(columnPos, "Peptide");
            chargeStateIndex = GetColumnIndex(columnPos, "Charge");
            peptideMassIndex = GetColumnIndex(columnPos, "MH");

            mqScoreIndex = GetColumnIndex(columnPos, "MQScore");
            totalPRMScoreIndex = GetColumnIndex(columnPos, "TotalPRMScore");
            fScoreIndex = GetColumnIndex(columnPos, "FScore");
            pValueIndex = GetColumnIndex(columnPos, "PValue");

            msgfSpecProbIndex = GetColumnIndex(columnPos, "MSGF_SpecProb");
            rankTotalPRMScoreIndex = GetColumnIndex(columnPos, "RankTotalPRMScore");
        }

        /// <summary>
        /// Return an Inspect filter object that is preset with filter criteria
        /// that is obtained (my means of a Mage pipeline) for the given FilterSetID from DMS
        /// </summary>
        public static FilterInspectResults MakeInspectResultChecker(string FilterSetID)
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

            // Create new Inspect filter object with retrieved filter criteria
            return new FilterInspectResults(filterCriteria.Rows, FilterSetID);
        }
    }
}
