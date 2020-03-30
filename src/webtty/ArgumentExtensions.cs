using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Net;

namespace WebTty
{
    public static class ArgumentExtensions
    {
        public static IPAddress TryConvertIPAddress(ArgumentResult result)
        {
            var token = result.Tokens.FirstOrDefault();

            if (token is null) return IPAddress.Loopback;

            if (string.Equals("localhost", token.Value, StringComparison.OrdinalIgnoreCase))
            {
                return IPAddress.Loopback;
            }

            if (string.Equals("any", token.Value, StringComparison.OrdinalIgnoreCase))
            {
                return IPAddress.Any;
            }

            if (IPAddress.TryParse(token.Value, out var address))
            {
                return address;
            }

            result.ErrorMessage = $"Invalid: {token.Value} is not a valid ip address";
            return null;
        }

        public static Argument<string> StartsWith(this Argument<string> arg, char character)
        {
            arg.AddValidator(result => {
                var value = result.GetValueOrDefault<string>();
                return value.StartsWith(character) ?
                        string.Empty : $"Argument {result.Argument.Name} should start with a {character}.";
            });

            return arg;
        }

        public static Argument<int> Between(this Argument<int> args, int min, int max)
        {
            args.AddValidator(result => {
                int port;

                try
                {
                    port = result.GetValueOrDefault<int>();
                }
                catch
                {
                    return $"Argument {result.Argument.Name} should be an integer.";
                }

                return port > min && port < max ?
                        string.Empty : $"Argument {result.Argument.Name} should be a value between {min} and {max}.";
            });

            return args;
        }
    }
}
