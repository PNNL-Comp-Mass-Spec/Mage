using System.Collections.Generic;

namespace Mage
{
    /// <summary>
    /// Process input rows into key/value store
    /// </summary>
    public class KVSink : BaseModule
    {
        private int mKeyColIdx;
        private int mValColIdx;

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
        public Dictionary<string, string> Values { get; } = new();

        /// <summary>
        /// Handler for ColumnDefAvailable events
        /// </summary>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            mKeyColIdx = InputColumnPos[KeyColumnName];
            mValColIdx = InputColumnPos[ValueColumnName];
        }

        /// <summary>
        /// Handler for DataRowAvailable events
        /// </summary>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable && args.Fields.Length >= mValColIdx)
            {
                Values[args.Fields[mKeyColIdx]] = args.Fields[mValColIdx];
            }
        }
    }
}
