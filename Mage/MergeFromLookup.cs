using System;
using System.Collections.Generic;

namespace Mage
{

    /// <summary>
    /// Mage filter module that merges a value from a lookup dictionary
    /// into a specified column in the output using a lookup key
    /// from another specified column in the output
    /// </summary>
    /// <remarks>This filter is used by the Analysis Manager's Phospho_FDR_Aggregator PlugIn</remarks>
    public class MergeFromLookup : ContentFilter
    {
        #region Member Variables

        private int _keyColIdx;
        private int _mergeColIdx;

        #endregion

        #region Properties

        /// <summary>
        /// Key-value lookup for merged value 
        /// </summary>
        public Dictionary<string, string> LookupKV { get; set; }

        /// <summary>
        /// Name of output column that receives the lookup merge value
        /// </summary>
        public string MergeColName { get; set; }

        /// <summary>
        /// Name of output column that contains key value for lookup
        /// </summary>
        public string KeyColName { get; set; }

        /// <summary>
        /// Set this to True if you want an exception to be thrown 
        /// if the LookupKV dictionary does not contain the key for a given data row
        /// </summary>
        /// <remarks>Default is False</remarks>
        public bool ThrowExceptionIfLookupFails { get; set; }

        #endregion

        /// <summary>
        /// Set up column indexes
        /// </summary>
        /// <remarks>This code will be executed after the column definitions have been created</remarks>
        override protected void ColumnDefsFinished()
        {
            if (!OutputColumnPos.TryGetValue(KeyColName, out _keyColIdx))
                throw new Exception("Key column '" + KeyColName + "' not found in the output columns, " + string.Join(",", OutputColumnPos.Keys));

            if (!OutputColumnPos.TryGetValue(MergeColName, out _mergeColIdx))
                throw new Exception("Merge column '" + MergeColName + "' not found in the output columns, " + string.Join(",", OutputColumnPos.Keys));
        }

        /// <summary>
        /// Pass all rows, apply column mapping, and merge lookup value
        /// </summary>
        protected override bool CheckFilter(ref string[] vals)
        {
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(vals);

                string columnValue;
                if (LookupKV.TryGetValue(outRow[_keyColIdx], out columnValue))
                {
                    outRow[_mergeColIdx] = columnValue;
                }
                else
                {
                    if (ThrowExceptionIfLookupFails)
                        throw new Exception("Key '" + outRow[_keyColIdx] + "' not found in the LookupKV dictionary");

                    outRow[_mergeColIdx] = string.Empty;
                }

                vals = outRow;
            }
            return true;
        }
    }

}
