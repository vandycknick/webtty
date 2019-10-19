namespace WebTty.Protocol
{

    public abstract class MessageDeserializerBase
    {
        public abstract object Deserialize(byte[] message);
    }

    public class MessageDeserializer<T> : MessageDeserializerBase
    {
        public override object Deserialize(byte[] message)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(message);
        }
    }
}
