using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Hosting;

namespace WebTty
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            var command = RootCommand();

            command.TreatUnmatchedTokensAsErrors = false;
            command.Handler = CommandHandler.Create<WebTtyHostOptions, ParseResult, CancellationToken>((options, parseResult, token) =>
            {
                options.Command = parseResult.UnparsedTokens.FirstOrDefault();
                options.Args = parseResult.UnparsedTokens.Skip(1).ToList();

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("WebTty.Program", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

                return new Program(
                    hostBuilder: WebTtyHost.CreateHostBuilder(options),
                    options: options,
                    logger: Log.Logger
                ).RunAsync(token);

            });
            return command.InvokeAsync(args);
        }

        public static Command RootCommand() =>
            new RootCommand("🔌 Simple command-line tool for sharing a terminal over the web.")
            {
               new Option(
                    new string[] { "-a", "--address" },
                    "IP address to use [localhost]. Use any to listen to any available address. Ex (0.0.0.0, any, 192.168.2.3, ...).")
                {
                    Argument = new Argument<IPAddress>(ArgumentExtensions.TryConvertIPAddress, () => IPAddress.Loopback)
                    {
                        Name = "address",
                        Arity = ArgumentArity.ZeroOrOne,
                    },
                    Required = false,
                },
                new Option(
                    new string[] { "-s", "--unix-socket" },
                    "Use the given Unix domain socket path for the server to listen to"
                )
                {
                    Argument = new Argument<string>(() => string.Empty)
                    {
                        Name = "filepath",
                        Arity = ArgumentArity.ZeroOrOne
                    },
                    Required = false,
                },
                new Option(
                    new string[] { "-p", "--port" },
                    "Port to use [5000]. Use 0 for a dynamic port."
                )
                {
                    Argument = new Argument<int>(() => 5000)
                    {
                        Name = "port",
                    }.Between(0, 65535),
                    Required = false,
                },
                new Option(
                    new string[] { "--path" },
                    "Path to use, defaults to /pty"
                )
                {
                    Argument = new Argument<string>(() => "/pty")
                    {
                        Arity = ArgumentArity.ZeroOrOne,
                    }.StartsWith('/'),
                    Required = false,
                },
                new Option(
                    new string[] { "--theme" },
                    "Theme to use, uses a simple black theme by default"
                )
                {
                    Argument = new Argument<string>(() => "default")
                    {
                        Arity = ArgumentArity.ZeroOrOne,
                    },
                    Required = false
                },
            };

        private readonly IHostBuilder _hostBuilder;
        private readonly ILogger _logger;
        private readonly WebTtyHostOptions _options;

        public Program(IHostBuilder hostBuilder, WebTtyHostOptions options, ILogger logger)
        {
            _hostBuilder = hostBuilder;
            _options = options;
            _logger = logger;
        }

        public async Task<int> RunAsync(CancellationToken token)
        {
            try
            {
                using var host = _hostBuilder.Build();

                await host.StartAsync(token);
                _logger.Information("Listening on: http://{address}:{port}", _options.Address, _options.Port);
                _logger.Information("Press CTRL+C to exit");

                var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                token.Register(_ => waitForStop.TrySetResult(null), null);

                await waitForStop.Task;

                _logger.Information("Application is shutting down...");
                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                _logger.Information("Goodbye. 👋");
                Log.CloseAndFlush();
            }
        }
    }
}
