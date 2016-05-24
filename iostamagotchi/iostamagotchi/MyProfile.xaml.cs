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
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace iostamagotchi
{
    public partial class MyProfile : PhoneApplicationPage
    {
        private int m_state;
        public MyProfile()
        {
            InitializeComponent();
            this.piScore.DataContext = App.MyProfileViewModel;
            this.piQuizlet.DataContext = App.MyProfileViewModel;
            this.lbHighScore.ItemsSource = App.MyProfileViewModel.ListLeaderBoard;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.MyProfileViewModel.LoadLeaderBoard();
			App.MyProfileViewModel.LoggedIn = App.Settings.GetValueOrDefault<string>("quizlet.token").Length != 0;
            App.MyProfileViewModel.Syncing = false;
            this.wbQuizlet.Visibility = Visibility.Collapsed;
        }

        private void bConnect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.wbQuizlet.Visibility = Visibility.Visible;
            Random r = new Random();
            this.m_state = r.Next();

            string client = App.Settings.GetValueOrDefault<string>("quizlet.clientId");
            string return_url = Uri.EscapeUriString("http://www.spaceteacher.com");
            string url = "https://quizlet.com/authorize/?response_type=code&scope=read&client_id=" +
                                    client + "&state=" + this.m_state.ToString();
            this.wbQuizlet.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void bSync_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.MyProfileViewModel.Progress = !App.MyProfileViewModel.Progress;
        }
		
		private bool strCmp(string a, string b)
        {
            if(a.Length < b.Length)
                return false;
            bool equal = false;
            for (int i = 0; i < b.Length; i++)
            {
 
                if (a[i] == b[i])
                    equal = true;
                else
                {
                    equal = false;
                    break;
                }
 
            }
 
            return equal;
        }

        private void downloadAccessToken(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                JObject jsonRoot = JObject.Parse(e.Result);
                App.Settings.AddOrUpdateValue("quizlet.token", (string)jsonRoot["access_token"]);
                App.Settings.Save();
                App.MyProfileViewModel.LoggedIn = true;
            }
            else
            {
                MessageBox.Show("Error during receive access token has occurred!");
            }
        }

        private void wbQuizlet_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.strCmp(e.Uri.AbsoluteUri, "http://www.spaceteacher.") == true)
            {
                this.wbQuizlet.Visibility = Visibility.Collapsed;
                if (e.Uri.Query.Length == 0)
                {
                    MessageBox.Show("Wrong response from server!");
                    return;
                }
                string[] parts = e.Uri.Query.Split('&');
                string error = parts.Where(s => s.Split('=')[0].Contains("error")).Select(s => s.Split('=')[1]).FirstOrDefault();
                if (error == "access_denied")
                {
                    MessageBox.Show("Access denied!");
                    return;
                }
                string state = parts.Where(s => s.Split('=')[0].Contains("state")).Select(s => s.Split('=')[1]).FirstOrDefault();

                // check out token
                if (state != this.m_state.ToString())
                {
                    MessageBox.Show("Incorrect token!");
                    return;
                }

                string code = parts.Where(s => s.Split('=')[0].Contains("code")).Select(s => s.Split('=')[1]).FirstOrDefault();

                // now ask for access token
                Dictionary<string, object> q = new Dictionary<string, object>();
                q.Add("grant_type", "authorization_code");
                q.Add("code", code);
                q.Add("redirect_uri", "http://www.spaceteacher.com");
                /*
                PostClient client = new PostClient(q);
                client.DownloadStringCompleted += this.downloadAccessToken;
                client.DownloadStringAsync(new Uri("https://api.quizlet.com/oauth/token", UriKind.Absolute));
                */
            }
        }
    }
}
