using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace MageFilePackager {

    /// <summary>
    /// A variation on a Mage sink module 
    /// that can acccumulate rows from multiple pipeline runs
    /// Using initial column definitions
    /// </summary>
    class Accummulator : SimpleSink {

        public Accummulator() {
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
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            if (!RetainColumnDefs) {
                base.HandleColumnDef(sender, args);
            }
        }

    }
}
