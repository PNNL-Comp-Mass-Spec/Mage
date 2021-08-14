using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Mage;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// This class handles merging protein data into the result data
    /// </summary>
    internal class MergeProteinData
    {
        public enum MergeModeConstants
        {
            XTandem = 0,
            InspectOrMSGFDB = 1,         // Used by Inspect and MSGF+
            MSPathFinder = 2
        }



        // Reference to row in protein buffer
        // Keys are ResultID, values are SequenceId
        private Dictionary<string, int> mProtDataLookup;

        // Indexes for the protein columns in the result
        private int ODX_Cleavage_State = -1;
        private int ODX_Terminus_State = -1;
        private int ODX_Protein_Name = -1;
        private int ODX_Protein_Expectation_Value_Log = -1;
        private int ODX_Protein_Intensity_Log = -1;

        // Index for the result column that contains
        // the key to use to look up the protein
        public int LookupColumn { get; set; } = -1;



        public MergeModeConstants MergeMode { get; }



        // Constructor
        public MergeProteinData(MergeModeConstants eMergeMode)
        {
            MergeMode = eMergeMode;
        }

        // Precalculate field indexes in protein buffer
        // (to save time later)
        public void SetMergeSourceColIndexes(Dictionary<string, int> colPos)
        {
            ODX_Cleavage_State = colPos["Cleavage_State"];
            ODX_Terminus_State = colPos["Terminus_State"];

            if (MergeMode == MergeModeConstants.XTandem)
            {
                ODX_Protein_Name = colPos["Protein_Name"];
                ODX_Protein_Expectation_Value_Log = colPos["Protein_Expectation_Value_Log(e)"];
                ODX_Protein_Intensity_Log = colPos["Protein_Intensity_Log(I)"];
            }

            if (MergeMode == MergeModeConstants.InspectOrMSGFDB)
            {
                // The Protein column is present in the original _msgfplus_syn.txt file
                // We are replacing the protein name listed with the name from the _msgfplus_syn_SeqToProteinMap.txt file
                ODX_Protein_Name = colPos["Protein"];
            }

            if (MergeMode == MergeModeConstants.MSPathFinder)
            {
                // The Protein column is present in the original _msgfplus_syn.txt file;
                // We are replacing the protein name listed with the name from the _msgfplus_syn_SeqToProteinMap.txt file
                ODX_Protein_Name = colPos["ProteinName"];
            }
        }

        // Lookup index relating Unique_Seq_ID
        // to index of first occurrence row in proteinData
        private Dictionary<int, int> mFirstOccurrenceIndex;

        // Protein data buffer
        private readonly List<ProteinInfo> mProteinDataSorted = new();

        /// <summary>
        /// Look up first protein identified for result
        /// and merge protein columns into result row
        /// </summary>
        /// <param name="outRow"></param>
        /// <param name="warningMessage">Warning message (empty if this function returns true)</param>
        /// <returns>True if success, false if a match was not found in the cached protein data</returns>
        public bool MergeFirstProtein(string[] outRow, out string warningMessage)
        {
            var resultID = outRow[LookupColumn];

            if (!mProtDataLookup.TryGetValue(resultID, out var sequenceID))
            {
                warningMessage = "resultID " + resultID + " not found in the result to protein lookup table";
                return false;
            }

            outRow[ODX_Cleavage_State] = string.Empty;
            outRow[ODX_Terminus_State] = string.Empty;
            outRow[ODX_Protein_Name] = string.Empty;

            if (!mFirstOccurrenceIndex.TryGetValue(sequenceID, out var rowIdx))
            {
                warningMessage = "sequenceID " + sequenceID + " not found in the FirstOccurrence dictionary";
                return false;
            }

            outRow[ODX_Cleavage_State] = mProteinDataSorted[rowIdx].Cleavage_State.ToString();
            outRow[ODX_Terminus_State] = mProteinDataSorted[rowIdx].Terminus_State.ToString();
            outRow[ODX_Protein_Name] = mProteinDataSorted[rowIdx].Protein_Name;

            if (ODX_Protein_Expectation_Value_Log > -1)
                outRow[ODX_Protein_Expectation_Value_Log] = mProteinDataSorted[rowIdx].Protein_Expectation_Value_LogE;
            if (ODX_Protein_Intensity_Log > -1)
                outRow[ODX_Protein_Intensity_Log] = mProteinDataSorted[rowIdx].Protein_Intensity_LogI;

            warningMessage = string.Empty;

            return true;
        }

        /// <summary>
        /// Look up single protein or list of proteins for result.
        /// If single protein, modify outRow and return null list.
        /// If multiple proteins, return in list.
        /// </summary>
        /// <param name="outRow"></param>
        /// <param name="matchFound">Output parameter: True if the ResultID and SequenceID are present in the cached data</param>
        /// <returns></returns>
        public Collection<string[]> MergeAllProteins(string[] outRow, out bool matchFound)
        {
            Collection<string[]> outRows = null;
            var resultID = outRow[LookupColumn];
            matchFound = false;

            if (!mProtDataLookup.TryGetValue(resultID, out var sequenceID))
            {
                return null;
            }

            if (!mFirstOccurrenceIndex.TryGetValue(sequenceID, out var rowIdx))
            {
                return null;
            }

            matchFound = true;

            var numberOfProteins = 0;
            // Starting at first protein, merge protein fields into outRow
            // and then walk down the protein list checking the sequence ID.
            // If single protein, outRow has been updated, and no futher action taken.
            // If more than one protein for current sequence, create list,
            // merge protein data into new copy of result, and add merged
            // result to list
            for (var rIdx = rowIdx; rIdx < mProteinDataSorted.Count; rIdx++)
            {
                if (mProteinDataSorted[rIdx].Unique_Seq_ID == sequenceID)
                {
                    numberOfProteins++;
                    if (numberOfProteins == 2)
                    {
                        outRows = new Collection<string[]>();
                    }

                    if (numberOfProteins >= 2 && outRows != null)
                    {
                        var row = new string[outRow.Length];
                        Array.Copy(outRow, row, outRow.Length);
                        outRows.Add(row);
                    }

                    outRow[ODX_Cleavage_State] = mProteinDataSorted[rIdx].Cleavage_State.ToString();
                    outRow[ODX_Terminus_State] = mProteinDataSorted[rIdx].Terminus_State.ToString();
                    outRow[ODX_Protein_Name] = mProteinDataSorted[rIdx].Protein_Name;
                    if (ODX_Protein_Expectation_Value_Log > -1)
                        outRow[ODX_Protein_Expectation_Value_Log] = mProteinDataSorted[rIdx].Protein_Expectation_Value_LogE;
                    if (ODX_Protein_Intensity_Log > -1)
                        outRow[ODX_Protein_Intensity_Log] = mProteinDataSorted[rIdx].Protein_Intensity_LogI;
                }
                else
                {
                    if (numberOfProteins >= 2 && outRows != null)
                    {
                        var row = new string[outRow.Length];
                        Array.Copy(outRow, row, outRow.Length);
                        outRows.Add(row);
                    }
                    break;
                }
            }
            return outRows;
        }

        // FUTURE: check for missing file names before loading file.
        public void GetProteinLookupData(ResultType.MergeFile mapMergeFile, ResultType.MergeFile protMergeFile, string resultDirectoryPath)
        {
            // Get result to protein mapping data
            var mapFileName = mapMergeFile.MergeFileName;
            var resultToSequenceMap = new SimpleSink();
            if (!string.IsNullOrEmpty(mapFileName))
            {
                var mapFileReader = new DelimitedFileReader
                {
                    FilePath = Path.Combine(resultDirectoryPath, mapFileName)
                };
                ProcessingPipeline.Assemble("Lookup Protein Map", mapFileReader, resultToSequenceMap).RunRoot(null);
            }

            // Get protein data
            var mProteinData = new SimpleSink();

            var protFileName = protMergeFile.MergeFileName;
            if (!string.IsNullOrEmpty(protFileName))
            {
                var protFileReader = new DelimitedFileReader
                {
                    FilePath = Path.Combine(resultDirectoryPath, protFileName)
                };
                ProcessingPipeline.Assemble("Lookup Protein Data", protFileReader, mProteinData).RunRoot(null);
            }

            if (!mProteinData.ColumnIndex.TryGetValue("Unique_Seq_ID", out var colIndexUniqueSeqID))
                colIndexUniqueSeqID = -1;
            if (!mProteinData.ColumnIndex.TryGetValue("Cleavage_State", out var colIndexCleavageState))
                colIndexCleavageState = -1;
            if (!mProteinData.ColumnIndex.TryGetValue("Terminus_State", out var colIndexTerminusState))
                colIndexTerminusState = -1;
            if (!mProteinData.ColumnIndex.TryGetValue("Protein_Name", out var colIndexProteinName))
                colIndexProteinName = -1;
            if (!mProteinData.ColumnIndex.TryGetValue("Protein_Expectation_Value_Log(e)", out var colIndexEValue))
                colIndexEValue = -1;
            if (!mProteinData.ColumnIndex.TryGetValue("Protein_Intensity_Log(I)", out var colIndexProteinIntensity))
                colIndexProteinIntensity = -1;

            // Extract the protein data from the SimpleSink so that we can sort it
            mProteinDataSorted.Clear();
            for (var rowIdx = 0; rowIdx < mProteinData.Rows.Count; rowIdx++)
            {
                var bValidProteinInfo = false;
                var oProteinInfo = new ProteinInfo();

                if (mProteinData.TryGetValueViaColumnIndex(colIndexUniqueSeqID, rowIdx, out int iValue))
                {
                    bValidProteinInfo = true;
                    oProteinInfo.Unique_Seq_ID = iValue;
                }

                if (bValidProteinInfo)
                {
                    if (mProteinData.TryGetValueViaColumnIndex(colIndexCleavageState, rowIdx, out iValue))
                        oProteinInfo.Cleavage_State = iValue;

                    if (mProteinData.TryGetValueViaColumnIndex(colIndexTerminusState, rowIdx, out iValue))
                        oProteinInfo.Terminus_State = iValue;

                    if (mProteinData.TryGetValueViaColumnIndex(colIndexProteinName, rowIdx, out string sValue))
                        oProteinInfo.Protein_Name = sValue;

                    if (mProteinData.TryGetValueViaColumnIndex(colIndexEValue, rowIdx, out sValue))
                        oProteinInfo.Protein_Expectation_Value_LogE = sValue;

                    if (mProteinData.TryGetValueViaColumnIndex(colIndexProteinIntensity, rowIdx, out sValue))
                        oProteinInfo.Protein_Intensity_LogI = sValue;

                    mProteinDataSorted.Add(oProteinInfo);
                }
            }

            mProteinDataSorted.Sort(new ProteinDataSorter());

            // Index relating Unique_Seq_ID to index of first occurrence row in proteinData
            mFirstOccurrenceIndex = new Dictionary<int, int>(mProteinDataSorted.Count);
            var currentSeqID = 0;
            for (var rowIdx = 0; rowIdx < mProteinDataSorted.Count; rowIdx++)
            {
                if (mProteinDataSorted[rowIdx].Unique_Seq_ID <= currentSeqID)
                {
                    continue;
                }
                mFirstOccurrenceIndex[mProteinDataSorted[rowIdx].Unique_Seq_ID] = rowIdx;
                currentSeqID = mProteinDataSorted[rowIdx].Unique_Seq_ID;
            }

            // Index for Result_ID to Unique_Seq_ID map
            // FUTURE: look up column index based on column header
            mProtDataLookup = new Dictionary<string, int>();
            foreach (var row in resultToSequenceMap.Rows)
            {
                if (int.TryParse(row[1], out var sID))
                {
                    mProtDataLookup[row[0]] = sID;
                }
            }
        }

        protected class ProteinInfo
        {
            public int Unique_Seq_ID { get; set; }
            public int Cleavage_State { get; set; }
            public int Terminus_State { get; set; }
            public string Protein_Name { get; set; }
            public string Protein_Expectation_Value_LogE { get; set; }
            public string Protein_Intensity_LogI { get; set; }

            public ProteinInfo()
            {
                Unique_Seq_ID = -1;
                Cleavage_State = 0;
                Terminus_State = 0;
                Protein_Name = string.Empty;
                Protein_Expectation_Value_LogE = string.Empty;
                Protein_Intensity_LogI = string.Empty;
            }
        }

        protected class ProteinDataSorter : IComparer<ProteinInfo>
        {
            int IComparer<ProteinInfo>.Compare(ProteinInfo x, ProteinInfo y)
            {
                return x.Unique_Seq_ID.CompareTo(y.Unique_Seq_ID);
            }
        }
    }
}
