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
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Model containing information about a study menu item
    /// </summary>
    public class ModelStudyMenuItem : BaseBindingModel
    {
        private String m_name;
        private String m_description;

        /// <summary>
        /// Name of Study menu item
        /// </summary>
        public String Name
        {
            get
            {
                return this.m_name;
            }
            set
            {
                this.m_name = value;
                this.NotifyPropertyChanged("Name");
            }
        }
        

        /// <summary>
        /// Description of Study menu item
        /// </summary>
        public String Description
        {
            get
            {
                return this.m_description;
            }
            set
            {
                this.m_description = value;
                this.NotifyPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Delegate for OnTap event handler of Study menu item
        /// </summary>
        public delegate void OnTap(object sender, System.Windows.Input.GestureEventArgs e);

        /// <summary>
        /// OnTap event handler of Study menu item
        /// </summary>
        public OnTap OnTapHandler;
    }
}
