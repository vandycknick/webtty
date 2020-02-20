using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using WebTty.Api;
using Serilog;

namespace WebTty.Hosting
{
    public static class WebTtyHost
    {
        public static IHostBuilder CreateHostBuilder() => CreateHostBuilder(new WebTtyHostOptions());

        public static IHostBuilder CreateHostBuilder(WebTtyHostOptions options)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(appConfig =>
                    appConfig.AddInMemoryCollection(options.ToDictionary()))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStaticWebAssets()
                        .PreferHostingUrls(false)
                        .SuppressStatusMessages(true)
                        .ConfigureServices(ConfigureServices)
                        .Configure(Configure)
                        .UseKestrel(kestrel =>
                        {
                            kestrel.Listen(options.Address, options.Port);

                            if (!string.IsNullOrEmpty(options.UnixSocket))
                            {
                                kestrel.ListenUnixSocket(options.UnixSocket);
                            }
                        });
                })
                .UseSerilog();
        }

        private static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
            services.AddOptions<StaticFileOptions>()
                .Configure(options => options.FileProvider = new ManifestEmbeddedFileProvider(typeof(WebTtyHost).Assembly, "wwwroot"));

            services.AddPty();
            services.AddResponseCompression();
            services.AddRazorPages();
        }

        private static void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();
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
