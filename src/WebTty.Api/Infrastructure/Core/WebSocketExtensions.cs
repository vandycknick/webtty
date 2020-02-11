using System.Buffers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Api.Infrastructure.Core
{
    internal static class WebSocketExtensions
    {
        public static bool IsOpen(this WebSocket socket)
        {
            return !(socket.State == WebSocketState.Aborted ||
                   socket.State == WebSocketState.Closed ||
                   socket.State == WebSocketState.CloseSent);
        }

        public static ValueTask SendAsync(this WebSocket webSocket, ReadOnlySequence<byte> buffer, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken = default)
        {
            if (buffer.IsSingleSegment)
            {
                return webSocket.SendAsync(buffer.First, webSocketMessageType, endOfMessage: true, cancellationToken);
            }
            else
            {
                return SendMultiSegmentAsync(webSocket, buffer, webSocketMessageType, cancellationToken);
            }
        }

        private static async ValueTask SendMultiSegmentAsync(WebSocket webSocket, ReadOnlySequence<byte> buffer, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken = default)
        {
            var position = buffer.Start;

            buffer.TryGet(ref position, out var prevSegment);
            while (buffer.TryGet(ref position, out var segment))
            {
                await webSocket.SendAsync(prevSegment, webSocketMessageType, endOfMessage: false, cancellationToken);
                prevSegment = segment;
            }

            await webSocket.SendAsync(prevSegment, webSocketMessageType, endOfMessage: true, cancellationToken);
        }
    }
}
