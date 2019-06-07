using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebTty
{
    class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        static Task Main(string[] args) =>
            CreateWebHostBuilder(args)
                .Build()
                .RunAsync();
    }
}
