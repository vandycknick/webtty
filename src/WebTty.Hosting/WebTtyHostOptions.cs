using System.Collections.Generic;
using System.Net;

namespace WebTty.Hosting
{
    public class WebTtyHostOptions
    {
        public IPAddress Address { get; set; } = IPAddress.Loopback;
        public string UnixSocket { get; set; }
        public int Port { get; set; } = 5000;
        public string Path { get; set; } = "/pty";
        public string Theme { get; set; } = "default";
        public string Command { get; set; }
        public List<string> Args { get; set; } = new List<string>();

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
