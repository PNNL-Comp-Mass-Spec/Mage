using System;
using System.Collections.Generic;

namespace Mage
{
    /// <summary>
    /// Processes input rows from standard tabular input
    /// and passes only selected ones to standard tabular output
    /// it is meant to be the base class for subclasses that actually do the filtering
    /// </summary>
    public class ContentFilter : BaseModule
    {
        #region Member Variables

        private int totalRowsCounter;
        private int passedRowsCounter;
        private const int reportRowBlockSize = 1000;

        private const int mMinimumReportIntervalMsec = 500;
        private DateTime mLastReportTimeUTC = DateTime.UtcNow;

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// <para>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </para>
        /// <para>
        /// Let base class process columns for us
        /// and pass the appropriate definitions to our listeners
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            var cd = OutputColumnDefs ?? InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            totalRowsCounter = 0;
            passedRowsCounter = 0;
            ColumnDefsFinished();
        }

        /// <summary>
        /// <para>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </para>
        /// <para>
        /// Check each input row against the filter and pass on the
        /// rows that are accepted
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                // Do filtering here
                var vals = args.Fields;
                if (CheckFilter(ref vals))
                {
                    passedRowsCounter++;
                    OnDataRowAvailable(new MageDataEventArgs(vals));
                }
                // Report progress
                if (++totalRowsCounter % reportRowBlockSize == 0)
                {
                    var msg = "Processed " + totalRowsCounter + " total rows, passed " + passedRowsCounter;
                    if (DateTime.UtcNow.Subtract(mLastReportTimeUTC).TotalMilliseconds >= mMinimumReportIntervalMsec)
                    {
                        OnStatusMessageUpdated(new MageStatusEventArgs(msg));
                        mLastReportTimeUTC = DateTime.UtcNow;
                    }
                }
            }
            else
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        #endregion

        #region Filtering Functions

        /// <summary>
        /// This function should be overriden by subclasses to do the actual filtering
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
        protected virtual bool CheckFilter(ref string[] vals)
        {
            const bool accepted = false;

            return accepted;
        }

        /// <summary>
        /// Called when all column definitions are complete
        /// this function can be overridden by subclasses to set up processing
        /// </summary>
        protected virtual void ColumnDefsFinished()
        {
        }

        /// <summary>
        /// Allows a content filter module to provide a file name conversion
        /// </summary>
        /// <param name="sourceFile">name of input file</param>
        /// <param name="fieldPos">index of field in file metadata to be used for renaming</param>
        /// <param name="fields">file metadata</param>
        /// <returns></returns>
        public virtual string RenameOutputFile(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
        {
            return sourceFile;
        }

        #endregion

    }
}
