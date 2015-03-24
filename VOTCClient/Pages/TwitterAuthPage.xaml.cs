using System.Windows;
using System.Windows.Controls;
using TweetSharp;
using VOTCClient.Core;

namespace VOTCClient.Pages
{
    public partial class TwitterAuthPage
    {
        public TwitterAuthPage()
        {
            InitializeComponent();
        }
        OAuthRequestToken _requestToken;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Kernel.TwitterClient != null)
                return;
            Kernel.TwitterClient = new TwitterService("bcPdqSrMo2zSoLRjvE20Z2B93", "2DPgdd0qqnpOxbglUMWs4okLjNL3IMgbuQezLCgwMr1XH4kYpw");

            _requestToken = Kernel.TwitterClient.GetRequestToken();
            var authUrl = Kernel.TwitterClient.GetAuthorizationUri(_requestToken).ToString();
            Webbrowser.Navigate(authUrl);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TextBox.Text.Length != 7)
                    return;

                var accessToken = Kernel.TwitterClient.GetAccessToken(_requestToken, TextBox.Text);
                Kernel.TwitterClient.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
                Kernel.TwitterClient.UserAgent = "VOTC";
                var user = Kernel.TwitterClient.VerifyCredentials(new VerifyCredentialsOptions { IncludeEntities = true });
                Kernel.TwitterUsername = user.ScreenName;
                Kernel.ProfilePicture = user.ProfileImageUrl.Replace("_normal.", "_400x400.");
                Kernel.UI.LoadImage();
                Kernel.TwitterToken = accessToken.Token;
                Kernel.TwitterSecret = accessToken.TokenSecret;
                Kernel.AuthWindow.Close();
            }
            catch
            {
                MessageBox.Show("Didn't work. Pin correct?", "FAIL");
            }
        }
    }
}
