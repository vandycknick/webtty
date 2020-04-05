using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using WebTty.Exec.Native;

namespace WebTty.Exec
{
    public sealed partial class Process
    {
        private sealed class Win32Pipe : IDisposable
        {
            private readonly SafeFileHandle _readSide;
            private readonly SafeFileHandle _writeSide;

            public SafeFileHandle Read => _readSide;

            public SafeFileHandle Write => _writeSide;

            public Win32Pipe()
            {
                if (!Kernel32.CreatePipe(out _readSide, out _writeSide, IntPtr.Zero, 0))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "failed to create pipe");
                }
            }

            public void Dispose()
            {
                _readSide.Dispose();
                _writeSide.Dispose();
            }
        }

        private STARTUPINFOEX startupInfo;
        private PROCESS_INFORMATION processInfo;
        private Win32Pipe _inputPipe;
        private Win32Pipe _outputPipe;
        private IntPtr _ptyHandle;

        private void StartCore()
        {
            if (!_attr.Sys.UseTty)
            {
                throw new Exception("Only a new process with controlled tty supported");
            }

            _inputPipe = new Win32Pipe();
            _outputPipe = new Win32Pipe();

            var size = new COORD
            {
                X = DEFAULT_WIDTH,
                Y = DEFAULT_HEIGHT,
            };

            var result = Kernel32.CreatePseudoConsole(
                size: size,
                hInput: _inputPipe.Read,
                hOutput: _outputPipe.Write,
                dwFlags: 0,
                phPC: out _ptyHandle
            );

            if (result != 0) throw new Win32Exception(result, "Could not create pseudo console.");

            startupInfo = ConfigureProcessThread(_ptyHandle, (IntPtr)Kernel32.PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE);
            processInfo = RunProcess(ref startupInfo, _fileName);

            if (_attr.RedirectStdin)
            {
                var inStream = new FileStream(_inputPipe.Write, FileAccess.Write);
                Stdin = new StreamWriter(inStream);
            }

            if (_attr.RedirectStdout)
            {
                var outStream = new FileStream(_outputPipe.Read, FileAccess.Read);
                Stdout = new StreamReader(outStream);
            }

            Pid = processInfo.dwProcessId;
            IsRunning = true;
        }

        private void KillCore()
        {
            // Free the attribute list
            if (startupInfo.lpAttributeList != IntPtr.Zero)
            {
                Kernel32.DeleteProcThreadAttributeList(startupInfo.lpAttributeList);
                Marshal.FreeHGlobal(startupInfo.lpAttributeList);
            }

            // Close process and thread handles
            if (processInfo.hProcess != IntPtr.Zero)
            {
                Kernel32.CloseHandle(processInfo.hProcess);
            }
            if (processInfo.hThread != IntPtr.Zero)
            {
                Kernel32.CloseHandle(processInfo.hThread);
            }
        }

        private void WaitCore()
        {
            var reset = new AutoResetEvent(false)
            {
                SafeWaitHandle = new SafeWaitHandle(processInfo.hProcess, ownsHandle: false)
            };

            reset.WaitOne(Timeout.Infinite);
        }

        private void SetWindowSizeCore(int height, int width)
        {
            var size = new COORD
            {
                X = (short)width,
                Y = (short)height,
            };

            Kernel32.ResizePseudoConsole(_ptyHandle, size);
        }

        private void CloseCore()
        {
            _inputPipe?.Dispose();
            _outputPipe?.Dispose();

            _inputPipe = null;
            _outputPipe = null;


            if (_ptyHandle != IntPtr.Zero)
            {
                Kernel32.ClosePseudoConsole(_ptyHandle);
                _ptyHandle = IntPtr.Zero;
            }
        }


        private static STARTUPINFOEX ConfigureProcessThread(IntPtr hPC, IntPtr attributes)
        {
            // this method implements the behavior described in https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#preparing-for-creation-of-the-child-process

            var lpSize = IntPtr.Zero;
            var success = Kernel32.InitializeProcThreadAttributeList(
                lpAttributeList: IntPtr.Zero,
                dwAttributeCount: 1,
                dwFlags: 0,
                lpSize: ref lpSize
            );
            if (success || lpSize == IntPtr.Zero) // we're not expecting `success` here, we just want to get the calculated lpSize
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not calculate the number of bytes for the attribute list.");
            }

            var startupInfo = new STARTUPINFOEX();
            startupInfo.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();
            startupInfo.lpAttributeList = Marshal.AllocHGlobal(lpSize);

            success = Kernel32.InitializeProcThreadAttributeList(
                lpAttributeList: startupInfo.lpAttributeList,
                dwAttributeCount: 1,
                dwFlags: 0,
                lpSize: ref lpSize
            );
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not set up attribute list.");
            }

            success = Kernel32.UpdateProcThreadAttribute(
                lpAttributeList: startupInfo.lpAttributeList,
                dwFlags: 0,
                attribute: attributes,
                lpValue: hPC,
                cbSize: (IntPtr)IntPtr.Size,
                lpPreviousValue: IntPtr.Zero,
                lpReturnSize: IntPtr.Zero
            );
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not set pseudoconsole thread attribute.");
            }

            return startupInfo;
        }

        private static PROCESS_INFORMATION RunProcess(ref STARTUPINFOEX sInfoEx, string commandLine)
        {
            int securityAttributeSize = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
            var pSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
            var tSec = new SECURITY_ATTRIBUTES { nLength = securityAttributeSize };
            var success = Kernel32.CreateProcess(
                lpApplicationName: null,
                lpCommandLine: commandLine,
                lpProcessAttributes: ref pSec,
                lpThreadAttributes: ref tSec,
                bInheritHandles: false,
                dwCreationFlags: Kernel32.EXTENDED_STARTUPINFO_PRESENT,
                lpEnvironment: IntPtr.Zero,
                lpCurrentDirectory: null,
                lpStartupInfo: ref sInfoEx,
                lpProcessInformation: out var processInfo
            );
            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), $"Could not create process. {commandLine}");
            }

            return processInfo;
        }

        public static string GetDefaultShell() => "cmd";
    }
}
