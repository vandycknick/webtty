using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using WebTty.Native.Interop;

namespace WebTty.Native.Terminal
{
    public sealed partial class Terminal
    {
        private PtyPipe inputPipe;
        private PtyPipe outputPipe;

        private Process process;

        private IntPtr ptyHandle = IntPtr.Zero;

        public static string GetDefaultShell() => "cmd";

        private sealed class PtyPipe : IDisposable
        {
            private readonly SafeFileHandle _readSide;
            private readonly SafeFileHandle _writeSide;

            public SafeFileHandle Read => _readSide;

            public SafeFileHandle Write => _writeSide;

            public PtyPipe()
            {
                if (!Kernel32.CreatePipe(out _readSide, out _writeSide, IntPtr.Zero, 0))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "failed to create pipe");
                }
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {

                    }

                    disposedValue = true;
                }
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }

        private void StartCore(string shell, int width, int height)
        {
            // var filename = ResolvePath(shell);
            var filename = shell;
            inputPipe = new PtyPipe();
            outputPipe = new PtyPipe();

            var size = new COORD
            {
                X = (short)width,
                Y = (short)height,
            };

            var result = Kernel32.CreatePseudoConsole(
                size: size,
                hInput: inputPipe.Read,
                hOutput: outputPipe.Write,
                dwFlags: 0,
                phPC: out ptyHandle
            );

            if (result != 0) throw new Win32Exception(result, "Could not create pseudo console.");

            process = ProcessFactory.Start(filename, (IntPtr)Kernel32.PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE, ptyHandle);

            var outStream = new FileStream(outputPipe.Read, FileAccess.Read);
            StandardOut = new StreamReader(outStream);

            var inStream = new FileStream(inputPipe.Write, FileAccess.Write);
            StandardIn = new StreamWriter(inStream);

            childpid = process.ProcessInfo.dwProcessId;
        }

        private void WaitForExitCore()
        {
            if (process == null) return;

            var reset = new AutoResetEvent(false)
            {
                SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false)
            };

            reset.WaitOne(Timeout.Infinite);
        }

        private void KillCore()
        {
            DisposeManagedState();
            process?.Dispose();
        }

        private void CloseCore()
        {
            inputPipe?.Dispose();
            outputPipe?.Dispose();

            inputPipe = null;
            outputPipe = null;

            process?.Dispose();
            process = null;

            if (ptyHandle != IntPtr.Zero)
            {
                Kernel32.ClosePseudoConsole(ptyHandle);
            }
        }

        public void SetWindowSize(int col, int rows)
        {
            var size = new COORD
            {
                X = (short)col,
                Y = (short)rows,
            };

            Kernel32.ResizePseudoConsole(ptyHandle, size);
        }
    }
}
