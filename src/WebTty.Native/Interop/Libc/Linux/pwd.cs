using System.Runtime.InteropServices;

namespace WebTty.Native.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct passwd
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_name;       /* username */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_passwd;     /* user password */
        public uid_t pw_uid;        /* user ID */
        public gid_t pw_gid;        /* group ID */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_gecos;      /* user information */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_dir;        /* home directory */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_shell;      /* shell program */
    };
}
