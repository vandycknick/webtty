using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static unsafe partial class Libc
    {
        [DllImport(libc)]
        internal static extern int strerror_r(int errnum, byte* buf, size_t buflen);
    }
}
