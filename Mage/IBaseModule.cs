using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mage {

    /// <summary>
    /// Interface for Mage pipeline module
    /// </summary>
    public interface IBaseModule : ISinkModule {

        /// <summary>
        /// name of this module in pipeline
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// </summary>
        void Prepare();

		/// <summary>
		/// Called after pipeline completes processing all of the data rows
		/// </summary>
		void PostProcess();

        /// <summary>
        /// called after pipeline run is complete - module can do any special cleanup
        /// </summary>
        void Cleanup();

        /// <summary>
        /// set the given property of the module by key/value pair
        /// </summary>
        /// <param name="key">name of poperty</param>
        /// <param name="val">value to set property to</param>
        void SetPropertyByName(string key, string val);

        /// <summary>
        /// set multiple properties of the module from list of key/value pairs
        /// (propertie must be of type string)
        /// </summary>
        /// <param name="parameters">list of key/value pairs</param>
        void SetParameters(Dictionary<string, string> parameters);
 
        // Standard tabular output stream

        /// <summary>
        /// event that is fired to send row data out via the module's standard tabular output
        /// </summary>
        event EventHandler<MageDataEventArgs> DataRowAvailable;


        /// <summary>
        /// event that is fired to send column definitions out via the module's standard tabular output
        /// </summary>
        event EventHandler<MageColumnEventArgs> ColumnDefAvailable;

        /// <summary>
        /// event that is fired to send a status update message
        /// </summary>
        event EventHandler<MageStatusEventArgs> StatusMessageUpdated;

        /// <summary>
        /// event that is fired to send a warning message
        /// </summary>
        event EventHandler<MageStatusEventArgs> WarningMessageUpdated;

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (for example, module that gets its data froma a database)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        void Run(Object state);

        /// <summary>
        /// stop processing at clean break point
        /// </summary>
        void Cancel();

        /// <summary>
        /// pipeline that contains this module (if any)
        /// </summary>
        ProcessingPipeline Pipeline { get; set; }
    }
}
