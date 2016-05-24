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
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.IO;

namespace iostamagotchi
{
    public partial class ManageCardForm : PhoneApplicationPage
    {
        private int m_setId;

        public ManageCardForm()
        {
            InitializeComponent();
            this.ContentPanel.DataContext = App.ManageFlashCardsViewModel;
            this.lpSet.ItemsSource = App.ManageFlashCardsViewModel.ListSets;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string CardId;
            string Sid;

            App.ManageFlashCardsViewModel.OpenConnection( false );
            switch (e.NavigationMode)
            {
                case NavigationMode.Back: // read SetId from Isolated Storage when returning from FullScreen List Picker
                    {
                        this.m_setId = -1;
                        if (App.TmpData.ContainsKey("ManageCardForm.SetIdx"))
                        {
                            this.lpSet.SelectedIndex = (int)(App.TmpData["ManageCardForm.SetIdx"]);
                        }
                        break;
                    }
                default:
                    {
                        if (this.NavigationContext.QueryString.TryGetValue("setId", out Sid))
                        {
                            this.m_setId = Convert.ToInt32(Sid);
                        }
                        else
                        {
                            this.m_setId = 0;
                        }

                        App.ManageFlashCardsViewModel.LoadListSets(1);
                        // load card, if defined
                        if (this.NavigationContext.QueryString.TryGetValue("cardId", out CardId))
                        {
                            this.prepareEditForm(Convert.ToInt32(CardId), true);
                        }
                        else
                        {
                            this.prepareNewForm();
                        }
                        if (this.m_setId > 0)
                        {
                            this.lpSet.SelectedIndex = CommonData.FindIdxBySetId(this.m_setId);
                        }
                        else
                        {
                            if (this.m_setId == 0)
                            {
                                this.lpSet.SelectedIndex = CommonData.FindIdxBySetId((int)App.ManageFlashCardsViewModel.Card.SetId);
                            }
                        }

                        break;
                    }
            }
            // check, if there are any sets -> if not, dont allow saving
            this.bSave.Visibility = this.lpSet.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            this.toggleEditable(App.ManageFlashCardsViewModel.Card.IsEditable == 1 || App.ManageFlashCardsViewModel.Card.CardId == 0);
        }

        private void prepareNewForm()
        {
            this.pTitle.Text = "New Card";
            this.ApplicationTitle.Text = "FLASH CARDS - New Card";
            App.ManageFlashCardsViewModel.Card = new CardTable();
            App.ManageFlashCardsViewModel.Card.Set = (from s in App.ManageFlashCardsViewModel.Dc.Sets where s.SetId == this.m_setId select s).FirstOrDefault<SetTable>();

            this.bDelete.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Prepares form for editation
        /// </summary>
        /// <param name="Id">Card ID</param>
        /// <param name="isNew">If the card was saved just now</param>
        private void prepareEditForm(int Id, bool isNew)
        {
            if (isNew)
            {
                IQueryable<CardTable> qc = from c in App.ManageFlashCardsViewModel.Dc.Cards where c.CardId == Id select c;
                if (qc.Count<CardTable>() == 0) // no card found => go to creating new one
                {
                    this.prepareNewForm();
                    return;
                }
                App.ManageFlashCardsViewModel.Card = qc.Single<CardTable>();
            }
            this.m_setId = (int)App.ManageFlashCardsViewModel.Card.SetId;
            this.lpSet.SelectedIndex = CommonData.FindIdxBySetId((int)App.ManageFlashCardsViewModel.Card.SetId);
            if (App.ManageFlashCardsViewModel.Card.IsEditable == 1)
            {
                this.ApplicationTitle.Text = "Edit Card - " + App.ManageFlashCardsViewModel.Card.FrontSide;
                this.pTitle.Text = "Edit Card";
            }
            else // only viewing card
            {
                this.ApplicationTitle.Text = "View Card - " + App.ManageFlashCardsViewModel.Card.FrontSide;
                this.pTitle.Text = "View Card";
            }
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
                App.ManageFlashCardsViewModel.Dc.Sets.Single(s => s.SetId == (this.lpSet.SelectedItem as SetTable).SetId).ListCards.Add(App.ManageFlashCardsViewModel.Card);
                if (App.ManageFlashCardsViewModel.Card.CardId == 0)
                {
                    App.ManageFlashCardsViewModel.Card.LastStudied = DateTime.Now;
                    App.ManageFlashCardsViewModel.Card.IsEditable = 1;
                    App.ManageFlashCardsViewModel.Card.MaxRank = 0;
                    App.ManageFlashCardsViewModel.Card.ActRank = 0;
                    App.ManageFlashCardsViewModel.Dc.Cards.InsertOnSubmit(App.ManageFlashCardsViewModel.Card);
                }
                App.ManageFlashCardsViewModel.Dc.SubmitChanges();
                MessageBox.Show("Saved");
                this.prepareEditForm(App.ManageFlashCardsViewModel.Card.CardId, false);
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
                CardTable ct = App.ManageFlashCardsViewModel.Dc.Cards.Single(c => c.CardId == App.ManageFlashCardsViewModel.Card.CardId);
                App.ManageFlashCardsViewModel.Dc.Cards.DeleteOnSubmit(ct);
                App.ManageFlashCardsViewModel.Dc.SubmitChanges();
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
            if (this.tbFrontSide.Text == "")
            {
                MessageBox.Show("Please enter front side of flash card");
                this.tbFrontSide.Focus();
                return false;
            }

            if (this.tbBackSide.Text == "")
            {
                MessageBox.Show("Please enter back side of flash card");
                this.tbBackSide.Focus();
                return false;
            }

            if (this.lpSet.SelectedIndex < 0)
            {
                MessageBox.Show("Please set of flash card");
                return false;
            }
            return true;
        }

        private void lpSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lpSet.Items.Count > 0 && this.lpSet.SelectedIndex > -1)
            {
                App.TmpData["ManageCardForm.SetIdx"] = this.lpSet.SelectedIndex;
            }
        }

        private void toggleEditable(bool editable)
        {
            this.tbBackSide.IsReadOnly = !editable;
            this.tbFrontSide.IsReadOnly = !editable;
            this.lpSet.IsEnabled = editable;
            if (editable)
            {
                this.bSave.Visibility = Visibility.Visible;
                if (App.ManageFlashCardsViewModel.Card.CardId != 0)
                {
                    this.bDelete.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.bSave.Visibility = Visibility.Collapsed;
                this.bDelete.Visibility = Visibility.Collapsed;
            }
        }
    }
}
