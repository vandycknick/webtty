using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using WebTty.Application.Common;
using Xunit;

namespace WebTty.Integration.Test
{
    public class WebTtyHostFactory : IAsyncLifetime
    {
        private readonly IHostBuilder hostBuilder;
        private IHost host;
        public TestServer Server;
        public Mock<IEngine> MockEngine;

        public WebTtyHostFactory()
        {
            MockEngine = new Mock<IEngine>();

            var options = CommandLineOptions.Build(new string[] { });
            var configSource = new CommandLineOptionsConfigSource(options);
            hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(builder => builder.Add(configSource))
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseStartup<Startup>();
                    webHost.UseTestServer();
                    webHost.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(MockEngine.Object);
                    });
                });
        }

        public async Task<WebSocket> OpenWebSocket(string path)
        {
            var client = Server.CreateWebSocketClient();
            var endpoint = new Uri(Server.BaseAddress, path);
            var socket = await client.ConnectAsync(endpoint, CancellationToken.None);
            return socket;
        }

        public T GetRequiredService<T>() => Server.Services.GetRequiredService<T>();

        public async Task InitializeAsync()
        {
            host = await hostBuilder.StartAsync();
            Server = host.GetTestServer();
        }

        public async Task DisposeAsync()
        {
            await host?.StopAsync();
            host?.Dispose();
            Server?.Dispose();
        }
    }
}
