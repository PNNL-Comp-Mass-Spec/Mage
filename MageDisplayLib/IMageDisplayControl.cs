using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MageDisplayLib
{

    /// <summary>
    /// How data rows will be served from display list
    /// </summary>
    public enum DisplaySourceMode
    {
        /// <summary>
        /// Serve all rows in display
        /// </summary>
        All,
        /// <summary>
        /// Serve only selected rows in display
        /// </summary>
        Selected
    }

    /// <summary>
    /// Defines common features of Mage list display user controls
    /// </summary>
    public interface IMageDisplayControl
    {

        /// <summary>
        /// Get or set the visible title field for this control
        /// </summary>
        string PageTitle { get; set; }

        /// <summary>
        /// Show or hide the header panel
        /// </summary>
        bool HeaderVisible { get; set; }

        /// <summary>
        /// Number of items currently in display
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Number of selected items currently in display
        /// </summary>
        int SelectedItemCount { get; }

        /// <summary>
        /// Get contents of first selected row as key/value pairs
        /// where key is column name and value is contents of column
        /// </summary>
        Dictionary<string, string> SeletedItemFields { get; }


        /// <summary>
        /// Get collection of columnn names
        /// </summary>
        Collection<string> ColumnNames { get; }

    }

}
