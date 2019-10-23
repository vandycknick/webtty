using System;
using System.Net;
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

                await host.StartAsync(cts.Token);
                Console.WriteLine($"Listening on http://{StringifyAddress(_options.Address)}:{_options.Port}");
                Console.WriteLine("");
                Console.WriteLine("Press CTRL+C to exit");
                await host.WaitForShutdownAsync(cts.Token);
            }
        }
    }
}
