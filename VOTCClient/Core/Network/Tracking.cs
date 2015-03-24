using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace VOTCClient.Core.Network
{
    internal static class Tracking
    {
        public static readonly Queue<string> DataQueue = new Queue<string>();
        public static readonly ManualResetEvent ManualResetEvent = new ManualResetEvent(false);
        public static readonly Thread ProcessinThread = new Thread(Loop);

        public static void Start() => ProcessinThread.Start();
        public static void Stop() => ProcessinThread.Abort();

        public static async void Loop()
        {
            while (Kernel.KeepThreadsRunning)
            {
                ManualResetEvent.WaitOne();
                try
                {
                    while (DataQueue.Count > 0)
                    {
                        using (var client = new TcpClient("79.133.51.71", 7913))
                        {
                            if (client.Connected)
                            {
                                using (var stream = client.GetStream())
                                {
                                    var message = DataQueue.Dequeue();
                                    var buffer = Encoding.UTF8.GetBytes(message);
                                    await stream.WriteAsync(buffer, 0, buffer.Length);
                                    await stream.FlushAsync();
                                }
                            }
                            else
                            {
                                await client.ConnectAsync(Kernel.RemoteHost, 7913);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //IoQueue.Add(Ex);
                }
                ManualResetEvent.Reset();
            }
        }


        public static void Add(string text)
        {
            DataQueue.Enqueue(text);
            ManualResetEvent.Set();
        }
    }
}
