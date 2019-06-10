using System;
using System.Runtime.InteropServices;
using WebTty.Native.Syscall;

namespace WebTty.Native
{
    class PosixException : Exception
    {
        public PosixException(int errno) :
            base(GetErrorMessage(errno))
        {
            HResult = errno;
        }

        public PosixException() :
            this(Libc.errno)
        { }

        private unsafe static string GetErrorMessage(int errno)
        {
            int bufferLength = 1024;
            byte* buffer = stackalloc byte[bufferLength];

            int rv = Libc.strerror_r(errno, buffer, bufferLength);

            return rv == 0 ? Marshal.PtrToStringAnsi((IntPtr)buffer) : $"errno {errno}";
        }

        public static void Throw() => throw new PosixException();
    }

    public static class Error
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
