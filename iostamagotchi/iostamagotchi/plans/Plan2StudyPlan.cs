using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace iostamagotchi
{
    public class Plan2StudyPlan : IStudyPlan
    {
        /// <summary>
        /// Bonus multiplier
        /// </summary>
        public float GetFactor()
        {
            return 1.7f;
        }
    }
}
