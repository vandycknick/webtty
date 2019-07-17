using System;
using System.Runtime.InteropServices;

namespace WebTty.Native.Interop
{
    internal static partial class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
    }
}
