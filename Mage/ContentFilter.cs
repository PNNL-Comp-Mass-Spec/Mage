using System;
using System.Collections.Generic;

namespace Mage {

    /// <summary>
    /// processes input rows from standard tabular input
    /// and passes only selected ones to standard tabular output
    /// it is meant to be the base clase for subclasses that actually do the filtering
    /// </summary>
    public class ContentFilter : BaseModule {

        #region Member Variables

        private int totalRowsCounter;
        private int passedRowsCounter;
	    private const int reportRowBlockSize = 1000;

	    private const int mMinimumReportIntervalMsec = 500;
	    private DateTime mLastReportTimeUTC = DateTime.UtcNow;

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// 
        /// let base class processess columns for us
        /// and pass the appropriate definitions to our listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            List<MageColumnDef> cd = OutputColumnDefs ?? InputColumnDefs;
            OnColumnDefAvailable(new MageColumnEventArgs(cd.ToArray()));
            totalRowsCounter = 0;
            passedRowsCounter = 0;
            ColumnDefsFinished();
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        ///    
        /// check each input row against the filter and pass on the
        /// rows that are accepted
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// </summary>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
				// do filtering here
				string[] vals = args.Fields;
				if (CheckFilter(ref vals))
				{
					passedRowsCounter++;
					OnDataRowAvailable(new MageDataEventArgs(vals));
				}
                // report progress
                if (++totalRowsCounter % reportRowBlockSize == 0) {
					string msg = "Processed " + totalRowsCounter + " total rows, passed " + passedRowsCounter;
					if (DateTime.UtcNow.Subtract(mLastReportTimeUTC).TotalMilliseconds >= mMinimumReportIntervalMsec)
					{
						OnStatusMessageUpdated(new MageStatusEventArgs(msg));
						mLastReportTimeUTC = DateTime.UtcNow;
					}
                }               
            } else {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        #endregion

        #region Filtering Functions

        /// <summary>
        /// this function should be overriden by subclasses to do the actual filtering
        /// </summary>
        /// <param name="vals"></param>
        /// <returns></returns>
		protected virtual bool CheckFilter(ref string[] vals)
        {
	        const bool accepted = false;

	        return accepted;
        }

	    /// <summary>
        /// called when all column definitions are complete
        /// this function can be overridden by subclasses to set up processing
        /// </summary>
        protected virtual void ColumnDefsFinished() {

        }

        /// <summary>
        /// allows a content filter module to provide a file nane conversion
        /// </summary>
        /// <param name="sourceFile">name of input file</param>
        /// <param name="fieldPos">index of field in file metadata to be used for renaming</param>
        /// <param name="fields">file metada</param>
        /// <returns></returns>
		public virtual string RenameOutputFile(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
		{
            return sourceFile;
        }

        #endregion
    }


}
