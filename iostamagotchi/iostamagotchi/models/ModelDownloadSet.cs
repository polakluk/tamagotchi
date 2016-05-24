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
    public class ModelDownloadSet : BaseBindingModel
    {
        private string m_title;
        private bool m_downloaded;
        private int m_cards;
        private bool m_isDownloading;

        /// <summary>
        /// Title of extern set
        /// </summary>
        public string Title
        {
            get
            {
                return this.m_title;
            }
            set
            {
                this.m_title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Determines if the set was already downloaded
        /// </summary>
        public bool IsDownloaded
        {
            get
            {
                return this.m_downloaded;
            }
            set
            {
                this.m_downloaded = value;
                this.NotifyPropertyChanged("IsDownloaded");
            }
        }

        /// <summary>
        /// Determines if the set is being downloaded
        /// </summary>
        public bool IsDownloading
        {
            get
            {
                return this.m_isDownloading;
            }
            set
            {
                this.m_isDownloading = value;
                this.NotifyPropertyChanged("IsDownloading");
            }
        }

        /// <summary>
        /// Number of cards in set
        /// </summary>
        public int Cards
        {
            get
            {
                return this.m_cards;
            }
            set
            {
                this.m_cards = value;
                this.NotifyPropertyChanged("Cards");
            }
        }

        /// <summary>
        /// Class name of source class
        /// </summary>
        public string ClassName;

        /// <summary>
        /// Url to download the set
        /// </summary>
        public string Url;

        /// <summary>
        /// Remoted ID
        /// </summary>
        public int RemoteId;
    }
 }
