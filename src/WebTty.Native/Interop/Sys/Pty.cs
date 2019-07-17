using System;
using WebTty.Native.Utils;

namespace WebTty.Native.Interop
{
    internal static partial class Sys
    {
        public static int ForkPtyAndExec(string programName, string[] args, string[] env, out int master, winsize winSize)
        {
            unsafe
            {
                byte** argvPtr = null;
                byte** envpPtr = null;

                try
                {
                    ArrayHelpers.AllocNullTerminatedArray(args, ref argvPtr);
                    ArrayHelpers.AllocNullTerminatedArray(env, ref envpPtr);
                    var pid = Libc.forkpty(out master, IntPtr.Zero, IntPtr.Zero, ref winSize);

                    if (pid < 0) throw new Exception("Could not create Pty");

                    if (pid == 0)
                    {
                        Libc.execve(programName, argvPtr, envpPtr);
                        throw new Exception("Execve should not return");
                    }

                    return pid;
                }
                finally
                {
                    ArrayHelpers.FreeArray(argvPtr, args.Length);
                    ArrayHelpers.FreeArray(envpPtr, env.Length);
                }
            }
        }

        /// <summary>
        /// Sends a request to the pseudo terminal to set the size to the specified one
        /// </summary>
        /// <param name="fd">File descriptor returned by ForkPty</param>
        /// <param name="winSize">The desired window size</param>
        /// <returns></returns>
        public static int PtySetWinSize(int fd, ref winsize winSize)
        {
            var r = Libc.ioctl(fd, Libc.TIOCSWINSZ, ref winSize);
            if (r == -1)
            {
                var lastErr = Libc.errno;
                Console.WriteLine(lastErr);
            }
            return r;
        }
    }
}
