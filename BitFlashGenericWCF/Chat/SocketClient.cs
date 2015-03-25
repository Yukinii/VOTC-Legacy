using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VOTCServer.Socket;

namespace BitFlashGenericWCF.Chat
{
    public class NetworkClient
    {
        private readonly byte[] _buffer;
        private readonly Socket _socket;
        private readonly NetworkServerSocket _server;
        public readonly string IP;
        public bool Alive;

        public NetworkClient(NetworkServerSocket server, Socket socket, int bufferLen)
        {
            Alive = true;
            _server = server;
            _socket = socket;
            _buffer = new byte[bufferLen];
            IP = (_socket.RemoteEndPoint as IPEndPoint)?.Address.ToString();
        }
        public void BeginReceive()
        {
            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Receive, null);
        }
        private void Receive(IAsyncResult res)
        {
            try
            {
                if (_socket == null)
                    return;

                var len = _socket.EndReceive(res);

                if (!Alive)
                    return;

                if (len > 0)
                {
                    var received = new byte[len];
                    Buffer.BlockCopy(_buffer, 0, received, 0, len);
                    _server.OnReceive?.Invoke(this, received);
                    BeginReceive();
                }
                else
                {
                    _server.InvokeDisconnect(this);
                }
            }
            catch
            {
                _server.InvokeDisconnect(this);
            }
        }
        public async void Send(byte[] packet)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            if (Alive)
                await Task.Factory.FromAsync(_socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, null, _socket), _socket.EndSend);
        }

        public void Disconnect()
        {
            _socket.Disconnect(false);
        }
    }
}
