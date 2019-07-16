using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal unsafe struct stat
    {
        public dev_t st_dev;
        public ino_t st_ino;
        public mode_t st_mode;
        public nlink_t st_nlink;
        public uid_t st_uid;
        public gid_t st_gid;
        public dev_t st_rdev;
        public timespec st_atimspec;
        public timespec st_mtimspec;
        public timespec st_ctimspec;
        public off_t st_size;
        public blkcnt_t st_blocks;
        public ulong st_blksize;
        private fixed long __unused[3];
    }

    internal static unsafe partial class Libc
    {

        [DllImport(libc, SetLastError = true)]
        internal static extern int fstat(int fd, out stat stat);

    }
}
