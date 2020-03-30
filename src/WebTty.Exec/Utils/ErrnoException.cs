using System;
using System.Runtime.InteropServices;
using WebTty.Exec.Native;

namespace WebTty.Exec.Utils
{
    internal class ErrnoException : Exception
    {
        public ErrnoException() : this(Libc.errno)
        { }

        public ErrnoException(int errno) :
            base(GetErrorMessage(errno))
        {
            HResult = errno;
        }

        private unsafe static string GetErrorMessage(int errno)
        {
            var bufferLength = 1024;
            var buffer = stackalloc byte[bufferLength];

            int rv = Libc.strerror_r(errno, buffer, bufferLength);

            return rv == 0 ? Marshal.PtrToStringAnsi((IntPtr)buffer) : $"errno {errno}";
        }

        public static void Throw() => throw new ErrnoException();
    }
}
