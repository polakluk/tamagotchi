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
    public partial class ManageCatForm : PhoneApplicationPage
    {
        public ManageCatForm()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.ManageFlashCardsViewModel.OpenConnection( false );
            // load cards, if Set was defined
            string SetId;
            if (this.NavigationContext.QueryString.TryGetValue("setId", out SetId))
            {
                this.prepareEditForm(Convert.ToInt32(SetId), true);
            }
            else
            {
                this.prepareNewForm();
            }
            this.ContentPanel.DataContext = App.ManageFlashCardsViewModel.Set;
            this.toggleEditable(App.ManageFlashCardsViewModel.Set.IsEditable == 1);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void prepareNewForm()
        {
            this.tbCategoryName.IsReadOnly = false;
            this.ApplicationTitle.Text = "Flash Card Set - New Set";
            App.ManageFlashCardsViewModel.Set = new SetTable();
            App.ManageFlashCardsViewModel.Set.Source = App.ManageFlashCardsViewModel.Dc.Sources.Single(s => s.ClassName == "Local");
            this.pTitle.Text = "New Set";
            this.tbCategoryName.Text = "";
            this.bSave.Content = "Add";
            this.bDelete.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Prepares form for editation
        /// </summary>
        /// <param name="Id">Set ID</param>
        /// <param name="isNew">If the set was saved just now</param>
        private void prepareEditForm(int Id, bool isNew)
        {
            if (isNew)
            {
                IQueryable<SetTable> qs = from s in App.ManageFlashCardsViewModel.Dc.Sets where s.SetId == Id select s;
                if (qs.Count<SetTable>() == 0) // no set found => go to creating new one
                {
                    this.prepareNewForm();
                    return;
                }
                App.ManageFlashCardsViewModel.Set = qs.Single<SetTable>();
            }
            this.pTitle.Text = App.ManageFlashCardsViewModel.Set.Title;
            this.ApplicationTitle.Text = "Flash Card Set - Edit Set";
            this.bSave.Content = "Save";
            this.bDelete.Visibility = Visibility.Visible;
        }

        private void bSave_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!this.validateForm()) // validate form
            {
                return;
            }

            try
            {
                if (App.ManageFlashCardsViewModel.Set.SetId == 0)
                {
                    App.ManageFlashCardsViewModel.Dc.Sets.InsertOnSubmit(App.ManageFlashCardsViewModel.Set);
                }
                App.ManageFlashCardsViewModel.Dc.SubmitChanges();
                MessageBox.Show("Saved");
                this.prepareEditForm(App.ManageFlashCardsViewModel.Set.SetId, false);
            }
            catch
            {
                MessageBox.Show("Saving failed!");
            }
        }

        private void bDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                SetTable st = (from s in App.ManageFlashCardsViewModel.Dc.Sets where s.SetId == App.ManageFlashCardsViewModel.Set.SetId select s).Single<SetTable>();
                int c = st.CardCount;
                App.ManageFlashCardsViewModel.Dc.Cards.DeleteAllOnSubmit<CardTable>(st.ListCards);
                App.ManageFlashCardsViewModel.Dc.Sets.DeleteOnSubmit(st);
                App.ManageFlashCardsViewModel.Dc.SubmitChanges();
                App.ManageFlashCardsViewModel.CheckSoughtSets();
                MessageBox.Show("Deleted");
                this.prepareNewForm();
            }
            catch
            {
                MessageBox.Show("Deleting failed");
            }
        }

        private bool validateForm()
        {
            if (this.tbCategoryName.Text == null)
            {
                MessageBox.Show("Please enter name of Set");
                this.tbCategoryName.Focus();
                return false;
            }
            return true;
        }

        private void toggleEditable(bool editable)
        {
            this.tbCategoryName.IsReadOnly = !editable && App.ManageFlashCardsViewModel.Set.SetId > 0;
            if (editable)
            {
                this.bSave.Visibility = Visibility.Visible;
            }
            else
            {
                this.bSave.Visibility = Visibility.Collapsed;
            }
        }
    }
}