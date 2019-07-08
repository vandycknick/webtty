using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;

namespace WebTty
{
    public class CommandLineOptions
    {
        public bool ShowHelp { get; private set; }
        public bool ShowVersion { get; private set; }
        public int Port { get; private set; } = 5000;
        public string Version => GetVersion();
        public string Name => GetName();
        public List<string> Rest { get; private set; }

        private readonly OptionSet _options;

        private CommandLineOptions()
        {
            _options = new OptionSet
            {
                { "p|port=", "Port to use [5000]. Use 0 for a dynamic port.", (int? port) => Port = port ?? 5000 },
                { "version", "Show current version", version => ShowVersion = version != null },
                { "?|h|help", "Show help information", help => ShowHelp = help != null },
            };
        }

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
        public static CommandLineOptions Build(string[] args) => new CommandLineOptions().Parse(args);

        public static string GetVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public static string GetName() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
    }
}
