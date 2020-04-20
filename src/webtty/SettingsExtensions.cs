using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WebTty.Hosting.Models;

namespace WebTty
{
    public static class SettingsExtensions
    {
        public static async Task<Settings> MergeWithFile(this Settings options, FileInfo configFile)
        {
            Settings config = Settings.Defaults;

            if (configFile != null && configFile.Exists)
            {
                using var stream = configFile.OpenRead();
                config = await JsonSerializer.DeserializeAsync<Settings>(stream, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                });
            }

            return new Settings
            {
                Address = options.Address ?? config.Address ?? Settings.Defaults.Address,
                UnixSocket = options.UnixSocket ?? config.UnixSocket ?? Settings.Defaults.UnixSocket,
                Port = options.Port ?? config.Port ?? Settings.Defaults.Port,
                Path = options.Path ?? config.Path ?? Settings.Defaults.Path,
                Theme = options.Theme ?? config.Theme ?? Settings.Defaults.Theme,
                Command = options.Command ?? config.Command ?? Settings.Defaults.Command,
                Args = options.Args ?? config.Args ?? Settings.Defaults.Args,
                Themes = options.Themes ?? config.Themes ?? Settings.Defaults.Themes,
            };
        }
    }
}
