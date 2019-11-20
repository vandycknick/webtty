using System;
using System.Collections;
using System.IO;

namespace WebTty.Exec
{
    public class ProcAttr
    {
        // If Dir is non-empty, the child changes into the directory before
        // creating the process.
        public string Dir { get; set; }

        // If Env is non-nil, it gives the environment variables for the
        // new process in the form returned by Environ.
        // If it is nil, the result of Environ will be used.
        public IDictionary Env { get; set; } = Environment.GetEnvironmentVariables();

        public bool RedirectStdin { get; set; }

        public bool RedirectStdout { get; set; }

        public bool RedirectStderr { get; set; }

        // Operating system-specific process creation attributes.
        // Note that setting this field means that your program
        // may not execute properly or even compile on some
        // operating systems.
        public SysProcAttr Sys { get; set; }
    }
}
