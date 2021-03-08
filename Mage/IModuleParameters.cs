using System.Collections.Generic;

namespace Mage
{
    /// <summary>
    /// An interface to be applied to GUI parameter panels to allow
    /// parameter values to be set and retrieved via a general access mechanism.
    /// </summary>
    public interface IModuleParameters
    {
        /// <summary>
        /// Get list of parameters
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetParameters();

        /// <summary>
        /// Set list of parameters
        /// </summary>
        /// <param name="paramList"></param>
        void SetParameters(Dictionary<string, string> paramList);
    }
}
