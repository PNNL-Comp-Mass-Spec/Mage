using System;

namespace Mage
{

    /// <summary>
    /// This class contains a canonical Mage command that can
    /// be sent between application components
    /// </summary>
    public class MageCommandEventArgs : EventArgs
    {

        /// <summary>
        /// Name of the action that the command is calling for
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// An optional modifier for the command
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// A optional mage pipeline that can perform the command
        /// or part of it
        /// </summary>
        public ProcessingPipeline Pipeline { get; set; }

        /// <summary>
        /// contstruct an empty commant event arg object
        /// </summary>
        public MageCommandEventArgs()
        {
            Action = "";
            Mode = "";
            Pipeline = null;
        }

        /// <summary>
        /// contstruct a commant event arg object with given action name
        /// </summary>
        /// <param name="action">name of the command acion</param>
        public MageCommandEventArgs(string action)
        {
            Action = action;
            Mode = "";
            Pipeline = null;
        }

        /// <summary>
        /// contstruct a commant event arg object with given action name and mode
        /// </summary>
        /// <param name="action">name of the command acion</param>
        /// <param name="mode">modifier for command action</param>
        public MageCommandEventArgs(string action, string mode)
        {
            Action = action;
            Mode = mode;
            Pipeline = null;
        }

        /// <summary>
        /// contstruct a commant event arg object with given action name, mode, and Mage pipeline to process
        /// </summary>
        /// <param name="action">name of the command acion</param>
        /// <param name="mode">modifier for command action</param>
        /// <param name="pipeline">Mage pipeline object that can be run to execute command action</param>
        public MageCommandEventArgs(string action, string mode, ProcessingPipeline pipeline)
        {
            Action = action;
            Mode = mode;
            Pipeline = pipeline;
        }
    }
}
