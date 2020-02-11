using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace WebTty.Test
{
    public class ProgramTests : IDisposable
    {
        public StringWriter MockedOut { get; set; }
        public Mock<IHost> MockedHost { get; set; }
        public IServiceCollection Services { get; set; }

        public ProgramTests()
        {
            MockedOut = new StringWriter();
            Console.SetOut(MockedOut);

            Services = new ServiceCollection();

            var lifetime = new Mock<IHostApplicationLifetime>();
            lifetime.SetupGet(l => l.ApplicationStopping).Returns(new CancellationToken(true));

            Services.AddSingleton(lifetime.Object);

            MockedHost = new Mock<IHost>();
            MockedHost.SetupGet(h => h.Services).Returns(Services.BuildServiceProvider());
        }

        public void Dispose()
        {
            MockedOut.Dispose();
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsCorrectHelpMessage()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--help" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            await cmd.ExecuteAsync();

            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Equal($"{options.Name}: {options.Version}", output.ToString().Split(Environment.NewLine).FirstOrDefault());
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsAllAvailableOptionsInHelpMessage()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--help" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            await cmd.ExecuteAsync();

            // Then
            var output = MockedOut.GetStringBuilder();
            var optionsWriter = new StringWriter();
            options.WriteOptions(optionsWriter);
            var optionString = optionsWriter.GetStringBuilder().ToString();

            Assert.Contains(optionString, output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroAfterPrintingHelpMessage()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--help" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsTheCurrentVersion()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--version" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            var output = MockedOut.GetStringBuilder();
            Assert.Equal($"{options.Version}\n", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroAfterPrintingTheVersion()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--version" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroWhenServerExitsGracefully()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenGivenAnInvalidOption()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--port" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();
            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains("Missing required value for option '--port'.", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenGivenAnInvalidOption()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--port" });

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionOnStartup()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            MockedHost
                .Setup(host => host.StartAsync(CancellationToken.None))
                .ThrowsAsync(new Exception("some error"));

            // When
            var cmd = new Program(options, MockedHost.Object);
            await cmd.ExecuteAsync();

            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains("some error", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenTheServerThrowsAnExceptionOnStartup()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            MockedHost
                .Setup(host => host.StartAsync(CancellationToken.None))
                .ThrowsAsync(new Exception("some error"));

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            //Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionWhileShuttingDown()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            MockedHost
                .Setup(host => host.StopAsync(CancellationToken.None))
                .ThrowsAsync(new Exception("some error"));


            // When
            var cmd = new Program(options, MockedHost.Object);
            await cmd.ExecuteAsync();
            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains("some error", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenTheServerThrowsAnExceptionWhileShuttingDown()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            MockedHost
                .Setup(host => host.StartAsync(CancellationToken.None))
                .ThrowsAsync(new Exception("some error"));

            // When
            var cmd = new Program(options, MockedHost.Object);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsAMesssageWhenTheServerIsUpAndRunning()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });

            // When
            var cmd = new Program(options, MockedHost.Object);
            await cmd.ExecuteAsync();
            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains($"Listening on http://{options.Address.ToString()}:{options.Port}", output.ToString());
            Assert.Contains("Press CTRL+C to exit", output.ToString());
        }
    }
}
