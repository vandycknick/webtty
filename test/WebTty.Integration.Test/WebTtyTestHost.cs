using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using WebTty.Api.Common;
using WebTty.Hosting;
using Xunit;

namespace WebTty.Integration.Test
{
    public class WebTtyTestHost : WebTtyHost, IAsyncLifetime
    {
        public Mock<IEngine> MockEngine;

        private TestServer _testServer;

        public WebTtyTestHost() : base()
        {
            MockEngine = new Mock<IEngine>();
        }

        public async Task InitializeAsync()
        {
            await StartAsync();
            _testServer = _app.GetTestServer();
        }

        public HttpClient GetTestClient() => _app.GetTestClient();

        public T GetRequiredService<T>() => _testServer.Services.GetRequiredService<T>();

        public async Task<WebSocket> OpenWebSocket(string path)
        {
            var client = _testServer.CreateWebSocketClient();
            var endpoint = new Uri(_testServer.BaseAddress, path);
            var socket = await client.ConnectAsync(endpoint, CancellationToken.None);
            return socket;
        }

        protected override IHostBuilder CreateApplication()
        {
            return base.CreateApplication()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(MockEngine.Object);
                    });
                });
        }

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            _testServer?.Dispose();
        }
    }
}
