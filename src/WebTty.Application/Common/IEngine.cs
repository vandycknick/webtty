using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Application.Entities;
using WebTty.Exec;

namespace WebTty.Application.Common
{
    public interface IEngine
    {
        Terminal StartNew();
        Terminal StartNew(string command);
        Terminal StartNew(string command, IReadOnlyCollection<string> args);
        bool TryGetProcess(Terminal terminal, out IProcess proc);
        bool TryGetProcess(string id, out IProcess proc);
        ValueTask Write(string id, ReadOnlyMemory<char> input, CancellationToken token);
        void KillAll();
        void Kill(string id);
    }
}
