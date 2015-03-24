using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VOTCServer.Scripts;
using VOTCServer.Socket;

namespace VOTCServer.Core
{
    public static class Kernel
    {
        public const string ScriptLocation = @"C:\Dropbox\VOTC\Scripts\";

        public static readonly ConcurrentDictionary<string, Script> AllScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> NewScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> FeaturedScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> PopularScripts = new ConcurrentDictionary<string, Script>();
        public static readonly List<NetworkClient> ConnectedClients = new List<NetworkClient>();
        public static List<string> CommandsReceived = new List<string>();
        public static int DownloadCount = 0;
        public static void WriteLine(object text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
