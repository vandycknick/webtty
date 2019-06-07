using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WebTty
{
    public static class WebTerminalExtensions
    {
        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseWebTerminal(new WebTerminalOptions());

            return app;
        }

        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app, string path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new WebTerminalOptions
            {
                Path = path,
            };

            app.UseWebTerminal(options);

            return app;
        }

        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app, WebTerminalOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<WebTerminalMiddleware>(Options.Create(options));

            return app;
        }
    }
}
