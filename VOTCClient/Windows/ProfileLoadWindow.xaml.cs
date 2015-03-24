using System;
using System.ComponentModel;
using System.Windows;
using VOTCClient.Core;
using VOTCClient.Pages;

namespace VOTCClient.Windows
{
    public partial class ProfileLoadWindow
    {
        public ProfileLoadWindow()
        {
            InitializeComponent();
            Kernel.ProfileWindow = this;
            SizeToContent = SizeToContent.WidthAndHeight;
            Content = new ProfileExplorer();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Content.GetType() == typeof (ProfileDetailView))
            {
                Content = new ProfileExplorer();
                e.Cancel = true;
            }
        }
    }
}
