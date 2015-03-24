using System.IO;
using System.Windows;
using Microsoft.Win32;
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
    public partial class ScriptUploaderDragDrop
    {
        public ScriptUploaderDragDrop()
        {
            InitializeComponent();
            AllowDrop = true;
        }

        private void label_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            var file = (string[])e.Data.GetData(DataFormats.FileDrop);
            GetFile(file[0]);
        }

        private static void GetFile(string file)
        {
            if (file.Contains(".cs"))
            {
                var filename = Path.GetFileName(file);
                if (!File.Exists(@"Scripts\" + filename))
                {
                    MessageBox.Show("The script has to be in your VOTC Scripts directory.");
                    return;
                }
                Kernel.UserScriptEngine.Compile();
                Kernel.ScriptUploaderWindow.Content = new ScriptUploaderUi(filename, true);
            }
            else
                MessageBox.Show("Only single C# files can be dropped here.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {DefaultExt = ".cs", Filter = "Scripts (.cs)|*.cs"};
            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                GetFile(filename);
            }
        }
    }
}
