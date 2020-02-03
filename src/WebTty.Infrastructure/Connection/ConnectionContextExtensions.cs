using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebTty.Infrastructure.Connection
{
    public static class ConnectionContextExtensions
    {
        internal static async IAsyncEnumerable<object> ReadMessages(this ConnectionContext context)
        {
            while (context.IsOpen)
            {
                var result = await context.Transport.Input.ReadAsync(context.Token);

                if (result.IsCompleted && result.Buffer.Length <= 0)
                {
                    break;
                }

                var buffer = result.Buffer;
                while (context._reader.TryReadMessage(ref buffer, out var message))
                {
                    yield return message;
                }

                context.Transport.Input.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted) break;
            }

            await context.Transport.Input.CompleteAsync();
            await context.Transport.Output.CompleteAsync();
        }

        public static async Task WriteMessageAsync(this ConnectionContext context, object message)
        {
            if (message != null)
            {
                context._writer.WriteMessage(message, context.Transport.Output);
                await context.Transport.Output.FlushAsync();
            }
        }
    }
}
