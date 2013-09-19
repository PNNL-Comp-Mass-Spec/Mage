using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace Mage {


    /// <summary>
    /// Process input rows into key/value store
    /// </summary>
    public class KVSink : BaseModule {

        #region Member Variables

        private Dictionary<string, string> mKV = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Name of input column that contains key
        /// </summary>
        public string KeyColumnName { get; set; }


        /// <summary>
        /// Name of input column that contains value
        /// </summary>
        public string ValueColumnName { get; set; }


        /// <summary>
        /// Get accumulated key/value store
        /// </summary>
        public Dictionary<string, string> Values { get { return mKV; } }

        #endregion

        private int mKeyColIdx;
        private int mValColIdx;

		/// <summary>
		/// Handler for ColumnDefAvailable events
		/// </summary>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            mKeyColIdx = InputColumnPos[KeyColumnName];
            mValColIdx = InputColumnPos[ValueColumnName];
        }

		/// <summary>
		/// Handler for DataRowAvailable events
		/// </summary>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                if (args.Fields.Length >= mValColIdx) {
                    mKV[args.Fields[mKeyColIdx]] = args.Fields[mValColIdx];
                }
            }
        }

    }
}
