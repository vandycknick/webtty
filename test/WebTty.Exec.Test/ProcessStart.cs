using Xunit;
using WebTty.Exec;
using System.Threading.Tasks;

namespace WebTty.Exec.Test
{
    public class ProcessStart
    {
        [Fact]
        public async Task Process_Start_CorrectlyRedirectsStdout()
        {
            // Given
            var attr = new ProcAttr
            {
                RedirectStdout = true,
            };
            var process = Process.Start("echo", new string[] { "hello" }, attr);

            // When
            var result = await process.Stdout.ReadToEndAsync();

            // Then
            Assert.Contains("hello", result);
            Assert.Equal(0, process.ExitCode);
        }

        [Fact]
        public void Process_Start_CorrectlyReturnsExitCodeFromChild()
        {
            // Given
            var attr = new ProcAttr
            {
            };
            var process = Process.Start("node", new string[] { "-e", "process.exit(122)" }, attr);

            // When
            process.Wait();

            // Then
            Assert.Equal(122, process.ExitCode);
        }
    }
}
