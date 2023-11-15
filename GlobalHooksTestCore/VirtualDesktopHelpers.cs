using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Productiv
{

    [StructLayout(LayoutKind.Sequential)]
    public struct HString : IDisposable
    {
        private readonly IntPtr handle;
        public static HString FromString(string s)
        {
            var h = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.ThrowExceptionForHR(WindowsCreateString(s, s.Length, h));
            return Marshal.PtrToStructure<HString>(h);
        }

        public void Delete()
        {
            WindowsDeleteString(handle);
        }

        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int WindowsCreateString([MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length, [Out] IntPtr hstring);

        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        private static extern int WindowsDeleteString(IntPtr hstring);

        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr WindowsGetStringRawBuffer(HString hString, IntPtr length);

        public void Dispose()
        {
            Delete();
        }

        public static implicit operator string(HString hString)
        {
            var str = Marshal.PtrToStringUni(WindowsGetStringRawBuffer(hString, IntPtr.Zero));
            hString.Delete();
            if (null != str)
                return str;
            else
                return string.Empty;
        }
    }
}

