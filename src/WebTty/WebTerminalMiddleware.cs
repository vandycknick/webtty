using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WebTty.Extensions;
using WebTty.Messages;
using WebTty.Messages.Commands;
using WebTty.Messages.Events;
using WebTty.Native.Terminal;
using WebTty.Serializers;

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

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var terminals = new List<Terminal>();

            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            using (var tokenSource = new CancellationTokenSource())
            {
                Exception error = null;

                try
                {
                    var pipe = socket.UsePipe();
                    await ProcessTerminalAsync(pipe, terminals, tokenSource.Token);
                }
                catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                {
                    // Client has closed the WebSocket connection without completing the close handshake
                    // Log.ClosedPrematurely(_logger, ex);
                    Console.WriteLine(ex);
                }
                catch (OperationCanceledException)
                {
                    // Ignore aborts, don't treat them like transport errors
                    Console.WriteLine("OperationCanceledException");
                }
                catch (Exception ex)
                {
                    error = ex;
                    Console.WriteLine("Unknown exception:");
                    Console.WriteLine(error);
                    Console.WriteLine("---");
                }
                finally
                {
                    tokenSource.Cancel();
                    if (socket.IsOpen())
                    {
                        // We're done sending, send the close frame to the client if the websocket is still open
                        await socket.CloseOutputAsync(error != null ? WebSocketCloseStatus.InternalServerError : WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }

                    foreach (var terminal in terminals)
                    {
                        terminal.Kill();
                        terminal.WaitForExit();
                    }
                }
            }

        }

        private async Task ProcessTerminalAsync(IDuplexPipe transport, List<Terminal> terminals, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await transport.Input.ReadAsync(token);

                if (result.IsCompleted && result.Buffer.Length <= 0)
                {
                    break;
                }

                try
                {
                    object command = null;
                    // Console.WriteLine("Received a message");
                    try
                    {
                        command = CommandDeserializer.Deserialize(result.Buffer);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error {ex}");
                        transport.Input.AdvanceTo(result.Buffer.Start);
                        continue;
                    }

                    switch (command)
                    {
                        case SendInputCommand inputCommand:
                        {
                            var terminal = terminals.FirstOrDefault(term => term.Id == inputCommand.TabId);
                            await terminal?.StandardIn.WriteAsync(inputCommand.Payload.AsMemory(), token);
                            break;
                        }

                        case OpenNewTabCommand newTabCommand:
                        {
                            var terminal = new Terminal();
                            terminal.Start();
                            terminal.StandardIn.AutoFlush = true;

                            terminals.Add(terminal);

                            var @event = new TabOpened
                            {
                                Id = terminal.Id,
                            };

                            var data = MessagePack.MessagePackSerializer.Serialize(
                                new object[]{ nameof(TabOpened), @event}
                            );

                            await transport.Output.WriteAsync(data);

                            var backend = Task.Factory.StartNew(
                                function: () => TerminalStdoutReaderAsync(terminal, transport.Output, token),
                                cancellationToken: token,
                                creationOptions: TaskCreationOptions.LongRunning,
                                scheduler: TaskScheduler.Default
                            );
                            break;
                        }

                        case ResizeTabCommand resizeTabCommand:
                        {
                            var terminal = terminals.FirstOrDefault(term => term.Id == resizeTabCommand.TabId);
                            terminal?.SetWindowSize(resizeTabCommand.Cols, resizeTabCommand.Rows);
                            break;
                        }

                        default:
                        {
                            Console.WriteLine("Unknown command");
                            break;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                transport.Input.AdvanceTo(result.Buffer.End);

                if (result.IsCompleted) break;
            }

            await transport.Input.CompleteAsync();
            await transport.Output.CompleteAsync();
        }

        private async Task TerminalStdoutReaderAsync(Terminal terminal, PipeWriter output, CancellationToken token)
        {
            const int maxReadSize = 1024;
            const int maxBufferSize = maxReadSize * sizeof(char);
            var buffer = new char[maxReadSize];
            var byteBuffer = new byte[maxBufferSize];

            while (!terminal.StandardOut.EndOfStream && !token.IsCancellationRequested)
            {
                try
                {
                    var read = await terminal.StandardOut.ReadAsync(buffer, 0, maxReadSize);
                    var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), byteBuffer);
                    var charSegment = new ArraySegment<byte>(byteBuffer, 0, bytesWritten);
                    var @event = new StdOutStream
                    {
                        TabId = terminal.Id,
                        Data = charSegment,
                    };
                    MessagePack.MessagePackSerializer.Serialize(output, new object[]{ nameof(StdOutStream), @event, "end"});

                    var result = await output.FlushAsync(token);

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

            await output.CompleteAsync();
        }
    }
}
