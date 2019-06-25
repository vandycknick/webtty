using System.Net.WebSockets;

namespace WebTty.Extensions
{
    public static class WebSocketExtensions
    {
        public static bool IsOpen(this WebSocket socket)
        {
            return !(socket.State == WebSocketState.Aborted ||
                   socket.State == WebSocketState.Closed ||
                   socket.State == WebSocketState.CloseSent);
        }
    }
}
