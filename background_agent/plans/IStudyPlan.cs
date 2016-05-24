using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace background_agent
{
    /// <summary>
    /// Interface handing set up class for a study plan
    /// </summary>
    public interface IStudyPlan
    {
        /// <summary>
        /// Number of notifications per day
        /// Only informative
        /// </summary>
        int GetRepetition();

        /// <summary>
        /// Decides, if notification should be displayed as Toast notification, or not
        /// </summary>
        bool GetDisplayNotification();

        /// <summary>
        /// Gets time of next notification
        /// </summary>
        /// <returns>Time of next notification</returns>
        TimeSpan GetNextNotification();

        /// <summary>
        /// Text displayed in notification
        /// </summary>
        /// <returns></returns>
        string GetNotificationText();
    }
}
