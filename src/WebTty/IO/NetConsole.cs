using System;
using System.IO;

namespace WebTty.IO
{
    public sealed class NetConsole : IConsole
    {
        public static IConsole Instance { get; } = new NetConsole();

        private NetConsole()
        {
        }

        public TextReader In => Console.In;

        public TextWriter Out => Console.Out;

        public TextWriter Error => Console.Error;

        public bool IsInputRedirected => Console.IsInputRedirected;

        public bool IsOutputRedirected => Console.IsOutputRedirected;

        public bool IsErrorRedirected => Console.IsErrorRedirected;

        public ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }

        public ConsoleColor ForegroundColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public void ResetColor() => Console.ResetColor();
    }
}
