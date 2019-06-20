using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace WebTty
{
    public class WebTerminalMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebTerminalOptions _options;

        public WebTerminalMiddleware(RequestDelegate next, IOptions<WebTerminalOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        private readonly List<Terminal> _terminals = new List<Terminal>();

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            if (context.WebSockets.IsWebSocketRequest)
            {
                var duplex = DuplexPipe.CreateConnectionPair(PipeOptions.Default, PipeOptions.Default);
                var webSocketHandler = new WebSocketHandler(duplex.Transport);

                using (var tokenSource = new CancellationTokenSource())
                {
                    try
                    {
                        await Task.WhenAll(
                            ProcessTerminalAsync(duplex.Application, tokenSource.Token),
                            webSocketHandler.ProcessRequestAsync(context, tokenSource.Token)
                        );

                        foreach (var terminal in _terminals)
                        {
                            terminal.Kill();
                            terminal.WaitForExit();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        tokenSource.Cancel();
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task ProcessTerminalAsync(IDuplexPipe transport, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await transport.Input.ReadAsync();
                var buffer = result.Buffer;

                try
                {
                    var h = MessagePack.MessagePackBinary.ReadArrayHeader(buffer.ToArray(), 0, out var read);
                    var type = MessagePack.MessagePackBinary.ReadInt32(buffer.ToArray(), read, out read);

                    if (type == TerminalMessageTypes.TERMINAL_INPUT)
                    {
                        var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalInputMessage>(buffer.ToArray());
                        var terminal = _terminals.FirstOrDefault(term => term.Id == msg.Id);

                        await terminal?.StandardIn.WriteAsync(msg.Body.AsMemory(), token);
                    }
                    else if (type == TerminalMessageTypes.TERMINAL_NEW_TAB)
                    {
                        var terminal = new Terminal();
                        terminal.Start();
                        terminal.StandardIn.AutoFlush = true;

                        _terminals.Add(terminal);

                        var msg = new TerminalNewTabCreatedMessage { Id = terminal.Id };
                        var data = MessagePack.MessagePackSerializer.SerializeUnsafe(msg);

                        await transport.Output.WriteAsync(data.AsMemory());

                        var backend = Task.Factory.StartNew(() => TerminalStdoutWriter(terminal, transport.Output, token));
                    }
                    else if (type == TerminalMessageTypes.TERMINAL_RESIZE)
                    {
                        var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalResizeMessage>(buffer.ToArray());
                        var terminal = _terminals.FirstOrDefault(term => term.Id == msg.Id);
                        terminal?.SetWindowSize(msg.Cols, msg.Rows);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                transport.Input.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            transport.Input.Complete();
            transport.Output.Complete();
        }

        private async Task TerminalStdoutWriter(Terminal terminal, PipeWriter output, CancellationToken token)
        {
            const int maxReadSize = 1024;
            const int maxBufferSize = maxReadSize * sizeof(char);
            var buffer = new char[maxReadSize];
            var byteBuffer = new byte[maxBufferSize];

            while (!terminal.StandardOut.EndOfStream && !token.IsCancellationRequested)
            {
                try
                {
                    var memory = output.GetMemory(maxBufferSize);
                    var read = await terminal.StandardOut.ReadAsync(buffer, 0, maxReadSize);
                    var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), byteBuffer);

                    var message = new WebTerminalOutputMessage { Type = 2, Id = terminal.Id, Body = byteBuffer.AsSpan(0, bytesWritten).ToArray() };
                    var data = MessagePack.MessagePackSerializer.SerializeUnsafe(message);

                    var dataMemory = data.AsMemory();
                    dataMemory.CopyTo(memory);
                    output.Advance(dataMemory.Length);

                    var result = await output.FlushAsync();

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }

            output.Complete();
        }
    }
}
