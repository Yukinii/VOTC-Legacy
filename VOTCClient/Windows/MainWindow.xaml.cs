using System;
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
using VOTCClient.Core.Speech;
using VOTCClient.Pages;
using Keyboard = InputManager.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            TextToSpeech.Speak("A I Initialized! Hello " + Kernel.CustomName + " I hope you had a nice day so far, well then, shall we start?");
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
               //Kernel.UI.TitleUpdater.Stop();
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
            GC.Collect(10, GCCollectionMode.Forced);
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
            if (Kernel.ChatWindow == null) return;
            Kernel.ChatWindow.Top = Top + Height;
            Kernel.ChatWindow.Left = Left;
        }
    }
}
