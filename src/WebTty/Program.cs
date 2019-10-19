using System;
using System.Threading.Tasks;
using Mono.Options;

namespace WebTty
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.Build(args);

                if (options.ShowHelp)
                {
                    options.WriteHelp();
                    return 0;
                }

                if (options.ShowVersion)
                {
                    Console.WriteLine($"{CommandLineOptions.GetVersion()}");
                    return 0;
                }

                await new TtyServer(options).RunAsync();
                return 0;
            }
            catch (OptionException e)
            {
                Console.WriteLine($"{CommandLineOptions.GetName()}: {CommandLineOptions.GetVersion()}");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine($"Try `{CommandLineOptions.GetName()} --help' for more information.");
                return 1;
            }
        }
    }
}
