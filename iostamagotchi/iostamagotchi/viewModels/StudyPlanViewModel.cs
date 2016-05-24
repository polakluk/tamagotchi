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
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace iostamagotchi
{

    /// <summary>
    /// This class handles manipulation with data on Study Plan page
    /// 
    /// Inspired by http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff769510(v=vs.105).aspx
    /// </summary>
    public class StudyPlanViewModel : BaseViewModel
    {
        private ObservableCollection<PlanTable> m_listPlans;
        private ObservableCollection<SetTable> m_listActiveSets;

        /// <summary>
        /// Shared Data context
        /// </summary>
        public CardsDataContext Dc;

        /// <summary>
        /// ID of selected Study Plan
        /// </summary>
        public int StudyPlan
        {
            get
            {
                return App.Settings.GetValueOrDefault<int>("plan.id");
            }
            set
            {
                if (value == -1)
                    return;

                App.Settings.AddOrUpdateValue("plan.id", value);
                this.NotifyPropertyChanged("StudyPlan");
            }
        }

        /// <summary>
        /// Determines, if toast notifications are allowed (option "No" overrides settings from study plan)
        /// </summary>
        public bool AllowNotifications
        {
            get
            {
                return App.Settings.GetValueOrDefault<bool>("plan.notifications");
            }
            set
            {
                App.Settings.AddOrUpdateValue("plan.notifications", value);
                this.NotifyPropertyChanged("AllowNotifications");
            }
        }

        /// <summary>
        /// List of available study plans
        /// </summary>
        public ObservableCollection<PlanTable> ListPlans
        {
            get
            {
                return this.m_listPlans;
            }
            set
            {
                this.m_listPlans = value;
                this.NotifyPropertyChanged("ListPlans");
            }
        }

        /// <summary>
        /// List of actively studied sets
        /// </summary>
        public ObservableCollection<SetTable> ListActiveSets
        {
            get
            {
                return this.m_listActiveSets;
            }
            set
            {
                this.m_listActiveSets = value;
                this.NotifyPropertyChanged("ListActiveSets");
            }
        }

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public StudyPlanViewModel()
        {
            this.ListActiveSets = new ObservableCollection<SetTable>();
            this.ListPlans = new ObservableCollection<PlanTable>();
            this.Dc = null;
        }

        /// <summary>
        /// Opens shared connection
        /// </summary>
        /// <param name="refresh">If it should reopen a new connection when one is already established</param>
        public void OpenConnection(bool refresh)
        {
            if (this.Dc == null)
            {
                this.Dc = new CardsDataContext();
            }
            else
            {
                if (refresh)
                {
                    this.Dc.Dispose();
                    this.Dc = new CardsDataContext();
                }
            }
        }

        /// <summary>
        /// Loads all available study plans
        /// </summary>
        public void LoadStudyPlans()
        {
            this.ListPlans.Clear();
            using (PlansDataContext dc = new PlansDataContext())
            {
                IList<PlanTable> il = (from p in dc.Plans where p.Enabled == true orderby p.Ordering select p).ToList<PlanTable>();
                for (int i = 0; i < il.Count; i++)
                {
                    this.ListPlans.Add(il[i] as PlanTable);
                }
            }
        }

        /// <summary>
        /// Loads all actively studied sets
        /// </summary>
        public void LoadActiveSets()
        {
            this.ListActiveSets.Clear();
            IList<SetTable> il = (from s in this.Dc.Sets where s.IsStudying == true select s).ToList<SetTable>();
            for (int i = 0; i < il.Count; i++)
            {
                (il[i] as SetTable).Health = CommonData.CalculateHealthSet(il[i] as SetTable);
            }
            il = il.OrderByDescending(s => s.Health).ToList<SetTable>();

            for (int i = 0; i < il.Count; i++)
            {
                this.ListActiveSets.Add(il[i] as SetTable);
            }
        }
    }
}
