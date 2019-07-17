using System;
using System.Collections.Generic;
using System.IO;

namespace WebTty.Native.Terminal
{
    public sealed partial class Terminal : IDisposable
    {
        public const int DEFAULT_WIDTH = 80;
        public const int DEFAULT_HEIGHT = 24;
        private static int iid = 0;

        public StreamWriter StandardIn { get; private set; }
        public StreamReader StandardOut { get; private set; }

        private int childpid;

        public int Id { get; private set; }
        public bool IsRunning { get; private set; }
        public int ExitCode { get; private set; } = 0;

        public Terminal()
        {
            Id = ++iid;
        }

        public void Start()
        {
            var shell = Shell.GetUserDefault();
            StartCore(shell, DEFAULT_WIDTH, DEFAULT_HEIGHT);
        }

        public void Kill() => KillCore();

        public void WaitForExit() => WaitForExitCore();

        private static string ResolvePath(string filename)
        {
            // If path rooted assume it is absolute path to executable
            if (Path.IsPathRooted(filename)) return filename;

            // Check if executable the current directory
            string path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            if (File.Exists(path)) return path;

            // Then check each directory listed in the PATH environment variables
            return FindProgramInPath(filename);
        }

        private static string FindProgramInPath(string program)
        {
            string pathEnvVar = Environment.GetEnvironmentVariable("PATH");
            if (pathEnvVar != null)
            {
                foreach (var path in pathEnvVar.Split(Path.PathSeparator))
                {
                    var fulPath = Path.Combine(path, program);
                    if (File.Exists(fulPath))
                    {
                        return fulPath;
                    }
                }
            }
            return null;
        }

        public static string[] GetEnvironmentVariables(string termName = null)
        {
            var l = new List<string>();
            if (termName == null)
                termName = "xterm-256color";

            l.Add("TERM=" + termName);

            // Without this, tools like "vi" produce sequences that are not UTF-8 friendly
            l.Add("LANG=en_US.UTF-8");

            var env = Environment.GetEnvironmentVariables();
            foreach (var x in new[] { "LOGNAME", "USER", "DISPLAY", "LC_TYPE", "USER", "HOME", "PATH" })
            {
                if (env.Contains(x)) l.Add($"{x}={env[x]}");
            }
            return l.ToArray();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    DisposeManagedState();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Terminal()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
