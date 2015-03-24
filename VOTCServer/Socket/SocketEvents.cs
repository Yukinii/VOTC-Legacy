namespace VOTCServer.Socket
{
    public delegate void NetworkClientConnection(NetworkClient client);
    public delegate void NetworkClientReceive(NetworkClient client, byte[] packet);
}
