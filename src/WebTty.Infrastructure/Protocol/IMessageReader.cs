using System.Buffers;

namespace WebTty.Infrastructure.Protocol
{
    public interface IMessageReader
    {
        bool TryParseMessage(ref ReadOnlySequence<byte> input, out object message);
    }
}
