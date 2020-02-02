using Microsoft.Extensions.Configuration;

namespace WebTty
{
    public class CommandLineOptionsConfigSource : IConfigurationSource
    {
        private readonly CommandLineOptions _options;
        public CommandLineOptionsConfigSource(CommandLineOptions options)
        {
            _options = options;
        }

        public class CommandLineConfigProvider : ConfigurationProvider
        {
            private readonly CommandLineOptions _options;
            public CommandLineConfigProvider(CommandLineOptions options)
            {
                _options = options;
            }

            public override void Load()
            {
                Data.Add("ShowHelp", _options.ShowHelp.ToString().ToLower());
                Data.Add("ShowVersion", _options.ShowVersion.ToString().ToLower());
                Data.Add("Address", _options.Address.ToString());
                Data.Add("UnixSocket", _options.UnixSocket);
                Data.Add("Port", _options.Port.ToString());
                Data.Add("Path", _options.Path);
                Data.Add("Theme", _options.Theme);
                Data.Add("Version", _options.Version);
                Data.Add("Name", _options.Name);
                Data.Add("Command", _options.Command);
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new CommandLineConfigProvider(_options);
    }
}
