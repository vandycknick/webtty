using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Messages;
using WebTty.Terminal;
using WebTty.Transport;

namespace WebTty.Handlers
{
    public class OpenNewTabRequestHandler : RequestHandler<OpenNewTabRequest, OpenNewTabReply>
    {
        private readonly TerminalManager _manager;
        private readonly IConnection _connection;

        public OpenNewTabRequestHandler(TerminalManager manager, IConnection connection)
        {
            _connection = connection;
            _manager = manager;
        }

        protected override OpenNewTabReply Handle(OpenNewTabRequest request)
        {
            var terminal = _manager.Start();

            _manager.ProcessOutput(terminal, _connection.Token);

            return new OpenNewTabReply(id: terminal.Id.ToString());
        }
    }
}
