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
    /// Model containing information about a single flash card during studying or practising
    /// </summary>
    public class ModelFlashCardStudy : BaseBindingModel
    {
        private string m_frontSide;
        private string m_backSide;
        private Boolean m_isVisible;
        private Boolean m_isBackVisible;
        private Boolean m_isHandleVisible;

        /// <summary>
        /// Text on front side of flash card
        /// </summary>
        public string FrontSide
        {
            get
            {
                return this.m_frontSide;
            }
            set
            {
                this.m_frontSide = value;
                this.NotifyPropertyChanged("FrontSide");
            }
        }

        /// <summary>
        /// Text on back side of flash card
        /// </summary>
        public string BackSide
        {
            get
            {
                return this.m_backSide;
            }
            set
            {
                this.m_backSide = value;
                this.NotifyPropertyChanged("BackSide");
            }
        }

        /// <summary>
        /// Determines, if the flash card is visible or not at the moment
        /// </summary>
        public Boolean IsCardVisible
        {
            get
            {
                return this.m_isVisible;
            }
            set
            {
                this.m_isVisible = value;
                this.NotifyPropertyChanged("IsCardVisible");
            }
        }

        /// <summary>
        /// Determines, if back side of the flash card is visible or not at the moment
        /// </summary>
        public Boolean IsBackVisible
        {
            get
            {
                return this.m_isBackVisible;
            }
            set
            {
                this.m_isBackVisible = value;
                this.NotifyPropertyChanged("IsBackVisible");
            }
        }

        /// <summary>
        /// Determines, if handle on the flash card is visible or not at the moment
        /// </summary>
        public Boolean IsHandleVisible
        {
            get
            {
                return this.m_isHandleVisible;
            }
            set
            {
                this.m_isHandleVisible = value;
                this.NotifyPropertyChanged("IsHandleVisible");
            }
        }

        /// <summary>
        /// Current flash card state (during animation)
        /// </summary>
        public eFlashCardStateStudy CardState;

        /// <summary>
        /// Timer handling changing flash card running animation
        /// </summary>
        public StudyDispatcherTimer AnimationTimer;

        /// <summary>
        /// Coordinates of point where touch originated
        /// </summary>
        public Point TouchStart;

        /// <summary>
        /// Coordinates of point where touch ended
        /// </summary>
        public Point TouchEnd;

        /// <summary>
        /// Card ID in database
        /// </summary>
        public int CardId;

        /// <summary>
        /// if the flash card was answered correctly or not
        /// </summary>
        public eFlachCardAnswerState AnswerState;
    }
}
