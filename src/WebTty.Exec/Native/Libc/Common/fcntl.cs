using System.Runtime.InteropServices;
using static WebTty.Exec.Native.Libraries;

namespace WebTty.Exec.Native
{
    internal static partial class Libc
    {
        public const int F_SETFD = 2;

        public const int FD_CLOEXEC = 1;

        [DllImport(libc, SetLastError = true)]
        public static extern int fcntl(int fd, int cmd, int arg);
    }
}
