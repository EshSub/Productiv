using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using VirtualDesktop1123H2;
//using VirtualDesktop11;
using VirtualDesktop;
using Windows.System.Threading;

namespace Productiv
{

    internal class EventHandlers
    {

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        static Desktop tempDesktop = null;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public static string OnMinMax(IntPtr handle)
        {
            Console.WriteLine("OnMinMax");
            Helpers.IsZoomed(handle);
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("OnMinMax: handle is zero");
                return "Handle is zero";
            }
            else if (Helpers.IsZoomed(handle))
            {
                OnMaximize(handle);
                return "Maximized";
            }
            else
            {
                OnMinimze();
                return "Minimized";
            }
        }

        public static void OnAppStart()
        {
            Console.WriteLine("OnAppStart");
        }


        public static void OnMaximize(IntPtr handle)
        {
            VirtualDesktopAdapters.CreateAndMoveToNewDesktop(handle);
        }


        public static void OnMinimze()
        {
            VirtualDesktopAdapters.RemoveCurrent();
        }

        public static void OnDestroy(IntPtr handle)
        {

            if (handle == IntPtr.Zero)
            {
                return;
            }
            else
            {
                //VirtualDesktopAdapters.RemoveDesktop(VirtualDesktopAdapters.GetDesktopFromHandle(handle));
                try
                {
                    VirtualDesktopAdapters.RemoveCurrent();
                }
                catch
                {

                }
            }
        }

        public static void OnCreateMaximized(IntPtr handle)
        {
            OnMaximize(handle);
        }

        public static void OnCreateMinimized(IntPtr Handle)
        {
            if (VirtualDesktopAdapters.IsCurrentDesktopUnamed())
            {
                //VirtualDesktopAdapters.MoveWindowToDesktop(Handle, VirtualDesktopAdapters.CurrentDesktop());
            }
            else
            {
                Desktop main = VirtualDesktopAdapters.GetDesktopFromIndex(0);
                VirtualDesktopAdapters.MoveWindowToDesktop(Handle, main);
                //main.MakeVisible();
            }
        }

        private static bool EnumWindowsCallbackMaxmizeWindows(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                if (Helpers.IsZoomed(hWnd))
                {
                    VirtualDesktopAdapters.CreateAndMoveToNewDesktop(hWnd);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static void OnSpreadMaximizedWindows()
        {
            EnumWindows(new EnumWindowsProc(EnumWindowsCallbackMaxmizeWindows), IntPtr.Zero);
        }

        public static void ActivateTaskSwitcher()
        {
            tempDesktop = VirtualDesktopAdapters.CurrentDesktop();
            VirtualDesktopAdapters.GetDesktopFromIndex(0).MakeVisible();

        }

        public static void DestroyTaskSwitcher()
        {
            if (tempDesktop != null)
            {
                tempDesktop.MakeVisible();
                tempDesktop = null;
            }
        }

    }
}


