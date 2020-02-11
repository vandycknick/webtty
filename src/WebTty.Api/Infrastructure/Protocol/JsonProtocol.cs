using System;
using System.Buffers;
using System.Text.Json;
using WebTty.Api.Infrastructure.Core;

namespace WebTty.Api.Infrastructure.Protocol
{
    public class JsonProtocol : IMessageReader, IMessageWriter
    {
        private readonly IMessageResolver _messageResolver;
        public JsonProtocol(IMessageResolver messageResolver)
        {
            _messageResolver = messageResolver;
        }

        public bool TryReadMessage(ref ReadOnlySequence<byte> input, out object message)
        {
            var options = new JsonReaderOptions();
            var reader = new Utf8JsonReader(input, options);


            if (!JsonDocument.TryParseValue(ref reader, out var doc))
            {
                message = default;
                return false;
            }


            if (!doc.RootElement.TryGetProperty("$type", out var typeProp))
            {
                // throw new Exception("No '$type' in json.");
                message = default;
                return false;
            }

            var name = typeProp.GetString();
            if (_messageResolver.TryGetMessageType(name, out var type))
            {
                reader = new Utf8JsonReader(input, options);
                message = JsonSerializer.Deserialize(ref reader, type);
                input = input.Slice(reader.BytesConsumed);
                return true;
            }

            message = default;
            return false;
        }

        public void WriteMessage(object message, IBufferWriter<byte> output)
        {
            if (_messageResolver.TryGetMessageId(message.GetType(), out var id))
            {
                var options = new JsonWriterOptions();
                var memory = MemoryBufferWriter.Get();
                using var writer = new Utf8JsonWriter((IBufferWriter<byte>)memory, options);

                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteStringValue(id);

                var str = JsonSerializer.Serialize(message);
                using var doc = JsonDocument.Parse(str);
                foreach (var property in doc.RootElement.EnumerateObject())
                    property.WriteTo(writer);

                writer.WriteEndObject();
                writer.Flush();

                memory.CopyTo(output);
            }
        }
    }
}
