using System;
using System.Collections.Generic;
using System.IO;
using Mono.Unix;
using Mono.Unix.Native;

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
            var size = new UnixWindowSize()
            {
                col = (short)155,
                row = (short)43,
            };

            var pid = Pty.ForkAndExec(
                "/usr/local/bin/bash",
                new string[] { "/usr/local/bin/bash" },
                GetEnvironmentVariables(),
                out pty,
                size
            );

            // var pid = Pty.ForkAndExec(
            //     "/Users/nickvd/.pyenv/shims/ipython",
            //     new string[] { "/Users/nickvd/.pyenv/shims/ipython" },
            //     GetEnvironmentVariables(),
            //     out pty,
            //     size
            // );

            childpid = pid;

            var ret = Syscall.waitpid(childpid, out var status, WaitOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }

            Console.WriteLine(pid);

            var stream = new UnixStream(pty);

            StandardIn = new StreamWriter(stream);
            StandardOut = new StreamReader(stream);
        }

        public bool IsRunning { get; private set; }
        public int ExitCode { get; private set; } = 0;

        public void WaitForExit()
        {
            int ret;

            ret = Syscall.waitpid(childpid, out var status, 0);

            IsRunning = false;

            if (ret == childpid)
            {
                ExitCode = Syscall.WEXITSTATUS(status);
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
                if (env.Contains(x))
                    l.Add($"{x}={env[x]}");
            return l.ToArray();
        }
    }
}
