using System.Runtime.InteropServices;
using static WebTty.Exec.Native.Libraries;

namespace WebTty.Exec.Native
{
    // https://github.com/torvalds/linux/blob/96b95eff4a591dbac582c2590d067e356a18aacb/include/uapi/linux/wait.h#L17
    internal enum WaitIdType
    {
        P_ALL = 0,
        P_PID = 1,
        P_PGID = 2,
        P_PIDFD = 3,
    }

    // https://github.com/torvalds/linux/blob/96b95eff4a591dbac582c2590d067e356a18aacb/include/uapi/linux/wait.h#L5
    internal enum WaitPidOptions
    {
        None = 0,
        WNOHANG = 0x00000001,
        WUNTRACED = 0x00000002,
        WSTOPPED = 0x00000002, // = WUNTRACED
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
