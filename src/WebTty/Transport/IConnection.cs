using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty.Transport
{
    public interface IConnection
    {
        CancellationToken Token { get; }
        IDuplexPipe Application { get; }
        IDuplexPipe Transport { get; }
        bool IsOpen();
        Task AbortAsync();
    }
}
