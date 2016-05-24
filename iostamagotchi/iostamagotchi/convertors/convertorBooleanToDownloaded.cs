using System;
using System.Windows.Data;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace iostamagotchi
{

    /// <summary>
    /// A Value converter which takes care of showing correct img next to downloading set
    /// </summary>
    public class convertorBooleanToDownloaded : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool v = (bool)value;
            var bitmap = new BitmapImage(v ? new System.Uri("/imgs/icons/" + App.CurrentTheme + "/check.png", System.UriKind.Relative)
                        : new System.Uri("/imgs/icons/" + App.CurrentTheme + "/download.png", System.UriKind.Relative));
            return bitmap;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = (string)value;
            return v.Contains("check.png");
        }
    }
}
