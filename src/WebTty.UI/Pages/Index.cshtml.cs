using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebTty.UI.Common;

namespace WebTty.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IOptions<WebClientConfig> _options;

        public IndexModel(IOptions<WebClientConfig> options)
        {
            _options = options;
        }
        public string Config { get; set; }
        public void OnGet()
        {
            Config = JsonSerializer.Serialize(_options.Value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            });
        }
    }
}
