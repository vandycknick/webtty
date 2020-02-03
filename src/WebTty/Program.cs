using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebTty
{
    public class Program
    {
        static async Task<int> Main(string[] args)
        {
            var options = CommandLineOptions.Build(args);
            using var cts = new CancellationTokenSource();
            using var host = CreateHostBuilder(options).Build();

            var command = new RootCommand(options, host.StartAsync, host.WaitForShutdownAsync);
            var result = await command.ExecuteAsync(cts.Token);
            return result;
        }

        private static IHostBuilder CreateHostBuilder(CommandLineOptions options)
        {
            var contentRoot = GetCurrentAssemblyRootPath();

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.Add(new CommandLineOptionsConfigSource(options)))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStaticWebAssets()
                        .UseContentRoot(contentRoot)
                        .PreferHostingUrls(false)
                        .SuppressStatusMessages(true)
                        .UseKestrel(kestrel =>
                        {
                            kestrel.Listen(options.Address, options.Port);

                            if (!string.IsNullOrEmpty(options.UnixSocket))
                            {
                                kestrel.ListenUnixSocket(options.UnixSocket);
                            }
                        })
                        .UseStartup<Startup>();
                });
        }

        private static string GetCurrentAssemblyRootPath()
        {
            var root = Assembly.GetExecutingAssembly().Location;
            var rootDirectory = Path.GetDirectoryName(root);
            return rootDirectory;
        }
    }
}
