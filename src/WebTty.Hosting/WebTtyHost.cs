using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Events;
using WebTty.Api;
using WebTty.Hosting.Models;
using WebTty.Hosting.Services;

namespace WebTty.Hosting
{
    public class WebTtyHost : IAsyncDisposable
    {
        private Microsoft.Extensions.Logging.ILogger _logger;

        protected IHost _app;
        private readonly string[] _args;
        private readonly Settings _options;

        public WebTtyHost() : this(Settings.Defaults, new string[] { })
        {

        }

        public WebTtyHost(Settings options, string[] args)
        {
            _options = options;
            _args = args;
        }

        public async Task RunAsync(CancellationToken token)
        {
            try
            {
                await StartAsync();

                var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                token.Register(_ => waitForStop.TrySetResult(null), null);

                await waitForStop.Task;
            }
            finally
            {
                await StopAsync();
                _logger?.LogInformation("Goodbye. 👋");
            }
        }

        public async Task StartAsync()
        {
            _app = CreateApplication().Build();

            _logger = _app.Services.GetRequiredService<ILogger<WebTtyHost>>();

            await _app.StartAsync();

            _logger.LogInformation("Listening on: http://{address}:{port}.", _options.Address, _options.Port);
            _logger.LogInformation("Press CTRL+C to exit.");
        }

        public async Task StopAsync()
        {
            _logger?.LogInformation("Application is shutting down...");
            await _app?.StopAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _app?.StopAsync();

            _app.Dispose();
            _app = null;
        }

        protected virtual IHostBuilder CreateApplication()
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(appConfig =>
                    appConfig.AddInMemoryCollection(_options.ToDictionary()))
                .ConfigureServices(services => services.AddSingleton(_options))
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
                            kestrel.Listen(_options.Address, _options.Port.Value);

                            if (!string.IsNullOrEmpty(_options.UnixSocket))
                            {
                                kestrel.ListenUnixSocket(_options.UnixSocket);
                            }
                        });
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .MinimumLevel.Is(LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                });
        }

        private static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
            services.AddOptions<StaticFileOptions>()
                .Configure(options =>
                {
                    options.FileProvider = new ManifestEmbeddedFileProvider(typeof(WebTtyHost).Assembly, "wwwroot");
                    options.OnPrepareResponse = ctx =>
                    {
                        if (ctx.Context.Request.Query.ContainsKey("v")) // Contains hash
                        {
                            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=31536000";
                        }
                    };
                });

            services.AddSingleton<StaticContentService>();

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
