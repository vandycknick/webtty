using MediatR;
using System.Linq;
using System.Threading.Tasks;
using WebTty.Common;

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
            var interfaces = message.GetType().GetInterfaces();

            if (
                interfaces.Contains(typeof(IRequest)) ||
                interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            )
            {
                var reply = await _mediator.Send((dynamic)message);
                return reply;
            }
            else if(interfaces.Contains(typeof(INotification)))
            {
                await _mediator.Publish(message);
                return null;
            }
            else
            {
                System.Console.WriteLine("no match found");
                return null;
            }
        }
    }
}
