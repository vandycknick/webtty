using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WebTty.Common;
using WebTty.Transport;

namespace WebTty.Terminal
{
    public class TerminalMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TerminalOptions _options;

        public TerminalMiddleware(RequestDelegate next, IOptions<TerminalOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, IConnection connection, ITransport transport, IConnectionHandler handler)
        {
            if (context.Request.Path != _options.Path)
            {
                await _next(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var applicationTask = handler.ProcessAsync();
            var transportTask = transport.ProcessAsync(context);

            await Task.WhenAny(applicationTask, transportTask);

            await connection.AbortAsync();
        }
    }
}
