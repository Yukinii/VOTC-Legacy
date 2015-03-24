using System;
using System.Runtime.InteropServices;
using System.Text;

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
namespace VOTCClient.Core.Extensions
{
    public struct Rect
    {
        public int Left;       // Specifies the x-coordinate of the upper-left corner of the rectangle.
        public int Top;        // Specifies the y-coordinate of the upper-left corner of the rectangle.
        public int Right;      // Specifies the x-coordinate of the lower-right corner of the rectangle.
        public int Bottom;     // Specifies the y-coordinate of the lower-right corner of the rectangle.

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Windowinfo
    {
        public uint cbSize;
        public Rect rcWindow;
        public Rect rcClient;
        public readonly uint dwStyle;
        public readonly uint dwExStyle;
        public readonly uint dwWindowStatus;
        public readonly uint cxWindowBorders;
        public readonly uint cyWindowBorders;
        public readonly ushort atomWindowType;
        public readonly ushort wCreatorVersion;

        public Windowinfo(bool? filler)
         : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (uint)(Marshal.SizeOf(typeof(Windowinfo)));
        }

    }
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref Windowinfo pwi);

        [DllImport("user32.dll")]
       public static extern int GetSystemMetrics(int smIndex);
        /// <summary>
        /// Gets the window text.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="text">The text.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        /// <summary>
        /// Creates the round rect RGN.
        /// </summary>
        /// <param name="nLeftRect">The n left rect.</param>
        /// <param name="nTopRect">The n top rect.</param>
        /// <param name="nRightRect">The n right rect.</param>
        /// <param name="nBottomRect">The n bottom rect.</param>
        /// <param name="nWidthEllipse">The n width ellipse.</param>
        /// <param name="nHeightEllipse">The n height ellipse.</param>
        /// <returns></returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
         int nLeftRect,
         int nTopRect, // y-coordinate of upper-left corner
         int nRightRect, // x-coordinate of lower-right corner
         int nBottomRect, // y-coordinate of lower-right corner
         int nWidthEllipse, // height of ellipse
         int nHeightEllipse // width of ellipse
        );
    }
}
