using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Application.Common;
using WebTty.Application.Entities;
using WebTty.Application.Events;
using WebTty.Application.Requests;
using WebTty.Infrastructure.Connection;
using WebTty.Infrastructure.Dispatcher;

namespace WebTty.Application
{
    public class PtyMessageHandler : IMessageHandler
    {
        private readonly TerminalEngine _engine;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public PtyMessageHandler(TerminalEngine engine)
        {
            _engine = engine;
        }

        public async ValueTask<object> Handle(object message, IConnectionContext context, CancellationToken token = default)
        {
            switch (message)
            {
                case OpenNewTabRequest _:
                    var reply = await OpenNewTabHandler();
                    return reply;

                case ResizeTabRequest request:
                    ResizeTabHandler(request);
                    return Unit.Value;

                case SendInputRequest request:
                    await SendInputRequestHandler(request, token);
                    return Unit.Value;

                case OpenOutputRequest request:
                    _ = Task.Factory.StartNew(
                        function: () => OpenOutputRequestHandler(request, context.WriteMessageAsync, _tokenSource.Token).ConfigureAwait(false),
                        cancellationToken: _tokenSource.Token,
                        creationOptions: TaskCreationOptions.LongRunning,
                        scheduler: TaskScheduler.Default
                    );
                    return Unit.Value;

                default:
                    return new UnknownMessageEvent(nameof(message));
            }
        }

        public async Task<OpenNewTabReply> OpenNewTabHandler()
        {
            var terminal = _engine.StartNew();

            if (_engine.TryGetTerminalProc(terminal, out var process))
            {
                await process.WaitUntilReady();
            }

            return new OpenNewTabReply(id: terminal.Id);
        }

        public void ResizeTabHandler(ResizeTabRequest request)
        {
            if (_engine.TryGetTerminalProc(request.TabId, out var process))
            {
                process.SetWindowSize(request.Rows, request.Cols);
            }
        }

        public ValueTask SendInputRequestHandler(SendInputRequest request, CancellationToken token)
        {
            return _engine.Write(
                request.TabId,
                request.Payload.AsMemory(),
                token
            );
        }

        public async Task OpenOutputRequestHandler(OpenOutputRequest request, Func<object, Task> writeMessageAsync, CancellationToken token)
        {
            var terminalId = request.TabId;
            if (_engine.TryGetTerminalProc(terminalId, out var process))
            {
                const int maxReadSize = 1024;
                const int maxBufferSize = maxReadSize * sizeof(char);
                var buffer = new char[maxReadSize];
                var byteBuffer = new byte[maxBufferSize];

                while (!process.Stdout.EndOfStream && !token.IsCancellationRequested)
                {
                    try
                    {
                        var read = await process.Stdout.ReadAsync(buffer.AsMemory(), token);

                        if (read == 0) continue;

                        var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), byteBuffer);
                        var byteSegment = new ArraySegment<byte>(byteBuffer, 0, bytesWritten);
                        var stdOut = new OutputEvent(
                            tabId: terminalId,
                            data: byteSegment
                        );

                        await writeMessageAsync(stdOut);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        break;
                    }
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            _tokenSource.Dispose();
            await _engine.KillAllAsync();
        }
    }
}
