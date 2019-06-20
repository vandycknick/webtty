using System;
using System.Runtime.InteropServices;
using static WebTty.Native.Libraries;

namespace WebTty.Native.Syscall
{
    public static partial class Libc
    {
        [DllImport(libc, SetLastError = true)]
        public extern static int forkpty(out int master, IntPtr dataReturn, IntPtr termios, ref winsize winsz);
    }
}
