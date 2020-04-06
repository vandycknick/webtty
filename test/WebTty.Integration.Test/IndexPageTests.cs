using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebTty.Integration.Test.Helpers;
using Xunit;

namespace WebTty.Integration.Test
{
    public class IndexPageTests : IClassFixture<WebTtyTestHost>
    {
        private readonly WebTtyTestHost _testHost;

        public IndexPageTests(WebTtyTestHost testHost)
        {
            _testHost = testHost;
        }

        [Fact]
        public async Task WebTtyServer_Returns_IndexPage_FromRoot()
        {
            // Given
            var client = _testHost.GetTestClient();

            // When
            var response = await client.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task WebTtyServer_Injects_ClientConfiguration()
        {
            // Given
            var client = _testHost.GetTestClient();

            // When
            var response = await client.GetAsync("/");
            var document = await HtmlHelpers.GetDocumentAsync(response);

            var configElement = document.GetElementById("config");
            var config = JsonDocument.Parse(configElement.TextContent);

            // Then
            Assert.Equal("script", configElement.TagName.ToLower());
            Assert.Equal("/pty", config.RootElement.GetProperty("ptyPath").GetString());
            Assert.Equal("default", config.RootElement.GetProperty("selectedTheme").GetString());
        }
    }
}
