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

        public async Task Invoke(HttpContext context)
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

        private static ArraySegment<byte> GetArraySegment(in ReadOnlySequence<byte> input)
        {
            if (input.IsSingleSegment)
            {
                var isArray = MemoryMarshal.TryGetArray(input.First, out var arraySegment);
                // This will never be false unless we started using un-managed buffers
                Debug.Assert(isArray);
                return arraySegment;
            }

            // Should be rare
            return new ArraySegment<byte>(input.ToArray());
        }

        private async Task ProcessTerminalAsync(IDuplexPipe transport, List<Terminal> terminals, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var result = await transport.Input.ReadAsync();
                var buffer = result.Buffer;

                if (result.IsCompleted && buffer.Length <= 0)
                {
                    break;
                }

                try
                {
                    var segment = GetArraySegment(result.Buffer);
                    var h = MessagePack.MessagePackBinary.ReadArrayHeader(segment.Array, segment.Offset, out var read);
                    var type = MessagePack.MessagePackBinary.ReadInt32(segment.Array, segment.Offset + read, out read);

                    if (type == TerminalMessageTypes.TERMINAL_INPUT)
                    {
                        var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalInputMessage>(segment);
                        var terminal = terminals.FirstOrDefault(term => term.Id == msg.Id);

                        await terminal?.StandardIn.WriteAsync(msg.Body.AsMemory(), token);
                    }
                    else if (type == TerminalMessageTypes.TERMINAL_NEW_TAB)
                    {
                        var terminal = new Terminal();
                        terminal.Start();
                        terminal.StandardIn.AutoFlush = true;

                        terminals.Add(terminal);

                        var msg = new TerminalNewTabCreatedMessage { Id = terminal.Id };
                        var data = MessagePack.MessagePackSerializer.SerializeUnsafe(msg);

                        await transport.Output.WriteAsync(data.AsMemory());

                        var backend = Task.Factory.StartNew(() => TerminalStdoutWriter(terminal, transport.Output, token));
                    }
                    else if (type == TerminalMessageTypes.TERMINAL_RESIZE)
                    {
                        var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalResizeMessage>(segment);
                        var terminal = terminals.FirstOrDefault(term => term.Id == msg.Id);
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

                    // https://github.com/dotnet/corefx/blob/edbee902747970e86dbcf19727e72b8216946bb8/src/Common/src/CoreLib/System/Runtime/InteropServices/MemoryMarshal.cs#L25
                    // https://github.com/dotnet/corefx/blob/edbee902747970e86dbcf19727e72b8216946bb8/src/Common/src/CoreLib/Internal/Runtime/CompilerServices/Unsafe.cs#L76
                    // https://github.com/dotnet/corefx/blob/edbee902747970e86dbcf19727e72b8216946bb8/src/Common/src/CoreLib/System/ArraySegment.cs#L29
                    if (MemoryMarshal.TryGetArray<byte>(memory, out var seg))
                    {
                        var byteArray = seg.Array;

                        var written = MessagePack.MessagePackSerializer.Serialize(
                            ref byteArray,
                            0,
                            message,
                            MessagePack.MessagePackSerializer.DefaultResolver
                        );

                        output.Advance(written);
                    }

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
