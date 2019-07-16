using System;
using System.Runtime.InteropServices;

namespace WebTty.Native.Interop
{
    internal class PosixException: Exception
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
}
