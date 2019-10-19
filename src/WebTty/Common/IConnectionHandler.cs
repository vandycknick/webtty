using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Common
{
    public interface IConnectionHandler
    {
        Task ProcessAsync();

        Task WriteAsync(object message);
    }
}
