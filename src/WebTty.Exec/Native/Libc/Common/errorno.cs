using System.Runtime.InteropServices;

namespace WebTty.Exec.Native
{
    internal static partial class Libc
    {
        public static int EPERM => 0x1;
        public static int ENOENT => 0x2;
        public static int ESRCH => 0x3;
        public static int EINTR => 0x4;
        public static int EIO => 0x5;
        public static int ENXIO => 0x6;
        public static int E2BIG => 0x7;
        public static int ENOEXEC => 0x8;
        public static int EBADF => 0x9;
        public static int ECHILD => 0xa;
        public static int EBUSY => 0x10;
        public static unsafe int errno => Marshal.GetLastWin32Error();
    }
}
