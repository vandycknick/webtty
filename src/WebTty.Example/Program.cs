using System;
using System.Threading.Tasks;
using WebTty.Exec;

namespace WebTty.Example
{
    static class Program
    {
        static void Main(string[] args)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var attr = new ProcAttr
            {
                RedirectStdout = true,

                Sys = new SysProcAttr
                {
                    UseTty = true,
                },
            };

            Console.WriteLine($"Start time: {DateTime.Now.ToLongTimeString()}");
            // var process = Process.Start("pwsh", new string[] { "-command", "get-command" }, attr);
            // var process = Process.Start("pwsh", new string[] { "-command", "Get-Date; git --version; Get-Date; ls --color=auto; Get-Date; stty -a; tty; git --version; Get-Date" }, attr);
            // var process = Process.Start("pwsh", new string[] { "-command", "git --version" }, attr);
            // var process = Process.Start("pwsh", new string[] { "-command", "pwd" }, attr);
            var process = Process.Start("pwsh", new string[] { "-command", "ls --color=auto" }, attr);
            // var process = Process.Start("bash", new string[] { "-c", "ls --color=auto" }, attr);
            // var process = Process.Start("bash", new string[] { "-c", "echo 'test'; date; echo -e '\\e[15;1R'; echo -e '\\033[6n'; echo 'done'" }, attr);
            // var process = Process.Start("bash", new string[] { "-c", "stty -a" }, attr);
            // var process = Process.Start("zsh", new string[] { "-c", "git --version; stty -a" }, attr);
            // var process = Process.Start("node", new string[] { "-e", "require('fs'); console.log('hello world'); fs.readdir(process.cwd(), (err, files) => console.log(err, files))" }, attr);

            _ = ReadStdOut(process, process.Stdout);
            process.Wait();

            // Console.WriteLine($"End time: {DateTime.Now.ToLongTimeString()}");
            watch.Stop();
            Console.WriteLine($"Total duration: {watch.ElapsedMilliseconds}ms.");
        }

        // private static readonly ArrayPool<byte> outputPool = ArrayPool<byte>.Shared;

        private static async Task ReadStdOut(IProcess process, System.IO.StreamReader reader)
        {
            const int maxReadSize = 1024;
            var buffer = new char[maxReadSize];

            // var stdout = reader.BaseStream;
            while (process.IsRunning)
            {
                // var buffer = outputPool.Rent(1024);
                try
                {
                    // var read = await stdout.ReadAsync(buffer, 0, 1024);
                    var read = await reader.ReadAsync(buffer, 0, 1024);

                    var ds = new string(buffer, 0, read);
                    // var ds = Encoding.UTF8.GetString(buffer, 0, read);
                    // Console.Write(ds.Replace("\u001b", ""));

                    Console.Write(ds);
                    await Console.Out.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
                // finally
                // {
                    // outputPool.Return(buffer);
                // }
            }
        }
    }
}
