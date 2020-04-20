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
        public static IServiceCollection AddMessaging(this IServiceCollection services) =>
            services.AddMessaging(options =>
            {
                options.MessageSource = Assembly.GetExecutingAssembly();
            });

        public static IServiceCollection AddMessaging(this IServiceCollection services, Action<MessagingOptions> configureOptions)
        {
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var options = new MessagingOptions();
            configureOptions(options);

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
