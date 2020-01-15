using Microsoft.Extensions.DependencyInjection;
using WebTty.Application.Common;
using WebTty.Infrastructure;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPty(this IServiceCollection services)
        {
            var resolver = new MessageResolver();
            services.AddMessaging(resolver);
            services.AddTransient<TerminalEngine>();
            services.AddSingleton<IMessageResolver>(_ => resolver);
            return services;
        }
    }
}
