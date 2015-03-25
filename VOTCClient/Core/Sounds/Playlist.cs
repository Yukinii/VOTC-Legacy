using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using AxWMPLib;
using VOTCClient.Core.Speech;
using WMPLib;

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

namespace VOTCClient.Core.Sounds
{
    public class Playlist :IDisposable
    {
        public bool PlayCustomSounds { get; internal set; }

        public bool SongEnded { get; private set; } = true;
        public Timer CheckSong { get; }
        public List<string> SongsInPlaylist = new List<string>();
        public int Index { get; private set; }
        public AxWindowsMediaPlayer MediaPlayer { get; }

        internal Playlist()
        {
            try
            {
                if (MediaPlayer != null)
                    return;
                var host = new WindowsFormsHost();
                MediaPlayer = new AxWindowsMediaPlayer();
                host.Child = MediaPlayer;
                MediaPlayer.CreateControl();
                IContainer playComponents = new Container();
                CheckSong = new Timer(playComponents);
                CheckSong.Tick += CheckSong_Tick;
                CheckSong.Interval = 500;
                MediaPlayer.PlayStateChange += MediaPlayer_PlayStateChange;
                Play();
            }
            catch
            {
                MessageBox.Show(@"
Windows Media Player has to be installed in order for VOTC to work
I'll show you how to install it, please install it asap. VOTC might be really unstable in this state.",@"FAIL");
                Process.Start("http://www.wikihow.com/Reinstall-Windows-Media-Player");
            }
        }
        
        public void AddSongs(IEnumerable<string> songs)
        {
            foreach (var T in songs)
            {
                AddSong(T);
            }
        }

        public void AddSong(string song)
        {
            SongsInPlaylist.Add(song);
            if(MediaPlayer == null)
                return;
            if (MediaPlayer.playState != WMPPlayState.wmppsPlaying)
                NextSong();
        }

        public async void DeleteSong()
        {
            if (MediaPlayer == null)
                return;
            await Task.Run(() =>
            {
                TextToSpeech.Speak("Hacking file permissions. ");
                var path = MediaPlayer.URL;
                SongsInPlaylist.Remove(path);
                File.Delete(path);
                TextToSpeech.Speak("File Deleted!");
            });
        }

        public void DeletePlaylist()
        {
            MediaPlayer?.Ctlcontrols.stop();
            SongsInPlaylist.Clear();
            Index = 0;
        }
        internal int Volume
        {
            set
            {
                if (MediaPlayer != null)
                    MediaPlayer.settings.volume = value;
            }
            get { return MediaPlayer?.settings.volume ?? 0; }
        }

        public void Play()
        {
            if (MediaPlayer == null)
                return;
            if (SongsInPlaylist.Count <= 0) return;
            if (SongsInPlaylist[Index] != null)
            {
                MediaPlayer.URL = SongsInPlaylist[Index];
            }
        }
        public void Play(int slot)
        {
            if (MediaPlayer == null)
                return;
            if (SongsInPlaylist[slot - 1] != null)
                MediaPlayer.URL = SongsInPlaylist[slot - 1];
        }
        public void Play(string name)
        {
            if (MediaPlayer == null)
                return;
            var slot = SongsInPlaylist.BinarySearch(name, null);
            if (slot >= 0 && slot < SongsInPlaylist.Count)
            {
                MediaPlayer.URL = SongsInPlaylist[slot];
            }
            else
            {
                AddSong(name);
                MediaPlayer.URL = name;
            }
        }

        public void Pause() => MediaPlayer?.Ctlcontrols.pause();

        public void Resume() => MediaPlayer?.Ctlcontrols.play();

        public void Stop() => MediaPlayer?.Ctlcontrols.stop();

        public void NextSong()
        {
            if (MediaPlayer == null)
                return;
            if (Index == SongsInPlaylist.Count - 1)
                return;

            Index++;
            MediaPlayer.Ctlcontrols.stop();
            MediaPlayer.URL = SongsInPlaylist[Index];
            MediaPlayer.Ctlcontrols.play();
        }

        public string GetCurrentMediaNameWithoutExtension() => Path.GetFileNameWithoutExtension(MediaPlayer.URL);

        public string GetCurrentMediaName() => Path.GetFileName(MediaPlayer.URL);

        public void PrevSong()
        {
            if (MediaPlayer == null)
                return;
            if (Index != 0)
            {
                Index--;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = SongsInPlaylist[Index];
                MediaPlayer.Ctlcontrols.play();
            }
            else
            {
                Index = SongsInPlaylist.Count - 1;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = SongsInPlaylist[Index];
                MediaPlayer.Ctlcontrols.play();
            }
        }
        void CheckSong_Tick(object sender, EventArgs e)
        {
            if (MediaPlayer == null)
                return;
            if (SongEnded)
            {
                NextSong();
                SongEnded = false;
                CheckSong.Stop();
            }
            Kernel.UI.UpdateMediaProgress(MediaPlayer.Ctlcontrols.currentPosition);
        }

        void MediaPlayer_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent wmpocxEventsPlayStateChangeEvent)
        {
            if (MediaPlayer == null)
                return;
            switch (MediaPlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                {
                    CheckSong.Start();
                    Kernel.UI.Dispatcher.BeginInvoke(new Action(() => Kernel.UI.MusicProgressBar.Maximum = MediaPlayer.currentMedia.duration), DispatcherPriority.Background);
                    break;
                }
                case WMPPlayState.wmppsMediaEnded:
                    SongEnded = true;
                    CheckSong.Start();
                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CheckSong.Dispose();
        }
    }
}
