using System; //VOTC LEGACY
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using VOTCClient.Core.Extensions;
using Font = SlimDX.Direct3D9.Font;

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
    public partial class Form1 : Form
    {
        private Margins _marg;

        //this is used to specify the boundaries of the transparent area
        internal struct Margins
        {
        }

        [DllImport("user32.dll", SetLastError = true)]

        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]

        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GwlExstyle = -20;

        public const int WsExLayered = 0x80000;

        public const int WsExTransparent = 0x20;

        public const int LwaAlpha = 0x2;

        public const int LwaColorkey = 0x1;

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        public static IntPtr FHandle;
        private readonly Device _device;
        private readonly Font _font;
        public Form1()
        {
            InitializeComponent();
            SetWindowLong(Handle, GwlExstyle,(IntPtr)(GetWindowLong(Handle, GwlExstyle) ^ WsExLayered ^ WsExTransparent));        
            SetLayeredWindowAttributes(Handle, 0, 255, LwaAlpha);
            FHandle = Handle;
            var presentParameters = new PresentParameters {Windowed = true, SwapEffect = SwapEffect.Discard, BackBufferFormat = Format.A8R8G8B8};
            _device = new Device(new Direct3D(), 0, DeviceType.Hardware, Handle,
            CreateFlags.HardwareVertexProcessing, presentParameters);
            _font = new Font(_device, new System.Drawing.Font(FontFamily.GenericSansSerif, 8));
            var dx = new Thread(DxThread) {IsBackground = true};
            dx.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var info = new Windowinfo();
            info.cbSize = (uint)Marshal.SizeOf(info);
            var activeWindowHandle = NativeMethods.GetForegroundWindow();
            NativeMethods.GetWindowInfo(activeWindowHandle, ref info);
            if (FHandle != activeWindowHandle)
            {
                Width = info.rcWindow.Right - info.rcWindow.Left;
                Height = info.rcWindow.Bottom - info.rcWindow.Top;
            }
            DwmExtendFrameIntoClientArea(Handle, ref _marg);  
            base.OnPaint(e);
        }

        private void DxThread()
        {
            const int y = 10;
            const int x = 10;
            while (true)
            {
                _device.Clear(ClearFlags.Target, Color.FromArgb(0, 0, 0, 0), 1.0f, 0);
                _device.SetRenderState(RenderState.ZEnable, false);
                _device.SetRenderState(RenderState.Lighting, false);
                _device.SetRenderState(RenderState.CullMode, 1);
                _device.SetTransform(0, Matrix.OrthoOffCenterLH(0, Width, Height, 0, 0, 1));
                _device.BeginScene();

                _font.DrawString(null, "STRING", x, y, Color.Aqua);

                _device.EndScene();
                _device.Present();
                Thread.Sleep(16);
            }
        }
    }
}
