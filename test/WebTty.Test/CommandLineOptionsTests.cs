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
            // Arrange
            var args = new string[] { arg1 };

            // Act
            var options = CommandLineOptions.Build(args);

            // Assert
            Assert.True(options.ShowHelp);
        }

        [Fact]
        public void CommandLineOptions_Build_ShouldSetShowHelpToFalseWhenNoHelpArgumentsAreGiven()
        {
            // Arrange
            var args = new string[] { "--hello", "--world" };

            // Act
            var options = CommandLineOptions.Build(args);

            // Assert
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
            // Arrange
            var args = new string[] { arg, value };

            // Act
            var options = CommandLineOptions.Build(args);

            // Assert
            Assert.Equal(expected: int.Parse(value), options.Port);
        }
    }
}
