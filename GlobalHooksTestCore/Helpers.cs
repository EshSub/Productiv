using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Productiv
{
    internal class Helpers
    {

        #region DLL imports

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        #endregion

        #region DLL structures
        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT minPosition;
            public POINT maxPosition;
            public RECT normalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion
        public static string GetWindowName(IntPtr Hwnd)
        {

            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder(256);
            GetWindowText(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        public static bool IsZoomed(IntPtr hWnd)
        {
            //IntPtr hWnd = /* Your window handle */;
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            if (GetWindowPlacement(hWnd, ref placement))
            {
                bool isMaximized = placement.showCmd == 3; // 3 represents maximized state
                if (isMaximized)
                {
                    Console.WriteLine("Window is maximized.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Window is not maximized.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Failed to retrieve window placement.");
                return false;
            }
        }
        public static string GetWindowApplicationName(IntPtr hWnd)
        {
            _ = GetWindowThreadProcessId(hWnd, out uint procId);
            var proc = Process.GetProcessById((int)procId);
            if (proc != null)
            {
                return proc.MainModule.FileVersionInfo.ProductName;
            }
            else { return "Window"; }

        }

        //public static string GetActiveWindowName()
        //{
        //    IntPtr hWnd = GetForegroundWindow();
        //    return GetWindowName(hWnd);
        //}
    }


}
