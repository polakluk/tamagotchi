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
using System.Windows.Threading;

namespace iostamagotchi
{
    public partial class TestPage : PhoneApplicationPage
    {
        private DispatcherTimer m_timer = null;

        public TestPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.updateGridAnswers(true);
            if (this.m_timer == null)
            {
                this.m_timer = new DispatcherTimer();
                this.m_timer.Tick += this.timerFadeOutPhase;
                this.m_timer.Interval = TimeSpan.FromMilliseconds(2500);
            }

            this.DataContext = App.TestViewModel;

            App.TestViewModel.LoadDataIsolated();

            if (App.TestViewModel.MainCard == null)
            {
                App.TestViewModel.InitializeTestCard();
            }

            string SetId;
            // decide, if we should use algorithm
            if (this.NavigationContext.QueryString.TryGetValue("setId", out SetId))
            {
                App.TestViewModel.GetStudySet(Convert.ToInt32(SetId));
            }
            else
            {
                App.TestViewModel.GetStudySet(App.Settings.GetValueOrDefault<int>("testing.setId"));
            }

            if (App.TestViewModel.TestingOn)
            {
                App.TestViewModel.LoadNextCard();
            }
        }

        private void timerFadeOutPhase(object sender, EventArgs e)
        {
            App.TestViewModel.DisplayCheering = false;
            // unselect all options
            for (int i = 0; i < 4; i++)
            {
                (this.gAnswers.Children[i] as Grid).Background = new SolidColorBrush();
                ((this.gAnswers.Children[i] as Grid).Background as SolidColorBrush).Color = Colors.Transparent;
            }
            this.m_timer.Stop();
            App.TestViewModel.LoadNextCard();
        }

        private void TestingPage_OrientationChanged(object sender, Microsoft.Phone.Controls.OrientationChangedEventArgs e)
        {
            switch (e.Orientation.ToString())
            {
                case "PortraitUp":
                    {
                        this.updateGridAnswers(true);
                        break;
                    }
                default:
                    {
                        this.updateGridAnswers(false);
                        break;
                    }
            }
        }

        private void updateGridAnswers(bool IsPortrait)
        {
            if (IsPortrait)
            {
                this.gAnswers.Height = 640;
                this.gAnswers.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                this.gAnswers.ColumnDefinitions[1].Width = GridLength.Auto;
                this.gAnswers.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                this.gAnswers.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                this.gAnswers.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
                this.gAnswers.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);

                Console.WriteLine(this.tbCheering.ActualHeight.ToString());
                Console.WriteLine(this.tbFront.ActualHeight.ToString());
                this.gAnswer3.SetValue(Grid.RowProperty, 2);
                this.gAnswer3.SetValue(Grid.ColumnProperty, 0);
                this.gAnswer4.SetValue(Grid.RowProperty, 3);
                this.gAnswer4.SetValue(Grid.ColumnProperty, 0);
            }
            else
            {
                this.gAnswers.Height = 352;
                this.gAnswers.ColumnDefinitions[0].Width = new GridLength(0.5, GridUnitType.Star);
                this.gAnswers.ColumnDefinitions[1].Width = new GridLength(0.5, GridUnitType.Star);
                this.gAnswers.RowDefinitions[0].Height = new GridLength(0.5, GridUnitType.Star);
                this.gAnswers.RowDefinitions[1].Height = new GridLength(0.5, GridUnitType.Star);
                this.gAnswers.RowDefinitions[2].Height = new GridLength(0, GridUnitType.Star);
                this.gAnswers.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Star);

                this.gAnswer3.SetValue(Grid.RowProperty, 0);
                this.gAnswer3.SetValue(Grid.ColumnProperty, 1);
                this.gAnswer4.SetValue(Grid.RowProperty, 1);
                this.gAnswer4.SetValue(Grid.ColumnProperty, 1);
            }
        }

        private void gAnswer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (sender as Grid);
            App.TestViewModel.AnswerState = Convert.ToInt32((string)g.Tag) == App.TestViewModel.AnswerIdx ? eFlachCardAnswerState.fcasYes : eFlachCardAnswerState.fcasNo;
            
            if(App.TestViewModel.AnswerState == eFlachCardAnswerState.fcasYes)
            {
                // mark this answer as correct
                g.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                // mark this answer as incorrect and select the correct one
                g.Background = new SolidColorBrush();
                (g.Background as SolidColorBrush).Color = Color.FromArgb(255, 182, 0, 0); //#FFB60000
                (this.gAnswers.Children[App.TestViewModel.AnswerIdx] as Grid).Background = new SolidColorBrush(Colors.Green);
            }
            App.TestViewModel.UpdateCard();
            this.m_timer.Start();
        }
    }
}
