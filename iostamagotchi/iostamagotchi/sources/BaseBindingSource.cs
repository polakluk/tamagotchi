using System;
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Base class for source supporting Data binding
    /// </summary>
    abstract public class BaseBindingSource : BaseSource, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
