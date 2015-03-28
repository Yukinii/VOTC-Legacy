using System; //VOTC LEGACY
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
namespace VOTCClient.Core.Hook
{
    public class KeyboardHook
    {
        #region Constant, Structure and Delegate Definitions
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);
        private KeyboardHookProc _keyboardHookProc;
        public IntPtr HInstance;
        public struct KeyboardHookStruct
        {
            public int VkCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DwExtraInfo;
        }

        const int WhKeyboardLl = 13;
        const int WmKeydown = 0x100;
        const int WmKeyup = 0x101;
        const int WmSyskeydown = 0x104;
        const int WmSyskeyup = 0x105;
        #endregion

        #region Instance Variables
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public List<Keys> HookedKeys = new List<Keys>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr _hhook = IntPtr.Zero;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public event KeyEventHandler KeyUp;
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="KeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~KeyboardHook()
        {
            Unhook();
        }
        #endregion

        private bool _installed;
        #region Public Methods
        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void Hook()
        {
            if(_installed)
                return;
            _installed = true;
            HInstance = LoadLibrary("User32");
            _keyboardHookProc = HookProc;
            _hhook = SetWindowsHookEx(WhKeyboardLl, _keyboardHookProc, HInstance, 0);
            KeyDown += Kernel.KeyDown;
            KeyUp += Kernel.KeyUp;
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void Unhook()
        {
            if(!_installed)
                return;
           
            _installed = false;
            UnhookWindowsHookEx(_hhook);
            KeyDown = null;
            KeyUp = null;
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        public int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code < 0)
                return CallNextHookEx(_hhook, code, wParam, ref lParam);
            var key = (Keys)lParam.VkCode;
            //if (HookedKeys.Contains(Key))
            //{
            var kea = new KeyEventArgs(key);
            if ((wParam == WmKeydown || wParam == WmSyskeydown) && (KeyDown != null))
            {
                KeyDown(this, kea);
            }
            else if ((wParam == WmKeyup || wParam == WmSyskeyup))
            {
                KeyUp?.Invoke(this, kea);
            }
            return kea.Handled ? 1 : CallNextHookEx(_hhook, code, wParam, ref lParam);
            //}
        }
        #endregion

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }
}
