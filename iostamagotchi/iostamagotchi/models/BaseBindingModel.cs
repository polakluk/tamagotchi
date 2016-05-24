using System;
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Base Binding Model for XAML models
    /// 
    /// Inspired by: http://www.a2zdotnet.com/View.aspx?Id=172#.UT0o7RyG0TV
    /// </summary>
    public abstract class BaseBindingModel : BaseModel, INotifyPropertyChanged
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
