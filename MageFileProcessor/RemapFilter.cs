using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace MageFileProcessor {

    [MageAttribute("Filter", "RemapFilter", "Simple column remapping filter", "Simple apply output column mapping")]
    class RemapFilter: ContentFilter {

        protected override bool CheckFilter(ref object[] fields) {
            bool accepted = false;

            if (OutputColumnDefs != null) {
                object[] outRow = MapDataRow(fields);
                fields = outRow;
            }
            accepted = true;

            return accepted;
        }

    }
}
