using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iostamagotchi
{
    /// <summary>
    /// Interface handing set up class for a study plan
    /// </summary>
    public interface IStudyPlan
    {
        /// <summary>
        /// Bonus multiplier
        /// </summary>
        float GetFactor();
    }
}
