using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*
    This file is part of VOTC.

    VOTC is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VOTC is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VOTC.  If not, see <http://www.gnu.org/licenses/>.
*/
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
