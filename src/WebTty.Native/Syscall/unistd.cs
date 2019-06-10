using System.Runtime.InteropServices;
using static WebTty.Native.Syscall.Libraries;

namespace WebTty.Native.Syscall
{
    public static unsafe partial class Libc
    {
        public static int STDIN_FILENO => 0;
        public static int STDOUT_FILENO => 1;
        public static int STDERR_FILENO => 2;


        [DllImport(libc, SetLastError = true)]
        public static extern unsafe int execve(string filename, byte** argv, byte** envp);


        public static int SEEK_SET => 0;
        public static int SEEK_CUR => 1;
        public static int SEEK_END => 2;

        [DllImport(libc, SetLastError = true)]
        public static extern off_t lseek(int fd, off_t offset, int whence);
        [DllImport(libc, SetLastError = true)]
        public static extern int ftruncate(int fd, off_t length);


        [DllImport(libc, SetLastError = true)]
        public static extern long read(int fd, void* buf, uint count);
        [DllImport(libc, SetLastError = true)]
        public static extern long write(int fd, void* buf, uint count);
    }
}
