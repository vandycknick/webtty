using System;
using System.Threading.Tasks;
using WebTty.Common;
using WebTty.Protocol;
using WebTty.Transport;

namespace WebTty.Terminal
{
    public class TerminalConnectionHandler : IConnectionHandler
    {
        private readonly IConnection _connection;
        private readonly IProtocol _protocol;
        private readonly IMessageDispatcher _dispatcher;

        public TerminalConnectionHandler(IConnection connection, IProtocol protocol, IMessageDispatcher dispatcher)
        {
            _connection = connection;
            _protocol = protocol;
            _dispatcher = dispatcher;
        }

        public async Task ProcessAsync()
        {
            var transport = _connection.Transport;
            var token = _connection.Token;

            while (!token.IsCancellationRequested)
            {
                var result = await transport.Input.ReadAsync(token);

                if (result.IsCompleted && result.Buffer.Length <= 0)
                {
                    break;
                }

                try
                {
                    var buffer = result.Buffer;

                    while (_protocol.TryParseMessage(ref buffer, out var message))
                    {
                        var reply = await _dispatcher.Dispatch(message);
                        await WriteAsync(reply);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }

                transport.Input.AdvanceTo(result.Buffer.End);

                if (result.IsCompleted) break;
            }

            await transport.Input.CompleteAsync();
            await transport.Output.CompleteAsync();
        }

        public async Task WriteAsync(object message)
        {
            var transport = _connection.Transport;

            if (message != null)
            {
                _protocol.WriteMessage(message, transport.Output);
                await transport.Output.FlushAsync();
            }
        }
    }
}
