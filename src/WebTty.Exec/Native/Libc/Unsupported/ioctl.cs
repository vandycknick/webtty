using System;

namespace WebTty.Exec.Native
{
    internal static partial class Libc
    {
        // https://github.com/torvalds/linux/blob/5bd4af34a09a381a0f8b1552684650698937e6b0/include/uapi/asm-generic/ioctls.h#L39
        public static long TIOCSCTTY => throw new PlatformNotSupportedException();

        public static long TIOCSWINSZ => throw new PlatformNotSupportedException();
    }
}
