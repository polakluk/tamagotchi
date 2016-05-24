using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Linq;

namespace iostamagotchi
{
    /// <summary>
    /// Class handling downloading cards from Quizlet
    /// </summary>
    public class QuizletSource : BaseOnlineSource
    {
        private string m_response;

        /// <summary>
        /// Performs background work for downloading set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void workerDoWorkSearching(object sender, DoWorkEventArgs e)
        {
            this.m_oc.Clear();
            // no searching word defined
            if (this.m_word.Length == 0)
            {
                this.m_isResult = false;
                return;
            }
            this.m_isWorking = true;

            this.m_syncEvent.Reset();
            Thread t = new Thread(this.retrieveSearch);
            t.Start();
            this.m_syncEvent.WaitOne();

            // did we receive any valid records?
            if (this.m_isResult == false)
            {
                return;
            }

            // we did so process them
            this.processSearchingResults();
        }

        /// <summary>
        /// Retrieves HTTP response from searching request
        /// </summary>
        private void retrieveSearch()
        {
            string q = "?q=" + Uri.EscapeUriString(this.m_word) + "&images_only=" + App.Settings.GetValueOrDefault<string>("quizlet.imagesOnly") +
                        "&per_page="+App.Settings.GetValueOrDefault<string>("quizlet.perPage")+
                        "&client_id=" + App.Settings.GetValueOrDefault<string>("quizlet.clientId");

            WebClient client = new WebClient();

            client.DownloadStringCompleted += this.getSearchResponse;
            client.DownloadStringAsync(new Uri("https://api.quizlet.com/2.0/search/sets" + q, UriKind.Absolute));
        }


        private void getSearchResponse(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.m_response = e.Result;
                this.m_isResult = true;
            }
            else
            {
                this.m_isResult = false;
            }
            this.m_syncEvent.Set();
        }

        /// <summary>
        /// Processes records from HTTP response and adds them to observable collection
        /// </summary>
        private void processSearchingResults()
        {
            JObject jsonRoot = JObject.Parse(this.m_response);
            this.m_response = "";
            JToken val = jsonRoot["error_code"];
            // no sets found
            if(val != null)
            {
                this.m_isResult = false;
                return;
            }

            using (CardsDataContext dc = new CardsDataContext())
            {
                IQueryable<SetTable> iqs;

                JArray sets = (JArray)jsonRoot["sets"];
                for (int i = 0; i < sets.Count; i++)
                {
                    iqs = from s in dc.Sets where s.RemoteId == (int)sets[i]["id"] select s;
                    ModelDownloadSet mds = new ModelDownloadSet();
                    mds.IsDownloaded = iqs.Count<SetTable>() == 1;
                    mds.IsDownloading = false;
                    mds.RemoteId = (int)sets[i]["id"];
                    mds.ClassName = "Quizlet";
                    mds.Cards = (int)sets[i]["term_count"];
                    mds.Title = (string)sets[i]["title"];
                    mds.Url = (string)sets[i]["url"];
                    this.m_oc.Add(mds);
                }
            }
        }

        /// <summary>
        /// Performs background work for downloading set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void workerDoWorkDownloading(object sender, DoWorkEventArgs e)
        {
            this.m_isWorking = true;

            this.m_syncEvent.Reset();
            Thread t = new Thread(this.retrieveDownload);
            t.Start();
            this.m_syncEvent.WaitOne();
        }

        /// <summary>
        /// Retrieves HTTP response from download request
        /// </summary>
        private void retrieveDownload()
        {
            Uri url = null;
            WebClient client = new WebClient();
            // no token access
            if (App.Settings.GetValueOrDefault<string>("quizlet.token").Length == 0)
            {
                string q = "?client_id=" + App.Settings.GetValueOrDefault<string>("quizlet.clientId");
                url = new Uri("https://api.quizlet.com/2.0/sets/" + this.m_downloadingSet.RemoteId.ToString() + q, UriKind.Absolute);
            }
            else
            {
                url = new Uri("https://api.quizlet.com/2.0/sets/" + this.m_downloadingSet.RemoteId.ToString(), UriKind.Absolute);
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + App.Settings.GetValueOrDefault<string>("quizlet.token");
            }

            client.DownloadStringCompleted += this.getDownloadResponse;
            client.DownloadStringAsync(url);
        }


        private void getDownloadResponse(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.m_response = e.Result;
                this.m_isResult = true;
                this.processResponseDownload();
            }
            else
            {
                this.m_isResult = false;
            }
            this.m_syncEvent.Set();
        }

        private void processResponseDownload()
        {
            JObject jsonRoot = JObject.Parse(this.m_response);
            this.m_response = "";
            int id = 0;
            using (CardsDataContext dc = new CardsDataContext())
            {

                SourceTable st = dc.Sources.Single(s => s.ClassName == "Quizlet");
                SetTable set = new SetTable();
                st.ListSets.Add(set);
                set.IsEditable = 0;
                set.RemoteId = (int)jsonRoot["id"];
                set.Title = (string)jsonRoot["title"];
                set.MaxPercentage = 0;
                set.LastStudied = DateTime.Now;

                dc.Sets.InsertOnSubmit(set);
                dc.SubmitChanges();
                id = set.SetId;
            }

            using (CardsDataContext dc = new CardsDataContext())
            {
                SetTable set = dc.Sets.Single(s => s.SetId == id);
                JArray cards = (JArray)jsonRoot["terms"];
                CardTable[] card = new CardTable[cards.Count];
                for (int i = 0; i < cards.Count; i++)
                {
                    card[i] = new CardTable();
                    card[i].BackSide = (string)cards[i]["definition"];
                    card[i].FrontSide = (string)cards[i]["term"];
                    card[i].LastStudied = DateTime.Now;
                    card[i].ActRank = 0;
                    card[i].MaxRank = 0;
                    set.ListCards.Add(card[i]);
                    dc.Cards.InsertOnSubmit(card[i]);
                }
                dc.SubmitChanges();
            }
        }
    }
}
