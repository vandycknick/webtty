using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

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
