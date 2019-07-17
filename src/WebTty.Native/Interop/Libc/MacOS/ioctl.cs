namespace WebTty.Native.Interop
{
    internal static partial class Libc
    {
        // https://github.com/apple/darwin-xnu/blob/0a798f6738bc1db01281fc08ae024145e84df927/bsd/sys/ttycom.h#L154
        public static long TIOCSWINSZ = 0x80087467;
    }
}
