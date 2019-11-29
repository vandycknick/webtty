using Xunit;

namespace WebTty.Test
{
    public class CommandLineOptionsTests
    {
        [Theory]
        [InlineData("-?")]
        [InlineData("-h")]
        [InlineData("--help")]
        public void CommandLineOptions_Build_ShouldSetShowHelpToTrueWhenPassedHelpOptions(string arg1)
        {
            // Given
            var args = new string[] { arg1 };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void CommandLineOptions_Build_ShouldSetVersionToTrueWhenPassedVersionOption()
        {
            // Given
            var args = new string[] { "--version" };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.True(options.ShowVersion);
        }

        [Fact]
        public void CommandLineOptions_Build_ShouldSetShowHelpToFalseWhenNoHelpArgumentsAreGiven()
        {
            // Given
            var args = new string[] { "--hello", "--world" };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.False(options.ShowHelp);
        }

        [Fact]
        public void CommandLineOptions_Build_SetsThePortTo5000ByDefault()
        {
            // Arrange
            var args = new string[] { };

            // Act
            var options = CommandLineOptions.Build(args);

            // Assert
            Assert.Equal(expected: 5000, options.Port);
        }

        [Theory]
        [InlineData("-p", "6666")]
        [InlineData("--port", "3000")]
        [InlineData("--port", "0")]
        public void CommandLineOptions_Build_SetsThePortToTheGivenValue(string arg, string value)
        {
            // Given
            var args = new string[] { arg, value };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.Equal(expected: int.Parse(value), options.Port);
        }

        [Fact]
        public void CommandLineOptions_Build_SetsPathToTtyByDefault()
        {
            // Given
            var args = new string[] { };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.Equal(expected: "/tty", options.Path);
        }

        [Fact]
        public void CommandLineOptions_Build_SetsPathToTheGivenValue()
        {
            // Given
            var args = new string[] { "--path", "/hello-world" };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.Equal(expected: "/hello-world", options.Path);
        }

        [Theory]
        [InlineData("-s", "/test/hello")]
        [InlineData("--unix-socket", "/test/world")]
        public void CommandLineOptions_Build_SetsUnixSocketToTheGivenValue(string arg, string value)
        {
            var args = new string[] { arg, value };

            // When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.Equal(expected: value, options.UnixSocket);
        }

        [Theory]
        [InlineData(new string[] { "-p", "5000", "-a", "192.0.0.1" }, "", "")]
        [InlineData(new string[] { "htop" }, "htop", "")]
        [InlineData(new string[] { "--", "ls", "-al" }, "ls", "-al")]
        [InlineData(new string[] { "--", "curl", "-i", "-X", "POST", "localhost:5000" }, "curl", "-i,-X,POST,localhost:5000")]
        public void CommandLineOptions_Build_SetsTheCorrectCommandAndArgsWhenGiven(string[] args, string command, string commandArgs)
        {
            // Given, When
            var options = CommandLineOptions.Build(args);

            // Then
            Assert.Equal(expected: command, options.Command);
            Assert.Equal(expected: commandArgs, string.Join(',', options.CommandArgs));
        }

        [Theory]
        [InlineData(new string[] { "-p" }, true, "Missing required value for option '-p'.")]
        [InlineData(new string[] { "--port" }, true, "Missing required value for option '--port'.")]
        [InlineData(new string[] { "--port", "-5" }, true, "'--port' must be greater than '0'.")]
        [InlineData(new string[] { "--port", "100000" }, true, "'--port' must be less than or equal to '65535'.")]
        [InlineData(new string[] { "--path", "should-start-wit-slash" }, true, "'--path' should start with a slash (/).")]
        [InlineData(new string[] { "--help" }, false, "")]
        public void CommandLineOptions_TryGetInvalidOptions_ValidatesCorrectly(string[] args, bool hasError, string errorMessage)
        {
            // Given
            var options = CommandLineOptions.Build(args);

            // When
            var result = options.TryGetInvalidOptions(out string message);

            // Then
            Assert.Equal(hasError, result);
            Assert.Equal(errorMessage, message);
        }
    }
}
