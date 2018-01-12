using Mage;

namespace MageFilePackager
{

    /// <summary>
    /// Remove unwanted rows from entity file searches
    /// </summary>
    class AfterSearchFilter : ContentFilter
    {

        // Indexes into the row field data array
        private int _itemIdx;

        // Precalulate field indexes
        protected override void ColumnDefsFinished()
        {
            // Set up indexes into fields array
            if (!InputColumnPos.TryGetValue("Item", out _itemIdx))
                _itemIdx = -1;
        }


        /// <summary>
        /// Filter each row
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        protected override bool CheckFilter(ref string[] vals)
        {

            // Reject any "file not found" rows
            if (string.IsNullOrEmpty(vals[_itemIdx]))
                return false;

            // Apply field mapping to output
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(vals);
                vals = outRow;
            }
            return true;
        }

    }

}
