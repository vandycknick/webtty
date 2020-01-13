using System;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Infrastructure.Connection;

namespace WebTty.Infrastructure.Dispatcher
{
    public interface IMessageHandler : IAsyncDisposable
    {
        ValueTask<object> Handle(object message, IConnectionContext context, CancellationToken token = default);
    }
}
