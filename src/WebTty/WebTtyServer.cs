using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WebTty.IO;
using WebTty.Common;

namespace WebTty
{
    public class WebTtyServer : IServer
    {
        private readonly CommandLineOptions _options;
        private readonly IConsole _console;

        public WebTtyServer(CommandLineOptions options, IConsole console)
        {
            _options = options;
            _console = console;
        }

        public async Task RunAsync(CancellationToken token = default)
        {
            var host = WebHost
                .CreateDefaultBuilder()
                .PreferHostingUrls(false)
                .SuppressStatusMessages(true)
                .UseKestrel(kestrel =>
                {
                    kestrel.Listen(_options.Address, _options.Port);

                    if (!string.IsNullOrEmpty(_options.UnixSocket))
                    {
                        kestrel.ListenUnixSocket(_options.UnixSocket);
                    }
                })
                .UseStartup<Startup>()
                .Build();

            await host.StartAsync(token);
            _console.WriteLine($"Listening on http://{_options.Address.ToString()}:{_options.Port}");
            _console.WriteLine("");
            _console.WriteLine("Press CTRL+C to exit");
            await host.WaitForShutdownAsync(token);
        }
    }
}
