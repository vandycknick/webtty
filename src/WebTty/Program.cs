using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Mono.Options;

namespace WebTty
{
    class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        static int Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.Build(args);

                if (options.ShowHelp)
                {
                    options.WriteHelp();
                    return 0;
                }

                CreateWebHostBuilder(args)
                        .Build()
                        .Run();
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
