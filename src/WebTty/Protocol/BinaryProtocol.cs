using System.Buffers;
using MessagePack;

namespace WebTty.Protocol
{
    public class BinaryProtocol : IProtocol
    {
        private readonly BinaryDeserializerMap _deserializerMap;

        public BinaryProtocol(BinaryDeserializerMap deserializerMap)
        {
            _deserializerMap = deserializerMap;
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
            var id = reader.ReadString();

            if (_deserializerMap.TryGetValue(id, out var deserializer))
            {
                message = deserializer.Deserialize(ref reader);
                return true;
            }

            message = default;
            return false;
        }

        public void WriteMessage(object message, IBufferWriter<byte> output)
        {
            var memory = MemoryBufferWriter.Get();
            var writer = new MessagePackWriter(memory);
            var name = message.GetType().Name;

            writer.WriteArrayHeader(2);
            writer.Write(name);

            MessagePackSerializer.Serialize(ref writer, message);

            writer.Flush();

            BinaryMessageHelpers.WriteLengthPrefix(memory.Length, output);
            memory.CopyTo(output);
            MemoryBufferWriter.Return(memory);
        }
    }
}
