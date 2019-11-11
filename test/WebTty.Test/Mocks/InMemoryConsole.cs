using System;
using System.IO;
using WebTty.IO;

namespace WebTty.Test.Mocks
{
    public class InMemoryConsole : IConsole
    {
        public InMemoryConsole() : this("")
        {
        }

        public InMemoryConsole(string input)
        {
            _input = input;
        }

        private readonly string _input;

        public StringWriter OutString = new StringWriter();
        public StringWriter ErrorString = new StringWriter();

        public TextReader In => new StringReader(_input);
        public TextWriter Out => OutString;
        public TextWriter Error => ErrorString;

        public bool IsInputRedirected { get; internal set; }

        public bool IsOutputRedirected { get; internal set; }

        public bool IsErrorRedirected { get; internal set; }

        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public void ResetColor()
        {
        }
    }
}
