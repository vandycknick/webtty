using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Options;

namespace WebTty
{
    public sealed class CommandLineOptions
    {
        public bool ShowHelp { get; private set; }
        public bool ShowVersion { get; private set; }
        public string Address { get; private set; } = "localhost";
        public int Port { get; private set; } = 5000;
        public string Version => GetVersion();
        public string Name => GetName();
        public List<string> Rest { get; private set; }

        private OptionException ParseException {get; set; }
        private readonly OptionSet _options;

        private CommandLineOptions()
        {
            _options = new OptionSet
            {
                { "a|address=", "IP address to use [localhost]. Use any to listen to any available address. Ex (0.0.0.0, any, 192.168.2.3, ...)", (string address) => Address = address ?? "localhost" },
                { "p|port=", "Port to use [5000]. Use 0 for a dynamic port.", (int? port) => Port = port ?? 5000 },
                { "version", "Show current version", version => ShowVersion = version != null },
                { "?|h|help", "Show help information", help => ShowHelp = help != null },
            };
        }

        public void WriteOptions(TextWriter writer) => _options.WriteOptionDescriptions(writer);

        public bool TryGetInvalidOptions(out string message)
        {
            if (ParseException != null)
            {
                message = ParseException.Message;
                return true;
            }

            message = string.Empty;
            return false;
        }

        private CommandLineOptions Parse(string[] args)
        {
            try
            {
                Rest = _options.Parse(args);
            }
            catch (OptionException ex)
            {
                ParseException = ex;
            }
            return this;
        }

        public static CommandLineOptions Build(string[] args) => new CommandLineOptions().Parse(args);

        private static string GetVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        private static string GetName() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
    }
}
