using System;
using System.Buffers;

namespace WebTty.Protocol
{
    public interface IProtocol
    {
        bool TryParseMessage(ref ReadOnlySequence<byte> input, out object message);

        void WriteMessage(object message, IBufferWriter<byte> output);
    }
}
