using System.Buffers;

namespace WebTty.Api.Infrastructure.Protocol
{
    public interface IMessageWriter
    {
         void WriteMessage(object message, IBufferWriter<byte> output);
    }
}
