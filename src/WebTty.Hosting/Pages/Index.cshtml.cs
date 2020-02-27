using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using WebTty.Hosting.Models;
using WebTty.Hosting.Services;

namespace WebTty.Hosting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Settings _settings;
        private readonly StaticContentService _staticContent;
        private readonly JsonSerializerOptions _options;

        public IndexModel(Settings settings, StaticContentService staticContent)
        {
            _settings = settings;
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
            public string SelectedTheme { get; set; }
            public IReadOnlyList<Theme> Themes { get; set; }
        }
        public void OnGet()
        {
            var model = new WebTtyClientVm
            {
                PtyPath = _settings.Path,
                SelectedTheme = _settings.Theme,
                Themes = _settings.Themes
            };

            _staticContent.TryGetFile("main.js", out (string name, string hash) file);

            Config = JsonSerializer.Serialize(model, _options);
            ScriptPath = $"{file.name}?v={file.hash}";
        }
    }
}
