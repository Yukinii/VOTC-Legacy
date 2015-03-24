using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using VOTCClient.Core;

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
