using System;
using System.Net;
using System.Net.Sockets;

namespace VOTCServer.Socket
{
    public class NetworkServerSocket
    {
        public readonly System.Net.Sockets.Socket Server;
        public int Port;

        public NetworkClientConnection OnConnect;
        public NetworkClientReceive OnReceive;
        public NetworkClientConnection OnDisconnect;

        public int ClientBufferSize;

        public NetworkServerSocket()
        {
            Server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        
        public void Prepare(int port, int backlog)
        {
            Port = port;
            Server.Bind(new IPEndPoint(IPAddress.Any, Port));
            Server.Listen(backlog);
        }
        public void BeginAccept()
        {
            Server.BeginAccept(Accept, null);
        }
        private void Accept(IAsyncResult res)
        {
            var clientSocket = Server.EndAccept(res);
            clientSocket.ReceiveBufferSize = ClientBufferSize;
            var client = new NetworkClient(this, clientSocket, ClientBufferSize);
            OnConnect?.Invoke(client);
            client.BeginReceive();
            BeginAccept();
        }
        public void InvokeDisconnect(NetworkClient client)
        {
            if (!client.Alive)
                return;
            client.Alive = false;
            OnDisconnect?.Invoke(client);
        }
    }
}
