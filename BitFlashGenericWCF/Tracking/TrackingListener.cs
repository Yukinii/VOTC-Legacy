using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BitFlashGenericWCF.Core;
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
namespace BitFlashGenericWCF.Tracking
{
    public static class TrackingListener
    {
        private static readonly Thread T = new Thread(Start);

        public static void Init()
        {
            T.Start();
        }
        static async void Start()
        {
            var listener = new TcpListener(IPAddress.Any, 7913);
            listener.Start();

            var bytes = new byte[1024];

            while (true)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    var stream = client.GetStream();
                    int I;
                    while ((I = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    {
                        using (var writer = new StreamWriter("Tracking.txt", true))
                        {
                            var data = Encoding.UTF8.GetString(bytes, 0, I).Trim();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Kernel.WriteLine(DateTime.Now.ToShortTimeString() + " | " + data, ConsoleColor.Blue);
                            Console.ResetColor();
                            await writer.WriteLineAsync(DateTime.Now.ToShortTimeString() + " | " + data);
                            if (Kernel.CommandsReceived.Count > 50)
                                Kernel.CommandsReceived.Clear();

                            Kernel.CommandsReceived.Insert(0, data.Split('|')[0]);
                        }
                    }
                    client.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}