using WebTty.Native.Interop;

namespace WebTty.Native.Terminal
{
    public sealed partial class Shell
    {
        public static string GetUserDefault()
        {
            var uid = Libc.getuid();
            var pwd = Libc.getpwuid(uid);
            return pwd.pw_shell;
        }
    }
}
