using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;
using WebTty.Hosting.Services;

namespace WebTty.Hosting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly StaticContentService _staticContent;
        private readonly JsonSerializerOptions _options;

        public IndexModel(IConfiguration configuration, StaticContentService staticContent)
        {
            _configuration = configuration;
            _staticContent = staticContent;
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

            _staticContent.TryGetFile("main.js", out (string name, string hash) file);

            Config = JsonSerializer.Serialize(model, _options);
            ScriptPath = $"{file.name}?v={file.hash}";
        }
    }
}
