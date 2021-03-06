﻿using System; //VOTC LEGACY
using System.Windows;
using System.Windows.Controls;
using TweetSharp;
using VOTCClient.Core;

/*
    This file is part of VOTC.

    VOTC is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VOTC is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VOTC.  If not, see <http://www.gnu.org/licenses/>.
*/
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
            try
            {
                if (Kernel.TwitterClient != null)
                    return;
                Kernel.TwitterClient = new TwitterService("bcPdqSrMo2zSoLRjvE20Z2B93", "2DPgdd0qqnpOxbglUMWs4okLjNL3IMgbuQezLCgwMr1XH4kYpw");

                _requestToken = Kernel.TwitterClient.GetRequestToken();
                var authUrl = Kernel.TwitterClient.GetAuthorizationUri(_requestToken).ToString();
                Webbrowser.Navigate(authUrl);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
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
