using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using WebTty.IO;

namespace WebTty.Transport
{
    public class HttpConnection : IConnection
    {
        private readonly CancellationTokenSource _tokenSource;

        public HttpConnection()
        {
            var connection = DuplexPipe.CreateConnectionPair(
                new PipeOptions(useSynchronizationContext: false),
                new PipeOptions(useSynchronizationContext: false)
            );

            Application = connection.Application;
            Transport = connection.Transport;
            _tokenSource = new CancellationTokenSource();
        }

        public CancellationToken Token => _tokenSource.Token;

        public IDuplexPipe Application { get; }

        public IDuplexPipe Transport { get; }

        public Task AbortAsync()
        {
            _tokenSource.Cancel();
            return Task.Delay(1);
        }

        public bool IsOpen()
        {
            return !_tokenSource.Token.IsCancellationRequested;
        }
    }
}
