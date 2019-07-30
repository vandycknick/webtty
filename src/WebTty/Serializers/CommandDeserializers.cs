using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WebTty.Messages;
using WebTty.Messages.Commands;

namespace WebTty.Serializers
{

    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string commandName):
            base($"Failed deserializing unknown command: {commandName}")
        {
        }
    }

    public static class CommandDeserializer
    {
        private static ArraySegment<byte> GetArraySegment(in ReadOnlySequence<byte> input)
        {
            if (input.IsSingleSegment)
            {
                var isArray = MemoryMarshal.TryGetArray(input.First, out var arraySegment);
                Debug.Assert(isArray);
                return arraySegment;
            }

            // Should be rare
            return new ArraySegment<byte>(input.ToArray());
        }

        public static object Deserialize(in ReadOnlySequence<byte> input)
        {
            var segment = GetArraySegment(input);
            var message = MessagePack.MessagePackSerializer.Deserialize<Message>(segment);

            switch (message.Type)
            {
                case nameof(OpenNewTabCommand):
                    return MessagePack.MessagePackSerializer.Deserialize<OpenNewTabCommand>(message.Payload);

                case nameof(ResizeTabCommand):
                    return MessagePack.MessagePackSerializer.Deserialize<ResizeTabCommand>(message.Payload);

                case nameof(SendInputCommand):
                    return MessagePack.MessagePackSerializer.Deserialize<SendInputCommand>(message.Payload);

                default:
                    throw new UnknownCommandException(message.Type);
            }
        }
    }
}
