
namespace Mage
{

    /// <summary>
    /// Interface for object that provides handlers for Sinks
    /// </summary>
    public interface ISinkModule
    {

        // Module receives standard tablular input via these handlers

        /// <summary>
        /// Handler for Mage standard tablular input data rows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void HandleDataRow(object sender, MageDataEventArgs args);

        /// <summary>
        /// Handler for Mage standard tabular input column definition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void HandleColumnDef(object sender, MageColumnEventArgs args);

    }
}
