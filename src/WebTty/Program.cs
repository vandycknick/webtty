using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Hosting;
using WebTty.Hosting.Models;

namespace WebTty
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            var command = RootCommand();

            command.TreatUnmatchedTokensAsErrors = false;
            command.Handler = CommandHandler.Create<Settings, ParseResult, CancellationToken>(async (settings, parseResult, token) =>
            {
                settings.Command = parseResult.UnparsedTokens.FirstOrDefault();
                settings.Args = parseResult.UnparsedTokens.Skip(1).ToList();

                settings = await MergeSettings(
                    settings,
                    parseResult.ValueForOption<FileInfo>("config")
                );

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("WebTty.Program", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

                var program = new Program(
                    hostBuilder: WebTtyHost.CreateHostBuilder(settings),
                    options: settings,
                    logger: Log.Logger
                );
                var result = await program.RunAsync(token);
                return result;
            });
            return command.InvokeAsync(args);
        }

        public static async Task<Settings> MergeSettings(Settings options, FileInfo configFile)
        {
            Settings config = Settings.Defaults;

            if (configFile != null && configFile.Exists)
            {
                using var stream = configFile.OpenRead();
                config = await JsonSerializer.DeserializeAsync<Settings>(stream, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                });
            }

            return new Settings
            {
                Address = options.Address ?? config.Address ?? Settings.Defaults.Address,
                UnixSocket = options.UnixSocket ?? config.UnixSocket ?? Settings.Defaults.UnixSocket,
                Port = options.Port ?? config.Port ?? Settings.Defaults.Port,
                Path = options.Path ?? config.Path ?? Settings.Defaults.Path,
                Theme = options.Theme ?? config.Theme ?? Settings.Defaults.Theme,
                Command = options.Command ?? config.Command ?? Settings.Defaults.Command,
                Args = options.Args ?? config.Args ?? Settings.Defaults.Args,
                Themes = options.Themes ?? config.Themes ?? Settings.Defaults.Themes,
            };
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
                    Argument = new Argument<string>
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
                    Argument = new Argument<int>
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
                    Argument = new Argument<string>
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
                    Argument = new Argument<string>
                    {
                        Arity = ArgumentArity.ZeroOrOne,
                    },
                    Required = false
                },
                new Option(
                    new string[] { "-c", "--config"},
                    "Path to a json config file, cli arguments always take precedence"
                )
                {
                    Argument = new Argument<FileInfo>()
                    {
                        Name = "configFile",
                        Arity = ArgumentArity.ZeroOrOne
                    },
                    Required = false
                }
            };

        private readonly IHostBuilder _hostBuilder;
        private readonly ILogger _logger;
        private readonly Settings _options;

        public Program(IHostBuilder hostBuilder, Settings options, ILogger logger)
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
