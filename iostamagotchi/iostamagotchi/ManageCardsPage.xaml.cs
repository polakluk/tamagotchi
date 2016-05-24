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
using System.Windows.Media.Imaging;

namespace iostamagotchi
{
    public partial class ManageCardsPage : PhoneApplicationPage
    {
        private string m_setId;
        public ManageCardsPage()
        {
            InitializeComponent();
            this.m_setId = "";
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
			this.ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["appbar" + this.PivotManageCards.SelectedIndex.ToString()];
        }

        private void tbTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            SetTable t = (tb.Parent as StackPanel).DataContext as SetTable;
			this.NavigationService.Navigate( new Uri( "/ManageCardsPage.xaml?idx=1&setId="+t.SetId.ToString(), UriKind.Relative ) );
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
			string idx = "";

            // load cards, if Set was defined
            App.ManageFlashCardsViewModel.OpenConnection( true );
            App.ManageFlashCardsViewModel.Quizlet.Dispatcher = this.Dispatcher;
            this.LayoutRoot.DataContext = App.ManageFlashCardsViewModel;
            this.piQuizlet.DataContext = App.ManageFlashCardsViewModel.Quizlet;
            this.tbSetName.DataContext = App.ManageFlashCardsViewModel;
            this.lbFlashcards.ItemsSource = App.ManageFlashCardsViewModel.ListCards;
            this.lbSearchResults.ItemsSource = App.ManageFlashCardsViewModel.ListDownloadSets;

            // load correct images
            this.iSearch.Source = new BitmapImage(new System.Uri("/imgs/icons/" + App.CurrentTheme + "/search.png", System.UriKind.Relative));

            App.ManageFlashCardsViewModel.LoadListSets(-1);
            if (this.NavigationContext.QueryString.TryGetValue("setId", out this.m_setId))
            {
                App.ManageFlashCardsViewModel.LoadListCards(Convert.ToInt32(this.m_setId));
                IQueryable<SetTable> qs = (from s in App.ManageFlashCardsViewModel.Dc.Sets where s.SetId == Convert.ToInt32(this.m_setId) select s);
                if (qs.Count<SetTable>() == 0) // if this set does not exist - pretend you're displaying all of them
                {
                    this.PivotManageCards.Title = "My Flashcards";
                    return;
                }
                else
                {
                    SetTable actSet =  qs.Single<SetTable>();
                    this.PivotManageCards.Title = "Set - " + actSet.Title;
                }
            }
            
            // navigate to Flash cards pivot
            if (this.NavigationContext.QueryString.TryGetValue("idx", out idx))
				this.PivotManageCards.SelectedIndex = Convert.ToInt32(idx);
		}
		
        private void AddCat_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ManageCatForm.xaml", UriKind.Relative));
        }

        private void tbTitle_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            SetTable t = tb.DataContext as SetTable;
            this.NavigationService.Navigate(new Uri("/ManageCatForm.xaml?setId=" + t.SetId.ToString(), UriKind.Relative));
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CardTable c = (sender as Grid).DataContext as CardTable;
            this.NavigationService.Navigate(new Uri("/ManageCardForm.xaml?" + "cardId=" + c.CardId.ToString(), UriKind.Relative));
        }

        private void gState_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetTable st = (sender as Grid).DataContext as SetTable;
            st.IsStudying = !st.IsStudying;
            App.ManageFlashCardsViewModel.Dc.SubmitChanges();
        }

        private void gDownloaded_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ModelDownloadSet ds = (sender as Grid).DataContext as ModelDownloadSet;
            App.ManageFlashCardsViewModel.DownloadSet(ds);
        }

        private void gSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.tbSetName.Text.Length > 0) // search only if there's something to look for
            {
                App.ManageFlashCardsViewModel.LoadListDownloadSets(this.tbSetName.Text);
            }
        }

        private void tbSetName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            if (e.Key == Key.Enter)
            {
                if (this.tbSetName.Text.Length > 0) // search only if there's something to look for
                {
                    App.ManageFlashCardsViewModel.LoadListDownloadSets(this.tbSetName.Text);
                }
            }
        }

        private void Practise_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.StudyViewModel.InitParams();
            SetTable st = (sender as TextBlock).DataContext as SetTable;
            this.NavigationService.Navigate(new Uri("/StudyingPage.xaml?alg=0&setId=" + st.SetId.ToString(), UriKind.Relative));
        }

        private void gTest_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.TestViewModel.InitParams();
            SetTable st = (sender as Grid).DataContext as SetTable;
            this.NavigationService.Navigate(new Uri("/TestPage.xaml?" + "setId=" + st.SetId.ToString(), UriKind.Relative));
        }

        private void AddCard_Click(object sender, EventArgs e)
        {
            if (this.m_setId == null)
            {
                this.NavigationService.Navigate(new Uri("/ManageCardForm.xaml?setId=0", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/ManageCardForm.xaml?setId="+this.m_setId, UriKind.Relative));
            }
        }
    }
}
