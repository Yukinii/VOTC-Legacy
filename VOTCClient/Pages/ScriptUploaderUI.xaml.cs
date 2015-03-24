using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Compression;
using Microsoft.Win32;
using VOTCClient.Core;
using VOTCClient.Core.External.Azure;
using VOTCClient.Core.IO;
using Image = System.Drawing.Image;

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
    public partial class ScriptUploaderUi
    {
        readonly string _scriptFile;
        private readonly bool _upload;
        public ScriptUploaderUi(string file, bool upload)
        {
            InitializeComponent();
            _upload = upload;
            Kernel.ScriptUploaderWindow.Title = Path.GetFileName(file);
            _scriptFile = file;
        }

        private async void submitbtn_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = File.ReadAllText(@"Scripts\" + _scriptFile);
            if (fileContent.Contains("using System"))
            {
                if (Nametxtbx.Text != string.Empty && Authortxtbx.Text != string.Empty && Forapptxtbx.Text != string.Empty && Reqforetxtbx.Text != string.Empty && Descriptiontxtbx.Text != string.Empty && Commandstxtbx.Text != string.Empty && Passwordbox.Text != String.Empty && Storebadgebox.Text != string.Empty && Headerimagebox.Text != String.Empty)
                {
                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(Kernel.RemoteHost);
                        request.KeepAlive = false;
                        request.Method = "Post";
                        request.ContentType = "text/xml";
                        request.Proxy = null;

                        var builder = new StringBuilder();
                        builder.AppendLine(Nametxtbx.Text);
                        builder.AppendLine(Authortxtbx.Text);
                        builder.AppendLine(Forapptxtbx.Text);
                        builder.AppendLine(Reqforetxtbx.Text);
                        builder.AppendLine(Descriptiontxtbx.Text);
                        builder.AppendLine(Headerimagebox.Text);
                        builder.AppendLine(Storebadgebox.Text);
                        builder.AppendLine(Passwordbox.Text);
                        builder.AppendLine(Commandstxtbx.Text);
                        builder.AppendLine(fileContent);
                        if (_upload)
                        {
                            var data = Bit.CompressString(builder.ToString());
                            Kernel.ScriptUploaderWindow.Content = new UploadScriptProgress("Compressed " + builder.Length / 1024 + "kb into " + data.Length / 1024 + "kb using BitCompress!");
                            request.ContentLength = data.Length;
                            using (var reqStream = await request.GetRequestStreamAsync())
                            {
                                await reqStream.WriteAsync(data, 0, data.Length);
                                await reqStream.FlushAsync();

                                while (!Kernel.Ready)
                                    await Task.Delay(100);
                            }
                            if (fileContent.Contains("Socket") || fileContent.Contains("Listen"))
                            {
                                MessageBox.Show("Your script is potentially harmful so it will be manually reviewed and will only show up as Potentially Harmful Script in the meantime.", "Success!");
                            }
                            else
                                MessageBox.Show("Your script has been uploaded successfully.\n\nIf you can't instantly see it in the Gallery, the Filename was in use. Please rename your Script and Upload it again\nor use the correct password to update it!", "Success!");
                        }
                    }
                    catch (Exception ex)
                    {
                        IoQueue.Add(ex);
                        MessageBox.Show("The server f**ed up. Please try again later...", "Oh shit.");
                    }
                }
                else
                {
                    MessageBox.Show("You need to fill out every field!", "Lol nope.");
                }
            }
            else
                MessageBox.Show("You need to add 'using System;' to the top your Script!", "Lol nope.");
        }

        private async void Headerimagebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!Headerimagebox.Text.Contains("http://"))
                {
                    BusyIndicator.IsBusy = true;
                    await Task.Delay(100);
                    var fileInfo = new FileInfo(Headerimagebox.Text);
                    Kernel.ScriptUploaderWindow.Title = "Size: " + fileInfo.Length/1024 + "kbs";
                    try
                    {
                        Image img;
                        using (var msPng = new MemoryStream(File.ReadAllBytes(Headerimagebox.Text)))
                        {
                            img = Image.FromStream(msPng);
                        }
                        using (var msJpeg = new MemoryStream())
                        {
                            img.Save(msJpeg, ImageFormat.Jpeg);
                            File.WriteAllBytes(Headerimagebox.Text, msJpeg.ToArray());
                        }
                    }
                    catch { }
                    fileInfo = new FileInfo(Headerimagebox.Text);
                    Kernel.ScriptUploaderWindow.Title += " | New Size: " + fileInfo.Length / 1024 + "kbs";
                }
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Headerimagebox.Text, UriKind.Absolute);
                bitmap.EndInit();
                Headerimage.Source = bitmap;
               
                if (bitmap.Width > 1280 || bitmap.Height > 720)
                {
                    MessageBox.Show("Sorry, the image is too big. max is 1280x720");
                    return;
                }

                if (Headerimagebox.Text.Contains("cdn.votc.xyz") || Headerimagebox.Text == "")
                    return;

                var fileName = HashIt(Headerimagebox.Text) + ".png";
                await ImageUpload.Upload(Headerimagebox.Text, fileName);
                Headerimagebox.Text = "http://cdn.votc.xyz/votcstore/" + fileName;
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
            finally
            {
                BusyIndicator.IsBusy = false;
            }
        }

        private async void Storebadgebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!Storebadgebox.Text.Contains("http://"))
                {
                    BusyIndicator.IsBusy = true;
                    await Task.Delay(100);
                    var fileInfo = new FileInfo(Storebadgebox.Text);
                    Kernel.ScriptUploaderWindow.Title = "Size: " + fileInfo.Length / 1024 + "kbs";
                    var info = new ProcessStartInfo
                    {
                        FileName = "Ressources\\optipng.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "\"" + Storebadgebox.Text + "\" -o7"
                    };
                    using (var p = Process.Start(info))
                    {
                        p?.WaitForExit();
                    }
                    info = new ProcessStartInfo
                    {
                        FileName = "Ressources\\pngcrush.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "\"" + Storebadgebox.Text + "\" -brute"
                    };
                    using (var p = Process.Start(info))
                    {
                        p?.WaitForExit();
                    }
                    info = new ProcessStartInfo
                    {
                        FileName = "Ressources\\pngout",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "\"" + Storebadgebox.Text + "\" /y /r"
                    };
                    using (var p = Process.Start(info))
                    {
                        p?.WaitForExit();
                    }
                    await Task.Delay(100);
                    fileInfo = new FileInfo(Storebadgebox.Text);
                    Kernel.ScriptUploaderWindow.Title += " | New Size: " + fileInfo.Length / 1024 + "kbs";
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Storebadgebox.Text, UriKind.Absolute);
                bitmap.EndInit();
                BadgeimagImage.Fill = new ImageBrush(bitmap);

                if (bitmap.Width > 256 || bitmap.Height > 256 || bitmap.Width != bitmap.Height)
                {
                    MessageBox.Show("Sorry, either the image is too big or not a square. Make it 128x128 for best results, max is 256x256");
                    return;
                }

                if (Storebadgebox.Text.Contains("cdn.votc.xyz") || Storebadgebox.Text == "")
                    return;

                var fileName = HashIt(Storebadgebox.Text) + ".png";
                await ImageUpload.Upload(Storebadgebox.Text, fileName);
                Storebadgebox.Text = "http://cdn.votc.xyz/votcstore/" + fileName;
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
            finally
            {
                BusyIndicator.IsBusy = false;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Kernel.ScriptUploaderWindow.Title != null)
                        Nametxtbx.Text = Kernel.ScriptUploaderWindow.Title.Replace(".cs", "");
                    Kernel.ActiveProfile = _scriptFile;
                    Nametxtbx.Text = Kernel.ScriptInfos.ScriptName;
                    Authortxtbx.Text = Kernel.ScriptInfos.Author;
                    Forapptxtbx.Text = Kernel.ScriptInfos.ProcessName;
                    Reqforetxtbx.Text = Kernel.ScriptInfos.RequireProcessInForeground.ToString();
                    Storebadgebox.Text = Kernel.ScriptInfos.StoreBadgeUrl;
                    if (Kernel.ScriptInfos.StoreBadgeUrl != "")
                        Headerimagebox.Text = Kernel.ScriptInfos.StoreHeaderUrl;
                    if (Kernel.ScriptInfos.StoreHeaderUrl != "")
                        Descriptiontxtbx.Text = Kernel.ScriptInfos.Description;
                    Commandstxtbx.Clear();
                    foreach (var voiceCommand in Kernel.ScriptInfos.Commands)
                    {
                        Commandstxtbx.Text += voiceCommand.Key + Environment.NewLine;
                    }
                    BusyIndicator.IsBusy = false;
                }), DispatcherPriority.DataBind);
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                Headerimagebox.Text = openFileDialog.FileName;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                Storebadgebox.Text = openFileDialog.FileName;
        }
        public string HashIt(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }
    }
}