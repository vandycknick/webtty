using System.ComponentModel;
using System.IO;
using WebTty.Native.Interop;

namespace WebTty.Native.Terminal
{
    public sealed partial class Terminal
    {
        private int ptyHandle;

        private void StartCore(string shell, int width, int height)
        {
            var filename = ResolvePath(shell);
            var size = new winsize
            {
                ws_col = (ushort)width,
                ws_row = (ushort)height
            };

            var pid = Sys.ForkPtyAndExec(
                filename,
                new string[] { filename },
                GetEnvironmentVariables(),
                out ptyHandle,
                size
            );

            childpid = pid;

            var ret = Libc.waitpid(childpid, out var _, WaitPidOptions.WNOHANG);

            if (ret == 0)
            {
                IsRunning = true;
            }

            var stream = new UnixStream(ptyHandle);

            StandardIn = new StreamWriter(stream);
            StandardOut = new StreamReader(stream);
        }

        private void WaitForExitCore()
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

        private void KillCore()
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

            int retVal;
            do
            {
                retVal = Libc.ioctl(ptyHandle, Libc.TIOCSWINSZ, ref size);
            } while (Sys.ShouldRetrySyscall(retVal));

            Sys.ThrowExceptionForLastErrorIf(retVal);
        }

        private void DisposeManagedState()
        {
        }
    }
}
