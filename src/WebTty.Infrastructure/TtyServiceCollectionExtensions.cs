using Microsoft.Extensions.DependencyInjection;
using WebTty.Infrastructure.Connection;
using WebTty.Infrastructure.Dispatcher;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Infrastructure
{
    public static class TtyServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IMessageResolver messageResolver)
        {
            var protocol = new BinaryProtocol(messageResolver);
            services.AddSingleton<IMessageWriter>(protocol);
            services.AddSingleton<IMessageReader>(protocol);
            services.AddSingleton<HttpConnectionHandler>();
            services.AddSingleton<HttpConnectionManager>();
            services.AddTransient<TtyConnectionContext>();
            services.AddTransient<IConnectionContext>(p => p.GetRequiredService<TtyConnectionContext>());
            services.AddTransient<IMessageDispatcher, DefaultMessageDispatcher>();

            return services;
        }
    }
}
