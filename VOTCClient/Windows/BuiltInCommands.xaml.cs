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

using System.ComponentModel;
using VOTCClient.Core;

namespace VOTCClient.Windows
{
    /// <summary>
    /// Interaction logic for BuiltInCommands.xaml
    /// </summary>
    public partial class BuiltInCommands
    {
        public BuiltInCommands()
        {
            InitializeComponent();
            Kernel.CommandWindow = this;
        }

        private void BuiltInCommands_OnClosing(object sender, CancelEventArgs e)
        {
            Kernel.CommandWindow = null;
        }
    }
}
