using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebTty
{
    public class TtyServer
    {
        private readonly CommandLineOptions _options;

        public TtyServer(CommandLineOptions options)
        {
            _options = options;
        }

        public async Task RunAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                var host = WebHost
                    .CreateDefaultBuilder(new string[] { })
                    .PreferHostingUrls(false)
                    .SuppressStatusMessages(true)
                    .UseKestrel(o =>
                    {
                        o.ListenLocalhost(5000);
                    })
                    .UseStartup<Startup>()
                    .Build();


                await host.StartAsync(cts.Token);
                Console.WriteLine($"Listening on http://localhost:{_options.Port}");
                Console.WriteLine("");
                Console.WriteLine("Press CTRL+C to exit");
                await host.WaitForShutdownAsync(cts.Token);
            }
        }
    }
}
