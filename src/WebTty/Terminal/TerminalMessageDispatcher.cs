using System.Threading.Tasks;
using MediatR;
using WebTty.Common;
using WebTty.Messages;

namespace WebTty.Terminal
{
    public class TerminalMessageDispatcher : IMessageDispatcher
    {
        private readonly IMediator _mediator;
        public TerminalMessageDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<object> Dispatch(object message)
        {
            switch (message)
            {
                case OpenNewTabRequest request:
                {
                    var reply = await _mediator.Send(request);
                    return reply;
                }

                case ResizeTabMessage resize:
                {
                    await _mediator.Publish(resize);
                    return null;
                }

                case StdInputRequest request:
                {
                    await _mediator.Send(request);
                    return null;
                }

                default:
                    return null;
            }
        }
    }
}
