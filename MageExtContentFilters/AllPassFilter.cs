﻿using Mage;

namespace MageExtContentFilters
{
    /// <summary>
    /// A filter that passes all rows, but applies any column mapping
    /// </summary>
    /// <remarks>
    /// This class will be auto-discovered by the ModuleDiscovery class in Mage
    /// The list of auto-discovered filters is then used to populate the gridview on form FilterSelectionForm.cs
    /// </remarks>
    [Mage(MageAttribute.FILTER_MODULE, "AllPassFilter", "All Pass", "Pass everything and apply output column mapping")]
    public class AllPassFilter : ContentFilter
    {
        // Ignore Spelling: gridview, Mage

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
