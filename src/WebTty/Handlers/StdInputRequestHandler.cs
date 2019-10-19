using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebTty.Messages;
using WebTty.Terminal;

namespace WebTty.Handlers
{
    public class StdInputRequestHandler : AsyncRequestHandler<StdInputRequest>
    {
        private readonly TerminalManager _manager;
        public StdInputRequestHandler(TerminalManager manager)
        {
            _manager = manager;
        }

        protected override Task Handle(StdInputRequest request, CancellationToken cancellationToken)
        {
            return _manager.SendInput(
                request.TabId,
                request.Payload.AsMemory(),
                cancellationToken
            ).AsTask();
        }
    }
}
