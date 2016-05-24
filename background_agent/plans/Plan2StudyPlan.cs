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

namespace background_agent
{
    public class Plan2StudyPlan : IStudyPlan
    {
        /// <summary>
        /// Number of notifications per day
        /// Only informative
        /// </summary>
        public int GetRepetition()
        {
            return 4;
        }

        /// <summary>
        /// Decides, if notification should be displayed as Toast notification, or not
        /// </summary>
        public bool GetDisplayNotification()
        {
            return true;
        }

        /// <summary>
        /// Gets time of next notification
        /// </summary>
        /// <returns>Time of next notification</returns>
        public TimeSpan GetNextNotification()
        {
            return TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Text displayed in notification
        /// </summary>
        /// <returns></returns>
        public string GetNotificationText()
        {
            return "Study Plan 2 Notification";
        }
    }
}
