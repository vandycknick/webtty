using System.Runtime.InteropServices;

namespace WebTty.Native.Syscall
{
    [StructLayout(LayoutKind.Sequential)]
    public struct passwd
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_name;       /* user name */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_passwd;     /* encrypted password */
        public uid_t pw_uid;         /* user uid */
        public gid_t pw_gid;         /* user gid */
        private time_t pw_change;      /* password change time */
        [MarshalAs(UnmanagedType.LPStr)]
        private string pw_class;      /* user access class */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_gecos;      /* Honeywell login info */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_dir;        /* home directory */
        [MarshalAs(UnmanagedType.LPStr)]
        public string pw_shell;      /* default shell */
        private time_t pw_expire;      /* account expiration */
        private int pw_fields;      /* internal: fields filled in */
    };
}
