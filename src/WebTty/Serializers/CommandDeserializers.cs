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

            return message.Type switch
            {
                nameof(OpenNewTabCommand) => MessagePack.MessagePackSerializer.Deserialize<OpenNewTabCommand>(message.Payload),

                nameof(ResizeTabCommand) => MessagePack.MessagePackSerializer.Deserialize<ResizeTabCommand>(message.Payload),

                nameof(SendInputCommand) => MessagePack.MessagePackSerializer.Deserialize<SendInputCommand>(message.Payload),

                _ => throw new UnknownCommandException(message.Type),
            };
        }
    }
}
