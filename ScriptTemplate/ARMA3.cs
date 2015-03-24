using System.Threading.Tasks;
using System.Windows.Input;
using VOTCClient.Core;
using VOTCClient.Core.Helpers;
using VOTCClient.Core.Scripts;
using Mouse = InputManager.Mouse;

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
namespace ScriptTemplate
{
    public class Arma3
    {
        public static ScriptInfo SetUp()
        {
            Kernel.UI.DisplayCmd("ARMA 3 Script loaded!", false);

            new VoiceCommand().Create("toggle auto hover", Key.X, "Auto hover toggled");
            new VoiceCommand().Create("toggle gear", Key.G);
            new VoiceCommand().Create("load assistant", new[] {Key.RightCtrl, Key.B});
            new VoiceCommand().Create("flaps up", new[] {Key.RightCtrl, Key.L});
            new VoiceCommand().Create("flaps down", new[] {Key.RightCtrl, Key.K});
            new VoiceCommand().Create("toggle zoom", Mouse.MouseKeys.Right);
            new VoiceCommand().Create("toggle gps", new[] {Key.RightCtrl, Key.M});
            new VoiceCommand().Create("compass", Key.K, "", 3000);
            new VoiceCommand().Create("watch", Key.O, "", 3000);
            new VoiceCommand().Create("dive", Key.Z);
            new VoiceCommand().Create("dive", Key.Z);

            var info = new ScriptInfo
            {
                ScriptName = "Arma3 Basic 1.0",
                Author = "BitFlash, LLC.",
                Description = "Basic Arma3 script for basic functionality on foot and in vehicles.",
                FriendlyGameName = "ARMA 3",
                Commands = CommandStorage.AllCommands,
                ProcessName = "arma3",
                RequireProcessInForeground = true
            };
            return info;
        }

        public static async Task<bool> IncommingVoicePacket(string voicePacket)
        {
            return await VoiceCommand.ExecuteCommand(voicePacket);
        }
    }
}