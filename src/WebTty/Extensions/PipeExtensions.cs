using System;
using System.IO;
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
                        pipe.Writer.Complete(ex);
                        throw;
                    }

                    var result = await pipe.Writer.FlushAsync().ConfigureAwait(false);
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                // Tell the PipeReader that there's no more data coming
                pipe.Writer.Complete();
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
                        ReadResult readResult = await pipe.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                        if (readResult.Buffer.Length > 0)
                        {
                            foreach (ReadOnlyMemory<byte> segment in readResult.Buffer)
                            {
                                await webSocket.SendAsync(segment, WebSocketMessageType.Binary, endOfMessage: true, cancellationToken).ConfigureAwait(false);
                            }
                        }

                        pipe.Reader.AdvanceTo(readResult.Buffer.End);

                        if (readResult.IsCompleted)
                        {
                            break;
                        }
                    }

                    pipe.Reader.Complete();
                }
                catch (Exception ex)
                {
                    pipe.Reader.Complete(ex);
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
