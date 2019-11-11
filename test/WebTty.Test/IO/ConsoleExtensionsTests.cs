using WebTty.IO;
using WebTty.Test.Mocks;
using Xunit;

namespace WebTty.Test.IO
{
    public class ConsoleExtensionsTests
    {
        [Fact]
        public void IConsoleExtensions_WriteLine_WritesANewLineToOut()
        {
            //Given
            var console = new InMemoryConsole();

            //When
            console.WriteLine();

            //Then
            var output = console.OutString.GetStringBuilder();
            Assert.Equal("\n", output.ToString());
        }

        [Fact]
        public void IConsoleExtensions_WriteLine_WritesTheGivenStringWithANewLineToOut()
        {
            //Given
            var console = new InMemoryConsole();

            //When
            console.WriteLine("Testline.");

            //Then
            var output = console.OutString.GetStringBuilder();
            Assert.Equal("Testline.\n", output.ToString());
        }

        [Fact]
        public void IConsoleExtensions_Write_WritesTheGivenStringToOut()
        {
            //Given
            var console = new InMemoryConsole();

            //When
            console.Write("One");
            console.Write("Two");

            //Then
            var output = console.OutString.GetStringBuilder();
            Assert.Equal("OneTwo", output.ToString());
        }
    }
}
