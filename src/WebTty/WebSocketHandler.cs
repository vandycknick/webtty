using System;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebTty
{
    public class WebSocketHandler
    {
        public IDuplexPipe _pipe { get; }
        public WebSocketHandler(IDuplexPipe pipe)
        {
            _pipe = pipe;
        }

        public async Task ProcessRequestAsync(HttpContext context, CancellationToken token)
        {
            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                Exception error = null;

                try
                {
                    await ProcessSocketAsync(socket, token);
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
                    // if (!_aborted && !token.IsCancellationRequested)
                    if (!token.IsCancellationRequested)
                    {
                        _pipe.Output.Complete(ex);

                        // We re-throw here so we can communicate that there was an error when sending
                        // the close frame
                        throw;
                    }
                }
                finally
                {
                    Console.WriteLine("Closing Websocket");
                    if (WebSocketIsOpen(socket))
                    {
                        // We're done sending, send the close frame to the client if the websocket is still open
                        await socket.CloseOutputAsync(error != null ? WebSocketCloseStatus.InternalServerError : WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }

                    _pipe.Output.Complete();
                    _pipe.Input.Complete();
                }
            }
        }

        public Task ProcessSocketAsync(WebSocket socket, CancellationToken token)
        {
            var tokenSource = new CancellationTokenSource();
            // Begin sending and receiving. Receiving must be started first because ExecuteAsync enables SendAsync.
            var receiving = StartReceiving(socket, token);
            var sending = StartSending(socket, token);

            // Wait for send or receive to complete
            return Task.WhenAny(receiving, sending).ContinueWith((task) =>
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            });
        }

        private async Task StartReceiving(WebSocket socket, CancellationToken token)
        {
            while (WebSocketIsOpen(socket))
            {
                var memory = _pipe.Output.GetMemory();

                // Exceptions are handled above where the send and receive tasks are being run.
                var receiveResult = await socket.ReceiveAsync(memory, token);

                // Need to check again for NetCoreApp2.2 because a close can happen between a 0-byte read and the actual read
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    return;
                }

                _pipe.Output.Advance(receiveResult.Count);

                var flushResult = await _pipe.Output.FlushAsync();

                // We canceled in the middle of applying back pressure
                // or if the consumer is done
                if (flushResult.IsCanceled || flushResult.IsCompleted)
                {
                    break;
                }
            }
        }

        private async Task StartSending(WebSocket socket, CancellationToken token)
        {
            while (WebSocketIsOpen(socket))
            {
                var result = await _pipe.Input.ReadAsync(token);
                var buffer = result.Buffer;

                try
                {
                    if (result.IsCanceled)
                    {
                        break;
                    }

                    if (!buffer.IsEmpty)
                    {
                        try
                        {
                            var webSocketMessageType = WebSocketMessageType.Binary;

                            if (WebSocketIsOpen(socket))
                            {
                                var position = buffer.Start;
                                while (buffer.TryGet(ref position, out var segment))
                                {
                                    var message = new WebTerminalMessage { Body = segment.ToArray() };
                                    var data = MessagePack.MessagePackSerializer.SerializeUnsafe(message);
                                    await socket.SendAsync(data, webSocketMessageType, true, CancellationToken.None);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            // if (!_aborted)
                            // {
                            //     Log.ErrorWritingFrame(_logger, ex);
                            // }
                            break;
                        }
                    }
                    else if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    _pipe.Input.AdvanceTo(buffer.End);
                }
            }
        }

        private static bool WebSocketIsOpen(WebSocket ws)
        {
            return !(ws.State == WebSocketState.Aborted ||
                   ws.State == WebSocketState.Closed ||
                   ws.State == WebSocketState.CloseSent);
        }
    }
}
