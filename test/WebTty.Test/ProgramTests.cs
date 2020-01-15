using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WebTty.Test.Mocks;
using Xunit;

namespace WebTty.Test
{
    public class ProgramTests
    {
        [Fact]
        public async Task Program_ExecuteAsync_PrintsCorrectHelpMessage()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--help" });

            //When
            var program = new Program(console, server.Object, options);
            await program.ExecuteAsync();

            var output = console.OutString.GetStringBuilder();

            //Then
            Assert.Equal($"{options.Name}: {options.Version}", output.ToString().Split(Environment.NewLine).FirstOrDefault());
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsAllAvailableOptionsInHelpMessage()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--help" });

            //When
            var program = new Program(console, server.Object, options);
            await program.ExecuteAsync();

            // Then
            var optionsWriter = new StringWriter();
            options.WriteOptions(optionsWriter);
            var optionString = optionsWriter.GetStringBuilder().ToString();

            var output = console.OutString.GetStringBuilder();
            Assert.Contains(optionString, output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroAfterPrintingHelpMessage()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--help" });

            //When
            var program = new Program(console, server.Object, options);
            var result = await program.ExecuteAsync();

            // Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_PrintsTheCurrentVersion()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--version" });

            //When
            var program = new Program(console, server.Object, options);
            await program.ExecuteAsync();

            //Then
            var output = console.OutString.GetStringBuilder();
            Assert.Equal($"{options.Version}\n", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroAfterPrintingTheVersion()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--version" });

            //When
            var program = new Program(console, server.Object, options);
            var result = await program.ExecuteAsync();

            //Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsZeroWhenServerExitsGracefully()
        {
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--version" });

            //When
            server.Setup(s => s.RunAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var program = new Program(console, server.Object, options);
            var result = await program.ExecuteAsync();

            //Then
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenGivenAnInvalidOption()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--port" });

            //When
            var program = new Program(console, server.Object, options);
            await program.ExecuteAsync();

            var output = console.OutString.GetStringBuilder();

            //Then
            Assert.Contains("Missing required value for option '--port'.", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenGivenAnInvalidOption()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { "--port" });

            //When
            var program = new Program(console, server.Object, options);
            var result = await program.ExecuteAsync();

            //Then
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Program_ExecuteAsync_WritesAnErrorMessageWhenTheServerThrowsAnException()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { });

            server.Setup(s => s.RunAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("some error"));

            //When
            var program = new Program(console, server.Object, options);
            await program.ExecuteAsync();

            var output = console.OutString.GetStringBuilder();

            //Then
            Assert.Contains("some error", output.ToString());
            Assert.Contains($"Try '{options.Name} --help' for more information.", output.ToString());
        }

        [Fact]
        public async Task Program_ExecuteAsync_ReturnsOneWhenTheServerThrowsAnException()
        {
            //Given
            var console = new InMemoryConsole();
            var server = new Mock<IServer>();
            var options = CommandLineOptions.Build(new string[] { });

            server.Setup(s => s.RunAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("some error"));

            //When
            var program = new Program(console, server.Object, options);
            var result = await program.ExecuteAsync();

            //Then
            Assert.Equal(1, result);
        }
    }
}
