using System.Threading.Tasks;
using System.Windows.Input;
using VOTCClient.Core.Helpers;
using VOTCClient.Core.Scripts;

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
namespace JediAcademy
{
    public class JediAcademy
    {
        public static ScriptInfo SetUp()
        {
            new VoiceCommand().Create("pull", Key.F2, "Get over here.");
            new VoiceCommand().Create("drain", Key.F12, "Get over here.", 2000);

            var info = new ScriptInfo
            {
                ScriptName = "Jedi Academy Multiplayer 1.0",
                Author = "BitFlash, LLC.",
                Description = "This Script allows you to use Forcepowers with your voice! This is also the tutorial script from the wiki :)",
                FriendlyGameName= "Star Wars: Jedi Knight III - Jedi Academy Multiplayer",
                ProcessName = "jamp",
                RequireProcessInForeground = false,
                Commands = CommandStorage.AllCommands,
                StoreBadgeUrl = "http://4.bp.blogspot.com/-8JzIuwduuPE/UsRCN8RhT2I/AAAAAAAALv8/RMro1SKNT7I/s1600/StarWars-Jedi-Knight-Academy-2-icon.png",
                StoreHeaderUrl = "http://www.theisozone.com/images/screens/pc-44293-11337793032.jpg"
            };
            return info;
        }
        public static async Task<bool> IncommingVoicePacket(string voicePacket)
        {
            return await VoiceCommand.ExecuteCommand(voicePacket).ConfigureAwait(false);
        }
    }
}