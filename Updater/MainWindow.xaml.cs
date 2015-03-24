using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

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
namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public int LocalVersion;
        public int RemoteVersion;

        public MainWindow()
        {
            InitializeComponent();
            Height = 70;
            Label.Content = "Checking for new update...";
        }

        public async Task<int> GetRemoteVersion()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var versionString = await client.DownloadStringTaskAsync("http://votc.xyz/downloads/Version.txt");
                    return int.Parse(versionString);
                }
            }
            catch
            {
                MessageBox.Show("Failed to read remote Version file. Updating canceled.");
                return 0;
            }
        }
        public async Task<int> GetLocalVersion()
        {
            try
            {
                if (!File.Exists("Version.txt"))
                    File.Create("Version.txt").Close();

                using (var stream = new StreamReader("Version.txt"))
                {
                    var versionString = await stream.ReadToEndAsync();
                    return int.Parse(versionString);
                }
            }
            catch
            {
                return 0;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RemoteVersion = await GetRemoteVersion();
            LocalVersion = await GetLocalVersion();
            Label.Content = string.Format("Current Version: {0} Remote Version: {1}", LocalVersion, RemoteVersion);
            if (RemoteVersion > LocalVersion)
                UpdateApplication();
            else
                StartApplication();
        }

        private void StartApplication()
        {
            if (File.Exists("VOTC Latest Development Version.zip"))
                File.Delete("VOTC Latest Development Version.zip");
            try
            {
                Process.Start("VOTC.exe","null");
                Environment.Exit(0);
            }            
            catch { MessageBox.Show("Application not found.."); }
        }

        private async void UpdateApplication()
        {
            try
            {
                using (var client = new WebClient())
                {
                    TextBox.Text = await client.DownloadStringTaskAsync("http://votc.xyz/downloads/News.txt");
                    while (Height < 176)
                    {
                        Height++;
                        await Task.Delay(4);
                    }
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    var data = await client.DownloadDataTaskAsync("http://votc.xyz/downloads/VOTC Latest Development Version.zip");
                    File.WriteAllBytes("VOTC Latest Development Version.zip",data);
                    using (var archive =new ZipArchive(File.Open("VOTC Latest Development Version.zip", FileMode.Open)))
                    {
                        archive.ExtractToDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),true);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to update. Try again later.");
            }
            StartApplication();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Label2.Content = string.Format("Downloaded {0} of {1} KB", e.BytesReceived / 1024, e.TotalBytesToReceive / 1024);
                Progressbar.Value = e.ProgressPercentage;
            }));
        }
    }
}