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
        public void CommnandLineOptions_Build_ShouldSetVersionToTrueWhenPassedVersionOption()
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

        [Theory]
        [InlineData(new string[]{ "-p" }, true, "Missing required value for option '-p'.")]
        [InlineData(new string[]{ "--help" }, false, "")]
        public void CommandLineOptions_TryGetInvalidOptions_ReturnsFalseAndAnErrorMessageWhenGivenAnInvalidOption(string[] args, bool hasError, string errorMessage)
        {
            //Given
            var options = CommandLineOptions.Build(args);

            //When
            var result = options.TryGetInvalidOptions(out string message);

            //Then
            Assert.Equal(hasError, result);
            Assert.Equal(errorMessage, message);
        }
    }
}
