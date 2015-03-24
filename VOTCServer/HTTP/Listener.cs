using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VOTCServer.Core;

namespace VOTCServer.HTTP
{
    static class Listener
    {
        public static int ListenPort;
        private static TcpListener _tcpListener;
        private static Thread _petT;
        public static void Start(int port)
        {
            ListenPort = port;

            // Start listening
            _tcpListener = new TcpListener(IPAddress.Any, ListenPort);
            _tcpListener.Start();
            _petT = new Thread(ListenLoop) {Name = "WebThread", IsBackground = true};
            _petT.Start();
        }
        public static void Stop()
        {
            _petT.Abort();
            _petT = null;
        }

        private static async void ListenLoop()
        {
            while (true)
            {
                try
                {
                    // Wait for connection
                    var socket = await _tcpListener.AcceptSocketAsync();
                    if (socket == null)
                        break;

                    var client = new Client(socket);
                    await Task.Factory.StartNew(client.Do);
                }
                catch (Exception e)
                {
                    Kernel.WriteLine(e, ConsoleColor.Red);
                }
            }
        }
    }
}
