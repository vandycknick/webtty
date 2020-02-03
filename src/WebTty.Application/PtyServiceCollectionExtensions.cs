using Microsoft.Extensions.DependencyInjection;
using WebTty.Application.Common;
using WebTty.Infrastructure;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Application
{
    public static class PtyServiceCollectionExtensions
    {
        public static IServiceCollection AddPty(this IServiceCollection services)
        {
            services.AddMessaging(typeof(PtyMessageHandler));
            services.AddTransient<TerminalEngine>();

            return services;
        }
    }
}
