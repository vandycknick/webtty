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
using Serilog;

namespace WebTty.Integration.Test
{
    public class StartupSmokeTest
    {
        [Fact]
        public async Task Server_Starts_AndConfiguresEndpoints()
        {
            // Given
            await using var host = new WebTtyTestHost();
            await host.InitializeAsync();

            // When
            var client = host.GetTestClient();
            var response = await client.GetAsync("/");
            var connection = await host.OpenWebSocket("pty");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(WebSocketState.Open, connection.State);
        }
    }
}
