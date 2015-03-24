using System;
using System.Windows;
using VOTCClient.Core;
using VOTCClient.Core.Helpers;

namespace VOTCClient.Pages
{
    public partial class ProfileDetailView
    {
        readonly string _profile;
        public ProfileDetailView(string profile)
        {
            InitializeComponent();
            _profile = Environment.CurrentDirectory + "\\Scripts\\" + profile;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    CommandStorage.AllCommands.Clear();
                    Kernel.ActiveProfile = _profile;
                    Authortxt.Text = Kernel.ScriptInfos.Author;
                    Descriptionbx.Text = Kernel.ScriptInfos.Description;
                    Nametxt.Text = Kernel.ScriptInfos.ScriptName;
                    Apptxt.Text = Kernel.ScriptInfos.FriendlyGameName;
                    foreach (var command in Kernel.ScriptInfos.Commands)
                    {
                        Cmdlist.Items.Add(command.Key);
                    }
                }
                catch
                {
                    // ignored
                }
            }
            catch
            {
                Kernel.ActiveProfile = _profile;
                Kernel.ProfileWindow.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Kernel.ActiveProfile = _profile;
            Kernel.ProfileWindow.Close();
        }
    }
}
