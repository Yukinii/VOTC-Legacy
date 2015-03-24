using System;
using System.ServiceModel;
using BitFlashGenericWCF;
using BitFlashGenericWCF.Database;

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
                Console.Title ="BitFlash Generic WCF Service Host";
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
            Console.ReadKey();
            Service.Close();
            Shutdown();
        }

        private static void Shutdown()
        {
            //Saving shit?
        }

        private static void Service_Faulted(object sender, EventArgs e)
        {
            Service.Faulted -= Service_Faulted;
            Service = new ServiceHost(typeof(Logic));
            Service.Faulted += Service_Faulted;
            Service.Open();
        }
    }
}