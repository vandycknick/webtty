using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebTty
{
    public class RootCommand
    {
        private readonly CommandLineOptions _options;
        private readonly Func<CancellationToken, Task> _startServerAsync;
        private readonly Func<CancellationToken, Task> _waitForShutdownAsync;

        public RootCommand(
            CommandLineOptions options,
            Func<CancellationToken, Task> startServerAsync,
            Func<CancellationToken, Task> waitForShutdownAsync
        )
        {
            _options = options;
            _startServerAsync = startServerAsync;
            _waitForShutdownAsync = waitForShutdownAsync;
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

                await _startServerAsync(token);
                Console.WriteLine($"Listening on http://{_options.Address.ToString()}:{_options.Port}");
                Console.WriteLine("");
                Console.WriteLine("Press CTRL+C to exit");
                await _waitForShutdownAsync(token);
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
