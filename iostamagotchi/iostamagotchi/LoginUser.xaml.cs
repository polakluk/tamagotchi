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
    public partial class LoginUserPage : PhoneApplicationPage
    {
        public LoginUserPage()
        {
            InitializeComponent();
        }

        private void phoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            switch (e.Orientation.ToString())
            {
                case "PortraitUp":
                    {
                        VisualStateManager.GoToState(this, "Portrait", true);
                        break;
                    }
                default:
                    {
                        VisualStateManager.GoToState(this, "Landscape", true);
                        break;
                    }
            }
        }
    }
}