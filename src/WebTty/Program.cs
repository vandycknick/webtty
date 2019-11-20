using System;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Common;
using WebTty.IO;

namespace WebTty
{
    public class Program
    {
        private readonly IConsole _console;
        private readonly IServer _server;
        private readonly CommandLineOptions _options;

        public Program(IConsole console, IServer server, CommandLineOptions options)
        {
            _console = console;
            _server = server;
            _options = options;
        }

        private void WriteHelp()
        {
            _console.WriteLine($"{_options.Name}: {_options.Version}");
            _console.WriteLine();
            _console.WriteLine("TODO: add more information here");
            _console.WriteLine();
            _console.WriteLine($"Usage: {_options.Name} [options]");
            _console.WriteLine();
            _console.WriteLine("Options");
            _console.WriteLine();
            _options.WriteOptions(_console.Out);
        }

        private void WriteErrorMessage(string message)
        {
            _console.WriteLine($"{_options.Name}: {_options.Version}");
            _console.WriteLine(message);
            _console.WriteLine($"Try '{_options.Name} --help' for more information.");
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
                    _console.WriteLine($"{_options.Version}");
                    return 0;
                }

                await _server.RunAsync(token);
                return 0;
            }
            catch (Exception ex)
            {
                WriteErrorMessage(ex.Message);
                return 1;
            }
        }

        static Task<int> Main(string[] args)
        {
            var console = NetConsole.Instance;
            var options = CommandLineOptions.Build(args);
            var server = new WebTtyServer(options, console);

            using var cts = new CancellationTokenSource();
            return new Program(console, server, options).ExecuteAsync(cts.Token);
        }
    }
}
