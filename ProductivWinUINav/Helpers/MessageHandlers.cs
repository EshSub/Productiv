using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Input;

namespace ProductivWinUINav
{
    public partial class App: Application
    {
        private void _GlobalHooks_CbtActivate(IntPtr Handle)
        {
            //if (debug.Checked)
            //{
            //    ListCbt.Items.Add("Activate: " + GetWindowName(Handle));
            //}
        }

        private void _GlobalHooks_CbtCreateWindow(IntPtr Handle)
        {
            //if (debug.Checked)
            //{
            //    ListCbt.Items.Add("Create: " + GetWindowName(Handle));
            //}
        }

        private void _GlobalHooks_CbtDestroyWindow(IntPtr Handle)
        {
            //ListCbt.Items.Add("Destroy: " + GetWindowName(Handle) + Handle.ToString());

        }

        private void _GlobalHooks_CbtMinMax(IntPtr Handle)
        {
            //if (Handle == null)
            //{
            //    return;
            //}
            //if (debug.Checked)
            //{
            //    ListShell.Items.Add("MinMax: " + Helpers.GetWindowName(Handle) + Helpers.GetWindowApplicationName(Handle) + Handle.ToString());

            //}
            //string windowName = Helpers.GetWindowName(Handle);

            //if (windowName != null && Handle == GetForegroundWindow())
            //{
            //    if (Helpers.IsZoomed(Handle))
            //    {
            //        if (debug.Checked)
            //        {
            //            ListShell.Items.Add("Maximized: " + Helpers.GetWindowApplicationName(Handle) + Helpers.GetWindowName(Handle) + Handle.ToString());
            //        }
            //        EventHandlers.OnMaximize(Handle);
            //    }
            //    else
            //    {
            //        if (debug.Checked)
            //        {
            //            ListShell.Items.Add("Minimized: " + Helpers.GetWindowApplicationName(Handle) + Helpers.GetWindowName(Handle) + Handle.ToString());
            //        }
            //        EventHandlers.OnMinimze();
            //    }
            //}
        }

        private void _GlobalHooks_ShellWindowActivated(IntPtr Handle)
        {
            //if (debug.Checked)
            //{
            //    ListShell.Items.Add("Activated: " + GetWindowName(Handle) + Handle.ToString());
            //}
        }

        private void _GlobalHooks_ShellWindowCreated(IntPtr Handle)
        {
            //if (debug.Checked)
            //{
            //    ListShell.Items.Add("Created: " + GetWindowName(Handle));
            //}
            //Thread.Sleep(200);
            //if (Helpers.IsZoomed(Handle))
            //{
            //    EventHandlers.OnCreateMaximized(Handle);
            //}
            //else
            //{
            //    EventHandlers.OnCreateMinimized(Handle);
            //}
            //VirtualDesktop.GetDesktops().
        }

        private void _GlobalHooks_ShellWindowDestroyed(IntPtr Handle)
        {
            //if (debug.Checked)
            //{
            //    ListCbt.Items.Add("Current Handle " + this.Handle.ToString() + " App hanlde " + Handle);
            //}
            //if (Handle == this.Handle)
            //{
            //    return;
            //}
            //EventHandlers.OnDestroy(Handle);
        }

        private void _GlobalHooks_ShellRedraw(IntPtr Handle)
        {
            //ListShell.Items.Add("Redraw: " + GetWindowName(Handle));
        }

        private void MouseLL_MouseMove(object sender, MouseEventArgs e)
        {
            //LblMouse.Text = "Mouse at: " + e.X + ", " + e.Y;
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            //this.Hide();
        }

        private void SpreadWindowsBtn_Click(object sender, EventArgs e)
        {
            //EventHandlers.OnSpreadMaximizedWindows();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox1.Checked)
            //{
            //    // Add the value in the registry so that the application runs at startup
            //    rkApp.SetValue(Application.ProductName, Application.ExecutablePath);
            //    //lblInfo.Content = "The application will run at startup";
            //}
            //else
            //{
            //    // Remove the value from the registry so that the application doesn't start
            //    rkApp.DeleteValue(Application.ProductName, false);
            //    //lblInfo.Content = "The application will not run at startup";
            //}
        }

    }
}
