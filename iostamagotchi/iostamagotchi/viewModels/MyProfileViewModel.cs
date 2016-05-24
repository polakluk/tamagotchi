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
    /// This class handles information about player
    /// 
    /// </summary>
    public class MyProfileViewModel : BaseViewModel
    {
        private bool m_logged;
        private bool m_progress;
        private bool m_syncing;
        private ObservableCollection<ModelUserLeaderBoard> m_listLeaderBoard;

        /// <summary>
        /// Determines, if user is logged in
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                return this.m_logged;
            }
            set
            {
                this.m_logged = value;
                this.NotifyPropertyChanged("LoggedIn");
            }
        }

        /// <summary>
        /// Determines, if there is something in progress
        /// </summary>
        public bool Progress
        {
            get
            {
                return this.m_progress;
            }
            set
            {
                this.m_progress = value;
                this.NotifyPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public MyProfileViewModel()
        {
            // Get the settings for this application.
            this.m_logged = false;
            this.m_progress = false;
            this.ListLeaderBoard = new ObservableCollection<ModelUserLeaderBoard>();
        }

        /// <summary>
        /// List users in leaderboard
        /// </summary>
        public ObservableCollection<ModelUserLeaderBoard> ListLeaderBoard
        {
            get
            {
                return this.m_listLeaderBoard;
            }
            set
            {
                this.m_listLeaderBoard = value;
                this.NotifyPropertyChanged("ListLeaderBoard");
            }
        }

        /// <summary>
        /// Determines, if syncing of leaderboard is going on
        /// </summary>
        public bool Syncing
        {
            get
            {
                return this.m_syncing;
            }
            set
            {
                this.m_syncing = value;
                this.NotifyPropertyChanged("Syncing");
            }
        }

        /// <summary>
        /// Load list of leaders
        /// </summary>
        public void LoadLeaderBoard()
        {
            this.ListLeaderBoard.Clear();
            for (int i = 0; i < 5; i++)
            {
                ModelUserLeaderBoard u = new ModelUserLeaderBoard();
                u.IsActUser = i == 2;
                u.Name = "Lukas Polak " + i.ToString();
                u.Score = (10000 - i * 1000);
                u.Url = "sth";
                this.ListLeaderBoard.Add(u);
            }
        }
    }
}
