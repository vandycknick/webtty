using System.Buffers;

namespace WebTty.Infrastructure.Protocol
{
    public interface IMessageWriter
    {
         void WriteMessage(object message, IBufferWriter<byte> output);
    }
}
