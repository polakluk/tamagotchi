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
    /// <summary>
    /// Model holding data for one item in leader board
    /// </summary>
    public class ModelUserLeaderBoard : BaseBindingModel
    {
        private string m_name;
        private string m_url;
        private int m_score;
        private bool m_isActUser;

        /// <summary>
        /// Username
        /// </summary>
        public string Name
        {
            get
            {
                return this.m_name;
            }
            set
            {
                if (this.m_name == value)
                {
                    return;
                }
                this.m_name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// User score
        /// </summary>
        public int Score
        {
            get
            {
                return this.m_score;
            }
            set
            {
                if (this.m_score == value)
                {
                    return;
                }
                this.m_score = value;
                this.NotifyPropertyChanged("Score");
            }
        }


        /// <summary>
        /// Avatar url
        /// </summary>
        public string Url
        {
            get
            {
                return this.m_url;
            }
            set
            {
                if (this.m_url == value)
                {
                    return;
                }
                this.m_url = value;
                this.NotifyPropertyChanged("Url");
            }
        }

        /// <summary>
        /// If user is this user
        /// </summary>
        public bool IsActUser
        {
            get
            {
                return this.m_isActUser;
            }
            set
            {
                if (this.m_isActUser == value)
                {
                    return;
                }
                this.m_isActUser = value;
                this.NotifyPropertyChanged("IsActUser");
            }
        }
    }
}
