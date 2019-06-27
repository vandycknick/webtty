using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebTty.UI
{
    public class UiMiddleware
    {
        private const string INDEX_VIEW = "WebTty.UI.Views.index.html";
        private readonly RequestDelegate _next;
        private readonly Assembly _assembly;
        private readonly string[] _resources;

        public UiMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _assembly = Assembly.GetExecutingAssembly();
            _resources = _assembly.GetManifestResourceNames();
        }
        public async Task Invoke(HttpContext context)
        {
            if (!_resources.Contains(INDEX_VIEW))
            {
                await _next(context);
                return;
            }

            if (context.Request.Path == "/")
            {
                var viewStream = _assembly.GetManifestResourceStream(INDEX_VIEW);

                context.Response.ContentType = "text/html";

                await viewStream.CopyToAsync(context.Response.Body);
                return;
            }

            await _next(context);
        }
    }
}
