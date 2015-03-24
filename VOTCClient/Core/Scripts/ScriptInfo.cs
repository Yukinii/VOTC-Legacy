using System.Collections.Concurrent;
using VOTCClient.Core.Helpers;

namespace VOTCClient.Core.Scripts
{
    public class ScriptInfo
    {
        public string ProcessName;
        public bool RequireProcessInForeground;
        public ConcurrentDictionary<string, VoiceCommand> Commands;
        public string ScriptName;
        public string Author;
        public string Description;
        public string FriendlyGameName;
        public string StoreBadgeUrl;
        public string StoreHeaderUrl;
    }
}
