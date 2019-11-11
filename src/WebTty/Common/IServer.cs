using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Common
{
    public interface IServer
    {
        Task RunAsync(CancellationToken token = default);
    }
}
