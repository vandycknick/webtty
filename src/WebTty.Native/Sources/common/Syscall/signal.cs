using System.Runtime.InteropServices;
using static WebTty.Native.Libraries;

namespace WebTty.Native.Syscall
{
    public static partial class Libc
    {
        public static int SIGHUP => 1;
        public static int SIGINT => 2;
        public static int SIGQUIT => 3;
        public static int SIGILL => 4;
        public static int SIGTRAP => 5;
        public static int SIGABRT => 6;
        public static int SIGIOT => SIGABRT;
        public static int SIGBUS => 7;
        public static int SIGFPE => 8;
        public static int SIGKILL => 9;
        public static int SIGUSR1 => 10;
        public static int SIGSEGV => 11;
        public static int SIGUSR2 => 12;
        public static int SIGPIPE => 13;
        public static int SIGALRM => 14;
        public static int SIGTERM => 15;
        public static int SIGSTKFLT => 16;
        public static int SIGCHLD => 17;
        public static int SIGCONT => 18;
        public static int SIGSTOP => 19;
        public static int SIGTSTP => 20;
        public static int SIGTTIN => 21;
        public static int SIGTTOU => 22;
        public static int SIGURG => 23;
        public static int SIGXCPU => 24;
        public static int SIGXFSZ => 25;
        public static int SIGVTALRM => 26;
        public static int SIGPROF => 27;
        public static int SIGWINCH => 28;
        public static int SIGIO => 29;
        public static int SIGPOLL => 29;
        public static int SIGPWR => 30;
        public static int SIGSYS => 31;

        [DllImport(libc, SetLastError = true)]
        public static extern int kill(pid_t pid, int signum);
    }
}
