using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Api.Common;
using WebTty.Api.Infrastructure;
using WebTty.Schema.Messages;

namespace WebTty.Api
{
    public static class WebTtyHostingExtensions
    {
        public static IServiceCollection AddPty(this IServiceCollection services)
        {
            services.AddMessaging(options => options.MessageSource = typeof(OpenNewTabRequest).Assembly );
            services.AddTransient<IEngine, TerminalEngine>();
            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            return services;
        }

        public static IEndpointConventionBuilder MapPty(this IEndpointRouteBuilder endpoints, string route = "/pty")
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            return endpoints.MapMessageHandler<PtyMessageHandler>(route);
        }
    }
}
