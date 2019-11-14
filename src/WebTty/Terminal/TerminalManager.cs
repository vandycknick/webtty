using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Common;
using WebTty.Messages;

namespace WebTty.Terminal
{
    public class TerminalManager
    {
        private readonly IConnectionHandler _handler;

        private readonly ConcurrentDictionary<string, Native.Terminal.Terminal> _terminals = new ConcurrentDictionary<string, Native.Terminal.Terminal>();

        public TerminalManager(IConnectionHandler handler)
        {
            _handler = handler;
        }

        public Native.Terminal.Terminal Start()
        {
            var terminal = new Native.Terminal.Terminal();

            _terminals.TryAdd(terminal.Id, terminal);
            return terminal;
        }

        public void ProcessOutput(Native.Terminal.Terminal terminal, CancellationToken token = default)
        {
            var backend = Task.Factory.StartNew(
                function: () => ProcessOutputCoreAsync(terminal, token),
                cancellationToken: token,
                creationOptions: TaskCreationOptions.LongRunning,
                scheduler: TaskScheduler.Default
            );
        }

        private async Task ProcessOutputCoreAsync(Native.Terminal.Terminal terminal, CancellationToken token)
        {
            terminal.Start();
            // This is here because of a weird race condition, I should figure that on e out
            await Task.Delay(10);

            const int maxReadSize = 1024;
            const int maxBufferSize = maxReadSize * sizeof(char);
            var buffer = new char[maxReadSize];
            var byteBuffer = new byte[maxBufferSize];

            using (var t = token.Register(() => Kill(terminal.Id)))
            {
                while (!terminal.StandardOut.EndOfStream && !t.Token.IsCancellationRequested)
                {
                    try
                    {
                        var read = await terminal.StandardOut.ReadAsync(buffer.AsMemory(), t.Token);

                        if (read == 0) continue;

                        var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), byteBuffer);
                        var byteSegment = new ArraySegment<byte>(byteBuffer, 0, bytesWritten);
                        var stdOut = new StdOutMessage
                        {
                            TabId = terminal.Id,
                            Data = byteSegment,
                        };

                        await _handler.WriteAsync(stdOut);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        break;
                    }
                }
            }

            Kill(terminal.Id);
        }

        public async ValueTask SendInput(string id, ReadOnlyMemory<char> input, CancellationToken token)
        {
            if (_terminals.TryGetValue(id, out var terminal))
            {
                await terminal.StandardIn.WriteAsync(input, token);
                await terminal.StandardIn.FlushAsync();
            }
        }

        public void Resize(string id, int cols, int rows)
        {
            if (_terminals.TryGetValue(id, out var terminal))
            {
                terminal.SetWindowSize(cols, rows);
            }
        }

        public void Kill(string id)
        {
            if (_terminals.TryRemove(id, out var removed))
            {
                Console.WriteLine($"Killing terminal with id {id}");
                removed.Kill();
                removed.Dispose();
            }
        }
    }
}
