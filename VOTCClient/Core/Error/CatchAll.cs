using System;
using System.Threading.Tasks;
using System.Windows;
using InputManager;
using VOTCClient.Core.IO;
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
namespace VOTCClient.Core.Error
{
    public static class CatchAll
    {
        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) 
                return;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            IoQueue.Add("[CATCH::ALL] Started global exception handler.");
            _initialized = true;
        }

        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Kernel.KeyboardHook.Unhook();
            MouseHook.UninstallHook();
            MessageBox.Show("Unfortunately, an exception occured. Please send the Logfile (found in the directory you ran this app from) to me, thanks!");

            IoQueue.Add("App::TaskScheduler_UnobservedTaskException");
            e.SetObserved();
            IoQueue.Add(e.Exception.Message);
            IoQueue.Add(e.Exception.Source);
            IoQueue.Add(e.Exception.StackTrace);
            IoQueue.Add(e.Exception.InnerException.Message);
            IoQueue.Add(e.Exception.InnerException.StackTrace);
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            Kernel.KeyboardHook.Unhook();
            MouseHook.UninstallHook();
            MessageBox.Show("Unfortunatelly, an exception occured. Please send the Logfile (found in the directory you ran this app from) to me, thanks!");

            IoQueue.Add("App::Domain->UnhandledException");
            var e = ex.ExceptionObject as Exception;
            if (e == null)
            {
                IoQueue.Add(ex.ToString());
                IoQueue.Add("Would have crashed the server? " + ex.IsTerminating);
            }
            else
            {
                IoQueue.Add(e.Source);
                IoQueue.Add(e.Message);
                IoQueue.Add(e.StackTrace);
                if (e.InnerException != null)
                {
                    IoQueue.Add(e.InnerException.Message);
                    IoQueue.Add(e.InnerException.StackTrace);
                }
                IoQueue.Add("Would have crashed the server? " + ex.IsTerminating);
            }
        }
    }
}
