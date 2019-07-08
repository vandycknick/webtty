using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Mono.Options;

namespace WebTty
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Windows is currently not supported. Support for windows is currently work in progress!");
            }

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
