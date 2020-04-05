using System.IO.Pipelines;
using System.Threading;
using WebTty.Api.Infrastructure.Protocol;

namespace WebTty.Api.Infrastructure.Connection
{
    public sealed class ConnectionContext
    {
        internal readonly IMessageReader _reader;
        internal readonly IMessageWriter _writer;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public CancellationToken Token => _tokenSource.Token;
        public bool IsOpen => !_tokenSource.Token.IsCancellationRequested;
        internal IDuplexPipe Application { get; }
        internal IDuplexPipe Transport { get; }

        public ConnectionContext(IMessageReader reader, IMessageWriter writer)
        {
            _reader = reader;
            _writer = writer;

            var (transport, application) = DuplexPipe.CreateConnectionPair(
                new PipeOptions(useSynchronizationContext: false),
                new PipeOptions(useSynchronizationContext: false)
            );

            Transport = transport;
            Application = application;
        }

        public void Close()
        {
            _tokenSource.Cancel();
        }
    }
}
