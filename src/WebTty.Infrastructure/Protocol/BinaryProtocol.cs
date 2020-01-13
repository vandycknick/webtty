using System.Buffers;
using MessagePack;
using WebTty.Infrastructure.Common;

namespace WebTty.Infrastructure.Protocol
{
    internal class BinaryProtocol : IMessageWriter, IMessageReader
    {
        private readonly IMessageResolver _messageResolver;
        public BinaryProtocol(IMessageResolver messageResolver)
        {
            _messageResolver = messageResolver;
        }

        public bool TryParseMessage(ref ReadOnlySequence<byte> input, out object message)
        {
            if (!BinaryMessageHelpers.TryParseMessage(ref input, out var payload))
            {
                message = default;
                return false;
            }

            var reader = new MessagePackReader(payload);

            _ = reader.ReadArrayHeader();
            var name = reader.ReadString();

            if (_messageResolver.TryGetMessageType(name, out var type))
            {
                message = MessagePackSerializer.Deserialize(type, ref reader);
                return true;
            }

            message = default;
            return false;
        }

        public void WriteMessage(object message, IBufferWriter<byte> output)
        {
            // TODO: this should not silently fail
            if (_messageResolver.TryGetMessageId(message.GetType(), out var id))
            {
                var memory = MemoryBufferWriter.Get();
                var writer = new MessagePackWriter(memory);

                writer.WriteArrayHeader(2);
                writer.Write(id);

                MessagePackSerializer.Serialize(ref writer, message);

                writer.Flush();

                BinaryMessageHelpers.WriteLengthPrefix(memory.Length, output);
                memory.CopyTo(output);
                MemoryBufferWriter.Return(memory);
            }
        }
    }
}
