using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mage {

    /// <summary>
    /// An interface to be applied to GUI parameter panels to allow
    /// parameter values to be set and retrieved via a general access mechanism.
    /// </summary>
    public interface IModuleParameters {

        /// <summary>
        /// get list of parameters
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetParameters();

        /// <summary>
        /// set list of parameters
        /// </summary>
        /// <param name="paramList"></param>
        void SetParameters(Dictionary<string, string> paramList);

    }
}
