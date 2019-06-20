using System.Runtime.InteropServices;
using static WebTty.Native.Libraries;

namespace WebTty.Native.Syscall
{
    public static partial class Libc
    {
        [DllImport(libc, SetLastError = true)]
        public extern static int ioctl(int fd, long cmd, ref winsize WinSz);

        [DllImport(libc, SetLastError = true)]
        public extern static int ioctl(int fd, long cmd, ref long size);
    }
}
