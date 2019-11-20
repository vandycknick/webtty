using System;
using System.Collections.Generic;
using System.ComponentModel;
using WebTty.Exec.Native;
using WebTty.Exec.Utils;
using WebTty.Exec.IO;

namespace WebTty.Exec
{
    public sealed partial class Process
    {
        private void StartCore()
        {
            var filename = ResolvePath(_fileName);
            var args = new List<string>
            {
                filename,
            };

            args.AddRange(_argv);

            Pid = ForkAndExec(
                filename,
                args.ToArray(),
                GetEnvironmentVariables(_attr.Env),
                _attr,
                out var stdin,
                out var stdout,
                out var stderr
            );

            if (_attr.RedirectStdin)
            {
                Stdin = new System.IO.StreamWriter(
                    new UnixFileDescriptorStream(stdin)
                );
            }

            if (_attr.RedirectStdout)
            {
                Stdout = new System.IO.StreamReader(
                    new UnixFileDescriptorStream(stdout)
                );
            }

            if (_attr.RedirectStderr)
            {
                Stderr = new System.IO.StreamReader(
                    new UnixFileDescriptorStream(stderr)
                );
            }

            var ret = Libc.waitpid(Pid, out var s, WaitPidOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }
        }

        private void KillCore()
        {
            if (Libc.kill(Pid, Libc.SIGKILL) != 0)
            {
                throw new Win32Exception();
            }
        }

        private void WaitCore()
        {
            int result;
            int exitCode;

            while (Error.ShouldRetrySyscall(result = Libc.waitpid(Pid, out exitCode, WaitPidOptions.None))) ;
            if (result == Pid)
            {

                if (Libc.WIFEXITED(exitCode))
                {
                    ExitCode = Libc.WEXITSTATUS(exitCode);
                }
                else if (Libc.WIFSIGNALED(exitCode))
                {
                    ExitCode = 128 + Libc.WTERMSIG(exitCode);
                }
                else
                {
                    ExitCode = -1;
                }
            }
            else
            {
                // Unexpected.
                Error.ThrowExceptionForLastError();
            }

            IsRunning = false;
        }

        private void SetWindowSizeCore(int height, int width)
        {
            var size = new winsize
            {
                ws_col = (ushort)width,
                ws_row = (ushort)height,
            };

            // TODO: add some error handling, or find a better way to key a reference to the master fd
            var stream = (UnixFileDescriptorStream)Stdin.BaseStream;

            int retVal;
            do
            {
                retVal = Libc.ioctl((int)stream.Fd, Libc.TIOCSWINSZ, ref size);
            } while (Error.ShouldRetrySyscall(retVal));

            Error.ThrowExceptionForLastErrorIf(retVal);
        }

        public void CloseCore()
        {

        }

        private static unsafe int ForkAndExec(
            string filename, string[] args, string[] env, ProcAttr attr,
            out int stdin, out int stdout, out int stderr
        )
        {
            byte** argvPtr = null;
            byte** envpPtr = null;

            try
            {
                ArrayHelpers.AllocNullTerminatedArray(args, ref argvPtr);
                ArrayHelpers.AllocNullTerminatedArray(env, ref envpPtr);

                int inFd, outFd, errFd;
                int masterFd, slaveFd;
                if (attr.Sys.UseTty)
                {
                    var size = new winsize
                    {
                        ws_col = DEFAULT_WIDTH,
                        ws_row = DEFAULT_HEIGHT
                    };

                    if (Libc.openpty(out masterFd, out slaveFd, IntPtr.Zero, IntPtr.Zero, ref size) == -1)
                    {
                        throw new Exception("Could not open a new pty");
                    }
                    inFd = outFd = errFd = slaveFd;
                }
                else
                {
                    throw new Exception("Only a new process with controlled tty supported");
                }

                var pid = Libc.fork();

                if (pid < 0)
                {
                    Libc.close(masterFd);
                    Libc.close(slaveFd);
                    Error.ThrowExceptionForLastError();
                }

                if (pid == 0)
                {
                    if (attr.Sys.UseTty)
                    {
                        Libc.close(masterFd);
                        Libc.setsid();

                        if (Libc.ioctl(slaveFd, Libc.TIOCSCTTY) == -1)
                        {
                            throw new Exception("Error trying to become controlling tty");
                        }
                    }

                    while (Libc.dup2(inFd, Libc.STDIN_FILENO) == -1 && Libc.errno == Libc.EBUSY);
                    while (Libc.dup2(outFd, Libc.STDOUT_FILENO) == -1 && Libc.errno == Libc.EBUSY);
                    while (Libc.dup2(errFd, Libc.STDERR_FILENO) == -1 && Libc.errno == Libc.EBUSY);

                    Libc.execve(filename, argvPtr, envpPtr);
                    throw new Exception("Execve should not return");
                }

                Libc.close(slaveFd);

                stdin = masterFd;
                stdout = masterFd;
                stderr = masterFd;

                return pid;
            }
            finally
            {
                ArrayHelpers.FreeArray(argvPtr, args.Length);
                ArrayHelpers.FreeArray(envpPtr, env.Length);
            }
        }

        public static string GetDefaultShell()
        {
            var uid = Libc.getuid();
            var pwd = Libc.getpwuid(uid);
            return pwd.pw_shell;
        }
    }
}
