using System.Linq;

namespace Mage
{

    /// <summary>
    /// This class is a thin wrapper around a Mage SimpleSink object
    /// and acts as a source module to serve it content
    /// </summary>
    public class SinkWrapper : BaseModule
    {

        private readonly SimpleSink mSink;

        /// <summary>
        /// constructor
        /// </summary>
        public SinkWrapper()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sink"></param>
        public SinkWrapper(SimpleSink sink)
        {
            mSink = sink;
        }

        /// <summary>
        /// Serve contents of SimpleSink object that we are wrapped around
        /// </summary>
        /// <param name="state"></param>
        public override void Run(object state)
        {
            OnColumnDefAvailable(new MageColumnEventArgs(mSink.Columns.ToArray()));
            foreach (var row in mSink.Rows)
            {
                if (Abort)
                    break;
                OnDataRowAvailable(new MageDataEventArgs(row));
            }
            OnDataRowAvailable(new MageDataEventArgs(null));
        }

    }
}
