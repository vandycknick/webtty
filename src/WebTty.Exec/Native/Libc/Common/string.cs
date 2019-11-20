using System.Runtime.InteropServices;
using static WebTty.Exec.Native.Libraries;

namespace WebTty.Exec.Native
{
    internal static unsafe partial class Libc
    {
        [DllImport(libc)]
        internal static extern int strerror_r(int errnum, byte* buf, size_t buflen);
    }
}
