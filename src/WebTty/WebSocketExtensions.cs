using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty
{
    internal static class WebSocketExtensions
    {
        public static ValueTask SendAsync(this WebSocket webSocket, ReadOnlySequence<byte> buffer, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken = default)
        {
            if (buffer.IsSingleSegment)
            {
                var isArray = MemoryMarshal.TryGetArray(buffer.First, out var segment);
                Debug.Assert(isArray);
                return new ValueTask(webSocket.SendAsync(segment, webSocketMessageType, endOfMessage: true, cancellationToken));
            }
            else
            {
                return SendMultiSegmentAsync(webSocket, buffer, webSocketMessageType, cancellationToken);
            }
        }

        private static async ValueTask SendMultiSegmentAsync(WebSocket webSocket, ReadOnlySequence<byte> buffer, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken = default)
        {
            var position = buffer.Start;
            while (buffer.TryGet(ref position, out var segment))
            {
                var isArray = MemoryMarshal.TryGetArray(segment, out var arraySegment);
                Debug.Assert(isArray);
                await webSocket.SendAsync(arraySegment, webSocketMessageType, endOfMessage: false, cancellationToken);
            }

            // Empty end of message frame
            await webSocket.SendAsync(new ArraySegment<byte>(Array.Empty<byte>()), webSocketMessageType, endOfMessage: true, cancellationToken);
        }
    }
}
