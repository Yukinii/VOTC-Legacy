using System.Threading.Tasks;
using VOTCClient.Core.External.Chatbot.Classes;

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
