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
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;

namespace iostamagotchi
{
    /// <summary>
    /// ViewModel for loading data for Study menu
    /// </summary>
    public class StudyMenuViewModel : BaseViewModel
    {
        private ObservableCollection<ModelStudyMenuItem> m_menuItems;
        private int m_tappedItem;

        /// <summary>
        /// Shared Data context
        /// </summary>
        public CardsDataContext Dc;

        /// <summary>
        /// Collection of menu items
        /// </summary>
        public ObservableCollection<ModelStudyMenuItem> MenuItems
        {
            get
            {
                return this.m_menuItems;
            }
            set
            {
                this.m_menuItems = value;
                this.NotifyPropertyChanged("MenuItems");
            }
        }

        /// <summary>
        /// Index of currently tapped menu item
        /// </summary>
        public int TappedItem
        {
            get
            {
                return this.m_tappedItem;
            }
            set
            {
                this.m_tappedItem = value;
                this.NotifyPropertyChanged("TappedItem");
            }
        }


        /// <summary>
        /// Loads menu items
        /// </summary>
        public void LoadItems()
        {
            this.MenuItems = new ObservableCollection<ModelStudyMenuItem>();

            this.MenuItems.Add(new ModelStudyMenuItem() { Name = "Study", Description = "Study hard", OnTapHandler = onTapMenuItemStudy });
            this.MenuItems.Add(new ModelStudyMenuItem() { Name = "Practise", Description = "Practise often to get better", OnTapHandler = onTapMenuItemPractise });
            this.MenuItems.Add(new ModelStudyMenuItem() { Name = "Test", Description = "Test often to check your progress", OnTapHandler = onTapMenuItemTest });
        }

        private void onTapMenuItemStudy(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.StudyViewModel.InitParams();
            PhoneApplicationFrame rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(
              new System.Uri("/StudyingPage.xaml?setId=0&alg=1", System.UriKind.Relative));
 
        }

        private void onTapMenuItemPractise(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.StudyViewModel.InitParams();
            PhoneApplicationFrame rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(
              new System.Uri("/StudyingPage.xaml?setId=0&alg=0", System.UriKind.Relative));

        }

        private void onTapMenuItemTest(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.TestViewModel.InitParams();
            PhoneApplicationFrame rootFrame = (App.Current as App).RootFrame;
            rootFrame.Navigate(
              new System.Uri("/TestPage.xaml?setId=0", System.UriKind.Relative));
        }

    }
}
