using System;
using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static partial class Libc
    {
        [DllImport(libutil, SetLastError = true)]
        internal extern static int forkpty(out int master, IntPtr name, IntPtr termios, ref winsize winp);

        [DllImport(libutil, SetLastError = true)]
        internal extern static int openpty(out int master, out int aslave, IntPtr name, IntPtr termios, ref winsize winp);

        [DllImport(libutil, SetLastError = true)]
        internal extern static int login_tty(int fd);
    }
}
