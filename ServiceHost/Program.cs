using System; //VOTC LEGACY
using System.ServiceModel;
using System.Text;
using BitFlashGenericWCF;
using BitFlashGenericWCF.Chat;
using BitFlashGenericWCF.Core;
using BitFlashGenericWCF.Database;
using BitFlashGenericWCF.Tracking;
using VOTCServer.Socket;

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
namespace GenericServiceHost
{
    /// <summary>
    /// You might not want to touch this class. The only changes allowed are changes to the shutdown logic
    /// eg. save / flush data to the disk.
    /// </summary>
    internal class Program
    {
        internal static ServiceHost Service;

        private static void Main()
        {
            try
            {
                API.Initialize(); //Basic security layer, each request has to contain a valid API Key
                ScriptDb.LoadScripts();
                Quotes.Load();
                TrackingListener.Init();
                var serverSock = new NetworkServerSocket { ClientBufferSize = 4096, OnConnect = Connecting, OnReceive = Receiving, OnDisconnect = Disconnecting };
                serverSock.Prepare(700, 100);
                serverSock.BeginAccept();
                Console.Title ="BitFlash VOTC Service Host";
                Service = new ServiceHost(typeof (Logic));//Everything thats used to configure the Server is found in the App.Config
                Service.Faulted += Service_Faulted;
                Service.Open();
                Console.WriteLine("Service running and active on:");
                Console.WriteLine();
                foreach (var baseAddress in Service.BaseAddresses)
                {
                    Console.WriteLine("\t"+baseAddress.AbsoluteUri);
                }
                Console.WriteLine();
                Console.WriteLine("Create client code class by executing \n\n\tsvcutil.exe " + Service.BaseAddresses[0]+ "?wsdl\n\nin the visual studio developer console!\n");
                Console.WriteLine("Usually Found at\n\n\t'Microsoft Visual Studio 1X.0\\Common7\\Tools\\VsDevCmd.bat'\n\n");
                Console.WriteLine("Press enter to terminate.");
            }
            catch (Exception exception)
            {
                Console.Write(exception);
                Console.ReadLine();
            }
            Console.ReadLine();
            Service.Close();
            Shutdown();
        }

        private static void Shutdown()
        {
            ScriptDb.SaveScripts();
        }

        private static void Service_Faulted(object sender, EventArgs e)
        {
            Service.Faulted -= Service_Faulted;
            Service = new ServiceHost(typeof(Logic));
            Service.Faulted += Service_Faulted;
            Service.Open();
        }
        static void Connecting(NetworkClient client)
        {
            Kernel.WriteLine("Connection attempted from: " + client.IP, ConsoleColor.DarkGreen);
            Kernel.ConnectedClients.TryAdd(client.IP,client);
        }
        static void Receiving(NetworkClient client, byte[] packet)
        {
            foreach (var connectedClient in Kernel.ConnectedClients.Values)
            {
                Console.WriteLine(Encoding.UTF8.GetString(packet));
                connectedClient.Send(packet);
            }
        }
        static void Disconnecting(NetworkClient client)
        {
            Kernel.WriteLine("Connection lost from: " + client.IP, ConsoleColor.Red);
            if (Kernel.ConnectedClients.ContainsKey(client.IP))
            {
                NetworkClient cli;
                Kernel.ConnectedClients.TryRemove(client.IP,out cli);
            }
            client.Disconnect();
        }
    }
}