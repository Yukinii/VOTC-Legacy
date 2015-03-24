using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using InputManager;

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
namespace KeymapHelper
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, string> _keysDown = new Dictionary<string, string>();
        public bool Sticky;
        public Form1()
        {
            InitializeComponent();
            KeyboardHook.KeyDown += KeyboardHook_KeyDown;
            KeyboardHook.KeyUp += KeyboardHook_KeyUp;
            KeyboardHook.InstallHook();
        }

        async void KeyboardHook_KeyUp(int vkCode)
        {
            var key = Enum.GetName(typeof(Keys), (Keys)vkCode);
            if (_keysDown.ContainsKey(key))
                _keysDown.Remove(key);
            if (Sticky)
                await Task.Delay(5000);
            UpdateLabel();
        }

        void KeyboardHook_KeyDown(int vkCode)
        {
            var key = Enum.GetName(typeof(Keys), (Keys)vkCode);
            var value = Enum.GetName(typeof (Key), KeyInterop.KeyFromVirtualKey(vkCode));
            if (!_keysDown.ContainsKey(key))
                _keysDown.Add(key, value);
            UpdateLabel();
        }

        void UpdateLabel()
        {
            label1.Text = "";
            label2.Text = "";
            foreach (var key in from entry in _keysDown orderby entry.Value ascending select entry)
            {
                label2.Text += key.Key + " + ";
                label1.Text += key.Value + " + ";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) => Sticky = checkBox1.Checked;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => KeyboardHook.UninstallHook();
    }
}
