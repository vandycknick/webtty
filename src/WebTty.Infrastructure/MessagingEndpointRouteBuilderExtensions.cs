using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Infrastructure.Connection;

namespace WebTty.Infrastructure
{
    public static class MessagingEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapMessageHandler<TMessageHandler>(this IEndpointRouteBuilder endpoints, string route) where TMessageHandler : IMessageHandler
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            var connectionHandler = endpoints.ServiceProvider.GetRequiredService<HttpConnectionHandler>();
            var app = endpoints.CreateApplicationBuilder();
            app.UseWebSockets();
            app.Run(context => connectionHandler.ExecuteAsync<TMessageHandler>(context));

            var handler = app.Build();
            return endpoints.Map(route, handler);
        }
    }
}
