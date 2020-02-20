using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
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

        public IHostBuilder ConfigureTestHostBuilder()
        {
            return WebTtyHost.CreateHostBuilder()
                 .ConfigureWebHost(webHost =>
                 {
                     webHost.UseTestServer();
                     webHost.ConfigureTestServices(services =>
                     {
                         services.AddSingleton(MockEngine.Object);
                     });
                 });
        }

        public WebTtyHostFactory()
        {
            MockEngine = new Mock<IEngine>();
            _hostBuilder = ConfigureTestHostBuilder();
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
