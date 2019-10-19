using MediatR;
using WebTty.Messages;
using WebTty.Terminal;

namespace WebTty.Handlers
{
    public class ResizeTabMessageHandler : NotificationHandler<ResizeTabMessage>
    {
        private readonly TerminalManager _manager;

        public ResizeTabMessageHandler(TerminalManager manager)
        {;
            _manager = manager;
        }

        protected override void Handle(ResizeTabMessage notification)
        {
            _manager.Resize(notification.TabId, notification.Cols, notification.Rows);
        }
    }
}
