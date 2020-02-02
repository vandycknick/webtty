using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace WebTty.Test
{
    public class CommandLineOptionsConfigSourceTests
    {
        [Theory]
        [InlineData("ShowHelp", "false")]
        [InlineData("ShowVersion", "false")]
        [InlineData("Address", "127.0.0.1")]
        [InlineData("UnixSocket", "/tmp/hello")]
        [InlineData("Port", "5000")]
        [InlineData("Path", "/pty")]
        [InlineData("Theme", "default")]
        [InlineData("Command", "world")]
        public void CommandLineOptionsConfigSource_Build_ReturnsACommandLineOptionsProvider(string key, string expected)
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "-s", "/tmp/hello", "--", "world"});
            var configSource = new CommandLineOptionsConfigSource(options);
            var builder = new Mock<IConfigurationBuilder>();

            // When
            var configProvider = configSource.Build(builder.Object);
            configProvider.Load();

            // Then
            var result = configProvider.TryGet(key, out var value);
            Assert.True(result);
            Assert.Equal(expected, value);
        }
    }
}
