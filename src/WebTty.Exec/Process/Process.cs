using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WebTty.Exec
{
    public sealed partial class Process : IProcess, IDisposable
    {
        private const int DEFAULT_WIDTH = 80;
        private const int DEFAULT_HEIGHT = 24;

        /// <summary>
        /// Starts a new process with the program,
        /// arguments and attributes specified by filename, argv and attr.
        /// The argv slice will become os.Args in the new process,
        /// so it normally starts with the program name.
        /// </summary>
        public static IProcess Start(string filename, IReadOnlyCollection<string> argv, ProcAttr attr)
        {
            if (filename == null || string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (argv == null)
            {
                throw new ArgumentNullException(nameof(argv));
            }

            var proc = new Process
            {
                _fileName = filename,
                _argv = argv,
                _attr = attr,
            };

            return proc.Start();
        }

        public StreamWriter Stdin { get; private set; } = StreamWriter.Null;
        public StreamReader Stdout { get; private set; } = StreamReader.Null;
        public StreamReader Stderr { get; private set; } = StreamReader.Null;
        public int Pid { get; private set; }
        public bool IsRunning { get; private set; }
        public int ExitCode { get; private set; }

        private string _fileName { get; set; }
        private IReadOnlyCollection<string> _argv { get; set; }
        private ProcAttr _attr { get; set; }

        private Process()
        {
        }

        private Process Start()
        {
            StartCore();
            return this;
        }


        /// <summary>
        /// </summary>
        public void Kill() => KillCore();

        /// <summary>
        /// </summary>
        public void Signal(int signal)
        {

        }

        /// <summary>
        /// </summary>
        public void Wait() => Wait(Timeout.Infinite);

        /// <summary>
        /// </summary>
        public void Wait(int timeout) => WaitCore();

        public void SetWindowSize(int height, int width) =>
            SetWindowSizeCore(height, width);

        public void Dispose()
        {
            Stdin.Dispose();
            Stdout.Dispose();
            Stderr.Dispose();

            CloseCore();
        }

        private static string[] GetEnvironmentVariables(IDictionary env)
        {
            var l = new List<string>();

            foreach (var key in env.Keys)
            {
                l.Add($"{key}={env[key]}");
            }
            return l.ToArray();
        }

        private static string ResolvePath(string filename)
        {
            // If path rooted then we have an absolute path to an executable
            if (Path.IsPathRooted(filename)) return filename;

            // Check if executable is in the current directory
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
    }
}
