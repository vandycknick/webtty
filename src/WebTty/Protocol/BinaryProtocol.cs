using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

            var segment = GetArraySegment(payload);

            _ = MessagePackBinary.ReadArrayHeader(segment.Array, segment.Offset, out var read);
            var id = MessagePackBinary.ReadString(segment.Array, segment.Offset + read, out var readId);

            if (_deserializerMap.TryGetValue(id, out var deserializer))
            {
                var messageBytes = MessagePackBinary.ReadBytes(segment.Array, segment.Offset + read + readId, out _);
                message = deserializer.Deserialize(messageBytes);
                return true;
            }
            else
            {
                message = default;
                return false;
            }
        }

        private static ArraySegment<byte> GetArraySegment(in ReadOnlySequence<byte> input)
        {
            if (input.IsSingleSegment)
            {
                var isArray = MemoryMarshal.TryGetArray(input.First, out var arraySegment);
                // This will never be false unless we started using un-managed buffers
                Debug.Assert(isArray);
                return arraySegment;
            }

            // Should be rare
            return new ArraySegment<byte>(input.ToArray());
        }

        public void WriteMessage(object message, IBufferWriter<byte> output)
        {
            var writer = MemoryBufferWriter.Get();
            var name = message.GetType().Name;

            MessagePackBinary.WriteArrayHeader(writer, 2);
            MessagePackBinary.WriteString(writer, name);

            var data = MessagePackSerializer.SerializeUnsafe(message);
            MessagePackBinary.WriteBytes(writer, data.Array, data.Offset, data.Count);

            BinaryMessageHelpers.WriteLengthPrefix(writer.Length, output);
            writer.CopyTo(output);
        }
    }
}
