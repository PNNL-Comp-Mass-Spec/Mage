using Mage;

namespace MageFileProcessor
{

    [MageAttribute("Filter", "XCorr", "XCorr Example", "Simple filter for SEQUEST results (XCorr > 2.0)")]
    class XCorrExample : ContentFilter
    {

        // Indexes into the row field data array
        private int xCorrIdx;

        // Precalulate field indexes
        protected override void ColumnDefsFinished()
        {
            // Set up indexes into fields array
            if (!this.InputColumnPos.TryGetValue("XCorr", out xCorrIdx))
                xCorrIdx = -1;
        }

        // This is called for each input data row that is being filtered.
        // the fields array contains value of each column for the row
        protected override bool CheckFilter(ref string[] fields)
        {
            var accepted = false;

            // Convert XCorr from text to number
            // and accept it if it meets minimum value
            double v;
            if (xCorrIdx >= 0)
                double.TryParse(fields[xCorrIdx], out v);
            else
                v = 0;

            if (v > 2.0)
            {
                accepted = true;
            }

            // Apply output column mapping (if active)
            if (accepted && OutputColumnDefs != null)
            {
                fields = MapDataRow(fields);
            }
            return accepted;
        }
    }
}
