using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace VOTCServer.Socket
{
    public class NetworkClient
    {
        private readonly byte[] _buffer;
        public readonly System.Net.Sockets.Socket Socket;
        public readonly NetworkServerSocket Server;
        public object Owner;
        public readonly string IP;
        public bool Alive;

        public NetworkClient(NetworkServerSocket server, System.Net.Sockets.Socket socket, int bufferLen)
        {
            Alive = true;
            Server = server;
            Socket = socket;
            _buffer = new byte[bufferLen];
            IP = (Socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();
        }
        public void BeginReceive()
        {
            Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Receive, null);
        }
        private void Receive(IAsyncResult res)
        {
            try
            {
                if (Socket == null)
                    return;

                var len = Socket.EndReceive(res);

                if (!Alive)
                    return;

                if (len > 0)
                {
                    var received = new byte[len];
                    Buffer.BlockCopy(_buffer, 0, received, 0, len);
                    Server.OnReceive?.Invoke(this, received);
                    BeginReceive();
                }
                else
                {
                    Server.InvokeDisconnect(this);
                }
            }
            catch
            {
                Server.InvokeDisconnect(this);
            }
        }
        public async void Send(byte[] packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            if (Alive)
                await Task.Factory.FromAsync(Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, null, Socket), Socket.EndSend);
        }

        public void Disconnect()
        {
            Socket.Disconnect(false);
        }
    }
}
