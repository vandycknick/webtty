using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WebTty.Native.Interop
{
    internal static partial class Kernel32
    {
        internal const uint PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;

        /// TODO: Add documentation
        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate bool PHANDLER_ROUTINE(CTRL_EVENT CtrlType);

        /// TODO: Add documentation
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetConsoleCtrlHandler(PHANDLER_ROUTINE HandlerRoutine, [MarshalAs(UnmanagedType.Bool)] bool add);

        /// TODO: Add documentation
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out IntPtr phPC);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

        /// TODO: Add documentation
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern int ClosePseudoConsole(IntPtr hPC);

        /// TODO: Add documentation
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, IntPtr lpPipeAttributes, int nSize);
    }

    /// <summary> The type of signal to be generated </summary>
    internal enum CTRL_EVENT
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD
    {
        public short X;
        public short Y;
    }
}
