using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WebTty.Hosting.Services
{
    public class StaticContentService
    {
        private readonly StaticFileOptions _fileOptions;
        private readonly ConcurrentDictionary<string, string> _fileHashes = new ConcurrentDictionary<string, string>();
        public StaticContentService(IOptions<StaticFileOptions> options)
        {
            _fileOptions = options.Value;
        }

        public bool TryGetFile(string fileName, out (string name, string hash) file)
        {
            if (_fileHashes.TryGetValue(fileName, out var hash))
            {
                file.name = fileName;
                file.hash = hash;
                return true;
            }
            else
            {
                var fileInfo = _fileOptions.FileProvider
                            .GetDirectoryContents(".")
                            .Where(file => file.Name.ToLower() == "main.js")
                            .FirstOrDefault();

                if (fileInfo == null)
                {
                    file.name = string.Empty;
                    file.hash = string.Empty;
                    return false;
                }
                else
                {
                    using var hasher = SHA1.Create();
                    using var stream = fileInfo.CreateReadStream();

                    file.name = fileInfo.Name;
                    file.hash = HexStringFromBytes(hasher.ComputeHash(stream));

                    _fileHashes.TryAdd(file.name, file.hash);
                    return true;
                }
            }
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
