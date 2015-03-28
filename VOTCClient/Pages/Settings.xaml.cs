using System; //VOTC LEGACY
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VOTCClient.Core;
using VOTCClient.Core.IO;
using VOTCClient.Core.Speech;

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
    public partial class Settings
    {
        bool _initialized;

        public Settings()
        {
            InitializeComponent();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(!_initialized)return;
            Confidencypercentage.Content = (int)Slider1.Value + "%";
            InternalSpeechRecognizer.MinimumConfidence = (int)Slider1.Value;
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_initialized) return;
            Volumepercentage.Content = (int)Slider2.Value + "%";
            Kernel.Playlist.Volume = (int)Slider2.Value;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized || UnknownCommandsCheckBox.IsChecked == null)
                return;
            InternalSpeechRecognizer.DisplayUnknownCommands = UnknownCommandsCheckBox.IsChecked.Value;
        }

        private void NaturalSpeakingCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            if (NaturalSpeakingCheckBox.IsChecked == null)
                return;
            
            InternalSpeechRecognizer.IsNaturalSpeaking = NaturalSpeakingCheckBox.IsChecked.Value;
            if (NaturalSpeakingCheckBox.IsChecked.Value)
            {
                InternalSpeechRecognizer.EnableNaturalSpeaking();
            }
            else
            {
                InternalSpeechRecognizer.DisableNaturalSpeaking();
            }
        }
        private void TextToSpeechCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized || TextToSpeechCheckBox.IsChecked == null)
                return;
            TextToSpeech.UseTts = TextToSpeechCheckBox.IsChecked.Value;
        }

        private void DebugModeCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            if (DebugModeCheckBox.IsChecked != null)
                Kernel.DebugMode = DebugModeCheckBox.IsChecked.Value;
        }
        private void voices_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized) return;
            TextToSpeech.Speaker.SelectVoice(VoiceBox.SelectedItem.ToString());
            TextToSpeech.Speak("Hello  " + Kernel.CustomName + " I'm " + VoiceBox.SelectedItem.ToString().Replace("IVONA 2", "").Replace("Microsoft", "").Replace("Desktop", ""));
            TextToSpeech.VoiceName = VoiceBox.SelectedItem.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Kernel.FacebookClient != null)
            {
                Kernel.FacebookAccessToken = "";
                Kernel.FacebookName = "";
                Kernel.FacebookClient = null;
                MessageBox.Show("Unlinked!", "Success");
            }
            else
            {
                MessageBox.Show("You are not logged into facebook.", "Fail");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Kernel.TwitterClient != null)
            {
                Kernel.TwitterClient = null;
                Kernel.TwitterUsername = "";
                Kernel.TwitterSecret = "";
                Kernel.TwitterToken = "";
                MessageBox.Show("Unlinked!", "Success");
            }
            else
            {
                MessageBox.Show("You are not logged into twitter.", "Fail");
            }
        }

        private void slider3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_initialized)
                return;
            TtsVolumeLabel.Content = (int) e.NewValue;
            TextToSpeech.Volume = (int) e.NewValue;
        }

        private void slider4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_initialized)
                return;
            TtsSpeedLabel.Content = (int)e.NewValue;
            TextToSpeech.Speed = (int) e.NewValue;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Kernel.Tracking = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Kernel.Tracking = false;
        }

        private void DevModeCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized) return;
            if (DeveloperModeCheckBox.IsChecked != null)
                Kernel.DeveloperMode = DeveloperModeCheckBox.IsChecked.Value;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_initialized) return;
            Kernel.PttKey = e.Key;
            PttKey.Text = Enum.GetName(typeof (Key), e.Key);
        }

        private void TexboxClick(object sender, MouseButtonEventArgs e) => PttKey.Text = "";

        private void Bla(object sender, TextChangedEventArgs e) => PttKey.Text = Enum.GetName(typeof (Key), Kernel.PttKey);

        private void Checkedchanged(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            if (PttCheckbox.IsChecked != null) Kernel.UsePtt = PttCheckbox.IsChecked.Value;
            if (Kernel.UsePtt)
            {
                InternalSpeechRecognizer.Engine.RecognizeAsyncStop();
                Kernel.KeyboardHook.Hook();
            }
            else
                Kernel.KeyboardHook.Unhook();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.facebook.com/voiceofthecitizens");
                Process.Start("https://www.facebook.com/bitflashLLC");
            }
            catch
            {
                // ignored
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://twitter.com/bitflashllc");
            }
            catch
            {
                // ignored
            }
        }

        private void training_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "C:\\windows\\sysnative\\speech\\speechux\\SpeechUXWiz.exe", Arguments = "UserTraining"
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
        }

        private void Microphoneset(object sender, RoutedEventArgs e)
        {
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = "RunDLL32 shell32.dll,Control_RunDLL mmsys.cpl,,1"
                    }
                };
                process.Start();
            }
            catch (Exception x)
            {
                IoQueue.Add(x);
            }
        }

        private void MusicdeleteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Kernel.EnableDeleteMusicVoiceCommand = true;
        }

        private void MusicdeleteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Kernel.EnableDeleteMusicVoiceCommand = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _initialized = true;
            MuicdeleteCheckBox.IsChecked = Kernel.EnableDeleteMusicVoiceCommand;
            Slider1.Value = InternalSpeechRecognizer.MinimumConfidence;
            Slider2.Value = Kernel.Playlist.Volume;
            UnknownCommandsCheckBox.IsChecked = InternalSpeechRecognizer.DisplayUnknownCommands;
            NaturalSpeakingCheckBox.IsChecked = InternalSpeechRecognizer.IsNaturalSpeaking;
            TextToSpeechCheckBox.IsChecked = TextToSpeech.UseTts;
            DebugModeCheckBox.IsChecked = Kernel.DebugMode;
            foreach (var voice in TextToSpeech.Speaker?.GetInstalledVoices())
            {
                VoiceBox.Items.Add(voice.VoiceInfo.Name);
            }
            if (VoiceBox.Items.Contains(TextToSpeech.VoiceName))
                VoiceBox.SelectedItem = TextToSpeech.VoiceName;
            Tracking.IsChecked = Kernel.Tracking;
            Scriptssocial.IsChecked = false;
            TtsSpeedSlider.Value = TextToSpeech.Speed;
            TtsVolumeSlider.Value = TextToSpeech.Volume;
            DeveloperModeCheckBox.IsChecked = Kernel.DeveloperMode;
            PttCheckbox.IsChecked = Kernel.UsePtt;
            PttKey.Text = Enum.GetName(typeof(Key), Kernel.PttKey);
        }
    }
}
