using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static unsafe partial class Libc
    {
        internal static int STDIN_FILENO => 0;
        internal static int STDOUT_FILENO => 1;
        internal static int STDERR_FILENO => 2;


        [DllImport(libc, SetLastError = true)]
        internal static extern unsafe int execve(string filename, byte** argv, byte** envp);


        internal static int SEEK_SET => 0;
        internal static int SEEK_CUR => 1;
        internal static int SEEK_END => 2;

        [DllImport(libc, SetLastError = true)]
        internal static extern off_t lseek(int fd, off_t offset, int whence);
        [DllImport(libc, SetLastError = true)]
        internal static extern int ftruncate(int fd, off_t length);


        [DllImport(libc, SetLastError = true)]
        internal static extern long read(int fd, void* buf, uint count);
        [DllImport(libc, SetLastError = true)]
        internal static extern long write(int fd, void* buf, uint count);

        [DllImport(libc)]
        internal static extern uid_t getuid();
        [DllImport(libc)]
        internal static extern uid_t geteuid();
        [DllImport(libc)]
        internal static extern gid_t getgid();
    }
}
