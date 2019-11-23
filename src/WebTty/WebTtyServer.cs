using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WebTty.IO;
using WebTty.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using MediatR;
using WebTty.Messages.Helpers;
using WebTty.Protocol;
using WebTty.Transport;
using WebTty.Terminal;
using Microsoft.Extensions.Hosting;
using WebTty.UI;

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
            services.AddResponseCompression();
            services.AddMediatR(config => config.AsScoped(), GetType());

            services.AddSingleton<IMessageResolver, MessageResolver>();
            services.AddSingleton<BinaryDeserializerMap>();
            services.AddSingleton<IProtocol, BinaryProtocol>();
            services.AddScoped<ITransport, WebSocketsTransport>();
            services.AddScoped<IConnection, HttpConnection>();
            services.AddScoped<IConnectionHandler, TerminalConnectionHandler>();
            services.AddScoped<IMessageDispatcher, TerminalMessageDispatcher>();

            services.AddScoped<TerminalManager>();
        }

        public void Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseResponseCompression();

            app.UseMiddleware<UiMiddleware>();
            app.UseWebSockets();
            app.UseWebTerminal(_options.Path);
        }

        public async Task RunAsync(CancellationToken token = default)
        {
            var host = _hostBuilder
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
