using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
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
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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

            if (context.WebSockets.IsWebSocketRequest)
            {
                var duplex = DuplexPipe.CreateConnectionPair(PipeOptions.Default, PipeOptions.Default);
                var webSocketHandler = new WebSocketHandler(duplex.Transport);

                using (var tokenSource = new CancellationTokenSource())
                {
                    try
                    {
                        var terminal = new Terminal();
                        terminal.Start();
                        terminal.StandardIn.AutoFlush = true;

                        ProcessTerminalAsync(terminal, duplex.Application, tokenSource.Token).Forget();
                        await webSocketHandler.ProcessRequestAsync(context, tokenSource.Token);

                        terminal.Kill();
                        terminal.WaitForExit();
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

        private Task ProcessTerminalAsync(Terminal terminal, IDuplexPipe transport, CancellationToken token)
        {
            var reader = Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    ReadResult result = await transport.Input.ReadAsync();
                    Console.WriteLine($"ProcessTerminalAsync READ");

                    ReadOnlySequence<byte> buffer = result.Buffer;
                    Console.WriteLine($"ProcessTerminalAsync buffer length {buffer.Length}");

                    try
                    {
                        var h = MessagePack.MessagePackBinary.ReadArrayHeader(buffer.ToArray(), 0, out var read);
                        Console.WriteLine($"ProcessTerminalAsync header {h}, read {read}");
                        var type = MessagePack.MessagePackBinary.ReadInt32(buffer.ToArray(), read, out read);
                        Console.WriteLine($"ProcessTerminalAsync type {type}, read {read}");

                        if (type != 3)
                        {
                            var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalInputMessage>(buffer.ToArray());
                            await terminal.StandardIn.WriteAsync(msg.Body.AsMemory(), token);

                            Console.WriteLine(msg.Body);
                        }
                        else
                        {
                            var msg = MessagePack.MessagePackSerializer.Deserialize<TerminalResizeMessage>(buffer.ToArray());
                            terminal.SetWindowSize(msg.Cols, msg.Rows);
                            Console.WriteLine($"TerminalResize cols {msg.Cols}, rows {msg.Rows}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }


                    // foreach (var item in buffer)
                    // {
                    //     var charsCnt = Encoding.UTF8.GetCharCount(item.Span);
                    //     var array = charPool.Rent(charsCnt);
                    //     try
                    //     {
                    //         Memory<char> chars = array;
                    //         var written = Encoding.UTF8.GetChars(item.Span, chars.Span);

                    //         // await terminal.StandardIn.WriteAsync(chars.Slice(0, written), CancellationToken.None);
                    //     }
                    //     finally
                    //     {
                    //         charPool.Return(array);
                    //     }
                    // }

                    transport.Input.AdvanceTo(buffer.End);

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                transport.Input.Complete();

            }, token);

            var writer = Task.Factory.StartNew(async () =>
            {
                const int minimumBufferSize = 1024 * sizeof(char);
                var buffer = new char[1024];
                while (!terminal.StandardOut.EndOfStream && !token.IsCancellationRequested)
                {
                    Memory<byte> memory = transport.Output.GetMemory(minimumBufferSize);
                    try
                    {
                        var read = await terminal.StandardOut.ReadAsync(buffer, 0, 1024);
                        var bytesWritten = Encoding.UTF8.GetBytes(buffer.AsSpan(0, read), memory.Span);

                        if (bytesWritten == 0)
                        {
                            continue;
                        }
                        transport.Output.Advance(bytesWritten);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        break;
                    }

                    // Make the data available to the PipeReader
                    FlushResult result = await transport.Output.FlushAsync();

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                transport.Output.Complete();
            }, token);

            return Task.WhenAll(reader, writer);
        }

        private static ArrayPool<char> charPool = ArrayPool<char>.Shared;
    }
}
