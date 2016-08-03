using System;
using System.Collections.ObjectModel;

namespace Mage
{

    /// <summary>
    /// This module provides simple pass through of standard tabular input
    /// to standard tabular output and provides the ability to termainate
    /// the pipeline when a present number of rows is reached.
    /// </summary>
    public class Terminator : BaseModule
    {

        #region Member Variables

        int mRowsProcessed;

        #endregion

        #region Properties

        /// <summary>
        /// Number of rows to accumulate in internal row buffer
        /// </summary>
        public int RowsToSave { get; set; }

        /// <summary>
        /// Get the column definitions
        /// </summary>
        public Collection<MageColumnDef> Columns { get { return new Collection<MageColumnDef>(InputColumnDefs); } }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a terminator module with no limit on input rows
        /// </summary>
        public Terminator()
        {
            RowsToSave = Int32.MaxValue;
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            // nothing to do here
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            OnColumnDefAvailable(args);
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (!Abort)
            {
                if (mRowsProcessed++ < RowsToSave)
                {
                    OnDataRowAvailable(args);
                }
                else
                {
                    OnDataRowAvailable(new MageDataEventArgs(null));
                    CancelPipeline();
                }
            }
        }

        #endregion

    }

}
