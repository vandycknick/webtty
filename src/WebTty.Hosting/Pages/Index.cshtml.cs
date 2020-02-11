using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;

namespace WebTty.Hosting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _options;
        private readonly StaticFileOptions _staticFileOptions;

        public IndexModel(IConfiguration configuration, IOptions<StaticFileOptions> options)
        {
            _configuration = configuration;
            _staticFileOptions = options.Value;
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
        }
        public string Config { get; set; }
        public string ScriptPath { get; set; }

        class WebTtyClientVm
        {
            public string PtyPath { get; set; }
            public string Theme { get; set; }
        }
        public void OnGet()
        {
            var model = new WebTtyClientVm
            {
                PtyPath = _configuration.GetValue<string>("Path"),
                Theme = _configuration.GetValue<string>("Theme"),
            };

            Config = JsonSerializer.Serialize(model, _options);
            ScriptPath = _staticFileOptions.FileProvider
                            .GetDirectoryContents(".")
                            .Select(file => file.Name.ToLower())
                            .Where(fileName =>
                                fileName.StartsWith("main") && fileName.EndsWith(".js"))
                            .FirstOrDefault();
        }
    }
}
