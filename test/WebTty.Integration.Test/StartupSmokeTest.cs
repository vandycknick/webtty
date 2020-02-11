using Moq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebTty.Api.Common;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net;
using System.Net.WebSockets;
using WebTty.Hosting;

namespace WebTty.Integration.Test
{
    public class StartupSmokeTest
    {
        [Fact]
        public async Task Server_Starts_AndConfiguresEndpoints()
        {
            //Given
            var mockEngine = new Mock<IEngine>();

            var options = CommandLineOptions.Build(new string[] { });
            var configSource = new CommandLineOptionsConfigSource(options);
            var builder = WebTtyHost.CreateEmptyBuilder()
                 .ConfigureAppConfiguration(builder => builder.Add(configSource))
                 .ConfigureWebHost(webHost =>
                 {
                     webHost.UseTestServer();
                     webHost.ConfigureTestServices(services =>
                     {
                         services.AddSingleton(mockEngine.Object);
                     });
                 });

            var host = await builder.StartAsync();
            var server = host.GetTestServer();
            var client = host.GetTestClient();

            var endpoint = new Uri(server.BaseAddress, "pty");
            var socketClient = server.CreateWebSocketClient();

            //When
            var response = await client.GetAsync("/");
            var connection = await socketClient.ConnectAsync(endpoint, CancellationToken.None);

            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(WebSocketState.Open, connection.State);
        }
    }
}
