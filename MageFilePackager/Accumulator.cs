using Mage;

namespace MageFilePackager
{

    /// <summary>
    /// A variation on a Mage sink module
    /// that can accumulate rows from multiple pipeline runs
    /// Using initial column definitions
    /// </summary>
    class Accumulator : SimpleSink
    {

        public Accumulator()
        {
            RetainColumnDefs = false;
        }

        /// <summary>
        /// Option to retain existing column definitions
        /// </summary>
        public bool RetainColumnDefs { get; set; }

        /// <summary>
        /// Conditionally retain existing column definitions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            if (!RetainColumnDefs)
            {
                base.HandleColumnDef(sender, args);
            }
        }

    }
}
