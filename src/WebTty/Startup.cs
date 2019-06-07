using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace WebTty
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHttpsRedirection();
            }

            app.UseWebSockets();
            app.UseWebTerminal();
        }
    }
}
