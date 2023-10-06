using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProductivWinUINav.Helpers
{
    class GeneralHelper
    {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetActiveWindow();

        public static IntPtr GetWindowHandle()
        {
            return GetActiveWindow();
        }
    }
}
