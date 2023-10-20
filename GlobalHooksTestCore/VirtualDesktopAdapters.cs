using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using VirtualDesktop1123H2;
using VirtualDesktop11;
using Windows.Media.Streaming.Adaptive;

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
            var desktop = Desktop.Create();
            var windowName = Helpers.GetWindowApplicationName(handle);
            desktop.MoveWindow(handle);
            desktop.MakeVisible();
            desktop.Move(1);
            desktop.SetName(windowName);
        }

        public static void RemoveCurrent()
        {
            var current = Desktop.Current;
            if (IsMain(current))
            {
                return;
            }
            current.Remove(Desktop.FromIndex(0));
        }

        public static bool IsMain(Desktop desktop)
        {
            var main = Desktop.FromIndex(0);
            return desktop.Equals(main);
        }

        public static bool IsCurrentDesktopUnamed()
        {
            return Desktop.HasDesktopNameFromIndex(Desktop.FromDesktop(Desktop.Current));
        }

        public static void MoveWindowToDesktop(IntPtr Handle, Desktop desktop)
        {
            desktop.MoveWindow(Handle);
        }

        public static Desktop CurrentDesktop()
        {
            return Desktop.Current;
        }

        public static Desktop GetDesktopFromIndex(int index)
        {
            return Desktop.FromIndex(index);
        }

        public static Desktop GetDesktopFromHandle(IntPtr Handle)
        {
            return Desktop.FromWindow(Handle);
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
