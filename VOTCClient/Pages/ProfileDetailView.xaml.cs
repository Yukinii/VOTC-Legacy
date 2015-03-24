using System;
using System.Windows;
using VOTCClient.Core;
using VOTCClient.Core.Helpers;
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
