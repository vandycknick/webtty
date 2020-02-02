using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebTty.Application;
using WebTty.UI.Common;

namespace WebTty
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Configuration["Logging.LogLevel.Default"] = "Error";
            services.AddPty();
            services.AddWebClient(config =>
            {
                config.PtyPath = Configuration.GetValue<string>("Path");
                config.Theme = Configuration.GetValue<string>("Theme");
            });
            services.AddResponseCompression();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapPty(Configuration.GetValue<string>("Path"));
            });
        }
    }
}
