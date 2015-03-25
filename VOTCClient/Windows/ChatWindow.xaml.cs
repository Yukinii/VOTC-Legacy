using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
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
namespace VOTCClient.Windows
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        public Task ReceiveThread;
        public TcpClient Client = new TcpClient();
        public NetworkStream Stream;
        public ChatWindow(Window mainWindow)
        {
            InitializeComponent();
            Width = mainWindow.Width-16;
            Top = mainWindow.Top + mainWindow.Height-8;
            Left = mainWindow.Left+8;

            SoundThingy.LoadedBehavior = MediaState.Manual;
            SoundThingy.UnloadedBehavior = MediaState.Manual;
            SoundThingy.Source = new Uri("Sounds/newmsg.wav", UriKind.Relative);
            SoundThingy.Volume = 0.5;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(string.IsNullOrWhiteSpace(InputboBox.Text))
                    return;
                
                SendMessage(InputboBox.Text);
                InputboBox.Text = "";
            }
        }

        private async void SendMessage(string text,bool closing = false)
        {
            try
            {
                if(!Client.Connected)
                    await Client.ConnectAsync("eubfwcf.cloudapp.net", 700);
                var imageUrl = Kernel.ProfilePicture;
                if (string.IsNullOrEmpty(imageUrl))
                    imageUrl = "http://i.epvpimg.com/2Wrnc.png";
                var entry2 = new ChatEntryJson(imageUrl, text, Kernel.CustomName);

                var json = JsonConvert.SerializeObject(entry2);
                var buffer = Encoding.UTF8.GetBytes(json);

                var entryJson = await JsonConvert.DeserializeObjectAsync<ChatEntryJson>(json);

                var entry = new ChatEntry(entryJson.ImageUrl, entryJson.Text, entryJson.Username,true); 
                await Stream.WriteAsync(buffer, 0, buffer.Length);
                await Stream.FlushAsync();

                var panel = new StackPanel {MaxWidth = Kernel.UI.Width,Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right};
                var block2 = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = "  ", VerticalAlignment = VerticalAlignment.Center};

                panel.Children.Add(entry.Text);
                panel.Children.Add(block2);
                panel.Children.Add(entry.Img);
                ChatBox.Children.Add(panel);
                ScrollViewer.ScrollToEnd();
            }
            catch
            {
                if (closing)
                    return;
                MessageBox.Show("Failed to send the message. Server did not respond", "Fail");
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Kernel.FacebookName) && string.IsNullOrEmpty(Kernel.TwitterUsername))
            {
                MessageBox.Show("You have to login to facebook or twitter first so we can identify you.");
                Close();
                return;
            }
            try
            {
                Client.Connect("eubfwcf.cloudapp.net", 700);
                Stream = Client.GetStream();
                SendMessage("Connected!");
                while (true)
                {
                    try
                    {
                        var data = new byte[4096];
                        var bytes = await Stream.ReadAsync(data, 0, data.Length);
                        var json = Encoding.UTF8.GetString(data, 0, bytes).Trim().Replace("\0", string.Empty);
                        var j = json.Split('}')[0] + "}";
                        var entryJson = await JsonConvert.DeserializeObjectAsync<ChatEntryJson>(j);
                        if(entryJson == null)
                            continue;
                        
                        var entry = new ChatEntry(entryJson.ImageUrl, entryJson.Text, entryJson.Username, false);

                        if (entry.UserName.Text == Kernel.CustomName)
                            continue;
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SoundThingy.Stop();
                            SoundThingy.Position = TimeSpan.Zero;
                            SoundThingy.Play();
                            var panel = new StackPanel { MaxWidth = Kernel.UI.Width, Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Color.FromRgb(194,224,224))};
                            var block = new TextBlock { Text = " said: ", VerticalAlignment = VerticalAlignment.Center,HorizontalAlignment = HorizontalAlignment.Left, TextWrapping = TextWrapping.Wrap };
                            var block2 = new TextBlock { Text = "  ", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left,TextWrapping = TextWrapping.Wrap};
                            panel.Children.Add(entry.Img);
                            panel.Children.Add(block2);
                            panel.Children.Add(entry.UserName);
                            panel.Children.Add(block);
                            panel.Children.Add(entry.Text);
                            ChatBox.Children.Add(panel);
                            ScrollViewer.ScrollToEnd();
                        }), DispatcherPriority.Normal);
                    }
                    catch (Exception ex)
                    {
                        IoQueue.Add(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to the chat.", "fail");
                IoQueue.Add(ex);
                Close();
            }
        }

        private void InputboBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ChatCache.Clear();
            SendMessage("Disconnected!",true);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SoundThingy.Volume = 0;
        }

        private void Mutebox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SoundThingy.Volume = 0.3;
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Kernel.ChatWindow = null;
            Close();
        }
    }

    public class ChatEntryJson
    {
        public string ImageUrl;
        public string Text;
        public string Username;

        public ChatEntryJson(string imageUrl, string text, string username)
        {
            ImageUrl = imageUrl;
            Text = text;
            Username = username;
        }
    }
    public class ChatEntry
    {
        public Ellipse Img =new Ellipse();
        public TextBlock UserName = new TextBlock();
        public TextBlock Text = new TextBlock();

        public ChatEntry(string imageUrl, string text, string username,bool send)
        {
            Text.Text = text;
            UserName.Text = username;
            var cachedImage = ChatCache.CacheLookup(imageUrl, username);
            var bitmap = new BitmapImage();
            var stream = File.OpenRead(cachedImage);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();
            stream.Dispose();
            var myImageSource = new ImageBrush {ImageSource = bitmap };
            Img.Fill = myImageSource;
            Img.Width = 42;
            Img.Height = 42;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.HorizontalAlignment = HorizontalAlignment.Right;
            Text.TextWrapping = TextWrapping.Wrap;
            Text.MaxWidth = 420;
            Img.VerticalAlignment = VerticalAlignment.Center;
            Img.HorizontalAlignment = HorizontalAlignment.Right;
            UserName.VerticalAlignment = VerticalAlignment.Center;
            UserName.HorizontalAlignment = HorizontalAlignment.Right;

            UserName.FontWeight = FontWeights.Bold;
            UserName.FontSize = 14.0;
            Text.FontSize = 14.0;
            if (!send)
                ChatCache.CacheImage(imageUrl, username);
        }
    }
}
