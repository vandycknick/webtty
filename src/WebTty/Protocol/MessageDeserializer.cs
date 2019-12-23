using System.Buffers;
using MessagePack;

namespace WebTty.Protocol
{

    public abstract class MessageDeserializerBase
    {
        public abstract object Deserialize(ReadOnlySequence<byte> bytes);
        public abstract object Deserialize(ref MessagePackReader reader);
    }

    public class MessageDeserializer<T> : MessageDeserializerBase
    {
        public override object Deserialize(ref MessagePackReader reader)
        {
            return MessagePackSerializer.Deserialize<T>(ref reader);
        }

        public override object Deserialize(ReadOnlySequence<byte> bytes)
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
    }
}
