using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using VOTCClient.Core;

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
namespace VOTCClient.Pages
{
    public partial class UploadScriptProgress
    {
        public UploadScriptProgress(string info)
        {
            Kernel.Ready = false;
            InitializeComponent();
            Compressioninfo.Text = info;
            var T = new Timer {Interval = 40};
            T.Elapsed += T_Elapsed;
            T.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Math.Abs(Progressbar.Value - 100) < 1)
                {
                    ((Timer) sender).Stop();
                    Kernel.Ready = true;
                    MessageBox.Show("All done, Chief!");
                    Kernel.ScriptUploaderWindow.Close();
                }
                Progressbar.Value += 1;
                Progressinfo.Text = (int)Progressbar.Value + "%";

            }), DispatcherPriority.Render);
        }
    }
}
