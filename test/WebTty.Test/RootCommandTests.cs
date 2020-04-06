using System;
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
        [InlineData("home")]
        [InlineData("loopback")]
        [InlineData("ff:ff:ff:ff")]
        [InlineData("192.165.1.1.1")]
        public async Task RootCommand_Invoke_ReturnsAnErrorWhenGivenAnInvalidIp(string value)
        {
            // Given
            var args = new string[]
            {
                "-a", value
            };
            var command = Program.CreateRootCommand(args);
            command.Handler = CommandHandler.Create(Noop);
            var console = new TestConsole();

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal($"Invalid: {value} is not a valid ip address", console.Error.ToString().Trim('\n', '\r'));
            Assert.Equal(1, result);
        }

        [Theory]
        [MemberData(nameof(GetValidIPAddressData))]
        public async Task RootCommand_Invoke_CorrectlySetsAddressForValidIP(string value, IPAddress expected)
        {
            // Given
            var args = new string[]
            {
                "-a", value
            };
            var command = Program.CreateRootCommand(args);
            var console = new TestConsole();
            IPAddress address = null;
            command.Handler = CommandHandler.Create<IPAddress>(a => address = a);

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal(expected, address);
            Assert.Equal(0, result);
        }


        [Theory]
        [InlineData("65535")]
        [InlineData("576865934")]
        public async Task RootCommand_Invoke_ReturnsErrorWhenPortHigherThanMaxValue(string arg)
        {
            // Given
            var args = new string[]
            {
                "-p", arg
            };
            var command = Program.CreateRootCommand(args);
            command.Handler = CommandHandler.Create(Noop);
            var console = new TestConsole();

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be a value between 0 and 65535.", console.Error.ToString().Trim('\n', '\r'));
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("-500")]
        public async Task RootCommand_Invoke_ReturnsErrorWhenPortLowerThanMinValue(string value)
        {
            // Given
            var args = new string[]
            {
                "-p", value
            };
            var command = Program.CreateRootCommand(args);
            command.Handler = CommandHandler.Create(Noop);
            var console = new TestConsole();

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be a value between 0 and 65535.", console.Error.ToString().Trim('\n', '\r'));
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("--9")]
        [InlineData("true")]
        [InlineData("hello")]
        public async Task RootCommand_Invoke_ReturnsAReadableErrorMessageWhenNotGivenAnInt(string value)
        {
            // Given
            var args = new string[]
            {
                "-p", value
            };
            var command = Program.CreateRootCommand(args);
            command.Handler = CommandHandler.Create(Noop);
            var console = new TestConsole();

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument port should be an integer.", console.Error.ToString().Trim('\n', '\r'));
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
            var args = new string[]
            {
                "-p", value
            };
            var command = Program.CreateRootCommand(args);
            var console = new TestConsole();
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
        [InlineData("\\pty")]
        [InlineData("no-slash")]
        public async Task RootCommand_Invoke_ReturnsErrorMessageWhenPathDoesNotStartWithAString(string value)
        {
            // Given
            var args = new string[]
            {
                "--path", value
            };
            var command = Program.CreateRootCommand(args);
            command.Handler = CommandHandler.Create(Noop);
            var console = new TestConsole();

            // When
            var result = await command.InvokeAsync(args, console);

            // Then
            Assert.Equal("Argument path should start with a /.", console.Error.ToString().Trim('\n', '\r'));
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task RootCommand_Invoke_CorrectlySetsPathValue()
        {
            // Given
            var args = new string[]
            {
                "--path", "/world"
            };
            var command = Program.CreateRootCommand(args);
            var console = new TestConsole();
            command.Handler = CommandHandler.Create<string>(Assertion);

            // When
            await command.InvokeAsync(args, console);

            // Then
            static void Assertion(string path)
            {
                Assert.Equal("/world", path);
            }
        }

        [Fact]
        public async Task RootCommand_Invoke_CorrectlySetsThemeValue()
        {
            // Given
            var args = new string[]
            {
                "--theme", "hello"
            };
            var command = Program.CreateRootCommand(args);
            var console = new TestConsole();
            command.Handler = CommandHandler.Create<string>(Assertion);

            // When
            await command.InvokeAsync(args, console);

            // Then
            static void Assertion(string theme)
            {
                Assert.Equal("hello", theme);
            }
        }

        [Fact]
        public async Task RootCommand_Invoke_CorrectlySetsUnixSocketValue()
        {
            // Given
            var args = new string[]
            {
                "--unix-socket", "hello"
            };
            var command = Program.CreateRootCommand(args);
            var console = new TestConsole();
            command.Handler = CommandHandler.Create<string>(Assertion);

            // When
            await command.InvokeAsync(args, console);

            // Then
            static void Assertion(string unixSocket)
            {
                Assert.Equal("hello", unixSocket);
            }
        }

        private static readonly Action Noop = () => {};

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
