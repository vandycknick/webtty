using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebTty.Transport;
using WebTty.UI;
using WebTty.Terminal;
using WebTty.Protocol;
using WebTty.Common;
using WebTty.Messages.Helpers;

namespace WebTty
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddMediatR(config => config.AsScoped(), typeof(Startup));

            services.AddSingleton<IMessageResolver, MessageResolver>();
            services.AddSingleton<BinaryDeserializerMap>();
            services.AddSingleton<IProtocol, BinaryProtocol>();
            services.AddScoped<ITransport, WebSocketsTransport>();
            services.AddScoped<IConnection, HttpConnection>();
            services.AddScoped<IConnectionHandler, TerminalConnectionHandler>();
            services.AddScoped<IMessageDispatcher, TerminalMessageDispatcher>();

            services.AddScoped<TerminalManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStatusCodePages();
            app.UseResponseCompression();
            app.UseMiddleware<UiMiddleware>();
            app.UseWebSockets();
            app.UseWebTerminal();
        }
    }
}
