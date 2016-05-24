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
using Microsoft.Phone.Controls;

namespace iostamagotchi
{
    public class TestViewModel : BaseBindingModel
    {
        private bool m_displayCheering;
        private string m_cheering;

        private int m_score;
        private int m_knownCards;
        private int m_totalCards;
        private int m_inRow;
        private int m_lastCheer;
        private SetTable m_set;
        private ObservableCollection<CardTable> m_cardsLeft; // cards left to test
        private ObservableCollection<CardTable> m_allCards; // all cards in set
        private CardTable m_mainCard;
        private string[] m_backs;

        /// <summary>
        /// Main card
        /// </summary>
        public CardTable MainCard
        {
            get
            {
                return this.m_mainCard;
            }
            set
            {
                if (this.m_mainCard == value)
                {
                    return;
                }
                this.m_mainCard = value;
                this.NotifyPropertyChanged("MainCard");
            }
        }

        /// <summary>
        /// Index of correct answer
        /// </summary>
        public int AnswerIdx;

        public string[] Backs
        {
            get
            {
                return this.m_backs;
            }
            set
            {
                this.m_backs = value;
                this.NotifyPropertyChanged("Backs");
            }
        }

        /// <summary>
        /// Determines state of the answer
        /// </summary>
        public eFlachCardAnswerState AnswerState;

        /// <summary>
        /// States if testing is going on
        /// </summary>
        public bool TestingOn;

        /// <summary>
        /// Shows cheering text
        /// </summary>
        public bool DisplayCheering
        {
            get
            {
                return this.m_displayCheering;
            }
            set
            {
                if (this.m_displayCheering == value)
                {
                    return;
                }
                this.m_displayCheering = value;
                this.NotifyPropertyChanged("DisplayCheering");
            }
        }

        /// <summary>
        /// Cheering text
        /// </summary>
        public string Cheering
        {
            get
            {
                return this.m_cheering;
            }
            set
            {
                if (this.m_cheering == value)
                {
                    return;
                }
                this.m_cheering = value;
                this.NotifyPropertyChanged("Cheering");
            }
        }

        /// <summary>
        /// Current players score
        /// </summary>
        public string Score
        {
            get
            {
                return this.m_score.ToString();
            }
            set
            {
                if (this.m_score.ToString() == value)
                {
                    return;
                }
                this.m_score = Convert.ToInt32(value);
                this.NotifyPropertyChanged("Score");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TestViewModel()
        {
            this.DisplayCheering = false;
            this.m_mainCard = null;
        }

        /// <summary>
        /// Initializes card to its default state
        /// </summary>
        public void InitializeTestCard()
        {
            this.m_mainCard = new CardTable();
        }

        /// <summary>
        /// Loads data from Isolated Storage
        /// </summary>
        public void LoadDataIsolated()
        {
            this.Score = App.Settings.GetValueOrDefault<int>("testing.score").ToString();
            this.m_knownCards = App.Settings.GetValueOrDefault<int>("testing.known");
            this.m_totalCards = App.Settings.GetValueOrDefault<int>("testing.total");
            this.m_inRow = App.Settings.GetValueOrDefault<int>("testing.inRow");
            this.m_lastCheer = App.Settings.GetValueOrDefault<int>("testing.lastCheer");
            this.TestingOn = App.Settings.GetValueOrDefault<bool>("testing.on");
        }

        /// <summary>
        /// Saves data to Isolated Storage
        /// </summary>
        public void SaveDataIsolated()
        {
            App.Settings.AddOrUpdateValue("testing.score", this.m_score);
            App.Settings.AddOrUpdateValue("testing.known", this.m_knownCards);
            App.Settings.AddOrUpdateValue("testing.total", this.m_totalCards);
            App.Settings.AddOrUpdateValue("testing.setId", this.m_set.SetId);
            App.Settings.AddOrUpdateValue("testing.inRow", this.m_inRow);
            App.Settings.AddOrUpdateValue("testing.on", this.TestingOn);
            App.Settings.AddOrUpdateValue("testing.lastCheer", this.m_lastCheer);
            App.Settings.Save();
        }

        /// <summary>
        /// Initialize all parameters to default values
        /// </summary>
        public void InitParams()
        {
            App.Settings.AddOrUpdateValue("testing.score", 0);
            App.Settings.AddOrUpdateValue("testing.known", 0);
            App.Settings.AddOrUpdateValue("testing.total", 0);
            App.Settings.AddOrUpdateValue("testing.inRow", 0);
            App.Settings.AddOrUpdateValue("testing.lastCheer", CommonData.CheerDefault);
        }

        /// <summary>
        /// Loads next card to test
        /// </summary>
        public void LoadNextCard()
        {
            this.AnswerState = eFlachCardAnswerState.fcasNone;

            // now lets pick a card
            IEnumerable<CardTable> iecl = this.m_cardsLeft.ToList<CardTable>();
            IEnumerable<CardTable> ieca = this.m_allCards.ToList<CardTable>();
            int count = iecl.Count<CardTable>();
            int countAll = ieca.Count<CardTable>() - 1;

            // do we have a flash card to study
            if (count == 0)
            {
                // no => end of studying
                this.endOfTesting();
            }
            else
            {
                // YES :)
                Random r = new Random();
                string[] backs = new string[4];
                this.MainCard = iecl.ElementAt<CardTable>(r.Next(count - 1));
                for (int i = 0; i < 4; i++)
                {
                    backs[i] = "";
                }

                this.AnswerIdx = r.Next(3);
                backs[this.AnswerIdx] = this.MainCard.BackSide;

                // now let me pick the other three random cards
                bool ok;
                for (int i = 0; i < 4; i++)
                {
                    // check if this place is not already taken by the correct answer
                    if(backs[i] != "")
                    {
                        i++;
                        if( i == 4) // we should stop here
                        {
                            break;
                        }
                    }
                    // now pick a random new card
                    do
                    {
                        ok = true;
                        backs[i] = ieca.ElementAt<CardTable>(r.Next(countAll)).BackSide;
                        // check, if we havent picked this card yet
                        for (int j = 0; ok && j < i; j++)
                        {
                            ok = backs[j] != backs[i];
                        }
                    }
                    while (ok == false || backs[this.AnswerIdx] == backs[i]);
                }
                this.Backs = backs;

                this.m_totalCards++;
            }
        }

        /// <summary>
        /// Updates card after user's input
        /// </summary>
        public void UpdateCard()
        {
            this.m_cardsLeft.Remove(this.MainCard);
            if (this.AnswerState == eFlachCardAnswerState.fcasYes)
            {
                this.m_knownCards++;
                this.m_inRow++;
                this.Score = (this.m_score + CommonData.AdjustScore(1)).ToString();
                if (this.m_inRow > 1) // at least second good answer in row
                {
                    Random r = new Random();
                    if (r.Next(this.m_lastCheer) == 0)
                    {
                        this.DisplayCheering = true;
                        this.Cheering = "Good job!";
                        this.m_lastCheer = CommonData.CheerDefault; // set delay to default
                    }
                    else // not this time? give me better chance for next time
                    {
                        this.m_lastCheer--;
                    }
                }
            }
            else
            {
                this.m_inRow = 0;
                this.m_lastCheer = CommonData.CheerDefault;
            }
            this.SaveDataIsolated();
        }

        /// <summary>
        /// Loads all information about the set
        /// </summary>
        /// <param name="setId">Set ID</param>
        public void GetStudySet(int setId)
        {
            using (CardsDataContext dc = new CardsDataContext())
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
                        MessageBox.Show("There is no set to use for testing at this moment");
                        this.m_set = null;
                        this.TestingOn = false;
                    }
                    else
                    {
                        this.getRandomSet();
                    }
                }
                else // we found it so we select it
                {
                    this.m_set = iqt.First<SetTable>();
                    this.TestingOn = this.m_set.CardCount > 3;
                }
            }

            // load list of all cards for the set twice
            this.m_cardsLeft = SetHelper.GetListReadyCards(this.m_set.SetId, 0, false);
            this.m_allCards = SetHelper.GetListReadyCards(this.m_set.SetId, 0, false);
        }

        private void getRandomSet()
        {
            ObservableCollection<SetTable> oc = SetHelper.GetListTestSets();
            int sets = oc.Count;
            this.m_knownCards = 0;
            this.m_totalCards = 0;
            this.m_score = 0;
            this.m_inRow = 0;
            this.m_lastCheer = 0;

            if (sets == 0)
            {
                MessageBox.Show("There is no set to study at this moment");
                this.m_set = null;
                this.TestingOn = false;
                this.SaveDataIsolated();
                return;
            }

            Random r;
            r = new Random();
            this.m_set = oc[r.Next(sets - 1)];
            this.TestingOn = true;
            this.SaveDataIsolated();
        }

        private int calculateScore()
        {
            int actScore = App.Settings.GetValueOrDefault<int>("player.score");
            int k = Math.Max(0, (this.getPercentage() - this.m_set.MaxPercentage) / 10);
            int score = CommonData.AdjustScore(k * this.m_knownCards + this.m_totalCards);
            using (ScoresDataContext dc = new ScoresDataContext())
            {
                ScoreTable st = new ScoreTable();
                st.Score = score;
                st.IsUploaded = false;
                st.Achieved = DateTime.Now;
                st.ActionType = (int)eActionType.atTest;
                dc.Scores.InsertOnSubmit(st);
                dc.SubmitChanges();
            }
            App.Settings.AddOrUpdateValue("player.score", (actScore + score));
            return score;
        }

        private int getPercentage()
        {
            int act = (int)((this.m_knownCards * 100) / this.m_totalCards);
            if (act > this.m_set.MaxPercentage)
            {
                using (CardsDataContext dc = new CardsDataContext())
                {
                    SetTable st = dc.Sets.Single(s => s.SetId == this.m_set.SetId);
                    st.MaxPercentage = act;
                    dc.SubmitChanges();
                }
            }
            return act;
        }

        private void endOfTesting()
        {
            int score = this.calculateScore();
            this.TestingOn = false;
            string msg = "End of testing.\n\nYou knew " + this.m_knownCards.ToString() + " out of " + this.m_totalCards.ToString() + ". ";
            msg += "\nScore: " + score.ToString();
            msg += "\nTotal score: " + App.Settings.GetValueOrDefault<int>("player.score").ToString() + "\n";
            msg += this.addEndMessage();

            this.SaveDataIsolated();
            MessageBox.Show(msg);
            PhoneApplicationFrame rootFrame = (App.Current as App).RootFrame;
            rootFrame.GoBack();
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
