using System;
using System.Text;
using System.Timers;
using VOTCServer.Core;
using VOTCServer.Database;
using VOTCServer.HTTP;
using VOTCServer.Scripts;
using VOTCServer.Socket;
using VOTCServer.Tracking;

namespace VOTCServer
{
    class Program
    {
        public static readonly Timer AutoSaveTimer = new Timer();
        public static void Main()
        {
            AutoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;
            AutoSaveTimer.Interval = 1000*60*3;//3min
            ScriptDb.LoadScripts();
            TrackingListener.Init();
            Listener.Start(141);

            var serverSock = new NetworkServerSocket {ClientBufferSize = 4096, OnConnect = Connecting, OnReceive = Receiving, OnDisconnect = Disconnecting};
            serverSock.Prepare(700, 100);
            serverSock.BeginAccept();

            AutoSaveTimer.Start();
            Kernel.WriteLine("Webserver Listening on port " + Listener.ListenPort,ConsoleColor.White);
            while (true)
            {
                var input = Console.ReadLine();
                if (input == null) 
                    continue;

                var cmd = input.Split(' ');

                switch (cmd[0])
                {
                        #region List Scripts
                    case "list":
                    {
                        Kernel.WriteLine("\tName \t\t Author \t\t Category", ConsoleColor.Blue);
                        foreach (var script in Kernel.AllScripts)
                        {
                            Kernel.WriteLine(script.Value.Name + "\t\tFrom: " + script.Value.Author + "\t" + Enum.GetName(typeof(Category), script.Value.Category), ConsoleColor.Blue);
                        }
                        break;
                    } 
                        #endregion
                        #region Set Category
                    case "set":
                    {
                        var builder = new StringBuilder();
                        for (var I = 2; I < cmd.Length; I++)
                        {
                            if (I < cmd.Length - 1)
                                builder.Append(cmd[I] + " ");
                            else
                            {
                                builder.Append(cmd[I]);
                            }
                        }
                        switch (cmd[1])
                        {
                            case "none":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.None;
                                break;
                            }
                            case "updated":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.Updated;
                                break;
                            }
                            case "harmful":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.Harmful;
                                break;
                            }
                            case "featured":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.Featured;
                                break;
                            }
                            case "new":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.New;
                                break;
                            }
                            case "popular":
                            {
                                Kernel.AllScripts[builder + ".cs"].Category = Category.Popular;
                                break;
                            }
                        }
                        Kernel.WriteLine("Set: " + builder + "'s category to " + cmd[1], ConsoleColor.Blue);
                        break;
                    } 
                        #endregion
                }
            }
        }

        static void Connecting(NetworkClient client)
        {
            Kernel.WriteLine("Connection attempted from: " + client.IP, ConsoleColor.DarkGreen);
            Kernel.ConnectedClients.Add(client);
        }
        static void Receiving(NetworkClient client, byte[] packet)
        {
            Console.WriteLine(Encoding.UTF8.GetString(packet));
            foreach (var connectedClient in Kernel.ConnectedClients)
            {
                connectedClient.Send(packet);
            }
        }
        static void Disconnecting(NetworkClient client)
        {
            if (Kernel.ConnectedClients.Contains(client))
            {
                Kernel.ConnectedClients.Remove(client);
            }
            client.Disconnect();
        }

        static void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScriptDb.SaveScripts();
        }
    }
}
