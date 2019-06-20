namespace WebTty.Native.Syscall
{
    public static partial class Libc
    {
        // https://github.com/apple/darwin-xnu/blob/0a798f6738bc1db01281fc08ae024145e84df927/bsd/sys/ttycom.h#L154
        // private const long mac_tiocswinsz = 0x80087467;

        // https://github.com/torvalds/linux/blob/5bd4af34a09a381a0f8b1552684650698937e6b0/include/uapi/asm-generic/ioctls.h#L39
        public static long TIOCSWINSZ = 0x5414;
    }
}
