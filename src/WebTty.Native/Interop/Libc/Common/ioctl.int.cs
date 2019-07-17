using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static partial class Libc
    {
        [DllImport(libc, SetLastError = true)]
        internal extern static int ioctl(int fd, long cmd, ref winsize WinSz);

        [DllImport(libc, SetLastError = true)]
        internal extern static int ioctl(int fd, long cmd, ref long size);
    }
}
