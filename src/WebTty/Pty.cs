using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Posix;

namespace WebTty
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UnixWindowSize
    {
        public short row, col, xpixel, ypixel;
    }

    public class Pty
    {
        [DllImport("libc")]
        extern static int forkpty(out int master, IntPtr dataReturn, IntPtr termios, ref UnixWindowSize WinSz);

        [DllImport("libc", SetLastError = true)]
        private static extern unsafe int execve(string filename, byte** argv, byte** envp);

        [DllImport("libc", SetLastError = true)]
        extern static int ioctl(int fd, long cmd, ref UnixWindowSize WinSz);

        [DllImport("libc", SetLastError = true)]
        extern static int ioctl(int fd, long cmd, ref long size);

        public static int ForkAndExec(string programName, string[] args, string[] env, out int master, UnixWindowSize winSize)
        {
            unsafe
            {
                byte** argvPtr = null;
                byte** envpPtr = null;

                try
                {
                    AllocNullTerminatedArray(args, ref argvPtr);
                    AllocNullTerminatedArray(env, ref envpPtr);
                    var pid = forkpty(out master, IntPtr.Zero, IntPtr.Zero, ref winSize);
                    if (pid < 0)
                        throw new Exception("Could not create Pty");

                    if (pid == 0)
                    {
                        execve(programName, argvPtr, envpPtr);
                        throw new Exception("Execve should not return");
                    }

                    return pid;
                }
                finally
                {
                    FreeArray(argvPtr, args.Length);
                    FreeArray(envpPtr, env.Length);
                }
            }
        }

        /// <summary>
        /// Sends a request to the pseudo terminal to set the size to the specified one
        /// </summary>
        /// <param name="fd">File descriptor returned by ForkPty</param>
        /// <param name="winSize">The desired window size</param>
        /// <returns></returns>
        public static int SetWinSize(int fd, ref UnixWindowSize winSize)
        {
            const long MAC_TIOCSWINSZ = 0x80087467;
            var r = ioctl(fd, MAC_TIOCSWINSZ, ref winSize);
            if (r == -1)
            {
                var lastErr = Marshal.GetLastWin32Error();
                Console.WriteLine(lastErr);
            }
            return r;
        }

        /// <summary>
        /// Returns the number of bytes available for reading on a file descriptor
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int AvailableBytes(int fd, ref long size)
        {
            const long MAC_FIONREAD = 0x4004667f;
            var r = ioctl(fd, MAC_FIONREAD, ref size);
            if (r == -1)
            {
                var lastErr = Marshal.GetLastWin32Error();
                Console.WriteLine(lastErr);
            }
            return r;
        }

        // https://github.com/dotnet/corefx/blob/4464cceace7615502a1003bc7488f206a47c82ee/src/Common/src/Interop/Unix/System.Native/Interop.ForkAndExecProcess.cs#L61
        private static unsafe void AllocNullTerminatedArray(string[] arr, ref byte** arrPtr)
        {
            int arrLength = arr.Length + 1; // +1 is for null termination

            // Allocate the unmanaged array to hold each string pointer.
            // It needs to have an extra element to null terminate the array.
            arrPtr = (byte**)Marshal.AllocHGlobal(sizeof(IntPtr) * arrLength);
            Debug.Assert(arrPtr != null);

            // Zero the memory so that if any of the individual string allocations fails,
            // we can loop through the array to free any that succeeded.
            // The last element will remain null.
            for (int i = 0; i < arrLength; i++)
            {
                arrPtr[i] = null;
            }

            // Now copy each string to unmanaged memory referenced from the array.
            // We need the data to be an unmanaged, null-terminated array of UTF8-encoded bytes.
            for (int i = 0; i < arr.Length; i++)
            {
                byte[] byteArr = Encoding.UTF8.GetBytes(arr[i]);

                arrPtr[i] = (byte*)Marshal.AllocHGlobal(byteArr.Length + 1); //+1 for null termination
                Debug.Assert(arrPtr[i] != null);

                Marshal.Copy(byteArr, 0, (IntPtr)arrPtr[i], byteArr.Length); // copy over the data from the managed byte array
                arrPtr[i][byteArr.Length] = (byte)'\0'; // null terminate
            }
        }

        // https://github.com/dotnet/corefx/blob/4464cceace7615502a1003bc7488f206a47c82ee/src/Common/src/Interop/Unix/System.Native/Interop.ForkAndExecProcess.cs#L92
        private static unsafe void FreeArray(byte** arr, int length)
        {
            if (arr != null)
            {
                // Free each element of the array
                for (int i = 0; i < length; i++)
                {
                    if (arr[i] != null)
                    {
                        Marshal.FreeHGlobal((IntPtr)arr[i]);
                        arr[i] = null;
                    }
                }

                // And then the array itself
                Marshal.FreeHGlobal((IntPtr)arr);
            }
        }
    }
}
