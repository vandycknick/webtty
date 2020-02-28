using System.Collections.Generic;
using System.Net;

namespace WebTty.Hosting.Models
{
    public class Settings
    {
        public static Settings Defaults = new Settings
        {
            Address = IPAddress.Loopback,
            Port = 5000,
            Path = "/pty",
            Theme = "default",
            Args = new List<string>(),
            Themes = new List<Theme>(),
        };

        public IPAddress Address { get; set; }
        public string UnixSocket { get; set; }
        public int? Port { get; set; }
        public string Path { get; set; }
        public string Theme { get; set; }
        public string Command { get; set; }
        public IReadOnlyList<string> Args { get; set; }
        public IReadOnlyList<Theme> Themes { get; set; }

        internal IDictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { nameof(Address), Address.ToString() },
                { nameof(UnixSocket), UnixSocket },
                { nameof(Port), Port.ToString() },
                { nameof(Path), Path },
                { nameof(Theme), Theme },
                { nameof(Command), Command },
                { nameof(Args), string.Join(' ', Args) },
            };
        }
    }
}
