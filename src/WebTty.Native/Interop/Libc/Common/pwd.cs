using System;
using System.Runtime.InteropServices;
using static WebTty.Native.Interop.Libraries;

namespace WebTty.Native.Interop
{
    internal static unsafe partial class Libc
    {
        internal static passwd getpwuid(uid_t uid)
        {
            var pwd = _getpwuid(uid);

            if (pwd == IntPtr.Zero) throw new NullReferenceException($"getpwuid returned null for uuid_t ({uid})");

            return (passwd)Marshal.PtrToStructure(pwd, typeof(passwd));
        }

        [DllImport(libc, EntryPoint = "getpwuid", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr _getpwuid(uid_t uid);
    }
}
