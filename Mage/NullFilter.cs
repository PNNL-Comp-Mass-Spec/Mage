
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
        protected override bool CheckFilter(ref string[] values)
        {
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(values);
                values = outRow;
            }
            return true;
        }
    }
}
