using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using SlimDX.DirectInput;
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
    /// Interaction logic for JoystickConfig.xaml
    /// </summary>
    public partial class JoystickConfig
    {
        readonly Timer _timer = new Timer();
        private JoystickState _state;
        private readonly Joystick _joystick;
        public JoystickConfig()
        {
            InitializeComponent();
            try
            {
                _joystick = GetSticks().FirstOrDefault();
                if (_joystick != null)
                {
                    _joystick.Acquire();
                    _state = _joystick.GetCurrentState();
                }
                else
                {
                    MessageBox.Show("No joystick found.", "Fail");
                    Close();
                }
                _timer.Elapsed += T_Elapsed;
                _timer.Interval = 10;
                _timer.Start();
            }
            catch
            {
                MessageBox.Show("No Joystick found!", "Fail");
                Close();
            }
        }

        void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var result = _joystick.Poll();
                _state = _joystick.GetCurrentState();
                Button.Content = result.Name;
            for (var I = 0; I < 8; I++)
            {
                if (_state.GetButtons()[I])
                    Button.Content = "Found SaveButton " + I;
            }
            X.Value = _state.X;
            Y.Value = -_state.Y;
            R.Value = _state.AngularVelocityX;
            Z.Value = -_state.Z;

            });
        }
        public static Joystick[] GetSticks()
        {
            var directInput = new DirectInput();
            var sticks = new List<Joystick>();
            foreach (var device in directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // create the device
                try
                {
                    var stick = new Joystick(directInput, device.InstanceGuid);
                    stick.Acquire();

                    foreach (var deviceObject in stick.GetObjects().Where(deviceObject => (deviceObject.ObjectType & ObjectDeviceType.Axis) != 0))
                    {
                        stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    }

                    sticks.Add(stick);

                    Console.WriteLine(stick.Information.InstanceName);
                }
                catch (DirectInputException)
                {
                }
            }
            return sticks.ToArray();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _timer.Dispose();
        }
    }
}
