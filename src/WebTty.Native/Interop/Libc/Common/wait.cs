using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal enum WaitPidOptions
    {
        None = 0,
        WNOHANG = 1,
        WUNTRACED = 2
    }

    internal static partial class Libc
    {

        public static int WEXITSTATUS(int status)
        {
            return (status & 0xFF00) >> 8;
        }

        public static bool WIFEXITED(int status)
        {
            return WTERMSIG(status) == 0;
        }

        public static bool WIFSIGNALED(int status)
        {
            return ((sbyte)(((status) & 0x7f) + 1) >> 1) > 0;
        }

        public static int WTERMSIG(int status)
        {
            return status & 0x7F;
        }

        [DllImport(libc, SetLastError = true)]
        internal static extern pid_t waitpid(pid_t pid, out int status, WaitPidOptions options);
    }
}
