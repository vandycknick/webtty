using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Api.Infrastructure.Connection;

namespace WebTty.Api.Infrastructure.Core
{
    internal sealed class Dispatcher
    {
        private readonly IMessageHandler _handler;
        public Dispatcher(IMessageHandler handler)
        {
            _handler = handler;
        }

        public async Task ProcessAsync(ConnectionContext context, CancellationToken token)
        {
            await foreach (var message in context.ReadMessages().WithCancellation(token))
            {
                var reply = await _handler.Handle(message, context, context.Token);

                if (reply == null) continue;

                if (ReflectionHelpers.IsIAsyncEnumerable(reply.GetType()))
                {
                    _ = Task.Factory.StartNew(
                        function: () => ConsumeEnumerable((IAsyncEnumerable<object>)reply, context, token).ConfigureAwait(false),
                        cancellationToken: context.Token,
                        creationOptions: TaskCreationOptions.LongRunning,
                        scheduler: TaskScheduler.Default
                    );
                }
                else
                {
                    await context.WriteMessageAsync(reply);
                }
            }
        }

        private async Task ConsumeEnumerable(IAsyncEnumerable<object> messages, ConnectionContext context, CancellationToken token)
        {
            await foreach (var message in messages.WithCancellation(token))
            {
                if (message != null)
                {
                    await context.WriteMessageAsync(message);
                }
            }
        }
    }
}
