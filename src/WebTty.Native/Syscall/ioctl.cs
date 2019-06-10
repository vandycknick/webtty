using System.Runtime.InteropServices;
using static WebTty.Native.Syscall.Libraries;

namespace WebTty.Native.Syscall
{
    public struct winsize
    {
        public ushort ws_row;
        public ushort ws_col;
        public ushort ws_xpixel;
        public ushort ws_ypixel;
    }

    public static partial class Libc
    {
        [DllImport(libc, SetLastError = true)]
        public extern static int ioctl(int fd, long cmd, ref winsize WinSz);

        [DllImport(libc, SetLastError = true)]
        public extern static int ioctl(int fd, long cmd, ref long size);
    }
}
