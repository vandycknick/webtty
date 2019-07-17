namespace WebTty.Native.Interop
{
    internal static partial class Sys
    {
        public static bool ShouldRetrySyscall(int ret)
        {
            if (ret == -1 && Libc.errno == Libc.EINTR)
            {
                return true;
            }

            return false;
        }

        public static void ThrowExceptionForLastError() => PosixException.Throw();

        public static void ThrowExceptionForLastErrorIf(int retval)
        {
            if (retval == -1)
            {
                ThrowExceptionForLastError();
            }
        }
    }
}
