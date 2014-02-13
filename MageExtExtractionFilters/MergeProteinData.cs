using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MageExtContentFilters;
using Mage;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters {

	/// <summary>
	/// This class handles merging protein data into the result data
	/// </summary>
	class MergeProteinData {

		public enum MergeModeConstants {
			XTandem = 0,
			Inspect = 1
		}

		#region Member Variables

		private MergeModeConstants mMergeMode;

		// reference to row in protein buffer, keyed by result id
		private Dictionary<string, int> mProtDataLookup = null;

		// indexes for the protein columns in the result
		private int ODX_Cleavage_State = -1;
		private int ODX_Terminus_State = -1;
		private int ODX_Protein_Name = -1;
		private int ODX_Protein_Expectation_Value_Log = -1;
		private int ODX_Protein_Intensity_Log = -1;

		// index for the result column that contains 
		// the key to use to look up the protein
		private int IDX_Lookup_Col = -1;
		public int LookupColumn {
			get { return IDX_Lookup_Col; }
			set { IDX_Lookup_Col = value; }
		}

		#endregion

		#region Properties

		public MergeModeConstants MergeMode {
			get {
				return mMergeMode;
			}
		}

		#endregion

		// Constructor
		public MergeProteinData(MergeModeConstants eMergeMode) {
			mMergeMode = eMergeMode;
		}

		// precalculate field indexes in protein buffer
		// (to save time later)
		public void SetMergeSourceColIndexes(Dictionary<string, int> colPos) {
			ODX_Cleavage_State = colPos["Cleavage_State"];
			ODX_Terminus_State = colPos["Terminus_State"];

			if (mMergeMode == MergeModeConstants.XTandem) {
				ODX_Protein_Name = colPos["Protein_Name"];
				ODX_Protein_Expectation_Value_Log = colPos["Protein_Expectation_Value_Log(e)"];
				ODX_Protein_Intensity_Log = colPos["Protein_Intensity_Log(I)"];
			}

			if (mMergeMode == MergeModeConstants.Inspect) {
				ODX_Protein_Name = colPos["Protein"];               // The Protein column is present in the original _msgfdb_syn.txt file; we are replacing the protein name listed with the name from the msgfdb_syn_SeqToProteinMap.txt file
			}

		}

		// lookup index relating Unique_Seq_ID 
		// to index of first occurrence row in proteinData
		Dictionary<int, int> mFirstOccurrenceIndex = null;

		// protein data buffer
		System.Collections.Generic.List<ProteinInfo> mProteinDataSorted = new System.Collections.Generic.List<ProteinInfo>();

		/// <summary>
		/// Look up first protein identified for result 
		/// and merge protein columns into result row
		/// </summary>
		/// <param name="outRow"></param>
		public void MergeFirstProtein(ref string[] outRow)
		{
			string resultID = outRow[IDX_Lookup_Col];
			int sequenceID;
			int rowIdx;

			outRow[ODX_Cleavage_State] = string.Empty;
			outRow[ODX_Terminus_State] = string.Empty;
			outRow[ODX_Protein_Name] = string.Empty;

			if (!mProtDataLookup.TryGetValue(resultID, out sequenceID)) {
				// ToDo: Report a warning
				sequenceID = -1;
			} else {
				if (mFirstOccurrenceIndex.TryGetValue(sequenceID, out rowIdx)) {
					outRow[ODX_Cleavage_State] = mProteinDataSorted[rowIdx].Cleavage_State.ToString();
					outRow[ODX_Terminus_State] = mProteinDataSorted[rowIdx].Terminus_State.ToString();
					outRow[ODX_Protein_Name] = mProteinDataSorted[rowIdx].Protein_Name;

					if (ODX_Protein_Expectation_Value_Log > -1)
						outRow[ODX_Protein_Expectation_Value_Log] = mProteinDataSorted[rowIdx].Protein_Expectation_Value_LogE;
					if (ODX_Protein_Intensity_Log > -1)
						outRow[ODX_Protein_Intensity_Log] = mProteinDataSorted[rowIdx].Protein_Intensity_LogI;

				}
			}
		}

		/// <summary>
		/// Look up single protein or list of proteins for result.
		/// If single protein, modify outRow and return null list.
		/// If multiple proteins, return in list.
		/// </summary>
		/// <param name="outRow"></param>
		/// <param name="matchFound">Output parameter: True if the ResultID and SequenceID are present in the cached data</param>
		/// <returns></returns>
		public Collection<string[]> MergeAllProteins(ref string[] outRow, out bool matchFound)
		{
			Collection<string[]> outRows = null;
			string resultID = outRow[IDX_Lookup_Col];
			int sequenceID;
			int rowIdx;
			matchFound = false;

			if (!mProtDataLookup.TryGetValue(resultID, out sequenceID)) {
				return outRows;
			}

			if (!mFirstOccurrenceIndex.TryGetValue(sequenceID, out rowIdx)) {
				return outRows;
			}

			matchFound = true;

			int numberOfProteins = 0;
			// starting at first protein, merge protein fields into outRow
			// and then walk down the protein list checking the sequence ID.
			// If single protein, outRow has been updated, and no futher action taken.
			// If more than one protein for current sequence, create list,
			// merge protein data into new copy of result, and add merged
			// result to list
			for (int rIdx = rowIdx; rIdx < mProteinDataSorted.Count; rIdx++) {
				if (mProteinDataSorted[rIdx].Unique_Seq_ID == sequenceID) {
					numberOfProteins++;
					if (numberOfProteins == 2) {
						outRows = new Collection<string[]>();
					}
					if (numberOfProteins >= 2) {
						string[] row = new string[outRow.Length];
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
				} else {
					if (numberOfProteins >= 2) {
						string[] row = new string[outRow.Length];
						Array.Copy(outRow, row, outRow.Length);
						outRows.Add(row);
						////                           Console.WriteLine("result_id:{0}, sequence_id:{1}, row:{2}, count:{3}", resultID, sequenceID, rIdx, outRows.Count);
					}
					break;
				}
			}
			return outRows;
		}

		// FUTURE: check for missing file names before loading file.
		public void GetProteinLookupData(ResultType.MergeFile mapMergeFile, ResultType.MergeFile protMergeFile, string resultFolderPath) {
			// get result to protein mapping data
			string mapFileName = mapMergeFile.MergeFileName;
			SimpleSink resultToSequenceMap = new SimpleSink();
			if (!string.IsNullOrEmpty(mapFileName)) {
				DelimitedFileReader mapFileReader = new DelimitedFileReader();
				mapFileReader.FilePath = System.IO.Path.Combine(resultFolderPath, mapFileName);
				ProcessingPipeline.Assemble("Lookup Protein Map", mapFileReader, resultToSequenceMap).RunRoot(null);
			}

			// get protein data
			SimpleSink mProteinData = new SimpleSink();

			string protFileName = protMergeFile.MergeFileName;
			if (!string.IsNullOrEmpty(protFileName)) {
				DelimitedFileReader protFileReader = new DelimitedFileReader();
				protFileReader.FilePath = System.IO.Path.Combine(resultFolderPath, protFileName);
				ProcessingPipeline.Assemble("Lookup Protein Data", protFileReader, mProteinData).RunRoot(null);
			}

			int colIndexUniqueSeqID;
			int colIndexCleavageState;
			int colIndexTerminusState;
			int colIndexProteinName;
			int colIndexEValue;
			int colIndexProteinIntensity;

			if (!mProteinData.ColumnIndex.TryGetValue("Unique_Seq_ID", out colIndexUniqueSeqID))
				colIndexUniqueSeqID = -1;
			if (!mProteinData.ColumnIndex.TryGetValue("Cleavage_State", out colIndexCleavageState))
				colIndexCleavageState = -1;
			if (!mProteinData.ColumnIndex.TryGetValue("Terminus_State", out colIndexTerminusState))
				colIndexTerminusState = -1;
			if (!mProteinData.ColumnIndex.TryGetValue("Protein_Name", out colIndexProteinName))
				colIndexProteinName = -1;
			if (!mProteinData.ColumnIndex.TryGetValue("Protein_Expectation_Value_Log(e)", out colIndexEValue))
				colIndexEValue = -1;
			if (!mProteinData.ColumnIndex.TryGetValue("Protein_Intensity_Log(I)", out colIndexProteinIntensity))
				colIndexProteinIntensity = -1;

			// Extract the protein data from the SimpleSink so that we can sort it
			mProteinDataSorted.Clear();
			for (int rowIdx = 0; rowIdx < mProteinData.Rows.Count; rowIdx++) {
				int iValue;
				string sValue = string.Empty;

				bool bValidProteinInfo = false;
				ProteinInfo oProteinInfo = new ProteinInfo();

				if (mProteinData.TryGetValueViaColumnIndex(colIndexUniqueSeqID, rowIdx, out iValue))
				{
					bValidProteinInfo = true;
					oProteinInfo.Unique_Seq_ID = iValue;
				}

				if (bValidProteinInfo) {
					if (mProteinData.TryGetValueViaColumnIndex(colIndexCleavageState, rowIdx, out iValue))
						oProteinInfo.Cleavage_State = iValue;

					if (mProteinData.TryGetValueViaColumnIndex(colIndexTerminusState, rowIdx, out iValue))
						oProteinInfo.Terminus_State = iValue;

					if (mProteinData.TryGetValueViaColumnIndex(colIndexProteinName, rowIdx, out sValue))
						oProteinInfo.Protein_Name = sValue;

					if (mProteinData.TryGetValueViaColumnIndex(colIndexEValue, rowIdx, out sValue))
						oProteinInfo.Protein_Expectation_Value_LogE = sValue;

					if (mProteinData.TryGetValueViaColumnIndex(colIndexProteinIntensity, rowIdx, out sValue))
						oProteinInfo.Protein_Intensity_LogI = sValue;

					mProteinDataSorted.Add(oProteinInfo);
				}
			}

			mProteinDataSorted.Sort(new ProteinDataSorter());

			// index relating Unique_Seq_ID to index of first occurrence row in proteinData
			mFirstOccurrenceIndex = new Dictionary<int, int>(mProteinDataSorted.Count);
			int currentSeqID = 0;
			for (int rowIdx = 0; rowIdx < mProteinDataSorted.Count; rowIdx++) {
				if (mProteinDataSorted[rowIdx].Unique_Seq_ID > currentSeqID) {
					mFirstOccurrenceIndex[mProteinDataSorted[rowIdx].Unique_Seq_ID] = rowIdx;
					currentSeqID = mProteinDataSorted[rowIdx].Unique_Seq_ID;
				}
			}

			// index for Result_ID to Unique_Seq_ID map
			// FUTURE: look up column index based on column header
			mProtDataLookup = new Dictionary<string, int>();
			int sID = 0;
			foreach (string[] row in resultToSequenceMap.Rows)
			{
				if (int.TryParse(row[1], out sID)) 
				{
					mProtDataLookup[row[0]] = sID;
				}
			}
		}

		protected class ProteinInfo {

			public int Unique_Seq_ID { get; set; }
			public int Cleavage_State { get; set; }
			public int Terminus_State { get; set; }
			public string Protein_Name { get; set; }
			public string Protein_Expectation_Value_LogE { get; set; }
			public string Protein_Intensity_LogI { get; set; }

			public ProteinInfo() {
				Unique_Seq_ID = -1;
				Cleavage_State = 0;
				Terminus_State = 0;
				Protein_Name = string.Empty;
				Protein_Expectation_Value_LogE = string.Empty;
				Protein_Intensity_LogI = string.Empty;
			}
		}

		protected class ProteinDataSorter : IComparer<ProteinInfo> {

			int IComparer<ProteinInfo>.Compare(ProteinInfo x, ProteinInfo y) {
				return x.Unique_Seq_ID.CompareTo(y.Unique_Seq_ID);
			}
		}
	}

}
