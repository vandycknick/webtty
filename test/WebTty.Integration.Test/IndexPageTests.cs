using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using WebTty.Integration.Test.Helpers;
using Xunit;

namespace WebTty.Integration.Test
{
    public class IndexPageTests
    {
        [Fact]
        public async Task WebTtyServer_Returns_IndexPage_FromRoot()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            var configSource = new CommandLineOptionsConfigSource(options);
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(builder => builder.Add(configSource))
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseStartup<Startup>();
                    webHost.UseTestServer();
                });

            using var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            // When
            var response = await client.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(body);

            await host.StopAsync();
        }

        [Fact]
        public async Task WebTtyServer_Injects_ClientConfiguration()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            var configSource = new CommandLineOptionsConfigSource(options);
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(builder => builder.Add(configSource))
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseStartup<Startup>();
                    webHost.UseTestServer();
                });

            using var host = await hostBuilder.StartAsync();
            var client = host.GetTestClient();

            // When
            var response = await client.GetAsync("/");
            var document = await HtmlHelpers.GetDocumentAsync(response);

            var configElement = document.GetElementById("config");
            var config = JsonDocument.Parse(configElement.TextContent);

            // Then
            Assert.Equal("script", configElement.TagName.ToLower());
            Assert.Equal("/pty", config.RootElement.GetProperty("ptyPath").GetString());
            Assert.Equal("default", config.RootElement.GetProperty("theme").GetString());

            await host.StopAsync();
        }
    }
}
