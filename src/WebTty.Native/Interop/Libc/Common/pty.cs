using System;
using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static partial class Libc
    {
        [DllImport(libutil, SetLastError = true)]
        internal extern static int forkpty(out int master, IntPtr dataReturn, IntPtr termios, ref winsize winsz);
    }
}
