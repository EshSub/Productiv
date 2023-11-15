using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.Streaming.Adaptive;
using WindowsDesktop;
using Desktop = WindowsDesktop.VirtualDesktop;

namespace Productiv
{

    internal class VirtualDesktopAdapters
    {
        public static void Create()
        {
            Desktop.Create();
        }

        public static void CreateAndMoveToNewDesktop(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("MoveToNewDesktop: handle is zero");
                return;
            }
            try
            {
                var desktop = Desktop.Create();
                var windowName = Helpers.GetWindowApplicationName(handle);
                desktop.Switch();
                //desktop.Move(1);
                MoveWindowToDesktop(handle, desktop);
                //desktop.Name = windowName;
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        public static void RemoveCurrent()
        {
            var current = Desktop.Current;
            if (IsMain(current))
            {
                return;
            }
            current.Remove(Desktop.GetDesktops()[0]);
        }

        public static bool IsMain(Desktop desktop)
        {
            var main = Desktop.GetDesktops()[0];
            return desktop.Equals(main);
        }

        public static bool IsCurrentDesktopUnamed()
        {
            if (Desktop.Current.Name == null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void MoveWindowToDesktop(IntPtr Handle, Desktop desktop)
        {
            //desktop.MoveWindow(Handle);
        }

        public static Desktop CurrentDesktop()
        {
            return Desktop.Current;
        }

        public static Desktop GetDesktopFromIndex(int index)
        {
            return Desktop.GetDesktops()[index];
        }

        public static Desktop GetDesktopFromHandle(IntPtr Handle)
        {
            return Desktop.FromHwnd(Handle);
        }

        public static void RemoveDesktop(Desktop desktop)
        {
            //TODO if the desktop is unnamed skip that also
            if (IsMain(desktop))
            {
                return;
            }
            desktop.Remove();
        }
    }
}
