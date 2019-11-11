using System;
using System.Net;
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
                .CreateDefaultBuilder(new string[] { })
                .PreferHostingUrls(false)
                .SuppressStatusMessages(true)
                .UseKestrel(o =>
                {
                    if (_options.Address.ToLower() == "localhost")
                    {
                        o.ListenLocalhost(_options.Port);
                    }
                    else if (_options.Address.ToLower() == "any")
                    {
                        o.ListenAnyIP(_options.Port);
                    }
                    else if (IPAddress.TryParse(_options.Address, out var address))
                    {
                        o.Listen(
                            address,
                            _options.Port
                        );
                    }
                    else
                    {
                        Console.WriteLine("TODO: add message here and kill app");
                    }
                })
                .UseStartup<Startup>()
                .Build();


            string StringifyAddress(string address)
            {
                return address switch
                {
                    "localhost" => "localhost",
                    "any" => "+",
                    _ => address,
                };
            }

            await host.StartAsync(token);
            _console.WriteLine($"Listening on http://{StringifyAddress(_options.Address)}:{_options.Port}");
            _console.WriteLine("");
            _console.WriteLine("Press CTRL+C to exit");
            await host.WaitForShutdownAsync(token);
        }
    }
}
