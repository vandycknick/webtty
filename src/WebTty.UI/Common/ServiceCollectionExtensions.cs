using System;
using Microsoft.Extensions.DependencyInjection;

namespace WebTty.UI.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebClient(this IServiceCollection services, Action<WebClientConfig> configureOptions)
        {
            services.Configure(configureOptions);
            return services;
        }
    }
}
