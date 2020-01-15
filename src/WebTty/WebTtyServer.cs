using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebTty.IO;
using WebTty.UI.Common;
using WebTty.Application;

namespace WebTty
{
    public class WebTtyServer : IServer
    {
        private readonly CommandLineOptions _options;
        private readonly IConsole _console;
        private readonly IWebHostBuilder _hostBuilder;

        public WebTtyServer(CommandLineOptions options, IConsole console, IWebHostBuilder hostBuilder)
        {
            _options = options;
            _console = console;
            _hostBuilder = hostBuilder;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddPty();
            services.AddWebClient(config => {
                config.PtyPath = _options.Path;
                config.Theme = _options.Theme;
            });
            services.AddResponseCompression();
            services.AddRazorPages();
        }

        private void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapPty(_options.Path);
            });
        }

        private string GetCurrentAssemblyRootPath()
        {
            var root = Assembly.GetExecutingAssembly().Location;
            var rootDirectory = Path.GetDirectoryName(root);
            return rootDirectory;
        }

        public async Task RunAsync(CancellationToken token = default)
        {
            var contentRoot = GetCurrentAssemblyRootPath();

            var host = _hostBuilder
                .UseStaticWebAssets()
                .UseContentRoot(contentRoot)
                .PreferHostingUrls(false)
                .SuppressStatusMessages(true)
                .UseKestrel(kestrel =>
                {
                    kestrel.Listen(_options.Address, _options.Port);

                    if (!string.IsNullOrEmpty(_options.UnixSocket))
                    {
                        kestrel.ListenUnixSocket(_options.UnixSocket);
                    }
                })
                .UseStartup<Program>()
                .ConfigureServices(ConfigureServices)
                .Configure(Configure)
                .Build();

            await host.StartAsync(token);
            _console.WriteLine($"Listening on http://{_options.Address.ToString()}:{_options.Port}");
            _console.WriteLine("");
            _console.WriteLine("Press CTRL+C to exit");
            await host.WaitForShutdownAsync(token);
        }
    }
}
