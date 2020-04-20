using System.Runtime.InteropServices;
using static WebTty.Exec.Native.Libraries;

namespace WebTty.Exec.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct termios
    {
        public int c_iflag;    /* input flags */
        public int c_oflag;    /* output flags */
        public int c_cflag;    /* control flags */
        public int c_lflag;    /* local flags */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] c_cc; /* control chars */

        public int c_ispeed;   /* input speed */
        public int c_ospeed;   /* output speed */
    }

    internal static partial class Libc
    {
        [DllImport(libc, SetLastError = true)]
        public static extern int tcgetattr(int fd, out termios t);

        [DllImport(libc, SetLastError = true)]
        public static extern int tcsetattr(int fd, int optional_actions, ref termios t);

        [DllImport(libc, SetLastError = true)]
        public static extern void cfmakeraw(ref termios t);

        public static int TCSAFLUSH => 2;
    }
}
