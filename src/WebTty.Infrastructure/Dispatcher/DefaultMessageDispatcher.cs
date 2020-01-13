using System.Threading;
using System.Threading.Tasks;
using WebTty.Infrastructure.Connection;

namespace WebTty.Infrastructure.Dispatcher
{
    internal class DefaultMessageDispatcher : IMessageDispatcher
    {
        private readonly IMessageHandler _handler;
        public DefaultMessageDispatcher(IMessageHandler handler)
        {
            _handler = handler;
        }

        public async Task ProcessAsync(TtyConnectionContext context, CancellationToken token)
        {
            await foreach (var message in context.ReadMessages().WithCancellation(token))
            {
                var reply = await _handler.Handle(message, context, context.Token);

                if (reply != null) await context.WriteMessageAsync(reply);
            }
        }
    }
}
