using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Infrastructure.Connection
{
    public interface IConnectionContext
    {
        CancellationToken Token { get; }
        Task WriteMessageAsync(object message);
        void Close();
    }
}
