using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using WebTty.Api;

namespace WebTty.Hosting
{
    public static class WebTtyHost
    {
        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHost(webhost =>
                    webhost
                        .ConfigureServices(ConfigureServices)
                        .Configure(Configure));
        }

        public static IHostBuilder CreateEmptyBuilder()
        {
            return new HostBuilder()
                .ConfigureWebHost(webhost =>
                    webhost
                        .ConfigureServices(ConfigureServices)
                        .Configure(Configure));
        }

        private static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddOptions<StaticFileOptions>()
                .Configure(options => options.FileProvider = new ManifestEmbeddedFileProvider(typeof(WebTtyHost).Assembly, "wwwroot"));
            services.AddPty();
            services.AddResponseCompression();
            services.AddRazorPages();
        }

        private static void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
            app.UseResponseCompression();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapPty(context.Configuration.GetValue<string>("Path"));
            });
        }
    }
}
