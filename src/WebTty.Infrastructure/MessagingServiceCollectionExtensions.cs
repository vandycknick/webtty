using System;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Infrastructure.Connection;
using WebTty.Infrastructure.Core;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Infrastructure
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Type messageSource)
        {
            services.AddSingleton(_ => new MessagingOptions
            {
                MessageSource = messageSource.Assembly
            });

            services.AddSingleton<IMessageWriter, BinaryProtocol>();
            services.AddSingleton<IMessageReader, BinaryProtocol>();
            services.AddSingleton<HttpConnectionHandler>();
            services.AddSingleton<HttpConnectionManager>();
            services.AddTransient<ConnectionContext>();
            services.AddSingleton<IMessageResolver, MessageResolver>();
            services.AddTransient<Dispatcher>();

            return services;
        }
    }
}
