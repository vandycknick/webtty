using System;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Extensions
{
    public static class PipeExtensions
    {
        public static PipeReader UsePipeReader(this WebSocket webSocket, int sizeHint = 0, PipeOptions pipeOptions = null, CancellationToken cancellationToken = default)
        {
            if (webSocket == null) throw new ArgumentNullException(nameof(webSocket));

            var pipe = new Pipe(pipeOptions ?? PipeOptions.Default);
            Task.Run(async delegate
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Memory<byte> memory = pipe.Writer.GetMemory(sizeHint);
                    try
                    {
                        var readResult = await webSocket.ReceiveAsync(memory, cancellationToken).ConfigureAwait(false);
                        if (readResult.Count == 0)
                        {
                            break;
                        }

                        pipe.Writer.Advance(readResult.Count);
                    }
                    catch (Exception ex)
                    {
                        await pipe.Writer.CompleteAsync(ex);
                        throw;
                    }

                    var result = await pipe.Writer.FlushAsync().ConfigureAwait(false);
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                // Tell the PipeReader that there's no more data coming
                await pipe.Writer.CompleteAsync();
            }).Forget();

            return pipe.Reader;
        }

        public static PipeWriter UsePipeWriter(this WebSocket webSocket, PipeOptions pipeOptions = null, CancellationToken cancellationToken = default)
        {
            if (webSocket == null) throw new ArgumentNullException(nameof(webSocket));

            var pipe = new Pipe(pipeOptions ?? PipeOptions.Default);
            Task.Run(async delegate
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var result = await pipe.Reader.ReadAsync(cancellationToken);
                        try
                        {
                            if (result.IsCanceled)
                            {
                                break;
                            }

                            if (!result.Buffer.IsEmpty )
                            {
                                if (webSocket.IsOpen())
                                {
                                    await webSocket.SendAsync(result.Buffer, WebSocketMessageType.Binary);
                                }
                                else
                                {
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
                            pipe.Reader.AdvanceTo(result.Buffer.End);
                        }
                    }

                    await pipe.Reader.CompleteAsync();
                }
                catch (Exception ex)
                {
                    await pipe.Reader.CompleteAsync(ex);
                    throw;
                }
            }).Forget();

            return pipe.Writer;
        }

        public static IDuplexPipe UsePipe(this WebSocket webSocket, int sizeHint = 0, PipeOptions pipeOptions = null, CancellationToken cancellationToken = default)
        {
            return new DuplexPipe(webSocket.UsePipeReader(sizeHint, pipeOptions, cancellationToken), webSocket.UsePipeWriter(pipeOptions, cancellationToken));
        }

        private static void Forget(this Task _)
        {
        }
    }
}
