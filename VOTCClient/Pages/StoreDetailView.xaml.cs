using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
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
    public partial class StoreDetailView
    {
        private dynamic _obj;
        public StoreDetailView(dynamic obj)
        {
            _obj = obj;
            InitializeComponent();
            Kernel.StoreWindow.Title = "VOTC Store >> " + _obj.Name.ToString().Replace(".cs", " Profile");
        }

        public string CacheLookup()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Cache\\Store\\"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Cache\\Store\\");

            foreach (var cacheFile in Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Cache\\Store\\").Where(cachedFile => Path.GetFileName(cachedFile) == _obj.Name.ToString().Replace(".cs", "") + ".header"))
            {
                return cacheFile;
            }
            using (var client = new WebClient())
            {
                string imageUrl = client.DownloadString(Kernel.RemoteHost + _obj.Name.ToString().Replace(".cs", "") + ".header");
                File.WriteAllBytes("Cache\\Store\\" + _obj.Name.ToString() + ".header", client.DownloadData(imageUrl));
            }
            return "Cache\\Store\\" + _obj.Name.ToString() + ".header";
        }

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText("Scripts\\" + _obj.Name.ToString() + ".cs", _obj.Contents.ToString());
                MessageBox.Show("Installed!", "Done!");
                Kernel.StoreWindow.Close();
                Kernel.StoreWindow = null;
                GC.Collect(10, GCCollectionMode.Forced);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) => await Task.Run(async() => await Dispatcher.BeginInvoke(new Action(() =>
        {
            AdDisplay.GetAd(375).ConfigureAwait(false);
            Ratingslbl.Content = "Rating: " + _obj.Rating.ToString();
            Downloadslbl.Content = "Downloads: " + _obj.Downloads.ToString();
            Image.Source = new ImageSourceConverter().ConvertFromString(CacheLookup()) as ImageSource;
            AuthorTxtbx.Text = _obj.Author.ToString();
            Descriptionbox.Text = _obj.Description.ToString() + Environment.NewLine + Environment.NewLine + "Foreground Required: " + _obj.Foreground.ToString();
            foreach (var cmd in _obj.Commands)
            {
                CommandsBox.Items.Add(cmd.ToString());
            }
            BusyIndicator.IsBusy = false;
        }), DispatcherPriority.Render));

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _obj = null;
            AdDisplay.Dispose();
            Ratingslbl = null;
            Downloadslbl = null;
            Image = null;
            AuthorTxtbx = null;
            Descriptionbox = null;
            CommandsBox = null;
            BusyIndicator = null;
        }
    }
}