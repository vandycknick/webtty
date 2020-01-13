using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Infrastructure.Dispatcher;
using WebTty.Infrastructure.Transport;

namespace WebTty.Infrastructure.Connection
{
    internal class HttpConnectionHandler
    {
        private readonly HttpConnectionManager _connections;

        public HttpConnectionHandler(HttpConnectionManager connections)
        {
            _connections = connections;
        }

        public async Task ExecuteAsync<TMessageHandler>(HttpContext context) where TMessageHandler : IMessageHandler
        {
            if (HttpMethods.IsGet(context.Request.Method))
            {
                var messageHandler = ActivatorUtilities.CreateInstance<TMessageHandler>(context.RequestServices);
                await ExecuteWebSocketAsync(context, messageHandler);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            }
        }

        private async Task ExecuteWebSocketAsync(HttpContext httpContext, IMessageHandler handler)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var guid = Guid.NewGuid();
                var context = (TtyConnectionContext)_connections.GetOrCreate(guid.ToString());
                var socket = new WebSocketsTransport();
                var dispatcher = new DefaultMessageDispatcher(handler);

                using (var source = new CancellationTokenSource())
                {
                    var transportTask = socket.ProcessAsync(httpContext, context.Application, source.Token);
                    var applicationTask = dispatcher.ProcessAsync(context, source.Token);

                    await Task.WhenAny(applicationTask, transportTask);
                }

                await handler.DisposeAsync();
                _connections.Remove(guid.ToString());
            }
            else
            {
                httpContext.Response.ContentType = "text/plain";
                httpContext.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
            }
        }
    }
}
