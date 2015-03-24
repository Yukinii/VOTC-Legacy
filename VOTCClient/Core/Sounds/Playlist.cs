using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
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

        bool _songEnded = true;
        readonly Timer _checkSong;
        public List<string> SongsInPlaylist = new List<string>();
        int _index;
        readonly WindowsMediaPlayer _mediaPlayer;

        internal Playlist()
        {
            try
            {
                _mediaPlayer = new WindowsMediaPlayer();
                //_MediaPlayer.CreateControl();
                IContainer playComponents = new Container();
                _checkSong = new Timer(playComponents);
                _checkSong.Tick += CheckSong_Tick;
                _checkSong.Interval = 500;
                _mediaPlayer.PlayStateChange += MediaPlayer_PlayStateChange;
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
            if(_mediaPlayer == null)
                return;
            if (_mediaPlayer.playState != WMPPlayState.wmppsPlaying)
                NextSong();
        }

        public async void DeleteSong()
        {
            if (_mediaPlayer == null)
                return;
            await Task.Run(() =>
            {
                TextToSpeech.Speak("Hacking file permissions. ");
                var path = _mediaPlayer.URL;
                SongsInPlaylist.Remove(path);
                File.Delete(path);
                TextToSpeech.Speak("File Deleted!");
            });
        }

        public void DeletePlaylist()
        {
            _mediaPlayer?.controls.stop();
            SongsInPlaylist.Clear();
            _index = 0;
        }

        internal int Volume
        {
            set
            {
                if (_mediaPlayer != null)
                    _mediaPlayer.settings.volume = value;
            }
            get { return _mediaPlayer?.settings.volume ?? 0; }
        }

        public void Play()
        {
            if (_mediaPlayer == null)
                return;
            if (SongsInPlaylist.Count <= 0) return;
            if (SongsInPlaylist[_index] != null)
            {
                _mediaPlayer.URL = SongsInPlaylist[_index];
            }
        }
        public void Play(int slot)
        {
            if (_mediaPlayer == null)
                return;
            if (SongsInPlaylist[slot - 1] != null)
                _mediaPlayer.URL = SongsInPlaylist[slot - 1];
        }
        public void Play(string name)
        {
            if (_mediaPlayer == null)
                return;
            var slot = SongsInPlaylist.BinarySearch(name, null);
            if (slot >= 0 && slot < SongsInPlaylist.Count)
            {
                _mediaPlayer.URL = SongsInPlaylist[slot];
            }
            else
            {
                AddSong(name);
                _mediaPlayer.URL = name;
            }
        }

        public void Pause() => _mediaPlayer?.controls.pause();

        public void Resume() => _mediaPlayer?.controls.play();

        public void Stop() => _mediaPlayer?.controls.stop();

        public void NextSong()
        {
            if (_mediaPlayer == null)
                return;
            if (_index == SongsInPlaylist.Count - 1)
                return;

            _index++;
            _mediaPlayer.controls.stop();
            _mediaPlayer.URL = SongsInPlaylist[_index];
            _mediaPlayer.controls.play();
        }

        public string GetCurrentMediaNameWithoutExtension() => Path.GetFileNameWithoutExtension(_mediaPlayer.URL);

        public string GetCurrentMediaName() => Path.GetFileName(_mediaPlayer.URL);

        public void PrevSong()
        {
            if (_mediaPlayer == null)
                return;
            if (_index != 0)
            {
                _index--;
                _mediaPlayer.controls.stop();
                _mediaPlayer.URL = SongsInPlaylist[_index];
                _mediaPlayer.controls.play();
            }
            else
            {
                _index = SongsInPlaylist.Count - 1;
                _mediaPlayer.controls.stop();
                _mediaPlayer.URL = SongsInPlaylist[_index];
                _mediaPlayer.controls.play();
            }
        }
        void CheckSong_Tick(object sender, EventArgs e)
        {
            if (_mediaPlayer == null)
                return;
            if (_songEnded)
            {
                NextSong();
                _songEnded = false;
                _checkSong.Stop();
            }
            Kernel.UI.UpdateMediaProgress(_mediaPlayer.controls.currentPosition);
        }

        void MediaPlayer_PlayStateChange(int e)
        {
            if (_mediaPlayer == null)
                return;
            switch (_mediaPlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                {
                    _checkSong.Start();
                    Kernel.UI.Dispatcher.BeginInvoke(new Action(() => Kernel.UI.MusicProgressBar.Maximum = _mediaPlayer.currentMedia.duration), DispatcherPriority.Background);
                    break;
                }
                case WMPPlayState.wmppsMediaEnded:
                    _songEnded = true;
                    _checkSong.Start();
                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _checkSong.Dispose();
        }
    }
}
