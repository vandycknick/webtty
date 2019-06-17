using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using WebTty.Native;
using WebTty.Native.Syscall;

namespace WebTty
{
    public class Terminal
    {
        public StreamWriter StandardIn { get; private set; }
        public StreamReader StandardOut { get; private set; }

        private int childpid;
        private int pty;

        public void Start()
        {
            var size = new winsize()
            {
                ws_col = 80,
                ws_row = 24,
            };

            var pid = Pty.ForkAndExec(
                "/usr/local/bin/bash",
                new string[] { "/usr/local/bin/bash" },
                GetEnvironmentVariables(),
                out pty,
                size
            );

            childpid = pid;

            var ret = Libc.waitpid(childpid, out var status, WaitPidOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }

            Console.WriteLine(pid);

            var stream = new FdStream(pty);

            StandardIn = new StreamWriter(stream);
            StandardOut = new StreamReader(stream);
        }

        public bool IsRunning { get; private set; }
        public int ExitCode { get; private set; } = 0;

        public void Kill()
        {
            if (Libc.kill(childpid, Libc.SIGKILL) != 0)
            {
                throw new Win32Exception();
            }
        }

        public void SetWindowSize(int col, int rows)
        {
            var size = new winsize
            {
                ws_col = (ushort)col,
                ws_row = (ushort)rows,
            };

            Pty.SetWinSize(pty, ref size);
        }

        public void WaitForExit()
        {
            int ret;

            ret = Libc.waitpid(childpid, out var status, WaitPidOptions.None);

            IsRunning = false;

            if (ret == childpid)
            {
                ExitCode = Libc.WEXITSTATUS(status);
            }
            else
            {
                ExitCode = -1;
            }
        }

        /// <summary>
        /// Provides a baseline set of environment variables that would be useful to run the terminal,
        /// you can customzie these accordingly.
        /// </summary>
        /// <returns></returns>
        public static string[] GetEnvironmentVariables(string termName = null)
        {
            var l = new List<string>();
            if (termName == null)
                termName = "xterm-256color";

            l.Add("TERM=" + termName);

            // Without this, tools like "vi" produce sequences that are not UTF-8 friendly
            l.Add("LANG=en_US.UTF-8");

            var env = Environment.GetEnvironmentVariables();
            foreach (var x in new[] { "LOGNAME", "USER", "DISPLAY", "LC_TYPE", "USER", "HOME", "PATH" })
            {
                if (env.Contains(x)) l.Add($"{x}={env[x]}");
            }
            return l.ToArray();
        }
    }
}
