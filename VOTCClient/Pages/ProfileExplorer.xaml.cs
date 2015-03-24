using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VOTCClient.Core;

namespace VOTCClient.Pages
{
    public partial class ProfileExplorer
    {
        public ProfileExplorer()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                foreach(var file in Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Scripts\\"))
                {
                    var file1 = file;
                    Dispatcher.BeginInvoke(new Action(() => ListBox.Items.Add(Path.GetFileName(file1))), DispatcherPriority.Background);
                }
            });
        }
        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(ListBox.SelectedItem == null)
                return;
            Kernel.ProfileWindow.Content = new ProfileDetailView(ListBox.SelectedItem.ToString());
        }
    }
}
