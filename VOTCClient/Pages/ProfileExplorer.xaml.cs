using System; //VOTC LEGACY
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
    public partial class ProfileExplorer
    {
        public ProfileExplorer()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                foreach(var file in Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Scripts\\"))
                {
                    var file1 = file;
                    Dispatcher.BeginInvoke(new Action(() => ListBox.Items.Add(Path.GetFileName(file1))), DispatcherPriority.Background);
                }
            });
        }
        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(ListBox.SelectedItem == null)
                return;
            Kernel.ProfileWindow.Content = new ProfileDetailView(ListBox.SelectedItem.ToString());
        }
    }
}
