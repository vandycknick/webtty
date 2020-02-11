using System.Buffers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Api.Infrastructure.Protocol;

namespace WebTty.Integration.Test.Helpers
{
    public static class WebSocketHelpers
    {
        private static ArrayBufferWriter<byte> _buffer = new ArrayBufferWriter<byte>();

        public static async ValueTask SendTextMessageAsync<T>(this WebSocket socket, IMessageWriter writer, T message, CancellationToken token = default)
        {
            await socket.SendMessageAsync(writer, message, WebSocketMessageType.Text, token);
        }

        public static async ValueTask SendBinaryMessageAsync<T>(this WebSocket socket, IMessageWriter writer, T message, CancellationToken token = default)
        {
            await socket.SendMessageAsync(writer, message, WebSocketMessageType.Binary, token);
        }

        public static async ValueTask SendMessageAsync<T>(this WebSocket socket, IMessageWriter writer, T message, WebSocketMessageType type, CancellationToken token = default)
        {
            _buffer.Clear();
            writer.WriteMessage(message, _buffer);

            await socket.SendAsync(_buffer.WrittenMemory, type, true, token);
        }

        public static async ValueTask<object> ReceiveMessageAsync(this WebSocket socket, IMessageReader reader, CancellationToken token = default)
        {
            _buffer.Clear();
            var memory = _buffer.GetMemory();

            await socket.ReceiveAsync(memory, token);

            var sequence = new ReadOnlySequence<byte>(memory);
            reader.TryReadMessage(ref sequence, out var response);
            return response;
        }
    }
}
