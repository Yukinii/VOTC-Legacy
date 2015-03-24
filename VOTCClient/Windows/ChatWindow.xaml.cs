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
using VOTCClient.Core.IO;
using Path = System.IO.Path;

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
            Width = mainWindow.Width-4;
            Top = mainWindow.Top + mainWindow.Height;
            Left = mainWindow.Left;

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

        private async void SendMessage(string text)
        {
            try
            {
                if(!Client.Connected)
                    await Client.ConnectAsync("79.133.51.71", 700);
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

                var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right};
                var block2 = new TextBlock { Text = "  ", VerticalAlignment = VerticalAlignment.Center };

                panel.Children.Add(entry.Text);
                panel.Children.Add(block2);
                panel.Children.Add(entry.Img);
                ChatBox.Children.Add(panel);
                ScrollViewer.ScrollToEnd();
            }
            catch
            {
                MessageBox.Show("Failed to send the message. Server did not respond", "Fail");
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Client.ConnectAsync("79.133.51.71", 700);
                Stream = Client.GetStream();
                SendMessage("Connected!");
                while (true)
                {
                    try
                    {
                        Stream = Client.GetStream();
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
                        Dispatcher.Invoke(() =>
                        {
                            SoundThingy.Stop();
                            SoundThingy.Position = TimeSpan.Zero;
                            SoundThingy.Play();
                            var panel = new StackPanel { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Color.FromRgb(194,224,224)), MaxWidth = Width};
                            var block = new TextBlock {Text = " said: ", VerticalAlignment = VerticalAlignment.Center};
                            var block2 = new TextBlock {Text = "  ", VerticalAlignment = VerticalAlignment.Center};
                            panel.Children.Add(entry.Img);
                            panel.Children.Add(block2);
                            panel.Children.Add(entry.UserName);
                            panel.Children.Add(block);
                            panel.Children.Add(entry.Text);
                            ChatBox.Children.Add(panel);
                            ScrollViewer.ScrollToEnd();
                        }, DispatcherPriority.Normal);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not connect to the chat.", "fail");
                        IoQueue.Add(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to the chat.", "fail");
                IoQueue.Add(ex);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Kernel.ChatWindow = null;
            Close();
        }

        private void InputboBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SendMessage("Disconnected!");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SoundThingy.Volume = 0;
        }

        private void Mutebox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SoundThingy.Volume = 0.3;
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
            var cachedImage = CacheLookup(imageUrl);

            var myImageSource = new ImageBrush {ImageSource = new BitmapImage(new Uri(cachedImage == "" ? imageUrl : cachedImage))};
            Img.Fill = myImageSource;
            Img.Width = 42;
            Img.Height = 42;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.TextWrapping = TextWrapping.Wrap;
            Img.VerticalAlignment = VerticalAlignment.Center;
            UserName.VerticalAlignment = VerticalAlignment.Center;
            
            UserName.FontWeight = FontWeights.Bold;
            UserName.FontSize = 13.0;
            Text.FontSize = 13.0;
            if (!send)
                CacheImage(imageUrl, username);
        }

        private static void CacheImage(string imageUrl,string username)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png"))
                            client.DownloadFile(imageUrl, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png");
                        else
                            client.DownloadFile(imageUrl, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png" + Kernel.Random.Next(1, 200000000));
                    }
                    Kernel.ChatCache.TryAdd(imageUrl,Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location )+ @"\Cache\ChatCache\" + username + ".png");
                }
                catch (Exception ex)
                {
                    IoQueue.Add(ex);
                }
            });
        }

        public string CacheLookup(string url)
        {
            string value;
            return Kernel.ChatCache.TryGetValue(url, out value) ? value : "";
        }
    }
}
