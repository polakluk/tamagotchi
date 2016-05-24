using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Data.Linq;

namespace iostamagotchi
{
    /// <summary>
    /// Interface implementing everything neccessary for updating DB schema from one version to the next
    /// </summary>
    interface IDbUpdate
    {
        /// <summary>
        /// Updates schema and returns result of this operation
        /// </summary>
        /// <returns>Result of this operation</returns>
        bool PerformUpdate();

        /// <summary>
        /// Returns, if this update contains only data. This is useful for filling db with initial data after installation.
        /// </summary>
        /// <returns></returns>
        bool OnlyData();
    }
}
