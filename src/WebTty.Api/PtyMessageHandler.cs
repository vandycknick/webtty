using System;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WebTty.Api.Common;
using WebTty.Api.Infrastructure;
using WebTty.Api.Models;
using WebTty.Schema.Messages;
using Microsoft.Extensions.Configuration;

namespace WebTty.Api
{
    public class PtyMessageHandler : IMessageHandler, IDisposable
    {
        private readonly IEngine _engine;
        private readonly IConfiguration _configuration;
        private readonly ILoggerAdapter<PtyMessageHandler> _logger;

        public PtyMessageHandler(IEngine engine, IConfiguration configuration, ILoggerAdapter<PtyMessageHandler> logger)
        {
            _engine = engine;
            _configuration = configuration;
            _logger = logger;
        }

        struct MessageResult
        {
            public static MessageResult Empty = new MessageResult
            {
                Success = true,
                Message = Unit.Value,
            };

            public bool Success { get; set; }
            public object Message { get; set; }
            public object Error { get; set; }
        }

        public async ValueTask<object> Handle(object message, CancellationToken token = default)
        {
            var result = await HandleInternal(message, token);
            return result.Success ? result.Message : result.Error;

            async Task<MessageResult> HandleInternal(object message, CancellationToken token)
            {
                _logger.LogDebug("Incoming message {messageName}", message.GetType().Name);
                switch (message)
                {
                    case OpenNewTabRequest request:
                        return await OpenNewTabHandler(request);

                    case ResizeTabRequest request:
                        ResizeTabHandler(request);
                        return MessageResult.Empty;

                    case SendInputRequest request:
                        await SendInputRequestHandler(request, token);
                        return MessageResult.Empty;

                    case OpenOutputRequest request:
                        var stream = ConsumeOutput(request, token);
                        return new MessageResult
                        {
                            Success = true,
                            Message = stream,
                        };

                    default:
                        _logger.LogWarning("UnknownMessage {messageName}", message.GetType().Name);
                        return new MessageResult
                        {
                            Success = false,
                            Error = new UnknownMessageEvent(message.GetType().Name)
                        };
                }

            }
        }

        private bool TryGetCommandAndArgs(out (string Command, IReadOnlyList<string> Args) details)
        {
            if (string.IsNullOrEmpty(_configuration["Command"]))
            {
                details.Command = null;
                details.Args = null;
                return false;
            }
            else
            {
                details.Command = _configuration["Command"];
                details.Args = string.IsNullOrEmpty(_configuration["Args"]) ?
                                    Array.Empty<string>() :
                                    _configuration["Args"].Split(' ');
                return true;
            }
        }

        private async Task<MessageResult> OpenNewTabHandler(OpenNewTabRequest request)
        {
            var terminal = TryGetCommandAndArgs(out var details) ?
                _engine.StartNew(details.Command, details.Args) :
                _engine.StartNew();

            if (_engine.TryGetProcess(terminal, out var process))
            {
                await process.WaitUntilReady();
                var reply = new OpenNewTabReply(id: terminal.Id, request.Id);
                return new MessageResult
                {
                    Success = true,
                    Message = reply,
                };
            }
            else
            {
                _logger.LogError("Error ({messageName}): can't find terminal with id {terminalId}", nameof(ResizeTabRequest), terminal.Id);
                var error = new ErrorReply(
                    id: Guid.NewGuid().ToString(),
                    parentId: request.Id,
                    message: $"Can't find terminal with id {terminal.Id}."
                );
                return new MessageResult
                {
                    Success = false,
                    Error = error
                };
            }
        }

        public void ResizeTabHandler(ResizeTabRequest request)
        {
            if (_engine.TryGetProcess(request.TabId, out var process))
            {
                process.SetWindowSize(request.Rows, request.Cols);
            }
            else
            {
                _logger.LogError("Error ({messageName}): can't find terminal with id {terminalId}", nameof(ResizeTabRequest), request.TabId);
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

        private readonly ArrayPool<byte> outputPool = ArrayPool<byte>.Shared;
        public async IAsyncEnumerable<object> ConsumeOutput(
            OpenOutputRequest request,
            [EnumeratorCancellation]
            CancellationToken token)
        {
            if (!_engine.TryGetProcess(request.TabId, out var process))
            {
                yield return new ErrorReply(
                    id: Guid.NewGuid().ToString(),
                    parentId: "",
                    message: $"Terminal with id {request.TabId} not found."
                );
                yield break;
            }

            var terminalId = request.TabId;
            var stdout = process.Stdout.BaseStream;
            while (process.IsRunning && !token.IsCancellationRequested)
            {
                var buffer = outputPool.Rent(1024);
                var read = 0;

                try
                {
                    read = await stdout.ReadAsync(buffer, 0, 1024);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unknown error reading terminal ({terminalId}) stdout", terminalId, ex);
                }

                if (read > 0)
                {
                    var byteSegment = new ArraySegment<byte>(buffer, 0, read);
                    yield return new OutputEvent(
                        tabId: terminalId,
                        data: byteSegment
                    );
                }

                outputPool.Return(buffer);
            }
        }

        public void Dispose()
        {
            _engine.KillAll();
        }
    }
}
