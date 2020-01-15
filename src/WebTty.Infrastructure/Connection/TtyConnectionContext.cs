using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Infrastructure.Connection
{
    internal class TtyConnectionContext : IConnectionContext
    {
        private readonly IMessageReader _reader;
        private readonly IMessageWriter _writer;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        public CancellationToken Token => _tokenSource.Token;
        public bool IsOpen => !_tokenSource.Token.IsCancellationRequested;
        public IDuplexPipe Application { get; }
        public IDuplexPipe Transport { get; }

        public TtyConnectionContext(IMessageReader reader, IMessageWriter writer)
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

        internal async IAsyncEnumerable<object> ReadMessages()
        {
            while (IsOpen)
            {
                var result = await Transport.Input.ReadAsync(Token);

                if (result.IsCompleted && result.Buffer.Length <= 0)
                {
                    break;
                }

                var buffer = result.Buffer;

                while (_reader.TryParseMessage(ref buffer, out var message))
                {
                    yield return message;
                }

                Transport.Input.AdvanceTo(result.Buffer.End);

                if (result.IsCompleted) break;
            }

            await Transport.Input.CompleteAsync();
            await Transport.Output.CompleteAsync();
        }

        public async Task WriteMessageAsync(object message)
        {
            if (message != null)
            {
                _writer.WriteMessage(message, Transport.Output);
                await Transport.Output.FlushAsync();

            }
        }
        public void Close()
        {
            _tokenSource.Cancel();
        }
    }
}
