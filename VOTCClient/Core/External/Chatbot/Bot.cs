using System.Threading.Tasks;
using VOTCClient.Core.External.Chatbot.Classes;

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
namespace VOTCClient.Core.External.Chatbot
{
    class Bot
    {
        private static IChatterBot _cleverBot;
        private static IChatterBotSession _cleverBotSession;
        public static void Initialize()
        {
            var factory = new ChatterBotFactory();

            _cleverBot = factory.Create(ChatterBotType.Cleverbot, "george");
            _cleverBotSession = _cleverBot.CreateSession();
        }

        public static async Task<string> TalkTo(string message)
        {
            return await Task.Run(() => _cleverBotSession.Think(message));
        }
    }
}
