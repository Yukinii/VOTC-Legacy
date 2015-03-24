using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
namespace VOTCClient.Windows
{
    public partial class FacebookPost
    {
        private string _input;

        public FacebookPost()
        {
            InitializeComponent();
            Kernel.FacebookPostWindow = this;
        }

        public FacebookPost(string input)
        {
            _input = input.Remove(0, 10);
            InitializeComponent();
            Kernel.FacebookPostWindow = this;
        }

        private void image_Loaded(object sender, RoutedEventArgs e)
        {
            if(Kernel.ProfilePicture == null)
            {
                var box = new FacebookTwitterAuth();
                box.ShowDialog();
            }
            if (Kernel.ProfilePicture == null)
                Close();

            TextBox.Text = _input;
            if (!string.IsNullOrEmpty(Kernel.ProfilePicture))
            {
                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(Kernel.ProfilePicture, UriKind.Absolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                Image.Source = src;
            }
            Facebookname.Text = Kernel.FacebookName;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBox.Text == "TextBox")
                return;
            _input = TextBox.Text;
        }
    }
}
