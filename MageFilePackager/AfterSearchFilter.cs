using Mage;

namespace MageFilePackager
{
    /// <summary>
    /// Remove unwanted rows from entity file searches
    /// </summary>
    internal class AfterSearchFilter : ContentFilter
    {
        // Indexes into the row field data array
        private int mItemIdx;

        // Precalculate field indexes
        protected override void ColumnDefsFinished()
        {
            // Set up indexes into fields array
            if (!InputColumnPos.TryGetValue("Item", out mItemIdx))
                mItemIdx = -1;
        }

        /// <summary>
        /// Filter each row
        /// </summary>
        /// <param name="values"></param>
        protected override bool CheckFilter(ref string[] values)
        {
            // Reject any "file not found" rows
            if (string.IsNullOrEmpty(values[mItemIdx]))
                return false;

            // Apply field mapping to output
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(values);
                values = outRow;
            }
            return true;
        }
    }
}
