using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Facebook;
using VOTCClient.Core;
using VOTCClient.Core.IO;

namespace VOTCClient.Windows
{
    public partial class FacebookTwitterAuth
    {
        private readonly string _url;
        public FacebookTwitterAuth(string url = "")
        {
            InitializeComponent();
            Kernel.AuthWindow = this;
            _url = url;
        }

        private const string Permissions = "offline_access,user_about_me,read_stream,publish_actions";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Uri loginUrl;
            if (_url == "")
            {
                if (Kernel.FacebookClient != null)
                    return;

                var fb = new FacebookClient();
                const string redirectUrl = "https://www.facebook.com/connect/login_success.html";
                loginUrl = fb.GetLoginUrl(new
                {
                    client_id = 1514638988814629,
                    redirect_uri = redirectUrl,
                    scope = Permissions,
                    display = "popup",
                    response_type = "token"
                });
            }
            else
            {
                var logoutParameters = new Dictionary<string, object>
                  {
                      { "next", "http://votc.bitflash.xyz" }
                  };
                loginUrl = Kernel.FacebookClient.GetLogoutUrl(logoutParameters);
            }
            Webbrowser.Navigated += WebBrowserNavigated;
            Webbrowser.Navigate(loginUrl);
        }

        private async void WebBrowserNavigated(object sender, NavigationEventArgs e)
        {
            // get token
            var url = e.Uri.Fragment;
            if (url.Contains("access_token") && url.Contains("#"))
            {
                url = (new Regex("#")).Replace(url, "?", 1);
                Kernel.FacebookAccessToken = HttpUtility.ParseQueryString(url).Get("access_token");
            }
            try
            {
                Kernel.FacebookClient = new FacebookClient(Kernel.FacebookAccessToken);
                dynamic friendsTaskResult = await Kernel.FacebookClient.GetTaskAsync("/me");
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Kernel.FacebookName = friendsTaskResult.first_name + " " + friendsTaskResult.last_name;
                }), DispatcherPriority.Normal);
                dynamic image = await Kernel.FacebookClient.GetTaskAsync("/me/picture?redirect=0&height=200&type=normal&width=200");
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Kernel.ProfilePicture = image["data"].url;
                    Kernel.UI.LoadImage();
                }), DispatcherPriority.Normal);
                Close();
            }
            catch(Exception ex)
            {
                IoQueue.Add("FacebookTwitterAuth "+ex.Message + " - "+ex.Source + " - " + ex.StackTrace);
            }
        }
    }
}
