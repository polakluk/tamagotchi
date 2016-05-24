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
using System.Linq;
using System.Collections.Generic;

namespace iostamagotchi
{
    /// <summary>
    /// ViewModel containing information about all flash cards and sets in application
    /// </summary>
    public class ManageFlashCardsViewModel : BaseViewModel
    {
        private ObservableCollection<SetTable> m_listSets;
        private ObservableCollection<CardTable> m_listCards;
        private ObservableCollection<ModelDownloadSet> m_listDownloadSets;
        private SetTable m_set;
        private CardTable m_card;

        /// <summary>
        /// Quizlet source
        /// </summary>
        public QuizletSource Quizlet;

        /// <summary>
        /// Shared Data context
        /// </summary>
        public CardsDataContext Dc;

        /// <summary>
        /// List of all available sets
        /// </summary>
        public ObservableCollection<SetTable> ListSets
        {
            get
            {
                return this.m_listSets;
            }
            set
            {
                if (this.m_listSets != null)
                {
                    this.m_listSets.Clear();
                }

                this.m_listSets = value;
                this.NotifyPropertyChanged("ListSets");
            }
        }

        /// <summary>
        /// List of cards
        /// </summary>
        public ObservableCollection<CardTable> ListCards
        {
            get
            {
                return this.m_listCards;
            }
            set
            {
                if (this.m_listCards != null)
                {
                    this.m_listCards.Clear();
                }
                this.m_listCards = value;
                this.NotifyPropertyChanged("ListCards");
            }
        }

        /// <summary>
        /// List of sets to download
        /// </summary>
        public ObservableCollection<ModelDownloadSet> ListDownloadSets
        {
            get
            {
                return this.m_listDownloadSets;
            }
            set
            {
                if (this.m_listDownloadSets != null)
                {
                    this.m_listDownloadSets.Clear();
                }
                this.m_listDownloadSets = value;
                this.NotifyPropertyChanged("ListDownloadSets");
            }
        }

        /// <summary>
        /// Actually loaded set
        /// </summary>
        public SetTable Set
        {
            get
            {
                return this.m_set;
            }
            set
            {
                this.m_set = value;
                this.NotifyPropertyChanged("Set");
            }
        }

        /// <summary>
        /// Actually loaded card
        /// </summary>
        public CardTable Card
        {
            get
            {
                return this.m_card;
            }
            set
            {
                this.m_card = value;
                this.NotifyPropertyChanged("Card");
            }
        }

        public string QuizletWord
        {
            get
            {
                return App.Settings.GetValueOrDefault<string>("quizlet.word");
            }
            set
            {
                App.Settings.AddOrUpdateValue("quizlet.word", value);
                this.NotifyPropertyChanged("QuizletWord");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ManageFlashCardsViewModel()
        {
            this.ListSets = new ObservableCollection<SetTable>();
            this.ListCards = new ObservableCollection<CardTable>();
            this.ListDownloadSets = new ObservableCollection<ModelDownloadSet>();
            this.Quizlet = new QuizletSource();
            this.Card = null;
            this.Set = null;
            this.Dc = null;
        }

        /// <summary>
        /// Opens shared connection
        /// </summary>
        /// <param name="refresh">If it should reopen a new connection when one is already established</param>
        public void OpenConnection(bool refresh)
        {
            if (this.Dc == null)
            {
                this.Dc = new CardsDataContext();
            }
            else
            {
                if (refresh)
                {
                    this.Dc.Dispose();
                    this.Dc = new CardsDataContext();
                    this.refreshInstances();
                }
            }
        }

        private void refreshInstances()
        {
            int id;
            
            if(this.Card != null)
            {
                id = this.Card.CardId;
                try
                {
                    this.Card = this.Dc.Cards.Single<CardTable>(c => c.CardId == id);
                }
                catch (InvalidOperationException)
                {
                    this.Card = new CardTable();
                }
            }

            if(this.Set != null)
            {
                id = this.Set.SetId;
                try
                {
                    this.Set = this.Dc.Sets.Single<SetTable>(s => s.SetId == id);
                }
                catch (InvalidOperationException)
                {
                    this.Set = new SetTable();
                }
            }
        }


        /// <summary>
        /// Loads list of sets from DB
        /// <param name="IsEnabled">Determines, if this filter needs to be applied (-1 means no)</param>
        /// </summary>
        public void LoadListSets(int IsEnabled)
        {
            this.ListSets.Clear();
            IQueryable<SetTable> ist = from s in this.Dc.Sets orderby s.Title select s;
            if (IsEnabled > -1) // add condition, if necessarry
            {
                ist = ist.Where(s => s.IsEditable == IsEnabled);
            }
            IList<SetTable> list = ist.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                this.ListSets.Add(list[i] as SetTable);
                this.ListSets[i].Health = CommonData.CalculateHealthSet(this.ListSets[i]);
            }
        }

        /// <summary>
        /// Loads list of cards in a specific set
        /// </summary>
        /// <param name="ID">ID of set</param>
        public void LoadListCards(int ID)
        {
            this.ListCards.Clear();
            IQueryable<CardTable> ist = from c in this.Dc.Cards where c.SetId == ID orderby c.FrontSide select c;
            IList<CardTable> list = ist.ToList();

            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    this.ListCards.Add(list[i] as CardTable);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Loads list of sets to download from Quizlet
        /// </summary>
        /// <param name="setName">Name of set to look for</param>
        public void LoadListDownloadSets(string setName)
        {
            if (this.Quizlet.IsWorking == false)
            {
                App.Settings.Save();
                this.Quizlet.StartSearching(this.ListDownloadSets, setName);
            }
        }

        /// <summary>
        /// Download set from Quizlet
        /// </summary>
        /// <param name="setName">Name of set to look for</param>
        public void DownloadSet(ModelDownloadSet item)
        {
            if (this.Quizlet.IsWorking == false && item.IsDownloaded == false)
            {
                this.Quizlet.StartDownloading(item);
            }
        }

        /// <summary>
        /// Refreshes information about downloaded set among sought sets
        /// </summary>
        public void CheckSoughtSets()
        {
            for (int i = 0; i < this.ListDownloadSets.Count; i++)
            {
                this.ListDownloadSets[i].IsDownloaded = (from s in this.Dc.Sets where s.RemoteId == this.ListDownloadSets[i].RemoteId select s).Count<SetTable>() > 0;
            }
        }
    }
}
