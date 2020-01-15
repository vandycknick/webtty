using Microsoft.AspNetCore.Http;
using System;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Infrastructure.Common;

namespace WebTty.Infrastructure.Transport
{
    internal class WebSocketsTransport
    {
        private volatile bool _aborted;

        public async Task ProcessAsync(HttpContext context, IDuplexPipe application, CancellationToken token)
        {
            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                try
                {
                    await ProcessSocketAsync(socket, application, token);
                }
                finally
                {
                    Console.WriteLine("Socket closed");
                }
            }
        }

        private async Task ProcessSocketAsync(WebSocket socket, IDuplexPipe application, CancellationToken token)
        {
            var receiving = StartReceiving(socket, application, token);
            var sending = StartSending(socket, application);

            var result = await Task.WhenAny(receiving, sending);

            if (result == receiving)
            {
                // Log.WaitingForSend(_logger);

                // We're waiting for the application to finish and there are 2 things it could be doing
                // 1. Waiting for application data
                // 2. Waiting for a websocket send to complete

                // Cancel the application so that ReadAsync yields
                application.Input.CancelPendingRead();

                using (var delayCts = new CancellationTokenSource())
                {
                    var resultTask = sending;
                    // var resultTask = await Task.WhenAny(sending, Task.Delay(_options.CloseTimeout, delayCts.Token));

                    if (resultTask != sending)
                    {
                        // We timed out so now we're in ungraceful shutdown mode
                        // Log.CloseTimedOut(_logger);

                        // Abort the websocket if we're stuck in a pending send to the client
                        _aborted = true;

                        socket.Abort();
                    }
                    else
                    {
                        delayCts.Cancel();
                    }
                }
            }
            else
            {
                // Log.WaitingForClose(_logger);

                // We're waiting on the websocket to close and there are 2 things it could be doing
                // 1. Waiting for websocket data
                // 2. Waiting on a flush to complete (backpressure being applied)
                using (var delayCts = new CancellationTokenSource())
                {
                    var resultTask = receiving;
                    // var resultTask = await Task.WhenAny(receiving, Task.Delay(_options.CloseTimeout, delayCts.Token));

                    if (resultTask != receiving)
                    {
                        // Abort the websocket if we're stuck in a pending receive from the client
                        _aborted = true;

                        socket.Abort();

                        // Cancel any pending flush so that we can quit
                        application.Output.CancelPendingFlush();
                    }
                    else
                    {
                        delayCts.Cancel();
                    }
                }
            }
        }

        private async Task StartReceiving(WebSocket socket, IDuplexPipe application, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Do a 0 byte read so that idle connections don't allocate a buffer when waiting for a read
                    var result = await socket.ReceiveAsync(Memory<byte>.Empty, token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }

                    var memory = application.Output.GetMemory();

                    var receiveResult = await socket.ReceiveAsync(memory, token);

                    // Need to check again for netcoreapp3.0 and later because a close can happen between a 0-byte read and the actual read
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }

                    // Log.MessageReceived(_logger, receiveResult.MessageType, receiveResult.Count, receiveResult.EndOfMessage);

                    application.Output.Advance(receiveResult.Count);

                    var flushResult = await application.Output.FlushAsync();

                    // We canceled in the middle of applying back pressure
                    // or if the consumer is done
                    if (flushResult.IsCanceled || flushResult.IsCompleted)
                    {
                        break;
                    }
                }
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
                Console.WriteLine("Unknown exception:");
                Console.WriteLine(ex);
                Console.WriteLine("---");
                if (!_aborted && !token.IsCancellationRequested)
                {
                    application.Output.Complete(ex);
                }
            }
            finally
            {
                // We're done writing
                application.Output.Complete();
            }

        }

        private async Task StartSending(WebSocket socket, IDuplexPipe application)
        {
            Exception error = null;

            try
            {
                while (true)
                {
                    var result = await application.Input.ReadAsync();
                    var buffer = result.Buffer;

                    // Get a frame from the application
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
                                // Log.SendPayload(_logger, buffer.Length);
                                if (socket.IsOpen())
                                {
                                    await socket.SendAsync(buffer, WebSocketMessageType.Binary);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!_aborted)
                                {
                                    Console.WriteLine(ex);
                                    // Log.ErrorWritingFrame(_logger, ex);
                                }
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
                        application.Input.AdvanceTo(buffer.End);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                // Send the close frame before calling into user code
                if (socket.IsOpen())
                {
                    try
                    {
                        // We're done sending, send the close frame to the client if the websocket is still open
                        await socket.CloseOutputAsync(error != null ? WebSocketCloseStatus.InternalServerError : WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        // Log.ClosingWebSocketFailed(_logger, ex);
                    }
                }

                application.Input.Complete();
            }
        }
    }
}
