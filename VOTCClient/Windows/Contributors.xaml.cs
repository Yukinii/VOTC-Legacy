using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
namespace VOTCClient.Windows
{
    /// <summary>
    /// Interaction logic for Contributors.xaml
    /// </summary>
    public partial class Contributors : Window
    {
        public Contributors()
        {
            InitializeComponent();
        }

        private void ContentElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.facebook.com/groups/623473311006319/");
        }

        private void ContentElement_OnMouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://500px.com/movey11");
        }
    }
}
