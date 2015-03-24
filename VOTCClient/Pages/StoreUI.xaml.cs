using System;
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
using VOTCClient.Core.Extensions;
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
        private readonly ConcurrentList<string> _items = new ConcurrentList<string>();

        public StoreUi()
        {
            InitializeComponent();
            ListBox.SelectedItem = Kernel.StoreCategory;
            if(Kernel.StoreWindow==null)
                return;
            Kernel.StoreWindow.Title = "VOTC Store - " + Enum.GetName(typeof (Category), Kernel.StoreCategory);
        }

        private async Task GetShit(string category)
        {
            await Task.Run(async () =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var New = await client.DownloadStringTaskAsync(Kernel.RemoteHost + "get" + category + ".bitflash");

                        New = New.Replace("Â", "");
                        if (New.Length > 0)
                            New = New.Remove(New.Length - 1, 1);
                        if (New.Length > 0)
                        {
                            var items = New.Split('§');
                            foreach (var item in items)
                                _items.Add(item);
                        }
                        _items.Remove(string.Empty);
                    }
                }
                catch
                {
                    Dispatcher.Invoke(() => { BusyIndicator.IsBusy = false; });
                }
            });
        }

        private async void scripts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            await Task.Delay(10);
            try
            {
                var container = (StoreItem) ((ListBox) sender).SelectedItem;
                using (var client = new WebClient())
                {
                    var json = await client.DownloadStringTaskAsync(Kernel.RemoteHost + container.Title);
                    Kernel.StoreWindow.Content = new StoreDetailView(await JsonConvert.DeserializeObjectAsync(json));
                }
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
                MessageBox.Show("Sorry, this profile could not be opened.", "FAIL");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Scripts.Items.Clear();
            _items.Clear();
            switch (Kernel.StoreCategory)
            {
                case Category.Featured:
                {
                    await GetShit("featured");
                    break;
                }
                case Category.Harmful:
                {
                    await GetShit("harmful");
                    break;
                }
                case Category.New:
                {
                    await GetShit("new");
                    break;
                }
                case Category.None:
                {
                    await GetShit("none");
                    break;
                }
                case Category.Popular:
                {
                    await GetShit("popular");
                    break;
                }
                case Category.Updated:
                {
                    await GetShit("updated");
                    break;
                }
                case Category.MostDownloaded:
                {
                    await GetShit("mostdownloaded");
                    break;
                }
            }
            foreach (var T in _items.Select(item => new StoreItem {Title = item}))
            {
                Scripts.Items.Add(T);
            }
            BusyIndicator.IsBusy = false;
        }

        public static string CacheLookup(string name)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Cache\\Store\\"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Cache\\Store\\");

            foreach (var cacheFile in Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Cache\\Store\\").Where(cachedFile => Path.GetFileName(cachedFile) == name + ".badge"))
            {
                return cacheFile;
            }
            using (var client = new WebClient())
            {
                var imageUrl = client.DownloadString(Kernel.RemoteHost + name + ".badge");
                File.WriteAllBytes("Cache\\Store\\" + name + ".badge", client.DownloadData(imageUrl));
            }
            return "Cache\\Store\\" + name + ".badge";
        }

        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BusyIndicator.IsBusy = true;
            await Task.Delay(10);
            if (!ListBox.IsEnabled)
                return;
            ListBox.IsEnabled = false;
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
            ListBox.IsEnabled = true;
        }

        private void StoreItemLoaded(object sender, RoutedEventArgs e)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            try
            {
                bitmap.UriSource = new Uri(CacheLookup((((StackPanel) e.Source).Children[1] as TextBlock)?.Text), UriKind.RelativeOrAbsolute);
            }
            catch
            {
                bitmap.UriSource = new Uri("Ressources/script.png", UriKind.Relative);
            }
            bitmap.EndInit();
            var brush = new ImageBrush {Stretch = Stretch.UniformToFill, ImageSource = bitmap};
            ((Ellipse) ((StackPanel) e.Source).Children[0]).Fill = brush;
        }
    }
}