using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace iostamagotchi
{
    public partial class StudyMenu : PhoneApplicationPage
    {
        public StudyMenu()
        {
            InitializeComponent();
            this.lbMenu.DataContext = App.StudyMenuViewModel;
        }

        /// <summary>
        /// Method handling tapping on listbox item by asking for its OnTap event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbMenuItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            ModelStudyMenuItem item = panel.DataContext as ModelStudyMenuItem;
            item.OnTapHandler(sender, e);
        }
    }
}
