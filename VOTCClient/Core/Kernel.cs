using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Windows.Input;
using System.Windows.Media;
using Facebook;
using TweetSharp;
using VOTCClient.Core.Extensions;
using VOTCClient.Core.Hook;
using VOTCClient.Core.IO;
using VOTCClient.Core.Scripts;
using VOTCClient.Core.Sounds;
using VOTCClient.Core.Speech;
using VOTCClient.Pages;
using VOTCClient.Windows;
using Keyboard = InputManager.Keyboard;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace VOTCClient.Core
{
    internal enum Category
    {
        None = 0,
        Featured = 1,
        New = 2,
        Harmful = 3,
        Popular = 4,
        Updated = 5,
        MostDownloaded = 6,
        BestRated = 7
    }
    public static class Kernel
    {
        #region private accessors
        private static string _activeProfile;
        private static bool _usePtt;
        private static bool _locked;
        #endregion

        public static readonly KeyboardHook KeyboardHook = new KeyboardHook();
        public static readonly Random Random = new Random(Environment.TickCount);

        internal static volatile bool KeepThreadsRunning = true;
        internal static volatile bool Ready;


        internal static bool UsePtt
        {
            get { return _usePtt; }
            set
            {
                _usePtt = value;
                if(UI == null)return;
                UI.Ellipse.Fill = !value ? new SolidColorBrush(Color.FromRgb(39, 245, 46)) : new SolidColorBrush(Color.FromRgb(245, 39, 39));
            }
        }
        internal static bool Recognize;
        internal static bool DebugMode = true;
        internal static bool DeveloperMode;
        internal static bool ChatBotActive;
        internal static bool Donated;
        internal static bool Tracking;
        internal static bool PttKeyDown;
        internal static bool EnableDeleteMusicVoiceCommand;

        public static string CustomName;

        //internal static string RemoteHost = "http://192.168.0.2:141/";
        internal const string RemoteHost = "http://79.133.51.71:141/";
        internal static string FacebookAccessToken;
        internal static string ProfilePicture;
        internal static string FacebookName;
        internal static string TwitterUsername;
        internal static string TwitterToken;
        internal static string TwitterSecret; 

        public static Ui UI;

        internal static MainWindow Window;
        internal static Store StoreWindow;
        internal static ProfileLoadWindow ProfileWindow;
        internal static ScriptUploader ScriptUploaderWindow;

        public static readonly Playlist Playlist=new Playlist();

        internal static ScriptEngine UserScriptEngine;
        internal static ScriptInfo ScriptInfos;

        internal static FacebookClient FacebookClient;
        internal static FacebookPost FacebookPostWindow;
        internal static TwitterService TwitterClient;
        internal static FacebookTwitterAuth AuthWindow;
        internal static ChatWindow ChatWindow;
        
        public static Key PttKey;
        
        internal static Category StoreCategory = Category.New;
        internal static List<string> Music;
        public static readonly ConcurrentDictionary<string, string> ChatCache = new ConcurrentDictionary<string, string>();



        internal static string ActiveProfile
        {
            get { return _activeProfile; }
            set
            {
                if (value == "" || UserScriptEngine == null)
                    return;
                _activeProfile = value;
                ScriptInfos = UserScriptEngine.SetUp();
                if (ScriptInfos != null)
                    UI.DisplayCmd("Loaded Profile!",false);
            }
        }

        internal static bool Locked
        {
            get { return _locked; }
            set
            {
                _locked = value;

                if (Window == null)
                    return;

                if (value)
                    Window.Title = "VOTC > " + CustomName + " | Profile: " + Path.GetFileName(ActiveProfile) + " | Song: " + Playlist.GetCurrentMediaNameWithoutExtension();
                else
                    Window.Title = "VOTC > Locked. Say 'Unlock' to unlock voice commands.";
            }
        }

        public static bool IsInGame(string gameName)
        {
            var processes = Process.GetProcessesByName(gameName);
            return (from processID in processes let foregroundWindowID = NativeMethods.GetForegroundWindow() where foregroundWindowID == processID.MainWindowHandle select processID).Any();
        }

        internal static void InitializeScriptEngine()
        {
            var userScriptEngineSettings = new ScriptSettings
            {
                ScriptLocation = Environment.CurrentDirectory + @"\Scripts\"
            };
            UserScriptEngine = new ScriptEngine(userScriptEngineSettings);
        }
        internal static void KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                e.Handled = true;
                Keyboard.KeyDown(e.KeyCode);
                if (Recognize || PttKeyDown)
                    return;
                if (e.KeyValue != (int)PttKey.ToKey())
                    return;
                PttKeyDown = true;
                UI.Ellipse.Fill = new SolidColorBrush(Color.FromRgb(39,245,46));
                InternalSpeechRecognizer.Engine.RecognizeAsync(RecognizeMode.Multiple);
                Recognize = true;
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
        }
        internal static void KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                e.Handled = true;
                Keyboard.KeyUp(e.KeyCode);
                if (!Recognize || !PttKeyDown)
                    return;
                if (e.KeyValue != (int)PttKey.ToKey())
                    return;
                PttKeyDown = false;
                UI.Ellipse.Fill = new SolidColorBrush(Color.FromRgb(245, 39, 39));
                InternalSpeechRecognizer.Engine.RecognizeAsyncStop();
                Recognize = false;
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
        }
    }
}
