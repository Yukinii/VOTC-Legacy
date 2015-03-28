using System; //VOTC LEGACY
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VOTCClient.Core.Network;

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
namespace VOTCClient.Core.IO
{
    internal static class IoQueue
    {
        static readonly ManualResetEvent Mre = new ManualResetEvent(false);
        static readonly ConcurrentQueue<string> PendingWrites = new ConcurrentQueue<string>();
        internal static Logger L;

        public static void Add(string message)
        {
            if (L == null)
                L = new Logger();
            if (message == null) return;
            PendingWrites.Enqueue(message);
            Mre.Set();
        }
        public static void Add(Exception message)
        {
            if (L == null)
                L = new Logger();
            if (message == null) return;
            PendingWrites.Enqueue(message.StackTrace);
            PendingWrites.Enqueue(message.Source);
            PendingWrites.Enqueue(message.Message);
            Mre.Set();
        }
        private static string TakeJob()
        {
            if (L == null)
                L = new Logger();
            string message;
            PendingWrites.TryDequeue(out message);
            if (PendingWrites.IsEmpty)
                Mre.Reset();
            if (Kernel.Tracking)
                Tracking.Add("Error: "+message);
            return message;
        }
        internal static void Abort() => L.IoThread.Abort();

        internal class Logger
        {
            private static StreamWriter _writer;
            public readonly Thread IoThread = new Thread(BeginWrite);
            internal Logger()
            {
                if (_writer != null)
                    return;

                _writer = !File.Exists(Application.StartupPath + "Log.txt") ? new StreamWriter(File.Create("Log.txt")) : new StreamWriter(File.Open(Application.StartupPath + "Log.txt",FileMode.Append));
                _writer.BaseStream.Position = _writer.BaseStream.Length;
                _writer.AutoFlush = true;
                IoThread.IsBackground = true;
                IoThread.Start();
            }
            private static void BeginWrite()
            {
                while (Kernel.KeepThreadsRunning)
                {
                    Mre.WaitOne();
                    while (!_writer.BaseStream.CanWrite)
                    {
                        if (!Kernel.KeepThreadsRunning)
                            break;
                        Thread.Sleep(100);
                    }
                    if (_writer.BaseStream.CanWrite)
                        _writer.WriteLine(TakeJob());
                }
            }
        }
    }
}
