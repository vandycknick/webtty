using System.Threading;
using System.Threading.Tasks;

namespace WebTty
{
    public interface IServer
    {
        Task RunAsync(CancellationToken token = default);
    }
}
