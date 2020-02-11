using System;
using System.Buffers;

namespace WebTty.Api.Infrastructure.Protocol
{
    public interface IMessageReader
    {
        bool TryReadMessage(ref ReadOnlySequence<byte> input, out object message);
    }
}
