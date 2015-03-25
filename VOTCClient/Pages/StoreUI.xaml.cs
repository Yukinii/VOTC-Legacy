using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using VOTCClient.Core;
using VOTCClient.Core.Cache;
using VOTCClient.Core.IO;
using Path = System.IO.Path;

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
    internal class StoreItem
    {
        public ImageBrush Icon { get; set; }
        public string Title { get; set; }
    }

    internal partial class StoreUi
    {
        private List<string> _items = new List<string>();

        public StoreUi()
        {
            InitializeComponent();
            ListBox.SelectedItem = Kernel.StoreCategory;
            if(Kernel.StoreWindow==null)
                return;
            Kernel.StoreWindow.Title = "VOTC Store - " + Enum.GetName(typeof (Category), Kernel.StoreCategory);
        }

        private async Task RequestScriptsAsync()
        {
            try
            {
                ListBox.IsEnabled = false;
                BusyIndicator.IsBusy = true;
                _items = await Kernel.Channel.GetNewestScriptsAsync("");
                BusyIndicator.IsBusy = false;
            }
            finally
            {
                ListBox.IsEnabled = true;
            }
        }

        private async void scripts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            try
            {
                var container = (StoreItem) ((ListBox) sender).SelectedItem;
                Kernel.StoreWindow.Content = new StoreDetailView(await JsonConvert.DeserializeObjectAsync(await Kernel.Channel.DownloadScriptAsync(container.Title, "")));
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
                MessageBox.Show("Sorry, this profile could not be opened.", "FAIL");
            }
            finally
            {
                BusyIndicator.IsBusy = false;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RequestScriptsAsync();
            Scripts.Items.Clear();
            foreach (var T in _items.Select(item => new StoreItem {Title = item}))
            {
                Scripts.Items.Add(T);
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            var replace = ListBox.SelectedItem.ToString().Replace("System.Windows.Controls.ListBoxItem: ", "");
            switch (replace)
            {
                case "All Scripts":
                {
                    Kernel.StoreCategory = Category.None;
                    break;
                }
                case "Best Rated":
                {
                    Kernel.StoreCategory = Category.BestRated;
                    break;
                }
                case "Featured":
                {
                    Kernel.StoreCategory = Category.Featured;
                    break;
                }
                case "Updated":
                {
                    Kernel.StoreCategory = Category.Updated;
                    break;
                }
                case "Newest":
                {
                    Kernel.StoreCategory = Category.New;
                    break;
                }
                case "Most Downloaded":
                {
                    Kernel.StoreCategory = Category.MostDownloaded;
                    break;
                }
                case "Potentially Harmful":
                {
                    Kernel.StoreCategory = Category.Harmful;
                    break;
                }
            }
            Kernel.StoreWindow.Title = "VOTC Store - " + Enum.GetName(typeof (Category), Kernel.StoreCategory);
            Page_Loaded(null, null);
        }

        private async void StoreItemLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                string imageFilePath = "";
                try
                {
                    imageFilePath = await StoreCache.CacheLookup((((StackPanel) e.Source).Children[1] as TextBlock)?.Text);
                }
                catch
                {
                    imageFilePath = "Ressources/script.png";
                }
                var stream = File.OpenRead(imageFilePath);
                //bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                stream.Close();
                stream.Dispose();
                var brush = new ImageBrush {Stretch = Stretch.UniformToFill, ImageSource = bitmap};
                ((Ellipse) ((StackPanel) e.Source).Children[0]).Fill = brush;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}