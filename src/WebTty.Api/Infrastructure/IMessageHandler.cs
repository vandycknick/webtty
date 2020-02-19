using System.Threading;
using System.Threading.Tasks;
using WebTty.Api.Infrastructure.Connection;

namespace WebTty.Api.Infrastructure
{
    public interface IMessageHandler
    {
        ValueTask<object> Handle(object message, CancellationToken token = default);
    }
}
