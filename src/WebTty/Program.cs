using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Hosting;

namespace WebTty
{
    public class Program
    {
        private static IHostBuilder CreateHostBuilder(CommandLineOptions options)
        {
            return WebTtyHost.CreateHostBuilder()
                .ConfigureAppConfiguration(builder => builder.Add(new CommandLineOptionsConfigSource(options)))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStaticWebAssets()
                        .PreferHostingUrls(false)
                        .SuppressStatusMessages(true)
                        .UseKestrel(kestrel =>
                        {
                            kestrel.Listen(options.Address, options.Port);

                            if (!string.IsNullOrEmpty(options.UnixSocket))
                            {
                                kestrel.ListenUnixSocket(options.UnixSocket);
                            }
                        });
                })
                .UseSerilog();
        }
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("WebTty.Program", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var options = CommandLineOptions.Build(args);
                using var cts = new CancellationTokenSource();
                using var host = CreateHostBuilder(options).Build();

                var result = await new Program(options, host, Log.Logger.ForContext<Program>()).ExecuteAsync(cts.Token);
                return result;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private readonly CommandLineOptions _options;
        private readonly IHost _host;
        private readonly ILogger _logger;

        public Program(CommandLineOptions options, IHost host, ILogger logger)
        {
            _options = options;
            _host = host;
            _logger = logger;
        }

        private void WriteHelp()
        {
            Console.WriteLine($"{_options.Name}: {_options.Version}");
            Console.WriteLine();
            Console.WriteLine("🔌 WebSocket based terminal emulator");
            Console.WriteLine();
            Console.WriteLine($"Usage: {_options.Name} [options] -- [command] [<arguments...>]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            _options.WriteOptions(Console.Out);
        }

        private void WriteErrorMessage(string message)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine($"Try '{_options.Name} --help' for more information.");
        }

        public async Task<int> ExecuteAsync(CancellationToken token = default)
        {
            try
            {
                if (_options.TryGetInvalidOptions(out var message))
                {
                    WriteErrorMessage(message);
                    return 1;
                }

                if (_options.ShowHelp)
                {
                    WriteHelp();
                    return 0;
                }

                if (_options.ShowVersion)
                {
                    Console.WriteLine($"{_options.Version}");
                    return 0;
                }

                await _host.StartAsync(token);
                _logger.Information("Listening on: http://{address}:{port}", _options.Address, _options.Port);
                _logger.Information("Press CTRL+C to exit");

                var lifetime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
                var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                lifetime.ApplicationStopping.Register(_ => waitForStop.TrySetResult(null), null);

                await waitForStop.Task;

                _logger.Information("Application is shutting down...");
                await _host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                WriteErrorMessage(ex.Message);
                return 1;
            }
        }
    }
}
