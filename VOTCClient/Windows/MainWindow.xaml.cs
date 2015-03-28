using System; //VOTC LEGACY
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using InputManager;
using Microsoft.VisualBasic;
using VOTCClient.Core;
using VOTCClient.Core.Error;
using VOTCClient.Core.Extensions;
using VOTCClient.Core.External.Chatbot;
using VOTCClient.Core.Network;
using VOTCClient.Core.Sounds;
using VOTCClient.Core.Speech;
using VOTCClient.Pages;
using Keyboard = InputManager.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

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
    public partial class MainWindow
    {
        internal readonly KonamiSequence Sequence = new KonamiSequence();
        private readonly FileSystemWatcher _watchedog = new FileSystemWatcher("Scripts");
        public static bool IsAdministrator() => (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);

        public MainWindow()
        {
            InitializeComponent();
            Config.LoadConfig();
            Bot.Initialize();
            Kernel.Window = this;
            Content = new Ui();
            SizeToContent = SizeToContent.WidthAndHeight;
            if (!IsAdministrator())
                MessageBox.Show("I will probably not work if you don't start me with Admin rights. (Right click VOTCClient.exe, then pick run as administrator)", "ERROR");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are trying to run a pre 0.1.1.0 version." +
                            "\n\nDue to massive code changes, this version is now canceled. " +
                            "It may still receive MANUAL updates on request but I hightly suggest to upgrade past version 0.1.1.0+" +
                            "\n\nHave fun using VOTC, and good luck!",
                            "This version has been discontinued");
            if (Kernel.Channel == null)
                Kernel.Channel = new LogicClient("MetadataExchangeHttpBinding_ILogic", "http://eubfwcf.cloudapp.net/RemoteExecute/mex");
            TextToSpeech.PrepareTextToSpeech();
            Keyboard.KeyPress(Keys.Alt);
            Kernel.Music = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "*.mp3").ToList();
            var remove = Kernel.Music.Where(entry => entry.Contains("AlbumArt_")).ToList();
            foreach (var entry in remove)
            {
                Kernel.Music.Remove(entry);
            }
            _watchedog.Changed += WatchedogEvent;
            _watchedog.Created += WatchedogEvent;
            _watchedog.Deleted += WatchedogEvent;
            _watchedog.Renamed += WatchedogEvent;
            _watchedog.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            _watchedog.Filter = "*.cs";
            _watchedog.EnableRaisingEvents = true;
            CatchAll.Init();
            Kernel.InitializeScriptEngine();
            if(Kernel.UsePtt)
                Kernel.KeyboardHook.Hook();
            if (Kernel.Tracking)
                Tracking.Start();
            if (string.IsNullOrEmpty(Kernel.CustomName))
                Kernel.CustomName = Interaction.InputBox("How do you want to be called?", "Chose your name", "Chief");
            
            Kernel.UI.DisplayCmd("READY!");
            TextToSpeech.Speak(await Kernel.Channel.GetQuoteAsync());
        }

        private static void WatchedogEvent(object sender, FileSystemEventArgs e)
        {
            if (!Kernel.DeveloperMode) 
                return;

            Kernel.UI.DisplayCmd("[FileSystemWatcher] File changed - recompiling.. " + e.Name, false);
            Kernel.InitializeScriptEngine();
        }
        
        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            Config.SaveConfig();
            if (Content.GetType() == typeof (Settings))
            {
                Kernel.Window.Content = new Ui();
                e.Cancel = true;
            }
            else
            {
                Kernel.KeyboardHook.Unhook();
                MouseHook.UninstallHook();
                Kernel.KeepThreadsRunning = false;
                Config.SaveConfig();
                Tracking.Stop();
                if (Kernel.ChatWindow != null)
                    Kernel.ChatWindow.Close();
                await Task.Delay(10000);
                Environment.Exit(0);
            }
            GC.Collect(0, GCCollectionMode.Forced);
            GC.Collect(1, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);
            GC.Collect(3, GCCollectionMode.Forced);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Sequence.IsCompletedBy((Keys)KeyInterop.VirtualKeyFromKey(e.Key))) return;
            if (!TextToSpeech.UseTts)
            {
                TextToSpeech.UseTts = true;
                TextToSpeech.Speak("Level up!");
                TextToSpeech.UseTts = false;
                Kernel.Donated = true;
            }
            else
                TextToSpeech.Speak("Level up!");
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (Kernel.ChatWindow != null)
            {
                Kernel.ChatWindow.Top = Top + Height - 8;
                Kernel.ChatWindow.Left = Left + 8;
            }
            if (Kernel.CommandWindow != null)
            {
                Kernel.CommandWindow.Width = Width;
                Kernel.CommandWindow.Top = Top - Kernel.CommandWindow.Height + 8;
                Kernel.CommandWindow.Left = Left;
            }
        }
    }
}
