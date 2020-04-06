using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using WebTty.Hosting;
using WebTty.Hosting.Models;

namespace WebTty
{
    public partial class Program
    {
        public static Command CreateRootCommand(string[] args)
        {
            var command = new RootCommand("🔌 Simple command-line tool for sharing a terminal over the web.")
            {
               new Option(
                    new string[] { "-a", "--address" },
                    "IP address to use [localhost]. Use any to listen to any available address. Ex (0.0.0.0, any, 192.168.2.3, ...).")
                {
                    Argument = new Argument<IPAddress>(ArgumentExtensions.TryConvertIPAddress, true)
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

            command.TreatUnmatchedTokensAsErrors = false;
            command.Handler = CommandHandler.Create<Settings, ParseResult, CancellationToken>(async (settings, parseResult, token) =>
            {
                settings.Command = parseResult.UnparsedTokens.FirstOrDefault();
                settings.Args = parseResult.UnparsedTokens.Skip(1).ToList();

                settings = await settings.MergeWithFile(parseResult.ValueForOption<FileInfo>("config"));

                await using var host = new WebTtyHost(settings, args);
                await host.RunAsync(token);
            });

            return command;
        }
    }
}
