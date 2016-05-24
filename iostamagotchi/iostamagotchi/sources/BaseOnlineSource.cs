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
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace iostamagotchi
{
    /// <summary>
    /// Base class for handling online sources
    /// </summary>
    public class BaseOnlineSource : BaseBindingSource
    {
        private ObservableCollection<ModelDownloadSet> m_targetOc = null;

        /// <summary>
        /// Sync event for threads
        /// </summary>
        protected ManualResetEvent m_syncEvent = new ManualResetEvent(false);

        /// <summary>
        /// Background worker for HTTP requests
        /// </summary>
        protected BackgroundWorker m_worker;

        /// <summary>
        /// States if background worker works at the moment
        /// </summary>
        protected bool m_isWorking = false;

        /// <summary>
        /// States if background worker is doing search task at the moment
        /// </summary>
        protected bool m_isSearching = false;

        /// <summary>
        /// States if HTTP request returned any valid records
        /// </summary>
        protected bool m_isResult;

        /// <summary>
        /// Searching word
        /// </summary>
        protected string m_word = "";

        /// <summary>
        /// Downloading set
        /// </summary>
        protected ModelDownloadSet m_downloadingSet;

        /// <summary>
        /// Collection of records from internet
        /// </summary>
        protected ObservableCollection<ModelDownloadSet> m_oc = new ObservableCollection<ModelDownloadSet>();

        /// <summary>
        /// Dispatcher for current page
        /// </summary>
        public Dispatcher Dispatcher;

        /// <summary>
        /// States, if background worker is currently working on something
        /// </summary>
        public bool IsWorking
        {
            get
            {
                return this.m_isWorking;
            }
        }

        /// <summary>
        /// States if background worker is doing search task at the moment
        /// </summary>
        public bool IsSearching
        {
            get
            {
                return m_isSearching;
            }
            set
            {
                this.m_isSearching = value;
                this.NotifyPropertyChanged("IsSearching");
            }
        }

        /// <summary>
        /// If HTTP request is finished
        /// </summary>
        public bool IsResultHttp;

        /// <summary>
        /// Downloads data
        /// <param name="item">Downloading set</param>
        /// </summary>
        public void StartDownloading(ModelDownloadSet item)
        {
            if (this.m_worker != null)
            {
                this.m_worker = null;
            }
            item.IsDownloading = true;
            this.m_isResult = false;
            this.m_downloadingSet = item;
            this.m_worker = new BackgroundWorker();

            this.m_worker.DoWork += new DoWorkEventHandler(this.workerDoWorkDownloading);
            this.m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.workerCompletedDownloading);
            this.m_worker.RunWorkerAsync();
            this.m_worker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Performs background work for downloading set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void workerDoWorkDownloading(object sender, DoWorkEventArgs e)
        {
            this.m_isWorking = true;
        }

        /// <summary>
        /// Action performed after set is downloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void workerCompletedDownloading(object sender, RunWorkerCompletedEventArgs e)
        {
            this.m_isWorking = false;
            if (this.m_isResult)
            {
                this.m_downloadingSet.IsDownloading = false;
                this.m_downloadingSet.IsDownloaded = true;
                MessageBox.Show("Download completed");
                App.ManageFlashCardsViewModel.OpenConnection(true);
                App.ManageFlashCardsViewModel.LoadListSets(-1);
            }
            else
            {
                this.m_downloadingSet.IsDownloading = false;
                MessageBox.Show("Error during downloading set has occurred!");
            }
        }

        /// <summary>
        /// Downloads data
        /// <param name="oc">Instance of Observable collection to which results of this action should be saved</param>
        /// <param name="word">Searching word</param>
        /// </summary>
        public void StartSearching(ObservableCollection<ModelDownloadSet> oc, string m_word)
        {
            if (this.m_worker != null)
            {
                this.m_worker = null;
            }
            this.m_word = m_word;
            this.m_targetOc = oc;
            this.IsSearching = true;
            this.m_worker = new BackgroundWorker();

            this.m_worker.DoWork += new DoWorkEventHandler(this.workerDoWorkSearching);
            this.m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.workerCompletedSearching);
            this.m_worker.RunWorkerAsync();
            this.m_worker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Performs background work for downloading set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void workerDoWorkSearching(object sender, DoWorkEventArgs e)
        {
            this.m_isWorking = true;
        }

        /// <summary>
        /// Action performed after set is downloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void workerCompletedSearching(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.m_isWorking = false;
                this.IsSearching = false;
                if (this.m_isResult)
                {
                    this.LoadRecordsToUi();
                }
                else
                {
                    this.m_oc.Clear();
                    this.m_oc.Add(this.emptyRecord());
                    this.LoadRecordsToUi();
                    MessageBox.Show("No records were found!");
                }
            }
            );
        }

        protected virtual ModelDownloadSet emptyRecord()
        {
            ModelDownloadSet ds = new ModelDownloadSet();
            ds.IsDownloaded = true;
            ds.IsDownloading = false;
            ds.ClassName = "Base";
            ds.Title = "No item found";
            return ds;
        }

        protected void LoadRecordsToUi()
        {
            this.m_targetOc.Clear();
            foreach (ModelDownloadSet row in this.m_oc)
            {
                this.m_targetOc.Add(row);
            }
        }
    }
}
