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
using Microsoft.Phone.Controls;
using System.Collections;

namespace iostamagotchi
{
    /// <summary>
    /// Modified version of ListPicker supporting setting selected items (useful for data binding)
    /// 
    /// Inspired by: http://community.appamundi.com/blogs/andywigley/archive/2012/02/02/how-to-databind-selecteditems-of-the-listpicker-and-recurringdayspicker.aspx
    /// (March 11, 2013)
    /// </summary>
    public class customListPicker : ListPicker
    {
        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        public new IList SelectedItems
        {
            get
            {
                return (IList)GetValue(SelectedItemsProperty);
            }
            set
            {
                base.SetValue(SelectedItemsProperty, value);
            }
        }
    }
}
