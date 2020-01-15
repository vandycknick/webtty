using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Infrastructure;

namespace WebTty.Application
{
    public static class PtyEndpointExtensions
    {
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
