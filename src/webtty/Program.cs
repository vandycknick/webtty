using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebTty.Hosting.Models;

namespace WebTty
{
    public partial class Program
    {
        public static Task<int> Main(string[] args)
        {
            var command = CreateRootCommand(args);

            var builder = new CommandLineBuilder(command);

            builder
                   .UseVersionOption()
                   .UseHelp()
                   .UseParseDirective()
                   .UseDebugDirective()
                   .UseSuggestDirective()
                   .RegisterWithDotnetSuggest()
                   .UseTypoCorrections()
                   .UseParseErrorReporting()
                   .UseExceptionHandler()
                   .CancelOnProcessTermination();

            var parser = builder.Build();
            return parser.InvokeAsync(args);
        }
    }
}
