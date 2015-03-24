using System;
using System.Windows.Input;
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
namespace VOTCClient.Core
{
    public static class Config
    {
        public static readonly IniFile File = new IniFile("Config.ini");
        public static void LoadConfig()
        {
            Kernel.EnableDeleteMusicVoiceCommand = File.GetBool("Config", "EnableDeleteMusicVoiceCommand", false);
            Kernel.PttKey = (Key)Enum.Parse(typeof(Key),  File.ReadString("Config", "PTTKey", "None"));
            Kernel.UsePtt =  File.GetBool("Config", "UsePTT", false);
            Kernel.DeveloperMode =  File.GetBool("Config", "DeveloperMode", false);
            Kernel.Tracking =  File.GetBool("Config", "Tracking", true);
            Kernel.Donated =  File.GetBool("Config", "Donated", false);
            TextToSpeech.Speed =  File.ReadInt("Config", "TTSSpeed", 1);
            TextToSpeech.Volume =  File.ReadInt("Config", "TTSVolume", 50);
            Kernel.CustomName =  File.ReadString("Config", "CustomName", null);
            Kernel.TwitterSecret =  File.ReadString("Config", "TwitterSecret", null);
            Kernel.TwitterToken =  File.ReadString("Config", "TwitterToken", null);
            Kernel.FacebookAccessToken = File.ReadString("Config", "FacebookToken", null);
            Kernel.Playlist.PlayCustomSounds =  File.GetBool("Config", "PlayCustomSounds", true);
            Kernel.Playlist.Volume =  File.ReadInt("Config", "Volume", 15);
            Kernel.DebugMode =  File.GetBool("Config", "DebugMode", true);
            InternalSpeechRecognizer.DisplayUnknownCommands =  File.GetBool("Config", "DisplayUnknownCommands", true);
            InternalSpeechRecognizer.MinimumConfidence =  File.ReadInt("Config", "MinimumConfidence", 75);
            InternalSpeechRecognizer.IsNaturalSpeaking =  File.GetBool("Config", "IsNaturalSpeaking", true);
            TextToSpeech.UseTts =  File.GetBool("Config", "UseTTS", true);
            TextToSpeech.VoiceName =  File.ReadString("Config", "VoiceName", "");
       }
        public static void SaveConfig()
        {
            File.SetValue("Config", "EnableDeleteMusicVoiceCommand", Kernel.EnableDeleteMusicVoiceCommand);
             File.SetValue("Config", "PTTKey", Enum.GetName(typeof(Key), Kernel.PttKey));
             File.SetValue("Config", "UsePTT", Kernel.UsePtt);
             File.SetValue("Config", "DeveloperMode", Kernel.DeveloperMode);
             File.SetValue("Config", "Tracking", Kernel.Tracking);
             File.SetValue("Config", "Donated", Kernel.Donated);
             File.SetValue("Config", "TTSSpeed", TextToSpeech.Speed);
             File.SetValue("Config", "TTSVolume", TextToSpeech.Volume);
             File.SetValue("Config", "CustomName", Kernel.CustomName);
             File.SetValue("Config", "TwitterSecret", Kernel.TwitterSecret);
             File.SetValue("Config", "TwitterToken", Kernel.TwitterToken);
             File.SetValue("Config", "FacebookToken", Kernel.FacebookAccessToken);
             File.SetValue("Config", "PlayCustomSounds", Kernel.Playlist.PlayCustomSounds);
             File.SetValue("Config", "Volume", Kernel.Playlist.Volume);
             File.SetValue("Config", "ActiveProfile", Kernel.ActiveProfile);
             File.SetValue("Config", "DisplayUnknownCommands", InternalSpeechRecognizer.DisplayUnknownCommands);
             File.SetValue("Config", "IsNaturalSpeaking", InternalSpeechRecognizer.IsNaturalSpeaking);
             File.SetValue("Config", "MinimumConfidence", InternalSpeechRecognizer.MinimumConfidence);
             File.SetValue("Config", "UseTTS", TextToSpeech.UseTts);
             File.SetValue("Config", "DebugMode", Kernel.DebugMode);
             File.SetValue("Config", "VoiceName", TextToSpeech.VoiceName);
             File.Flush();
        }
    }
}
