using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace WebTty.Test
{
    public class RootCommandTests
    {

        [Theory]
        [InlineData("65535")]
        [InlineData("576865934")]
        public async Task RootCommand_Invoke_ReturnsErrorWhenPortHigherThanMaxValue(string arg)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-p", arg
            };

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be a value between 0 and 65535.", console.Error.ToString().Trim('\n'));
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("-500")]
        public async Task RootCommand_Invoke_ReturnsErrorWhenPortLowerThanMinValue(string value)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-p", value
            };

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be a value between 0 and 65535.", console.Error.ToString().Trim('\n'));
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("--9")]
        [InlineData("true")]
        [InlineData("hello")]
        public async Task RootCommand_Invoke_ReturnsAReadableErrorMessageWhenNotGivenAnInt(string value)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-p", value
            };

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be an integer.", console.Error.ToString().Trim('\n'));
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("+5", 5)]
        [InlineData("6", 6)]
        [InlineData("8080", 8080)]
        [InlineData("5000", 5000)]
        public async Task RootCommand_Invoke_CorrectlySetsPortForValidPortValues(string value, int expected)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-p", value
            };
            int port = 0;
            command.Handler = CommandHandler.Create<int>(p =>
            {
                port = p;
            });

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal(expected, port);
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData("home")]
        [InlineData("loopback")]
        [InlineData("fff:fff:fff:fff")]
        [InlineData("192.165.1.1.1")]
        public async Task RootCommand_Invoke_ReturnsAnErrorWhenGivenAnInvalidIp(string value)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-a", value
            };

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal($"Invalid: Option: -a {value}", console.Error.ToString().Trim('\n'));
            Assert.Equal(1, result);
        }

        [Theory]
        [MemberData(nameof(GetValidIPAddressData))]
        public async Task RootCommand_Invoke_CorrectlySetsAddressForValidIP(string value, IPAddress expected)
        {
            // Given
            var command = Program.RootCommand();
            var console = new TestConsole();
            var args = new string[]
            {
                "-a", value
            };
            IPAddress address = null;
            command.Handler = CommandHandler.Create<IPAddress>(a =>
            {
                address = a;
            });

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal(expected, address);
            Assert.Equal(0, result);
        }

        public static IEnumerable<object[]> GetValidIPAddressData()
        {
            yield return new object[] { "any", IPAddress.Any };
            yield return new object[] { "localhost", IPAddress.Loopback };
            yield return new object[] { "192.165.1.1", IPAddress.Parse("192.165.1.1") };
            yield return new object[] { "0.0.0.0", IPAddress.Parse("0.0.0.0") };
            yield return new object[] { "127.0.0.1", IPAddress.Parse("127.0.0.1") };
        }
    }
}
