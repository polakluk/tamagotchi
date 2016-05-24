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
using Microsoft.Phone.Scheduler;

namespace iostamagotchi
{
     static public class Agent
    {
        public static string AgentName = "SpacechiAgent";

        public static void InitAgent()
        {
            PeriodicTask agent = new PeriodicTask(Agent.AgentName);
            agent.Description = "This tasks reminds you of studying in application Spacechi on regular basis";
            try
            {
                ScheduledActionService.Add(agent);
                ScheduledActionService.LaunchForTest(Agent.AgentName, TimeSpan.FromSeconds(10));
            }
            catch (InvalidOperationException)
            {
            }
            catch (SchedulerServiceException)
            {

            } 
        }

        public static void StopAgent()
        {
            try
            {
                ScheduledActionService.Remove(Agent.AgentName);
            }
            catch (InvalidOperationException)
            {
            }
            catch (SchedulerServiceException)
            {

            }
        }
    }
}
