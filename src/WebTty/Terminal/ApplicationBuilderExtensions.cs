using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WebTty.Terminal
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseWebTerminal(new TerminalOptions());

            return app;
        }

        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app, string path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = new TerminalOptions
            {
                Path = path,
            };

            app.UseWebTerminal(options);

            return app;
        }

        public static IApplicationBuilder UseWebTerminal(this IApplicationBuilder app, TerminalOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<TerminalMiddleware>(Options.Create(options));

            return app;
        }
    }
}
