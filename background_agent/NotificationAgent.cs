using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System;
using System.Threading;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;

namespace background_agent
{
    public class NotificationAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public NotificationAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            if (task.Name.Equals("SpacechiAgent", StringComparison.OrdinalIgnoreCase))
            {
                TimeSpan nextNotification;
                bool displayToast = true;
                Mutex mutex = new Mutex(true, "SpacechiAgentData");
                mutex.WaitOne();
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                IStudyPlan plan = this.getStudyPlan();
                if (setting.Contains("notifications.missed") == false)
                {
                    setting["notifications.missed"] = "0";
                }
                displayToast = plan.GetDisplayNotification();
                if (setting.Contains("plan.notifications") && displayToast)
                {
                    displayToast = (bool)setting["plan.notifications"];
                }

                nextNotification = plan.GetNextNotification();
                if (displayToast == false) // no notifications
                {
                    ScheduledActionService.LaunchForTest(task.Name, nextNotification);
                    mutex.ReleaseMutex();
                    NotifyComplete();
                    return;
                }

                int missed = Convert.ToInt32(setting["notifications.missed"]) + 1;
                setting["notifications.missed"] = (missed + 1).ToString();
                setting.Save();
                mutex.ReleaseMutex();

                // display notification only if allowed
                if (displayToast)
                {
                    ShellToast toast = new ShellToast();
                    toast.Title = "Spacechi";
                    toast.Content = plan.GetNotificationText();
                    toast.NavigationUri = new Uri("/StudyingPage.xaml?alg=1&setId=0", UriKind.Relative);
                    toast.Show();
                }

                ShellTile tile = ShellTile.ActiveTiles.First();
                if (tile != null)
                {
                    StandardTileData data = new StandardTileData();

                    data.Title = "Spacechi";
                    data.Count = missed;

                    tile.Update(data);
                }

                ScheduledActionService.LaunchForTest(task.Name, nextNotification);
            }
 
            NotifyComplete();
        }

        private IStudyPlan getStudyPlan()
        {
            IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            if (setting.Contains("plan.name") == false)
            {
                setting["plan.name"] = "Plan1";
            }
            Assembly a = Assembly.Load("background_agent");
            Type[] AssTypes = a.GetTypes();
            string s = (string)setting["plan.name"] + "StudyPlan";
            Type plan = null;
            for (int i = 0; i < AssTypes.Length; i++)
            {
                if (AssTypes[i].Name == s)
                {
                    plan = AssTypes[i];
                    break;
                }
            }

            return Activator.CreateInstance(plan) as IStudyPlan;
        }
    }
}