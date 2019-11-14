using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using Microsoft.Win32.SafeHandles;
using WebTty.Native.Interop;
using WebTty.Native.Utils;

namespace WebTty.Native.Terminal
{
    public sealed partial class Terminal
    {
        private int _ptyHandle;
        private UnixStream _ptyStream;

        public static string GetDefaultShell()
        {
            var uid = Libc.getuid();
            var pwd = Libc.getpwuid(uid);
            return pwd.pw_shell;
        }

        private void StartCore(string shell, int width, int height)
        {
            var filename = ResolvePath(shell);
            var size = new winsize
            {
                ws_col = (ushort)width,
                ws_row = (ushort)height
            };

            var pid = ForkPtyAndExec(
                filename,
                new string[] { filename },
                GetEnvironmentVariables(),
                out _ptyHandle,
                size
            );

            processId = pid;

            var ret = Libc.waitpid(processId, out var _, WaitPidOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }

            _ptyStream = new UnixStream(_ptyHandle);
            StandardIn = new StreamWriter(_ptyStream);
            StandardOut = new StreamReader(_ptyStream);
        }

        private int WaitPidNoHang(int pid, out int exitCode)
        {
            int result;
            int status;
            while (Sys.ShouldRetrySyscall(result = Libc.waitpid(pid, out status, WaitPidOptions.WNOHANG))) ;

            if (result > 0)
            {
                if (Libc.WIFEXITED(status))
                {
                    exitCode = Libc.WEXITSTATUS(status);
                }
                else if (Libc.WIFSIGNALED(status))
                {
                    exitCode = 128 + Libc.WTERMSIG(status);
                }
                else
                {
                    exitCode = -1;
                }
            }
            else
            {
                exitCode = -1;
            }

            return result;
        }

        private void ReapChildren(bool all = true)
        {
            int result;
            int exitCode;

            while (Sys.ShouldRetrySyscall(result = Libc.waitpid(processId, out exitCode, WaitPidOptions.None))) ;

            if (result == processId)
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
                Sys.ThrowExceptionForLastError();
            }

            if (all) // Very ugly at the moment but will reap some leftover processes (not all the time though)
            {
                int pid;
                do
                {
                    pid = WaitPidNoHang(-1, out var _);
                } while (pid > 0);
            }

            IsRunning = false;
        }

        private void WaitForExitCore()
        {

        }

        private void KillCore()
        {
            if (Libc.kill(processId, Libc.SIGKILL) != 0)
            {
                throw new Win32Exception();
            }
        }

        public void CloseCore()
        {
            StandardIn.Dispose();
            StandardOut.Dispose();
            _ptyStream.Dispose();

            ReapChildren();
        }

        public void SetWindowSize(int col, int rows)
        {
            var size = new winsize
            {
                ws_col = (ushort)col,
                ws_row = (ushort)rows,
            };

            int retVal;
            do
            {
                retVal = Libc.ioctl(_ptyHandle, Libc.TIOCSWINSZ, ref size);
            } while (Sys.ShouldRetrySyscall(retVal));

            Sys.ThrowExceptionForLastErrorIf(retVal);
        }

        private static int ForkPtyAndExec(string programName, string[] args, string[] env, out int master, winsize winSize)
        {
            unsafe
            {
                byte** argvPtr = null;
                byte** envpPtr = null;

                try
                {
                    ArrayHelpers.AllocNullTerminatedArray(args, ref argvPtr);
                    ArrayHelpers.AllocNullTerminatedArray(env, ref envpPtr);

                    if (Libc.openpty(out master, out var slave, IntPtr.Zero, IntPtr.Zero, ref winSize) == -1)
                    {
                        throw new Win32Exception("Could not open a pty");
                    }

                    var pid = Libc.fork();

                    if (pid < 0)
                    {
                        Libc.close(master);
                        Libc.close(slave);
                        throw new Exception("Error during fork");
                    }

                    if (pid == 0)
                    {


                        Libc.close(master);

                        if (Libc.login_tty(slave) == -1)
                        {
                            Console.WriteLine($"could not login to tty {Libc.errno}");
                            // throw new Exception("could not login to tty");
                        }

                        Libc.execve(programName, argvPtr, envpPtr);
                        throw new Exception("Execve should not return");
                    }

                    Libc.close(slave);

                    return pid;
                }
                finally
                {
                    ArrayHelpers.FreeArray(argvPtr, args.Length);
                    ArrayHelpers.FreeArray(envpPtr, env.Length);
                }
            }
        }
    }
}
