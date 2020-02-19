using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using WebTty.Api.Common;
using WebTty.Hosting;
using Xunit;

namespace WebTty.Integration.Test
{
    public class WebTtyHostFactory : IAsyncLifetime
    {
        private readonly IHostBuilder _hostBuilder;
        private IHost _host;
        public TestServer Server;
        public Mock<IEngine> MockEngine;

        public IHostBuilder ConfigureTestHostBuilder(CommandLineOptions options)
        {
            var configSource = new CommandLineOptionsConfigSource(options);
            return WebTtyHost.CreateHostBuilder()
                 .ConfigureAppConfiguration(builder => builder.Add(configSource))
                 .ConfigureWebHost(webHost =>
                 {
                     webHost.UseTestServer();
                     webHost.ConfigureTestServices(services =>
                     {
                         services.AddSingleton(MockEngine.Object);
                     });
                 })
                 .UseSerilog();
        }

        public WebTtyHostFactory()
        {
            MockEngine = new Mock<IEngine>();

            var options = CommandLineOptions.Build(new string[] { });
            _hostBuilder = ConfigureTestHostBuilder(options);
        }

        public async Task<WebSocket> OpenWebSocket(string path)
        {
            var client = Server.CreateWebSocketClient();
            var endpoint = new Uri(Server.BaseAddress, path);
            var socket = await client.ConnectAsync(endpoint, CancellationToken.None);
            return socket;
        }

        public HttpClient GetTestClient() => _host.GetTestClient();

        public T GetRequiredService<T>() => Server.Services.GetRequiredService<T>();

        public async Task InitializeAsync()
        {
            _host = await _hostBuilder.StartAsync();
            Server = _host.GetTestServer();
        }

        public async Task DisposeAsync()
        {
            await _host?.StopAsync();
            _host?.Dispose();
            Server?.Dispose();
        }
    }
}
