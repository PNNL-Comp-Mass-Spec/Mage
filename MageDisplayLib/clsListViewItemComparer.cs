using System;
using System.Collections;
using System.Windows.Forms;

namespace MageDisplayLib
{
    internal class ListViewItemComparer : IComparer
    {
        private readonly int m_SortCol;
        private readonly bool m_SortAscending;
        private readonly bool m_SortNumeric;
        private readonly bool m_SortDate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column"></param>
        /// <param name="sortAscending"></param>
        /// <param name="sortNumeric"></param>
        /// <param name="sortDate"></param>
        public ListViewItemComparer(int column, bool sortAscending, bool sortNumeric, bool sortDate)
        {
            m_SortCol = column;
            m_SortAscending = sortAscending;
            m_SortNumeric = sortNumeric;
            m_SortDate = sortDate;
        }

        // Compares two ListViewItem rows, x, and y
        public int Compare(object x, object y)
        {
            var row1 = x as ListViewItem;
            var row2 = y as ListViewItem;

            if (row1 == null && row2 == null)
                return 0;

            if (row1 == null)
                return -1;

            if (row2 == null)
                return -1;

            var comparisonResult = 0;
            var stringSort = true;

            float val1 = 0;
            float val2 = 0;

            var date1 = DateTime.MinValue;
            var date2 = DateTime.MinValue;

            var item1 = row1.SubItems[m_SortCol].Text;
            var item2 = row2.SubItems[m_SortCol].Text;

            if (m_SortNumeric)
            {
                // User has specified that the two values should be treated as integers
                // Treat an empty cell as a value of 0
                // Otherwise, try to parse as a float; if the parse fails, then auto-treat as strings

                try
                {
                    stringSort = item1.Length > 0 && !float.TryParse(item1, out val1) || item2.Length > 0 && !float.TryParse(item2, out val2);

                    if (!stringSort)
                    {
                        comparisonResult = val1.CompareTo(val2);
                    }
                }
                catch
                {
                    // Conversion or comparison error
                    // Enable string sorting
                    stringSort = true;
                }
            }
            else
            {
                if (m_SortDate)
                {
                    // User has specified that the two values should be treated as dates
                    // Treat an empty cell as a value of 0
                    // Otherwise, try to parse as a date; if the parse fails, then auto-treat as strings

                    try
                    {
                        stringSort = item1.Length > 0 && !DateTime.TryParse(item1, out date1) || item2.Length > 0 && !DateTime.TryParse(item2, out date2);

                        if (!stringSort)
                        {
                            comparisonResult = date1.CompareTo(date2);
                        }
                    }
                    catch
                    {
                        // Conversion or comparison error
                        // Enable string sorting
                        stringSort = true;
                    }
                }
            }

            if (stringSort)
            {
                comparisonResult = string.CompareOrdinal(row1.SubItems[m_SortCol].Text, row2.SubItems[m_SortCol].Text);
            }

            if (m_SortAscending)
                return comparisonResult;

            // Reverse the sort by changing the sign of intComparisonResult
            if (comparisonResult < 0)
                comparisonResult = -comparisonResult;

            return comparisonResult;
        }
    }
}
