using System.Runtime.InteropServices;
using static WebTty.Native.Syscall.Libraries;

namespace WebTty.Native.Syscall
{
    public static unsafe partial class Libc
    {
        [DllImport(libc)]
        public static extern int strerror_r(int errnum, byte* buf, size_t buflen);
    }
}
