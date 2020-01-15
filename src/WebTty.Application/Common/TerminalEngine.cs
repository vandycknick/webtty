using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Application.Entities;
using WebTty.Exec;

namespace WebTty.Application.Common
{
    public class TerminalEngine
    {
        class TerminalProcess
        {
            public Terminal Terminal { get; set; }
            public IProcess Process { get; set; }
        }

        private readonly ConcurrentDictionary<string, TerminalProcess> _terminals = new ConcurrentDictionary<string, TerminalProcess>();

        public Terminal StartNew() => StartNew(Process.GetDefaultShell());
        public Terminal StartNew(string command) => StartNew(command, Array.Empty<string>());

        public Terminal StartNew(string command, IReadOnlyCollection<string> args)
        {
            var id = Guid.NewGuid();

            var attr = new ProcAttr
            {
                RedirectStdin = true,
                RedirectStdout = true,

                Sys = new SysProcAttr
                {
                    UseTty = true,
                },
            };

            var proc = Process.Start(command, args, attr);
            var terminal = new Terminal
            {
                Id = id.ToString(),
                Command = command,
            };

            _terminals.TryAdd(terminal.Id, new TerminalProcess
            {
                Terminal = terminal,
                Process = proc,
            });
            return terminal;
        }

        public bool TryGetTerminalProc(Terminal terminal, out IProcess proc) => TryGetTerminalProc(terminal.Id, out proc);

        public bool TryGetTerminalProc(string id, out IProcess proc)
        {
            if (_terminals.TryGetValue(id, out var term))
            {
                proc = term.Process;
                return true;
            }

            proc = null;
            return false;
        }

        public async ValueTask Write(string id, ReadOnlyMemory<char> input, CancellationToken token)
        {
            if (_terminals.TryGetValue(id, out var term))
            {
                await term.Process.Stdin.WriteAsync(input, token);
                await term.Process.Stdin.FlushAsync();
            }
        }

        public async Task KillAllAsync()
        {
            await Task.Delay(0);

            foreach (var key in _terminals.Keys)
            {
                KillAsync(key);
            }
        }

        public void KillAsync(string id)
        {
            if (_terminals.TryRemove(id, out var term))
            {
                Console.WriteLine($"Killing terminal with id {id}");
                term.Process.Kill();
                term.Process.Wait();
                term.Process.Dispose();
            }
        }
    }
}
