using System.Threading;
using System.Threading.Tasks;
using WebTty.Infrastructure.Connection;

namespace WebTty.Infrastructure.Dispatcher
{
    internal interface IMessageDispatcher
    {
        Task ProcessAsync(TtyConnectionContext context, CancellationToken token);
    }
}
