using Xunit;
using WebTty.Exec;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace WebTty.Exec.Test
{
    public class ProcessStart
    {

        [Fact]
        public async Task Process_Start_CorrectlyRedirectsStdin()
        {
            //Given
            var script = @"
var readline = require('readline');

var rl = readline.createInterface({
  input: process.stdin,
  terminal: false
});

rl.on('line', function (line) {
  console.log(line);
  process.exit(0);
});
";
            var attr = new ProcAttr
            {
                RedirectStdin = true,
                RedirectStdout = true,
            };
            var process = Process.Start("node", new string[] { "-e", script }, attr);

            //When
            await process.Stdin.WriteLineAsync("hello node");
            await process.Stdin.FlushAsync();

            var result = await process.Stdout.ReadLineAsync();

            //Then
            process.Wait();
            Assert.Contains("hello node", result);
        }

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
            process.Wait();
            Assert.Contains("hello", result);
            Assert.Equal(0, process.ExitCode);
        }

        [Fact]
        public async Task Process_Start_CorrectlyRedirectsStdErr()
        {
            // Given
            var attr = new ProcAttr
            {
                RedirectStderr = true,
            };
            var process = Process.Start("node", new string[] { "-e", "console.error('oh no')" }, attr);

            // When
            var result = await process.Stderr.ReadToEndAsync();

            // Then
            process.Wait();
            Assert.Contains("oh no", result);
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

        [Fact]
        public void Process_Start_ThrowsAnErrorWhenCommandIsNull()
        {
            // Given
            var attr = new ProcAttr();

            // When, Then
            Assert.Throws<ArgumentNullException>(() =>
            {
                Process.Start(null, null, attr);
            });
        }

        [Fact]
        public void Process_Start_ThrowsAnErrorWhenCommandIsEmptyString()
        {
            // Given
            var attr = new ProcAttr();

            // When, Then
            Assert.Throws<ArgumentNullException>(() =>
            {
                Process.Start("", null, attr);
            });
        }

        [Fact]
        public void Process_Start_ThrowsAnErrorArgsAreNull()
        {
            // Given
            var attr = new ProcAttr();

            // When, Then
            Assert.Throws<ArgumentNullException>(() =>
            {
                Process.Start("hello", null, attr);
            });
        }

        [Fact]
        public async Task Process_Start_CorrectlyPassingGivenEnvVarsToSubProcess()
        {
            //Given
            var rand = Guid.NewGuid().ToString();
            var env = new Dictionary<string, string>
            {
                { "RND_TEST_VAL", rand }
            };
            var attr = new ProcAttr
            {
                Env = env,
                RedirectStdout = true,
            };

            //When
            var process = Process.Start("node", new string[] { "-e", "console.log(process.env)" }, attr);
            var result = await process.Stdout.ReadToEndAsync();
            process.Wait();

            //Then
            Assert.Contains($"RND_TEST_VAL: '{rand}'", result);
        }
    }
}
