using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mage {

    /// <summary>
    /// A filter that passes all rows, but applies any column mapping
    /// </summary>
    public class NullFilter : ContentFilter {

        /// <summary>
        /// Pass all rows and apply column mapping
        /// </summary>
        protected override bool CheckFilter(ref object[] vals) {
            if (OutputColumnDefs != null) {
                object[] outRow = MapDataRow(vals);
                vals = outRow;
            }
            return true;
        }
    }
    
}
