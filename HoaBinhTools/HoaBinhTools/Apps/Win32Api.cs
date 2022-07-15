using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HoaBinhTools.Apps
{
    public static class Win32Api
    {
        // A delegate which is used by EnumChildWindows to execute a callback method.
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowCaption(IntPtr hWnd, StringBuilder lpString, int maxCount);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        public static async Task ClickOk()
        {
            const string windowTitle = "Autodesk Revit 2022";
            while (FindWindowByCaption(IntPtr.Zero, windowTitle) == IntPtr.Zero)
            {
                await DelayWork(5);
            }

            // Loop though the child windows, and execute the EnumChildWindowsCallback method
            EnumChildWindows(FindWindowByCaption(IntPtr.Zero, windowTitle), EnumChildWindowsCallback, IntPtr.Zero);
        }

        #region Utilities

        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            const uint WM_LBUTTONDOWN = 0x0201;
            const uint WM_LBUTTONUP = 0x0202;

            var sb = new StringBuilder(256);

            // Get the control's text.
            GetWindowCaption(handle, sb, 256);
            var text = sb.ToString();

            // If the text on the control == &OK send a left mouse click to the handle.
            if (text != @"&OK")
                return true;

            PostMessage(handle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(handle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            return true;
        }

        private static async Task DelayWork(int i)
        {
            await Task.Delay(i);
        }

        #endregion
    }
}
