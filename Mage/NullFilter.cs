
namespace Mage
{

    /// <summary>
    /// A filter that passes all rows, but applies any column mapping
    /// </summary>
    public class NullFilter : ContentFilter
    {

        /// <summary>
        /// Pass all rows and apply column mapping
        /// </summary>
		protected override bool CheckFilter(ref string[] vals)
        {
            if (OutputColumnDefs != null)
            {
                string[] outRow = MapDataRow(vals);
                vals = outRow;
            }
            return true;
        }
    }

}
