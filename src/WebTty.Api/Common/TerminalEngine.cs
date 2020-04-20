using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Exec;
using WebTty.Api.Models;

namespace WebTty.Api.Common
{
    public class TerminalEngine : IEngine
    {
        class TerminalProcess
        {
            public Terminal Terminal { get; set; }
            public IProcess Process { get; set; }
        }

        private readonly ConcurrentDictionary<string, TerminalProcess> _terminals = new ConcurrentDictionary<string, TerminalProcess>();
        private readonly ILogger<TerminalEngine> _logger;

        public TerminalEngine(ILogger<TerminalEngine> logger)
        {
            _logger = logger;
        }

        public Terminal StartNew() => StartNew(Process.GetDefaultShell());
        public Terminal StartNew(string command) => StartNew(command, Array.Empty<string>());

        public Terminal StartNew(string command, IReadOnlyCollection<string> args)
        {
            var id = Guid.NewGuid();

            var attr = new ProcAttr
            {
                RedirectStdin = true,
                RedirectStdout = true,
                RedirectStderr = true,

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

        public bool TryGetProcess(Terminal terminal, out IProcess proc) => TryGetProcess(terminal.Id, out proc);

        public bool TryGetProcess(string id, out IProcess proc)
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

        public void KillAll()
        {
            foreach (var key in _terminals.Keys)
            {
                Kill(key);
            }
        }

        public void Kill(string id)
        {
            if (_terminals.TryRemove(id, out var term))
            {
                _logger.LogInformation("Killing terminal with id {terminalId}", id);
                term.Process.Kill();
                term.Process.Wait();
                term.Process.Dispose();
            }
        }
    }
}
