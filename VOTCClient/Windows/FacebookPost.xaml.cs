using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VOTCClient.Core;

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
