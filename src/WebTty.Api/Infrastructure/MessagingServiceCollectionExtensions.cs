using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WebTty.Api.Infrastructure.Connection;
using WebTty.Api.Infrastructure.Core;
using WebTty.Api.Infrastructure.Protocol;

namespace WebTty.Api.Infrastructure
{
    public static class MessagingServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            var options = new MessagingOptions
            {
                MessageSource = Assembly.GetExecutingAssembly(),
            };

            return services.AddMessaging(options);
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services, MessagingOptions options)
        {
            services.AddSingleton(options);
            services.AddSingleton<JsonProtocol>();
            services.AddSingleton<BinaryProtocol>();
            services.AddSingleton<IMessageWriter>(provider =>
                options.Format == MessageFormat.Json ?
                    (IMessageWriter)provider.GetRequiredService<JsonProtocol>() :
                    provider.GetRequiredService<BinaryProtocol>());
            services.AddSingleton<IMessageReader>(provider =>
                options.Format == MessageFormat.Json ?
                    (IMessageReader)provider.GetRequiredService<JsonProtocol>() :
                    provider.GetRequiredService<BinaryProtocol>());

            services.AddSingleton<HttpConnectionHandler>();
            services.AddSingleton<HttpConnectionManager>();
            services.AddTransient<ConnectionContext>();
            services.AddSingleton<IMessageResolver, MessageResolver>();
            services.AddTransient<Dispatcher>();

            return services;
        }
    }
}
