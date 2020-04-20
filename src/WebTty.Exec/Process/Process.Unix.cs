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
        private const int READ_END_OF_PIPE = 0;
        private const int WRITE_END_OF_PIPE = 1;

        private void StartCore()
        {
            var filename = ResolvePath(_fileName);
            var args = new List<string>
            {
                filename,
            };

            // TODO: figure out what best todo here
            // Problem when first arg is an empty string stuff goes awfully wrong
            // Some things to validate:
            // - Check if this is only a problem for the first arg
            // - Check if this is only a problem when there is just one arg
            // - Is it better to just throw an error?
            foreach (var arg in _argv)
            {
                if (!string.IsNullOrWhiteSpace(arg) && !string.IsNullOrEmpty(arg))
                {
                    args.Add(arg);
                }
            }

            Pid = ForkAndExec(
                filename: filename,
                argv: args.ToArray(),
                envp: GetEnvironmentVariables(_attr.Env),
                cwd: _attr.Dir,
                useTty: _attr.Sys != null ? _attr.Sys.UseTty : false,
                redirectStdin: _attr.RedirectStdin,
                redirectStdout: _attr.RedirectStdout,
                redirectStderr: _attr.RedirectStderr,
                stdin: out var stdin,
                stdout: out var stdout,
                stderr: out var stderr
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

            var ret = Libc.waitpid(Pid, out var status, WaitPidOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }
            else if(ret == Pid)
            {
                IsRunning = false;
                ExitCode = GetUnixExitCode(status);
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
            int status;

            if (IsRunning == false) return;

            while (Error.ShouldRetrySyscall(result = Libc.waitpid(Pid, out status, WaitPidOptions.None))) ;
            if (result == Pid)
            {
                ExitCode = GetUnixExitCode(status);
            }
            else
            {
                // Unexpected.
                Error.ThrowExceptionForLastError();
            }

            IsRunning = false;
        }

        private int GetUnixExitCode(int status)
        {
            if (Libc.WIFEXITED(status))
            {
                return Libc.WEXITSTATUS(status);
            }
            else if (Libc.WIFSIGNALED(status))
            {
                return 128 + Libc.WTERMSIG(status);
            }
            else
            {
                return -1;
            }
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

        private void CloseCore()
        {

        }

        internal static void CreateCloseOnExecPipe(int[] pipeFds)
        {
            int result;

            while (Error.ShouldRetrySyscall(result = Libc.pipe(pipeFds)));

            Error.ThrowExceptionForLastErrorIf(result);

            while (Error.ShouldRetrySyscall(result = Libc.fcntl(pipeFds[0], Libc.F_SETFD, Libc.FD_CLOEXEC)));

            if (result == 0)
            {
                while (Error.ShouldRetrySyscall(result = Libc.fcntl(pipeFds[1], Libc.F_SETFD, Libc.FD_CLOEXEC)));
            }

            if (result != 0)
            {
                int tmpErrno = Libc.errno;
                Libc.close(pipeFds[0]);
                Libc.close(pipeFds[1]);

                Error.ThrowExceptionFor(tmpErrno);
            }
        }

        // TODO: restructure this function
        // For example posix_spawn from chromium seems to have a
        // maintainable flow: https://chromium.googlesource.com/native_client/nacl-newlib/+/bf66148d14c7fca26b9198dd5dc81e743893bb66/newlib/libc/posix/posix_spawn.c
        private static unsafe int ForkAndExec(
            string filename, string[] argv, string[] envp,
            string cwd, bool useTty, bool redirectStdin,
            bool redirectStdout, bool redirectStderr,
            out int stdin, out int stdout, out int stderr
        )
        {
            byte** argvPtr = null;
            byte** envpPtr = null;

            bool success = true;

            ArrayHelpers.AllocNullTerminatedArray(argv, ref argvPtr);
            ArrayHelpers.AllocNullTerminatedArray(envp, ref envpPtr);

            int[] stdInFds = new[] { -1, -1 };
            int[] stdOutFds = new[] { -1, -1 };
            int[] stdErrFds = new[] { -1, -1 };
            int masterFd = -1;
            int slaveFd = -1;

            try
            {
                int inFd, outFd, errFd;

                if (filename == null || argv == null || envp == null)
                {
                    success = false;
                    throw new ArgumentException("Provide the correct arguments");
                }

                if (Libc.access(filename, Libc.X_OK) != 0)
                {
                    success = false;
                    throw new Exception("The given file is not accessible");
                }

                if (useTty)
                {
                    var size = new winsize
                    {
                        ws_col = DEFAULT_WIDTH,
                        ws_row = DEFAULT_HEIGHT
                    };

                    if (Libc.openpty(out masterFd, out slaveFd, IntPtr.Zero, IntPtr.Zero, ref size) == -1)
                    {
                        success = false;
                        throw new Exception("Could not open a new pty");
                    }
                }
                else
                {
                    try
                    {
                        if (redirectStdin) CreateCloseOnExecPipe(stdInFds);
                        if (redirectStdout) CreateCloseOnExecPipe(stdOutFds);
                        if (redirectStderr) CreateCloseOnExecPipe(stdErrFds);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        throw ex;
                    }
                }

                var pid = Libc.fork();

                if (pid < 0)
                {
                    success = false;
                    Error.ThrowExceptionForLastError();
                }

                if (pid == 0)
                {
                    if (useTty)
                    {
                        Libc.close(masterFd);
                        Libc.setsid();

                        if (Libc.ioctl(slaveFd, Libc.TIOCSCTTY) == -1)
                        {
                            success = false;
                            Error.ThrowExceptionForLastError();
                        }

                        inFd = outFd = errFd = slaveFd;
                    }
                    else
                    {
                        inFd = stdInFds[READ_END_OF_PIPE];
                        outFd = stdOutFds[WRITE_END_OF_PIPE];
                        errFd = stdErrFds[WRITE_END_OF_PIPE];
                    }

                    // TODO: this code is just horrible and the likely hood introducing bugs here is quite high.
                    // I should refactor this asap. But first I would love to get some more tests in place that
                    // could catch any regressions..
                    if (redirectStdin)
                    {
                        while (Error.ShouldRetrySyscall(Libc.dup2(inFd, Libc.STDIN_FILENO)) && Libc.errno == Libc.EBUSY);
                    }

                    if (redirectStdout)
                    {
                        while (Error.ShouldRetrySyscall(Libc.dup2(outFd, Libc.STDOUT_FILENO)) && Libc.errno == Libc.EBUSY);
                    }

                    if (redirectStderr)
                    {
                        while (Error.ShouldRetrySyscall(Libc.dup2(errFd, Libc.STDERR_FILENO)) && Libc.errno == Libc.EBUSY);
                    }

                    if (!string.IsNullOrEmpty(cwd))
                    {
                        var ret = Libc.chdir(cwd);

                        if (ret == -1)
                        {
                            success = false;
                            Error.ThrowExceptionForLastError();
                        }
                    }

                    Libc.execve(filename, argvPtr, envpPtr);

                    // Exec syscall should never return, and thus we should never get here, if we do then exit immediately
                    Libc._exit(Libc.errno != 0 ? Libc.errno : -1);
                }

                Libc.close(slaveFd);

                if (useTty)
                {
                    stdin = masterFd;
                    stdout = masterFd;
                    stderr = masterFd;
                }
                else
                {
                    stdin = stdInFds[WRITE_END_OF_PIPE];
                    stdout = stdOutFds[READ_END_OF_PIPE];
                    stderr = stdErrFds[READ_END_OF_PIPE];
                }

                return pid;
            }
            finally
            {
                // Regardless of success or failure, close the parent's copy of the child's end of
                // any opened pipes. The parent doesn't need them anymore.
                CloseIfOpen(stdInFds[READ_END_OF_PIPE]);
                CloseIfOpen(stdOutFds[WRITE_END_OF_PIPE]);
                CloseIfOpen(stdErrFds[WRITE_END_OF_PIPE]);

                if (!success)
                {
                    // Cleanup all open fd
                    CloseIfOpen(masterFd);
                    CloseIfOpen(slaveFd);

                    CloseIfOpen(stdInFds[WRITE_END_OF_PIPE]);
                    CloseIfOpen(stdOutFds[READ_END_OF_PIPE]);
                    CloseIfOpen(stdErrFds[READ_END_OF_PIPE]);
                }

                ArrayHelpers.FreeArray(argvPtr, argv.Length);
                ArrayHelpers.FreeArray(envpPtr, envp.Length);
            }
        }

        private static void CloseIfOpen(int fd)
        {
            if (fd >= 0)
            {
                Libc.close(fd); // Ignoring errors from close is a deliberate choice
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
