using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebTty.Exec;

namespace WebTty.Terminal
{
    public class Terminal : IDisposable
    {
        private IProcess proc { get; set; }

        public StreamWriter Input => proc.Stdin;
        public StreamReader Output => proc.Stdout;
        public Guid Id { get; set; }
        public bool IsRunning => proc.IsRunning;
        private bool Started { get; set; }

        public Terminal()
        {
            Id = Guid.NewGuid();
        }

        public void Start() =>
            Start(Process.GetDefaultShell());

        public void Start(string command) =>
            Start(command, Array.Empty<string>());

        public void Start(string command, IReadOnlyCollection<string> args)
        {
            var attr = new ProcAttr
            {
                RedirectStdin = true,
                RedirectStdout = true,

                Sys = new SysProcAttr
                {
                    UseTty = true,
                },
            };

            proc = Process.Start(command, args, attr);
        }

        public void Stop()
        {
            proc.Kill();
            proc.Wait();
        }

        public async Task WaitUntilReady()
        {
            if (!IsRunning && !Started)
            {
                while (!IsRunning)
                {
                    await Task.Delay(1);
                }

                Started = true;
            }
        }

        public void SetWindowSize(int height, int width) =>
            proc.SetWindowSize(height, width);

        public void Dispose()
        {
            proc.Dispose();
        }
    }
}
