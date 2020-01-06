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
        private readonly CommandLineOptions _options;
        private readonly ConcurrentDictionary<string, Terminal> _terminals = new ConcurrentDictionary<string, Terminal>();

        public TerminalManager(IConnectionHandler handler, CommandLineOptions options)
        {
            _handler = handler;
            _options = options;
        }

        public Terminal Start()
        {
            var terminal = new Terminal();

            if (!string.IsNullOrEmpty(_options.Command))
            {
                terminal.Start(_options.Command, _options.CommandArgs);
            }
            else
            {
                terminal.Start();
            }

            _terminals.TryAdd(terminal.Id.ToString(), terminal);
            return terminal;
        }

        public void ProcessOutput(Terminal terminal, CancellationToken token = default)
        {
            var backend = Task.Factory.StartNew(
                function: () => ProcessOutputCoreAsync(terminal, token),
                cancellationToken: token,
                creationOptions: TaskCreationOptions.LongRunning,
                scheduler: TaskScheduler.Default
            );
        }

        private async Task ProcessOutputCoreAsync(Terminal terminal, CancellationToken token)
        {
            await terminal.WaitUntilReady();

            const int maxReadSize = 1024;
            const int maxBufferSize = maxReadSize * sizeof(char);
            var buffer = new char[maxReadSize];
            var byteBuffer = new byte[maxBufferSize];

            using (var t = token.Register(() => Kill(terminal.Id)))
            {
                while (!terminal.Output.EndOfStream && !t.Token.IsCancellationRequested)
                {
                    try
                    {
                        var read = await terminal.Output.ReadAsync(buffer.AsMemory(), t.Token);

                        if (read == 0) continue;

                        var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), byteBuffer);
                        var byteSegment = new ArraySegment<byte>(byteBuffer, 0, bytesWritten);
                        var stdOut = new StdOutMessage(
                            tabId: terminal.Id.ToString(),
                            data: byteSegment
                        );

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
                await terminal.Input.WriteAsync(input, token);
                await terminal.Input.FlushAsync();
            }
        }

        public void Resize(string id, int cols, int rows)
        {
            if (_terminals.TryGetValue(id, out var terminal))
            {
                terminal.SetWindowSize(rows, cols);
            }
        }

        public void Kill(Guid id)
        {
            if (_terminals.TryRemove(id.ToString(), out var removed))
            {
                Console.WriteLine($"Killing terminal with id {id}");
                removed.Stop();
                removed.Dispose();
            }
        }
    }
}
