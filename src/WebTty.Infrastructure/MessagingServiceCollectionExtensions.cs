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
            var options = new MessagingOptions
            {
                MessageSource = messageSource.Assembly
            };

            return services.AddMessaging(options);
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services, MessagingOptions options)
        {
            services.AddSingleton(options);
            services.AddSingleton<JsonProtocol>();
            services.AddSingleton<BinaryProtocol>();
            services.AddSingleton<IMessageWriter>(provider =>
            {
                return options.Format == MessageFormat.Json ?
                    (IMessageWriter)provider.GetRequiredService<JsonProtocol>() :
                    provider.GetRequiredService<BinaryProtocol>();
            });



            if (options.Format == MessageFormat.Json)
            {
                services.AddSingleton<IMessageWriter, JsonProtocol>();
                services.AddSingleton<IMessageReader, JsonProtocol>();
            }
            else
            {
                services.AddSingleton<IMessageWriter, BinaryProtocol>();
                services.AddSingleton<IMessageReader, BinaryProtocol>();
            }
            services.AddSingleton<HttpConnectionHandler>();
            services.AddSingleton<HttpConnectionManager>();
            services.AddTransient<ConnectionContext>();
            services.AddSingleton<IMessageResolver, MessageResolver>();
            services.AddTransient<Dispatcher>();

            return services;
        }
    }
}
