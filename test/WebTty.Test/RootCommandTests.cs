using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebTty.Test
{
    public class RootCommandTests : IDisposable
    {
        public StringWriter MockedOut { get; set; }

        public RootCommandTests()
        {
            MockedOut = new StringWriter();
            Console.SetOut(MockedOut);
        }

        public void Dispose()
        {
            MockedOut.Dispose();
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_PrintsCorrectHelpMessage()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--help" });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            await cmd.ExecuteAsync();

            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Equal($"{options.Name}: {options.Version}", output.ToString().Split(Environment.NewLine).FirstOrDefault());
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_PrintsAllAvailableOptionsInHelpMessage()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--help" });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
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
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_PrintsTheCurrentVersion()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--version" });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();

            // Then
            var output = MockedOut.GetStringBuilder();
            Assert.Equal($"{options.Version}\n", output.ToString());
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_ReturnsZeroAfterPrintingTheVersion()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--version" });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_ReturnsZeroWhenServerExitsGracefully()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RootCommand_ExecuteAsync_WritesAnErrorMessageWhenGivenAnInvalidOption()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { "--port" });
            Func<CancellationToken, Task> dummy = token => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
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
            static Task dummy(CancellationToken token) => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionOnStartup()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            static Task startServerAsync(CancellationToken token)
            {
                throw new Exception("some error");
            }

            static Task waitForShutDownAsync(CancellationToken token) => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, startServerAsync, waitForShutDownAsync);
            var result = await cmd.ExecuteAsync();

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
            static Task startServerAsync(CancellationToken token)
            {
                throw new Exception("some error");
            }

            static Task waitForShutDownAsync(CancellationToken token) => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, startServerAsync, waitForShutDownAsync);
            var result = await cmd.ExecuteAsync();

            //Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionWhileRunning()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            static Task startServerAsync(CancellationToken token) => Task.CompletedTask;
            static Task waitForShutDownAsync(CancellationToken token)
            {
                throw new Exception("some error while running");
            }

            // When
            var cmd = new RootCommand(options, startServerAsync, waitForShutDownAsync);
            var result = await cmd.ExecuteAsync();
            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains("some error while running", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenTheServerThrowsAnExceptionWhileRunning()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            static Task startServerAsync(CancellationToken token) => Task.CompletedTask;
            static Task waitForShutDownAsync(CancellationToken token)
            {
                throw new Exception("some error while running");
            }

            // When
            var cmd = new RootCommand(options, startServerAsync, waitForShutDownAsync);
            var result = await cmd.ExecuteAsync();

            // Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsAMesssageWhenTheServerIsUpAndRunning()
        {
            // Given
            var options = CommandLineOptions.Build(new string[] { });
            static Task dummy(CancellationToken token) => Task.CompletedTask;

            // When
            var cmd = new RootCommand(options, dummy, dummy);
            var result = await cmd.ExecuteAsync();
            var output = MockedOut.GetStringBuilder();

            // Then
            Assert.Contains($"Listening on http://{options.Address.ToString()}:{options.Port}", output.ToString());
            Assert.Contains("Press CTRL+C to exit", output.ToString());
        }
    }
}
