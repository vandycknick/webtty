using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace WebTty.Integration.Test
{
    public class PtyEndpointsTests : IClassFixture<WebTtyHostFactory>
    {
        private readonly WebTtyHostFactory _factory;

        public PtyEndpointsTests(WebTtyHostFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PtyEndpoint_Returns_405WhenNotAGetRequest()
        {
            // Given
            var client = _factory.GetTestClient();

            // When
            var content = new StringContent("Hello");
            var response = await client.PostAsync("/pty", content);

            // Then
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task PtyEndpoint_Returns_426WhenNoUpgrade()
        {
            // Given
            var client = _factory.GetTestClient();

            // When
            var response = await client.GetAsync("/pty");

            // Then
            Assert.Equal(HttpStatusCode.UpgradeRequired, response.StatusCode);
            Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
        }
    }
}
