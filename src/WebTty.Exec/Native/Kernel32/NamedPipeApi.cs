using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WebTty.Exec.Native
{
    internal static partial class Kernel32
    {
        /// TODO: Add documentation
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, IntPtr lpPipeAttributes, int nSize);
    }
}
