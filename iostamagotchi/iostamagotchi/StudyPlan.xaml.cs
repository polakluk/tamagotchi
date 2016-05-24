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
    public partial class StudyPlan : PhoneApplicationPage
    {
        public StudyPlan()
        {
            InitializeComponent();
            this.piPlan.DataContext = App.StudyPlanViewModel;
            this.lpProgram.ItemsSource = App.StudyPlanViewModel.ListPlans;
            this.lbActiveSets.ItemsSource = App.StudyPlanViewModel.ListActiveSets;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e); 
            App.StudyPlanViewModel.OpenConnection(false);

            App.StudyPlanViewModel.LoadStudyPlans();
            App.StudyPlanViewModel.LoadActiveSets();

            this.lpProgram.SelectedIndex = App.Settings.GetValueOrDefault<int>("plan.idx");
        }

        private void gState_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SetTable st = ((sender as Grid).Parent as StackPanel).DataContext as SetTable;
            st.IsStudying = !st.IsStudying;
            App.StudyPlanViewModel.Dc.SubmitChanges();
        }

        private void spItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.StudyViewModel.InitParams();
            SetTable st = (sender as StackPanel).DataContext as SetTable;
            this.NavigationService.Navigate(new System.Uri("/StudyingPage.xaml?alg=1&setId=" + st.SetId.ToString(), System.UriKind.Relative));
        }

        private void lpProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lpProgram.SelectedIndex > -1 && this.lpProgram.Items.Count > 0 && e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                App.StudyPlanViewModel.StudyPlan = (this.lpProgram.Items[this.lpProgram.SelectedIndex] as PlanTable).PlanId;
                App.Settings.AddOrUpdateValue("plan.idx", this.lpProgram.SelectedIndex);
                App.Settings.AddOrUpdateValue("plan.name", (this.lpProgram.Items[this.lpProgram.SelectedIndex] as PlanTable).ClassName);
                App.Settings.Save();
            }
        }
		
        private void gTest_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.TestViewModel.InitParams();
            SetTable st = ((sender as TextBlock).Parent as Grid).DataContext as SetTable;
            this.NavigationService.Navigate(new System.Uri("/TestPage.xaml?setId="+st.SetId.ToString(), System.UriKind.Relative));
        }

        private void gPractise_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.StudyViewModel.InitParams();
            SetTable st = ((sender as TextBlock).Parent as Grid).DataContext as SetTable;
            this.NavigationService.Navigate(new System.Uri("/StudyingPage.xaml?alg=0&setId=" + st.SetId.ToString(), System.UriKind.Relative));
        }		
    }
}
