using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;

namespace WebTty
{
    class CommandLineOptions
    {
        public bool ShowHelp { get; private set; }
        public List<string> Rest { get; private set; }
        public string Version => GetVersion();
        public string Name => GetName();

        private readonly OptionSet _options;

        private CommandLineOptions()
        {
            _options = new OptionSet
            {
                { "?|h|help", "Show help information", h => ShowHelp = h != null }
            };
        }

        public bool ShouldShowHelp() => ShowHelp;

        public void WriteHelp()
        {
            Console.WriteLine($"{Name}: {Version}");
            Console.WriteLine();
            Console.WriteLine("TODO: add more information here");
            Console.WriteLine();
            Console.WriteLine($"Usage: {Name} [options]");
            Console.WriteLine();
            Console.WriteLine("Options");
            Console.WriteLine();
            _options.WriteOptionDescriptions(Console.Out);
        }

        private CommandLineOptions Parse(string[] args)
        {
            Rest = _options.Parse(args);
            return this;
        }
        public static CommandLineOptions Build(string[] args) =>
            new CommandLineOptions().Parse(args);

        public static string GetVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public static string GetName() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
    }
}
