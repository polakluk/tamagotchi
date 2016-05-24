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
using System.Windows.Threading;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using Microsoft.Phone.Controls;

namespace iostamagotchi
{
    /// <summary>
    /// ViewModel for studying
    /// </summary>
    public class StudyViewModel : BaseViewModel
    {
        private CardTable m_actCard = null;
        private ModelFlashCardStudy m_flashCard = null;
        private int m_setId;
        private int m_knownCards;
        private int m_improvedCards;
        private ObservableCollection<CardTable> m_actOc = null;
        private int m_actRank;
        private int m_totalCards;

        /// <summary>
        /// States, if studying is on
        /// </summary>
        public bool StudyingOn;

        /// <summary>
        /// Decides, if studying algorithm should be used
        /// </summary>
        public bool UseAlgorithm;

        /// <summary>
        /// Displayed flash cards
        /// </summary>
        public ModelFlashCardStudy FlashCard
        {
            get
            {
                return this.m_flashCard;
            }
            set
            {
                this.m_flashCard = value;
                this.NotifyPropertyChanged("FlashCard");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StudyViewModel()
        {
            this.initFlashCard();
        }

        private void initFlashCard()
        {
            this.FlashCard = new ModelFlashCardStudy();
            this.FlashCard.AnimationTimer = new StudyDispatcherTimer();
            this.FlashCard.AnimationTimer.Tick += this.TickAnimationHandler;
            this.FlashCard.AnimationTimer.Interval = TimeSpan.FromMilliseconds(400);
            this.FlashCard.AnimationTimer.ParentCard = this.FlashCard;
            this.FlashCard.IsCardVisible = true;
            this.FlashCard.IsHandleVisible = false;
        }

        /// <summary>
        /// Loads another flash card from set
        /// </summary>
        public void LoadNextCard()
        {
            this.FlashCard.CardState = eFlashCardStateStudy.fcssNone;
            this.FlashCard.TouchStart = new Point(-1, -1);
            this.FlashCard.TouchEnd = new Point(-1, -1);
            this.FlashCard.AnswerState = eFlachCardAnswerState.fcasNone;

            // now lets pick a card
            IEnumerable<CardTable> iec = null;
            int count = 0;
            if (this.UseAlgorithm) // player follows algorithm
            {
                for (; this.m_actRank < CommonData.MaxRank; this.m_actRank++)
                {
                    iec = this.m_actOc.Where(c => c.ActRank == this.m_actRank);
                    count = iec.Count<CardTable>();
                    // we found some cards here
                    if (count != 0)
                    {
                        break;
                    }
                }
            }
            else // player practises
            {
                iec = this.m_actOc.ToList<CardTable>();
                count = iec.Count<CardTable>();
            }

            // do we have a flash card to study
            if (count == 0)
            {
                // no => end of studying
                this.endOfStudying();
            }
            else
            {
                // YES :)
                Random r = new Random();
                this.m_actCard = iec.ElementAt<CardTable>(r.Next(count - 1));
                this.FlashCard.FrontSide = this.m_actCard.FrontSide;
                this.FlashCard.BackSide = this.m_actCard.BackSide;
                this.FlashCard.CardId = this.m_actCard.CardId;
                this.m_totalCards++;
            }
        }

        private void TickAnimationHandler(object sender, EventArgs e)
        {
            StudyDispatcherTimer timer = sender as StudyDispatcherTimer;
            switch (timer.ParentCard.CardState)
            {
                case eFlashCardStateStudy.fcssFadingOut:
                    {
                        App.StudyViewModel.UpdateCurrentCard();
                        App.StudyViewModel.LoadNextCard();
                        App.StudyViewModel.SaveDataToIsolated();
                        timer.ParentCard.IsCardVisible = true;
                        timer.ParentCard.CardState = eFlashCardStateStudy.fcssFadingIn;
                        break;
                    }
                case eFlashCardStateStudy.fcssFadingIn:
                    {
                        timer.ParentCard.CardState = eFlashCardStateStudy.fcssNone;
                        timer.Stop();
                        break;
                    }
            }
        }

        /// <summary>
        /// Updates data about the current flash cards (after answering, if player knew it or not)
        /// </summary>
        public void UpdateCurrentCard()
        {
            using (CardsDataContext dc = new CardsDataContext())
            {
                try
                {
                    this.m_actOc.Remove(this.m_actCard);
                    // player knew the answer
                    if (this.UseAlgorithm) // playes follows algorithm
                    {
                        CardTable ct = dc.Cards.Single(c => c.CardId == this.FlashCard.CardId);
                        if (this.FlashCard.AnswerState == eFlachCardAnswerState.fcasYes)
                        {
                            ct.ActRank++;
                            if (ct.MaxRank < ct.ActRank)
                            {
                                ct.MaxRank = ct.ActRank;
                                this.m_improvedCards++;
                            }
                            this.m_knownCards++;
                        }
                        else
                        {
                            ct.ActRank = 0;
                        }
                        ct.LastStudied = DateTime.Now;
                        dc.SubmitChanges();
                    }
                    else // player practises words
                    {
                        if (this.FlashCard.AnswerState == eFlachCardAnswerState.fcasYes)
                        {
                            this.m_knownCards++;
                        }
                    }
                }
                catch
                {
                    ChangeConflictCollection col = dc.ChangeConflicts;
                    for (int i = 0; i < col.Count; i++)
                    {
                        col[i].Resolve();
                    }
                    dc.SubmitChanges();
                }

            }
            this.SaveDataToIsolated();
        }

        /// <summary>
        /// Initialize all parameters to default values
        /// </summary>
        public void InitParams()
        {
            App.Settings.AddOrUpdateValue("study.known", 0);
            App.Settings.AddOrUpdateValue("study.total", 0);
            App.Settings.AddOrUpdateValue("study.rank", 0);
            App.Settings.AddOrUpdateValue("study.improved", 0);
        }

        /// <summary>
        /// Saves actual state data to Isolated Storage
        /// </summary>
        public void SaveDataToIsolated()
        {
            App.Settings.AddOrUpdateValue("study.setId", this.m_setId);
            App.Settings.AddOrUpdateValue("study.known", this.m_knownCards);
            App.Settings.AddOrUpdateValue("study.total", this.m_totalCards);
            App.Settings.AddOrUpdateValue("study.on", this.StudyingOn);
            App.Settings.AddOrUpdateValue("study.rank", this.m_actRank);
            App.Settings.AddOrUpdateValue("study.improved", this.m_improvedCards);
            App.Settings.AddOrUpdateValue("study.algorithm", this.UseAlgorithm);
            App.Settings.Save();
        }

        /// <summary>
        /// Loads state data from Isolated Storage
        /// </summary>
        public void LoadDataFromIsolated()
        {
            this.m_totalCards = App.Settings.GetValueOrDefault<int>("study.total");
            this.m_knownCards = App.Settings.GetValueOrDefault<int>("study.known");
            this.m_setId = App.Settings.GetValueOrDefault<int>("study.setId");
            this.StudyingOn = App.Settings.GetValueOrDefault<bool>("study.on");
            this.m_actRank = App.Settings.GetValueOrDefault<int>("study.rank");
            this.m_improvedCards = App.Settings.GetValueOrDefault<int>("study.improved");
            this.UseAlgorithm = App.Settings.GetValueOrDefault<bool>("study.algorithm");
        }

        /// <summary>
        /// Loads current studying set
        /// </summary>
        /// <param name="setId">Id of selected studying set</param>
        public void GetStudySet(int setId)
        {
            this.FlashCard.IsBackVisible = false;
            this.FlashCard.IsHandleVisible = false;
            using(CardsDataContext dc = new CardsDataContext())
            {
                IQueryable<SetTable> iqt = from s in dc.Sets where s.SetId == setId select s;
                // selected set doesnt found => pick a new one
                if (iqt.Count<SetTable>() == 0)
                {
                    int sets;
                    // get a random one
                    iqt = from s in dc.Sets where s.IsStudying == true select s;
                    sets = iqt.Count<SetTable>();
                    if (sets == 0)
                    {
                        MessageBox.Show("There is no set to study at this moment");
                        this.m_setId = 0;
                        this.StudyingOn = false;
                    }
                    else
                    {
                        this.getRandomSet();
                    }
                }
                else // we found it so we select it
                {
                    this.StudyingOn = true;
                    this.m_setId = setId;
                }
            }
            // load all cards ready to study
            this.m_actOc = SetHelper.GetListReadyCards(this.m_setId, -1, this.UseAlgorithm);
        }

        private void endOfStudying()
        {
            int score = this.calculateScore();
            this.StudyingOn = false;
            string msg = "End of studying.\n\nYou knew " + this.m_knownCards.ToString() + " out of " + this.m_totalCards.ToString()+". ";
            msg += "\nScore: " + score.ToString();
            msg += "\nTotal score: " + App.Settings.GetValueOrDefault<int>("player.score").ToString() + "\n";
            msg += this.addEndMessage();

            this.m_knownCards = 0;
            this.m_totalCards = 0;
            this.m_actRank = 0;
            this.m_actOc = null;
            this.SaveDataToIsolated();
            MessageBox.Show(msg);
            PhoneApplicationFrame rootFrame = (App.Current as App).RootFrame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
            else // called from notification
            {
                rootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void getRandomSet()
        {
            ObservableCollection<SetTable> oc = SetHelper.GetListReadySets(this.UseAlgorithm);
            int sets = oc.Count;
            this.m_knownCards = 0;
            this.m_totalCards = 0;
            this.m_actRank = 0;
            if (sets == 0)
            {
                MessageBox.Show("There is no set to study at this moment");
                this.m_setId = 0;
                this.StudyingOn = false;
                this.SaveDataToIsolated();
                return;
            }

            Random r;
            r = new Random();
            this.m_setId = oc[r.Next(sets - 1)].SetId;
            this.StudyingOn = true;
            this.SaveDataToIsolated();
        }

        private int calculateScore()
        {
            int actScore = App.Settings.GetValueOrDefault<int>("player.score");
            int k1 = 1;
            int k2 = (int)(4 * (this.UseAlgorithm ? 1 : 0.5));
            int score = CommonData.AdjustScore((k1 * k2) * this.m_knownCards + this.m_totalCards);
            using (ScoresDataContext dc = new ScoresDataContext())
            {
                ScoreTable st = new ScoreTable();
                st.Score = score;
                st.IsUploaded = false;
                st.Achieved = DateTime.Now;
                st.ActionType = this.UseAlgorithm == true ? (int)eActionType.atStudy : (int)eActionType.atPractise;
                dc.Scores.InsertOnSubmit(st);
                dc.SubmitChanges();
            }
            App.Settings.AddOrUpdateValue("player.score", ( actScore + score ));
            return score;
        }

        private string addEndMessage()
        {
            string msg;
            if (this.m_knownCards < 3)
            {
                msg = "Study more and you will get better!";
            }
            else
            {
                msg = "Good job!";
            }

            return msg;
        }
    }
}
