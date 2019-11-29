using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using FluentValidation;
using Mono.Options;

namespace WebTty
{
    public sealed class CommandLineOptions
    {
        public bool ShowHelp { get; private set; }
        public bool ShowVersion { get; private set; }
        public IPAddress Address { get; private set; } = IPAddress.Loopback;
        public string UnixSocket { get; private set; }
        public int Port { get; private set; } = 5000;
        public string Path { get; private set; } = "/tty";
        public string Version => GetVersion();
        public string Name => GetName();
        public string Command => Rest.FirstOrDefault() ?? "";
        public IReadOnlyCollection<string> CommandArgs => Rest.Skip(1).ToList();

        private IReadOnlyCollection<string> Rest { get; set; }

        private Exception ParseException { get; set; }
        private readonly OptionSet _options;

        private CommandLineOptions()
        {
            _options = new OptionSet
            {
                {
                    "a|address=",
                    "IP address to use [localhost]. Use any to listen to any available address. Ex (0.0.0.0, any, 192.168.2.3, ...)",
                    a => Address = ParseIPAddress(a)
                },
                {
                    "s|unix-socket=",
                    "Use the given Unix domain socket path for the server to listen to",
                    socket => UnixSocket = socket
                },
                {
                    "p|port=",
                    "Port to use [5000]. Use 0 for a dynamic port.",
                    (int? port) => Port = port ?? 5000
                },
                {
                    "path=",
                    "Path to use, defaults to /tty",
                    path => Path = string.IsNullOrEmpty(path) ? "/tty" : path
                },
                {
                    "version",
                    "Show current version",
                    version => ShowVersion = version != null
                },
                {
                    "?|h|help",
                    "Show help information",
                    help => ShowHelp = help != null
                },
            };
        }

        public void WriteOptions(TextWriter writer) => _options.WriteOptionDescriptions(writer);

        public bool TryGetInvalidOptions(out string message)
        {
            if (ParseException != null)
            {
                message = ParseException.Message;
                return true;
            }

            var validator = new CommandLineOptionsValidator();
            var result = validator.Validate(this);

            if (!result.IsValid)
            {
                message = result.Errors.FirstOrDefault().ErrorMessage;
                return true;
            }

            message = string.Empty;
            return false;
        }

        private CommandLineOptions Parse(string[] args)
        {
            try
            {
                Rest = _options.Parse(args);
            }
            catch (Exception ex)
            {
                ParseException = ex;
            }
            return this;
        }

        private IPAddress ParseIPAddress(string value)
        {
            if (string.Equals("localhost", value, StringComparison.OrdinalIgnoreCase))
            {
                return IPAddress.Loopback;
            }

            if (string.Equals("any", value, StringComparison.OrdinalIgnoreCase))
            {
                return IPAddress.Any;
            }

            if (!IPAddress.TryParse(value, out var address))
            {
                throw new FormatException($"'{value}' is not a valid IP address");
            }
            return address;
        }

        public static CommandLineOptions Build(string[] args) => new CommandLineOptions().Parse(args);

        private static string GetVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        private static string GetName() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

        public class CommandLineOptionsValidator : AbstractValidator<CommandLineOptions>
        {
            public CommandLineOptionsValidator()
            {
                RuleFor(options => options.Path)
                    .NotEmpty()
                    .Custom((path, context) =>
                    {
                        if (!path.StartsWith('/'))
                        {
                            context.AddFailure("'--path' should start with a slash (/).");
                        }
                    });

                RuleFor(options => options.Port)
                    .NotNull()
                    .GreaterThan(0)
                    .LessThanOrEqualTo(65535)
                    .WithName("--port");
            }
        }
    }
}
